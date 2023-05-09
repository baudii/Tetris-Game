using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class Block : MonoBehaviour
{
    [SerializeField] Vector2 perfectCenterOffset;
    [SerializeField] protected Vector2 rotationPivot;

    public UnityAction<Transform> OnBlockHitGround;
    BlockSpawner spawner;
    protected GhostBlock ghost;

    public Color blockColor;
    float maxDelta = 0.4f;
    float delta = 0.4f;

    float normalFallDelta = 1;
    float fastFallDelta = 0.017f;
    float fallDelta;

    public void Init(BlockSpawner spawner, float fallSpeed)
    {
        this.spawner = spawner;
        normalFallDelta = fallSpeed;
        fallDelta = normalFallDelta;
        blockColor = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
        ghost = Instantiate(transform.GetChild(0)).gameObject.AddComponent<GhostBlock>();
        ghost.Init(spawner, this);
        ghost.UpdatePosition();
        StartCoroutine(Fall());
    }
/*
    private void InitializeAudioClips()
    {
        foreach (var item in audioClips.clips)
        {
            if (item.name == "fall") fallSFX = item.clip;
            if (item.name == "rotate") rotateSFX = item.clip;
            if (item.name == "move") moveSFX = item.clip;
        }
    }*/


    public GameObject GetBlockIcon(Transform parent)
    {
        var tr = Instantiate(transform.GetChild(0), parent);
        foreach(Transform t in tr)
        {
            t.localPosition = t.localPosition - (Vector3)perfectCenterOffset;
        }
        return tr.gameObject;
    }

    public void FastFall()
    {
        fallDelta = Mathf.Min(normalFallDelta, fastFallDelta);
        StopAllCoroutines();
        StartCoroutine(Fall());
    }

    public void StopFastFall()
    {
        fallDelta = normalFallDelta;
    }

    public void ResetMovementValues()
    {
        maxDelta = 0.4f;
        delta = 0.4f;
    }
    public void UpdateMovement(Vector3 dir)
    {
        if (dir.magnitude != 1)
            return;
        delta += Time.deltaTime;
        if (delta < maxDelta) return;
        delta = 0;
        maxDelta = Mathf.Max(0.03f, maxDelta -= 0.3f);

        Move(dir);
    }

    IEnumerator Fall()
    {
        float delta = fallDelta;
        while (true)
        {
            delta += Time.deltaTime;
            yield return null;
            if (delta < fallDelta)
                continue;
            if (!IsValidMove(Vector3.down))
                break;

            delta = 0;
            Move(Vector3.down);

        }
        OnBlockHitGround.Invoke(transform.GetChild(0));
        OnBlockHitGround = null;
        Destroy(this);
    }

    public void Move(Vector3 dir)
    {
        if (dir == Vector3.zero)
            return;

        transform.position += dir;
        if (!IsValidMove())
            transform.position -= dir;

        ghost.UpdatePosition();
    }

    protected virtual void PerformRotation()
    {
        transform.RotateAround(transform.TransformPoint(rotationPivot), Vector3.forward, -90);
        if (!SetValidPosition())
        {
            transform.RotateAround(transform.TransformPoint(rotationPivot), Vector3.forward, 90);
        }
    }

    public void Rotate()
    {
        PerformRotation();

        ghost.UpdatePosition();
    }

    protected bool SetValidPosition()
    {
        foreach (Transform child in transform.GetChild(0))
        {
            var newPos = child.position;
            if (newPos.x < 0)
            {
                transform.position += Vector3.right * Mathf.Abs(Mathf.Floor(newPos.x));
            }
            if (newPos.x >= 10)
            {
                transform.position += Vector3.left * (Mathf.Floor(newPos.x) - 9);
            }
            if (newPos.y < 0)
            {
                transform.position += Vector3.up * Mathf.Abs(Mathf.Floor(newPos.y));
            }
            if (spawner.IsOccupied(newPos.x, newPos.y))
            {
                return false;
            }
        }
        return true;
    }

    bool IsValidMove(Vector3 offset = default)
    {
        foreach (Transform child in transform.GetChild(0))
        {
            var newPos = child.position + offset;
            if (newPos.y < 0 || newPos.x < 0 || newPos.x >= 10)
            {
                return false;
            }
            if (spawner.IsOccupied(newPos.x, newPos.y))
            {
                return false;
            }
        }
        return true;
    }
}
