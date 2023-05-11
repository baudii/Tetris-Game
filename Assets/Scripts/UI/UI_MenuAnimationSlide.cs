using UnityEngine;
using System.Collections;

public class UI_MenuAnimationSlide : MonoBehaviour
{
    [SerializeField] float slideSpeed;
    [SerializeField] RectTransform rt;
    public void Slide()
    {
        rt.anchoredPosition = new Vector2(Screen.width, rt.anchoredPosition.y);
        StartCoroutine(Interpolate());
    }

    IEnumerator Interpolate()
    {
        float t = 0;
        float start = rt.anchoredPosition.x;
        float[] goals = {-100, 50, 0};
        int i = 0;

        while (i < goals.Length)
        {
            rt.anchoredPosition = new Vector2(Mathf.Lerp(start, goals[i], t), rt.anchoredPosition.y);
            yield return null;
            if (t >= 1)
            {
                start = rt.anchoredPosition.x;
                t = 0;
                i++;
            }
            t += Time.unscaledDeltaTime * slideSpeed;
        }
    }
}
