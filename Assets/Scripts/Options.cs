using UnityEngine;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    [SerializeField] TogglerData[] togglers;
    [SerializeField] AudioMixer mixer;

    const string musicVolKey = "musicVol";
    const string sfxVolKey = "SFXVol";
    
    public void Init()
    {
        var musicVolume = PlayerPrefs.GetFloat(musicVolKey, 1);
        var sfxVolume = PlayerPrefs.GetFloat(sfxVolKey, 1);
        var musicMuteState = PlayerPrefs.GetInt(musicVolKey, 1);
        var sfxMuteState = PlayerPrefs.GetInt(sfxVolKey, 1);

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        if (musicMuteState == 0)
            Mute(musicVolKey);
        if (sfxMuteState == 0)
            Mute(sfxVolKey);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(musicVolKey, value);
        mixer.SetFloat(musicVolKey, Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(sfxVolKey, value);
        mixer.SetFloat(sfxVolKey, Mathf.Log10(value) * 20);
    }

    public void ToggleMuteMusic() => ToggleMute(musicVolKey);
    public void ToggleMuteSFX() => ToggleMute(sfxVolKey);

    void ToggleMute(string key)
    {
        if (key != sfxVolKey && key != musicVolKey)
            return;

        int state = PlayerPrefs.GetInt(key, 1);

        if (state == 1)
            Mute(key);
        else
            UnMute(key);
    }

    void Mute(string key)
    {
        foreach (var toggler in togglers)
        {
            if (toggler.key == key)
            {
                toggler.cross.SetState(true);
                toggler.slider.SetState(false);
            }
        }
        mixer.GetFloat(key, out var volume);
        PlayerPrefs.SetFloat(key, volume);
        PlayerPrefs.SetInt(key, 0);
        mixer.SetFloat(key, -80);
    }

    void UnMute(string key)
    {
        foreach (var toggler in togglers)
        {
            if (toggler.key == key)
            {
                toggler.cross.SetState(false);
                toggler.slider.SetState(true);
            }
        }
        float volume = PlayerPrefs.GetFloat(key);
        PlayerPrefs.SetInt(key, 1);
        mixer.SetFloat(key, volume);
    }

    [System.Serializable]
    class TogglerData
    {
        public Toggler cross, slider;
        public string key;
    }
}
