using UnityEngine;

public class Options : MonoBehaviour
{
    const string music_key = "music_sound";
    void Awake()
    {
        
    }

    public void OnMusicValueChanged(float value)
    {
        PlayerPrefs.SetFloat(music_key, value);
    }

    public void OnMusicValueChanged(bool value)
    {
        PlayerPrefs.SetInt(music_key, value ? 1:0);
    }

    public void OnSFXValueChanged(float value)
    {

    }
    public void OnSFXValueChanged(bool value)
    {

    }

    void SetValues(float floatVal, bool boolVal, string key)
    {

    }
}
