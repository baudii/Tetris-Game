using UnityEngine;
public class CustomGrid
{
    public int[,] grid;

    int width;
    int height;
    float cellSize;
    Vector3 originPos;
    GridObject obj;
    public CustomGrid(int width, int height, float cellSize, Vector3 originPos, GridObject obj = null)
    {
        grid = new int[width, height];
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPos = originPos;
        this.obj = obj;
    }
    public Vector3 GetCellWorldPos(int x, int y, bool center = false)
    {
        var worldPos = new Vector3(x, y, 0) * cellSize + originPos;
        if (center) worldPos = worldPos.AddTo(x: cellSize / 2).AddTo(y: cellSize / 2);
        return worldPos;
    }

    public void GetCellByWorldPos(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - originPos).x / cellSize);
        y = Mathf.FloorToInt((worldPos - originPos).y / cellSize);
    }

    public bool SetValue(int value, int x, int y)
    {
        if (!IsValid(x, y))
        {
            return false;
        }

        grid[x, y] = value;

        if (obj.textObjects != null)
            obj.textObjects[x, y].text = value.ToString();
        return true;
    }
    public bool SetValue(int value, Vector3 worldPos)
    {
        int x, y;
        GetCellByWorldPos(worldPos, out x, out y);
        return SetValue(value, x, y);
    }

    public int GetValue(int x, int y)
    {
        if (!IsValid(x, y)) return -1;

        return grid[x, y];
    }
    public int GetValue(Vector3 worldPos)
    {
        int x, y;
        GetCellByWorldPos(worldPos, out x, out y);

        return GetValue(x, y);
    }
    bool IsValid(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
            return false;
        return true;
    }
}
