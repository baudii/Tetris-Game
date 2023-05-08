using UnityEngine;
using UnityEngine.UI;

public class ParallaxBG : MonoBehaviour
{
    [SerializeField] RawImage bgRawImage;
    float x, y;
    float prefW, prefH;
    void Awake()
    {

        prefW = Screen.width / 1000f;
        prefH = Screen.height / 1000f;
    }
    void Update()
    {
        if (x > 1) x -= 1;
        if (y > 1) y -= 1;
        x += Time.deltaTime * 0.03f;
        y += Time.deltaTime * 0.01f;
        bgRawImage.uvRect = new Rect(x, y, prefW, prefH);
    }
}
