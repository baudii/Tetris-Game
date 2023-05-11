using UnityEngine;
using UnityEngine.Events;

public class Toggler : MonoBehaviour
{
    [SerializeField] UnityEvent<bool> onStateChange;
    [SerializeField] bool initialState;
    bool state;

    void Awake()
    {
        state = initialState;
    }
    public void Toggle()
    {
        state = !state;
        onStateChange?.Invoke(state);
    }

    public void SetState(bool state)
    {
        this.state = state;
        onStateChange?.Invoke(state);
    }
}
