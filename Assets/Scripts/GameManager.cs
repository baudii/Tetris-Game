using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameOverScreen gameOverScreen;
    [SerializeField] AudioMixer mixer;

    (bool,float) isMutedSFX, isMutedMusic;
    public void Pause(bool value)
    {
        if (value)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void Restart()
    {
        Pause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        AudioCrossFade.Instance.SetExactSourse(0);
    }

    public void ToMenu()
    {
        Pause(false);
        SceneManager.LoadScene(0);
    }

    public void ShowGameOver(int score)
    {
        Pause(true);
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.SetScores(score);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("musicVol", -Mathf.Pow(value, 2));
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVol", -Mathf.Pow(value, 2));
    }

    public void Mute(string name)
    {
        if (name != "SFXVol" && name != "musicVol")
            return;

        ref var isMuted = ref IsMuted(name);

        if (isMuted.Item1)
        {
            mixer.SetFloat(name, isMuted.Item2);
            isMuted = (!isMuted.Item1, 0);
        }
        else
        {
            mixer.GetFloat(name, out var val);
            isMuted = (!isMuted.Item1, val);
            mixer.SetFloat(name, -80);
        }
    }

    ref (bool,float) IsMuted(string name)
    {
        if (name == "SFXVol")
            return ref isMutedSFX;
        return ref isMutedMusic;
    }
}
