using UnityEngine;
using UnityEngine.UI;

public class ToggleSprite : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] Sprite touchControlsImage;
    [SerializeField] Sprite keyboardControlsImage;
#if !UNITY_WEBGL
    private void Awake()
    {
        gameObject.SetActive(false);
    }
#endif
    public void SetImage(string ControlType)
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }
        if (ControlType == GameManager.TouchControlType)
        {
            img.sprite = touchControlsImage;
        }
        else if (ControlType == GameManager.KeyboardControlType)
        {
            img.sprite = keyboardControlsImage;
        }
        else
        {
            Debug.LogWarning("Control type is not set!");
        }
    }
}
