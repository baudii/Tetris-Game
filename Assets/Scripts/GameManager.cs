using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField] InGameMenu inGameMenu;
    [SerializeField] Options options;

    void Start()
    {
        options.Init();
        Application.targetFrameRate = 60;
    }

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
        AudioCrossFade.Instance.SetExactSourse(1);
    }

    public void ToMenu()
    {
        Pause(false);
        AudioCrossFade.Instance.SetExactSourse(0);
        SceneManager.LoadScene(0);
    }

    public void ShowGameOver(int score)
    {
        Pause(true);
        inGameMenu.gameObject.SetActive(true);
        inGameMenu.OnGameOver(score);
    }

    public void Play(int scene)
    {
        AudioCrossFade.Instance.SetExactSourse(1);
        SceneManager.LoadScene(scene);
    }
}
