using UnityEngine;
using UnityEngine.Events;

public class Toggler : MonoBehaviour
{
    [SerializeField] UnityEvent<bool> onStateChange;
    [SerializeField] UnityEvent onStateTrue, onStateFalse;
    [SerializeField] bool initialState;
    bool state;

    void Awake()
    {
        state = initialState;
    }
    public void Toggle()
    {
        SetState(!state);
    }

    public void SetState(bool state)
    {
        this.state = state;
        onStateChange?.Invoke(state);
        if (state)
            onStateTrue?.Invoke();
        else
            onStateFalse?.Invoke();
    }
}
