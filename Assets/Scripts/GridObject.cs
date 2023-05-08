using UnityEngine;
using System.Collections.Generic;

public class GridObject : MonoBehaviour
{
    [SerializeField, Range(0, 100)] int width, height;
    [SerializeField] float cellSize;
    [SerializeField] bool drawGrid;


    [HideInInspector]
    public TextMesh[,] textObjects;
    public static CustomGrid grid;
    void Awake()
    {
        grid = new CustomGrid(width, height, cellSize, transform.position.AddTo(x: -width * cellSize / 2, y: -height * cellSize / 2), this);
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        grid = new CustomGrid(width, height, cellSize, transform.position.AddTo(x: -width * cellSize / 2, y: -height * cellSize / 2), this);
        if (drawGrid)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(grid.GetCellWorldPos(0, height), grid.GetCellWorldPos(width, height));
            Gizmos.DrawLine(grid.GetCellWorldPos(width, height), grid.GetCellWorldPos(width, 0));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pos = grid.GetCellWorldPos(x, y);
                    
                    Gizmos.DrawLine(pos, pos.AddTo(y: cellSize));
                    Gizmos.DrawLine(pos, pos.AddTo(x: cellSize));
                }
            }
        }
    }
#endif

}
