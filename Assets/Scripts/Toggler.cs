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
        print(gameObject.name + ":" + state);
        onStateChange?.Invoke(state);
    }
}
