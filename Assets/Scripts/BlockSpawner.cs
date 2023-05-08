using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] bool touchInput;
    [SerializeField] Block[] tetrisBlocks;
    [SerializeField] ScoringSystem scoringSystem;
    [SerializeField] Transform nextBlockSlot;
    [SerializeField] LineClear lineClearPrefab;
    [Header("Audio")]
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip lineClearSFX, fallSFX;


    Block nextBlock;
    Block currentBlock;
    GameObject nextBlockIcon;
    Block[] shuffledBlocks;
    Transform[,] grid = new Transform[20, 10];
    int i = 0;
    GameManager gm;

    [HideInInspector]
    public Vector3 movementDir;
    bool releasedArrow;
    [HideInInspector]
    public bool rotationInput, fastFall, stopFastFall;

    void Start()
    {
        shuffledBlocks = Shuffle(tetrisBlocks);
        nextBlock = shuffledBlocks[i];
        gm = FindObjectOfType<GameManager>();
        Spawn();
    }

    void Update()
    {
        if (currentBlock == null || touchInput)
            return;

        movementDir = Vector3.zero.WhereX(Input.GetAxisRaw("Horizontal"));

        if (Input.GetKeyDown(KeyCode.Space)) rotationInput = true;
        if (Input.GetKeyDown(KeyCode.DownArrow)) fastFall = true;
        if (Input.GetKeyUp(KeyCode.DownArrow)) stopFastFall = true;
    }

    void FixedUpdate()
    {
        if (currentBlock == null)
            return;

        if (movementDir.magnitude == 0) currentBlock.ResetMovementValues();
        else currentBlock.UpdateMovement(movementDir);

        if (releasedArrow)
        {
            movementDir = Vector3.zero;
            releasedArrow = false;
        }

        if (rotationInput)
        {
            currentBlock.Rotate();
            rotationInput = false;
        }
        if (stopFastFall)
        {
            currentBlock.StopFastFall();
            stopFastFall = false;
        }
        if (fastFall)
        {
            fastFall = false;
            currentBlock.FastFall();
        }
    }

    public void RecieveRotationInput() => rotationInput = true;
    public void RecieveFastFallInput() => fastFall = true;
    public void RecieveStopFastFallInput() => stopFastFall = true;
    public void RecieveDirInput(string dir)
    {
        if (dir == "right")
        {
            movementDir = Vector3.right;
        }
        else if (dir == "left")
        {
            movementDir = Vector3.left;
        }
        else releasedArrow = true;
    }

    public void Spawn()
    {
        currentBlock = Instantiate(nextBlock, transform.position, Quaternion.identity);
        currentBlock.Init(this, scoringSystem.GetFallSpeed());

        currentBlock.OnBlockHitGround += OnBlockLand;

        i++;
        if (i % shuffledBlocks.Length == 0)
        {
            shuffledBlocks = Shuffle(tetrisBlocks);
            i = 0;
        }
        nextBlock = shuffledBlocks[i];
        if (nextBlockIcon != null)
            Destroy(nextBlockIcon);
        nextBlockIcon = nextBlock.GetBlockIcon(nextBlockSlot);
    }

    public bool IsOccupied(float x, float y)
    {
        if (x < 0 || y < 0 || x >= 10 || y >= 20)
            return false;
        return grid[Mathf.FloorToInt(y), Mathf.FloorToInt(x)] != null;
    }

    void OnBlockLand(Transform block)
    {
        audioSrc.PlayOneShot(fallSFX);
        currentBlock = null;
        while (block.childCount > 0) {
            var child = block.GetChild(0);
            if (child.position.y >= 20)
            {
                gm.ShowGameOver(scoringSystem.GetScore());
                return;
            }
            child.parent = transform;
            grid[Mathf.FloorToInt(child.position.y), Mathf.FloorToInt(child.position.x)] = child;
        }
        Destroy(block.parent.gameObject);

        var removedLines = ClearLines();
        if (removedLines.Count == 0)
        {
            Spawn();
            return;
        }
        audioSrc.PlayOneShot(lineClearSFX);

        CameraShaker.Instance.ShakeOnce(removedLines.Count * 2f, 2f, 0.1f, 0.7f);

        StartCoroutine(AnimateLineClear(removedLines));
    }

    void PrintGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            string row = "";
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null)
                    row += "1";
                else row += "0";
            }
            print(row);
        }
    }

    IEnumerator AnimateLineClear(List<int> removedLines)
    {
        var offsets = GetOffsets(removedLines);

        foreach(var lineNum in removedLines)
        {
            Instantiate(lineClearPrefab, new Vector3(transform.position.x, lineNum + 0.5f, -1f), Quaternion.identity).Init(1);
            Instantiate(lineClearPrefab, new Vector3(transform.position.x, lineNum + 0.5f, -1f), Quaternion.identity).Init(-1);
        }

        float t = 0;
        yield return new WaitForSeconds(0.3f);
        while (true)
        {
            foreach (var offset in offsets)
            {
                offset.Item1.position = Vector3.Lerp(offset.Item2, offset.Item3, t);
            }

            if (t >= 1) break;
            t += Time.deltaTime * 8;
            yield return null;
        }

        Spawn();
    }

    List<int> ClearLines()
    {
        List<int> removedLines = new List<int>();
        int totalLines = 0;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            if(IsFullLine(i))
            {
                totalLines++;
                DeleteLine(i);
                removedLines.Add(i);
            }
        }
        scoringSystem.OnClearLines(totalLines);
        return removedLines;
    }

    List<(Transform, Vector3, Vector3)> GetOffsets(List<int> removedLines)
    {
        var lastRowToStack = removedLines[0];
        List<(Transform, Vector3, Vector3)> offsets = new List<(Transform, Vector3, Vector3)>();
        for (int i = removedLines[0]; i < grid.GetLength(0); i++)
        {
            if (removedLines.Contains(i))
                continue;

            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null)
                {
                    grid[lastRowToStack, j] = grid[i, j];
                    grid[i, j] = null;
                    var tr = grid[lastRowToStack, j].transform;
                    offsets.Add((tr, tr.position, tr.position + Vector3.down * Mathf.Abs(i - lastRowToStack)));
                }
            }
            lastRowToStack++;
        }
        return offsets;
    }

    private void DeleteLine(int i)
    {
        for (int j = 0; j < grid.GetLength(1); j++)
        {
            Destroy(grid[i, j].gameObject);
            grid[i, j] = null;
        }
    }

    bool IsFullLine(int i)
    {
        for (int j = 0; j < grid.GetLength(1); j++)
        {
            if (grid[i, j] == null)
                return false;
        }
        return true;
    }

    Block[] Shuffle(Block[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            var last = list.Length - i;
            var t = Random.Range(0, last);
            if (t == last-1)
                continue;

            var tmp = list[last-1];
            list[last-1] = list[t];
            list[t] = tmp;
        }
        return list;
    }
}
