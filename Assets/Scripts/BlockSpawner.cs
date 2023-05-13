using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] Block[] tetrisBlocks;
    [SerializeField] ScoringSystem scoringSystem;
    [SerializeField] Transform nextBlockSlot;
    [SerializeField] LineClear lineClearPrefab;
    [SerializeField] GameObject plainBlockPrefab;
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

    Vector3 movementDir;

    float fastFallSpeed = 0.017f;
    float normalFallSpeed = 1;

    void Start()
    {
        shuffledBlocks = Shuffle(tetrisBlocks);
        nextBlock = shuffledBlocks[i];
        gm = FindObjectOfType<GameManager>();
        Spawn();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            RecieveDirInput(-1f);
        if (Input.GetKey(KeyCode.RightArrow))
            RecieveDirInput(1f);
        if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            RecieveDirInput(0);

        if (Input.GetKeyDown(KeyCode.Space)) RecieveRotationInput();
        if (Input.GetKeyDown(KeyCode.DownArrow)) FastFall(true);
        if (Input.GetKeyUp(KeyCode.DownArrow)) FastFall(false);
    }

    void FixedUpdate()
    {
        currentBlock?.SmoothMovement(movementDir);
    }

    public void RecieveRotationInput() => currentBlock?.Rotate();

    public void RecieveDirInput(float dir)
    {
        movementDir = new Vector3(dir, 0, 0);
    }

    bool isFastFall;
    float fastFallDelta;
    public bool IsFastFall()
    {
        if (isFastFall)
        {
            fastFallDelta += Time.deltaTime;
            if (fastFallDelta >= fastFallSpeed)
            {
                fastFallDelta -= fastFallSpeed;
                return true;
            }
            return false;
        }

        fastFallSpeed = Mathf.Min(fastFallSpeed, normalFallSpeed);
        fastFallDelta = fastFallSpeed;

        return false;
    }
    public void FastFall(bool value)
    {
        isFastFall = value;
    }

    public void Spawn()
    {
        currentBlock = Instantiate(nextBlock, transform.position, Quaternion.identity);
        currentBlock.Init(this);
        normalFallSpeed = scoringSystem.GetFallSpeed();
        currentBlock.OnBlockHitGround += OnBlockLand;

        Invoke(nameof(BeginBlockFall), .3f);

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

    void BeginBlockFall()
    {
        currentBlock.StartFall(normalFallSpeed);
    }

    public bool IsOccupied(float x, float y)
    {
        if (x < 0 || y < 0 || x >= 10 || y >= 20)
            return false;
        return grid[Mathf.FloorToInt(y), Mathf.FloorToInt(x)] != null;
    }

    void OnBlockLand(Transform block)
    {
        CancelInvoke();
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
#if UNITY_EDITOR
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
#endif

    IEnumerator AnimateLineClear(List<int> removedLines)
    {
        var offsets = GetOffsets(removedLines);

        foreach(var lineNum in removedLines)
        {
            Instantiate(lineClearPrefab, new Vector3(transform.position.x, lineNum + 0.5f, -1f), Quaternion.identity).Init(1);
            Instantiate(lineClearPrefab, new Vector3(transform.position.x, lineNum + 0.5f, -1f), Quaternion.identity).Init(-1);
        }

        yield return new WaitForSeconds(.4f);

        yield return StartCoroutine(SlideItems(offsets));
        yield return new WaitForSeconds(.4f);

        var greyRows = scoringSystem.GetGreyRowsAmount();
        if (greyRows > 0)
            yield return StartCoroutine(SpawnGreyRow(greyRows));


        Spawn();
    }

    IEnumerator SpawnGreyRow(int amount)
    {
        var offsets = GetPositiveOffsets(amount);

        yield return StartCoroutine(SlideItems(offsets));


        for (int i = 0; i < amount; i++)
        {
            int hole = Random.Range(0, grid.GetLength(1));
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (j == hole)
                    continue;
                var go = Instantiate(plainBlockPrefab, transform);
                go.transform.position = new Vector3(j + 0.5f, i + 0.5f, 0);
                grid[i, j] = go.transform;
            }
        }
    }

    IEnumerator SlideItems(List<(Transform, Vector3, Vector3)> offsets)
    {
        float t = 0;
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


    List<(Transform, Vector3, Vector3)> GetPositiveOffsets(int amount)
    {
        List<(Transform, Vector3, Vector3)> offsets = new List<(Transform, Vector3, Vector3)>();
        for (int i = grid.GetLength(0) - 1; i >= 0; i--)
        {
            
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == null)
                    continue;

                if (i + amount >= grid.GetLength(0))
                {
                    gm.ShowGameOver(scoringSystem.GetScore());
                    return null;
                }

                var t = grid[i, j];
                offsets.Add((t, t.position, t.position + (Vector3.up * amount)));
                grid[i + amount, j] = grid[i, j];
                grid[i, j] = null;
            }
        }
        return offsets;
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
                    var tr = grid[i, j];
                    offsets.Add((tr, tr.position, tr.position + Vector3.down * Mathf.Abs(i - lastRowToStack)));
                    grid[lastRowToStack, j] = grid[i, j];
                    grid[i, j] = null;
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
