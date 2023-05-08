using UnityEngine;
using UnityEngine.UI;

namespace KK
{
    public class FlexibleGridLayout : LayoutGroup
    {
        enum FitType
        {
            Uniform, Width, Height, FixedRows, FixedColumns
        }

        [SerializeField] int rows, columns;
        [SerializeField] Vector2 cellSize;
        [SerializeField] Vector2 spacing;
        [SerializeField] FitType fitType;
        [SerializeField] bool fitX, fitY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
            {
                float sqrt = Mathf.Sqrt(transform.childCount);
                rows = Mathf.CeilToInt(sqrt);
                columns = Mathf.CeilToInt(sqrt);
            }
            if (fitType == FitType.Width || fitType == FitType.FixedColumns)
            {
                rows = Mathf.CeilToInt(transform.childCount / columns);
            }
            if (fitType == FitType.Height || fitType == FitType.FixedRows)
            {
                columns = Mathf.CeilToInt(transform.childCount / rows);
            }

            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float cellWidth = (parentWidth / columns) - ((spacing.x / columns) * (columns - 1)) - (padding.left / columns) - (padding.right / columns);
            //(parentWidth - (spacing.x * (columns - 1)) - padding.left - padding.right) / columns;
            //(parentWidth / columns) - ((spacing.x / columns) * (columns - 1)) - (padding.left / columns) - (padding.right / columns);
            float cellHeight = (parentHeight / rows) - ((spacing.y / rows) * (rows - 1)) - (padding.top / rows) - (padding.bottom / rows);
            //(parentHeight - (spacing.y * (columns -1) - padding.top - padding.bottom)) / rows;
            //(parentHeight / rows) - ((spacing.y / rows) * (columns - 1)) - (padding.top / rows) - (padding.bottom / rows);

            if (fitX) cellSize.x = cellWidth;
            if (fitY) cellSize.y = cellHeight;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                var rowCount = i / columns;
                var columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);

            }
        }
        
        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
            
        }

        public override void SetLayoutVertical()
        {

        }

    }
}
