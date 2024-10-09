using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] TogglerData[] togglers;
    [SerializeField] AudioMixer mixer;
    [SerializeField] ToggleSprite controlsButton;

    const string musicVolKey = "musicVol";
    const string musicMuteKey = "musicMute";
    const string sfxVolKey = "SFXVol";
    const string sfxMuteKey = "SFXMute";

    public void Init()
    {
        var musicVolume = PlayerPrefs.GetFloat(musicVolKey, 0.3f);
        var sfxVolume = PlayerPrefs.GetFloat(sfxVolKey, 0.3f);
        var musicMuteState = PlayerPrefs.GetInt(musicMuteKey, 1);
        var sfxMuteState = PlayerPrefs.GetInt(sfxMuteKey, 1);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        
        foreach (var toggler in togglers)
        {
            if (toggler.key == musicVolKey)
            {
                var s = toggler.slider.GetComponent<Slider>();
                s.value = musicVolume;
            }
            else
            {
                var s = toggler.slider.GetComponent<Slider>();
                s.value = sfxVolume;
            }
        }
        if (musicMuteState == 0)
            Mute(musicVolKey, musicMuteKey);
        if (sfxMuteState == 0)
            Mute(sfxVolKey, sfxMuteKey);

        controlsButton.SetImage(GameManager.ControlType);
    }


    public void SetImage(string currentControls)
    {
        controlsButton.SetImage(currentControls);
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

    public void ToggleMuteMusic() => ToggleMute(musicVolKey, musicMuteKey);
    public void ToggleMuteSFX() => ToggleMute(sfxVolKey, sfxMuteKey);

    void ToggleMute(string volKey, string muteKey)
    {
        if (volKey != sfxVolKey && volKey != musicVolKey)
            return;

        int state = PlayerPrefs.GetInt(muteKey, 1);

        if (state == 1)
            Mute(volKey, muteKey);
        else
            UnMute(volKey, muteKey);
    }

    void Mute(string volKey, string muteKey)
    {
        foreach (var toggler in togglers)
        {
            if (toggler.key == volKey)
            {
                toggler.cross.SetState(true);
                toggler.slider.SetState(false);
            }
        }
        mixer.GetFloat(volKey, out var volume);
        PlayerPrefs.SetFloat(volKey, Mathf.Pow(10, volume / 20));
        PlayerPrefs.SetInt(muteKey, 0);
        mixer.SetFloat(volKey, -80);
    }

    void UnMute(string volKey, string muteKey)
    {
        print(volKey);
        print(muteKey);
        foreach (var toggler in togglers)
        {
            if (toggler.key == volKey)
            {
                toggler.cross.SetState(false);
                toggler.slider.SetState(true);
            }
        }
        float volume = PlayerPrefs.GetFloat(volKey, 0.243f);
        print(volume);
        PlayerPrefs.SetInt(muteKey, 1);
        mixer.SetFloat(volKey, Mathf.Log10(volume) * 20);
    }

    [System.Serializable]
    class TogglerData
    {
        public Toggler cross, slider;
        public string key;
    }
}
