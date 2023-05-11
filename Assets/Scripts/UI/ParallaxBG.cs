using UnityEngine;
using UnityEngine.UI;

public class ParallaxBG : MonoBehaviour
{
    [SerializeField] float xSpeed, ySpeed;
    [SerializeField] RawImage bgRawImage;
    [SerializeField] bool adjustSize;
    float x, y;
    float prefW, prefH;
    void Awake()
    {
        if (adjustSize)
        {
            prefW = Screen.width / 1000f;
            prefH = Screen.height / 1000f;
        }
        else
        {
            prefW = 1;
            prefH = 1;
        }
    }
    void Update()
    {
        if (x > 1) x -= 1;
        if (y > 1) y -= 1;
        x += Time.deltaTime * xSpeed;
        y += Time.deltaTime * ySpeed;
        bgRawImage.uvRect = new Rect(x, y, prefW, prefH);
    }
}
