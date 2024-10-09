using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class UI_Button : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool disabled;
    [SerializeField] bool staticButton;
    [SerializeField] UnityEvent OnPress;
    [SerializeField] UnityEvent OnRelease;
    [SerializeField] UnityEvent OnHold;
    [SerializeField] UnityEvent OnPonterEntered;
    [SerializeField] UnityEvent OnPonterExited;
    [SerializeField] Image mainImage;
    [SerializeField] Color hoverTint;
    [SerializeField] Color clickTint;

    Vector2 initialPos;
    Color initialColor;
    RectTransform rt;
    bool isDisabled;
    bool pointerWithinButton;
    bool pressed;
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        initialPos = rt.anchoredPosition;
        initialColor = mainImage.color;
        isDisabled = disabled;
    }

    public void DisableButton(bool value)
    {
        isDisabled = value;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDisabled)
            return;
        OnPonterEntered?.Invoke();
        pointerWithinButton = true;
        mainImage.color *= hoverTint;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDisabled)
            return;
        pointerWithinButton = false;
        pressed = false;
        OnPonterExited?.Invoke();
        StopAllCoroutines();
        ResetButtonToInitialState();
        mainImage.color = initialColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDisabled || pressed)
            return;
        if (!staticButton)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0);
        pressed = true;
        mainImage.color *= clickTint;
        OnPress?.Invoke();
        StartCoroutine(Hold());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDisabled || !pointerWithinButton)
            return;
        pressed = false;
        OnRelease?.Invoke();
        ResetButtonToInitialState();
        StopAllCoroutines();
        mainImage.color = initialColor;
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

        if (OnHold == null)
            yield break;

        while (true)
        {
            OnHold.Invoke();
            yield return null;
        }
    }
}
