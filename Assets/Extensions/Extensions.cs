using UnityEngine;

public static class Extensions
{
    public static Vector3 WhereX(this Vector3 vector, float x)
    {
        vector.x = x;
        return vector;
    }
    public static Vector3 WhereY(this Vector3 vector, float y)
    {
        vector.y = y;
        return vector;
    }
    public static Vector3 WhereZ(this Vector3 vector, float z)
    {
        vector.z = z;
        return vector;
    }

    public static Vector3 AddTo(this Vector3 vector, float x = 0, float y = 0, float z = 0)
    {
        vector.x += x;
        vector.y += y;
        vector.z += z;
        return vector;
    }

    public static TextMesh CreateWorldText(string text, Vector3 localPosition, bool is3D = false, int fontSize = 40)
    {
        var localEulerAngles = Vector3.zero;
        if (is3D)
            localEulerAngles.x = 90;
        return CreateWorldText(null, text, localPosition, Vector3.one / 20, localEulerAngles, fontSize, Color.white, TextAnchor.UpperLeft, TextAlignment.Left, 100);
    }
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, Vector3 scale, Vector3 localEulerAngles, int fontSize, Color color, TextAnchor textAnchor, TextAlignment alignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = scale;
        transform.localEulerAngles = localEulerAngles;

        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = alignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        return textMesh;
    }
}
