using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class UI_Button : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool staticButton;
    [SerializeField] UnityEvent OnPress;
    [SerializeField] UnityEvent OnRelease;
    [SerializeField] UnityEvent OnHold;
    [SerializeField] UnityEvent OnPonterExited;
    [SerializeField] Image mainImage;
    [SerializeField] Color hoverTint;
    [SerializeField] Color clickTint;

    Vector2 initialPos;
    Color initialColor;
    RectTransform rt;
    bool isDisabled;
    bool canPress;
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        initialPos = rt.anchoredPosition;
        initialColor = mainImage.color;
    }

    public void DisableButton(bool value)
    {
        isDisabled = value;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDisabled)
            return;
        canPress = true;
        mainImage.color *= hoverTint;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDisabled)
            return;
        canPress = false;

        OnPonterExited?.Invoke();
        StopAllCoroutines();
        ResetButtonToInitialState();
        mainImage.color = initialColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDisabled)
            return;
        if (!staticButton)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
        mainImage.color *= clickTint;
        OnPress?.Invoke();
        StartCoroutine(Hold());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDisabled || !canPress)
            return;
        OnRelease?.Invoke();
        ResetButtonToInitialState();
        StopAllCoroutines();
    }

    void ResetButtonToInitialState()
    {
        if (!staticButton)
            rt.anchoredPosition = initialPos;
        mainImage.color = initialColor;
    }

    IEnumerator Hold()
    {
        yield return null;
        while (true)
        {
            OnHold?.Invoke();
            yield return null;
        }
    }
}
