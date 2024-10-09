using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Options options;
    [SerializeField] InGameMenu inGameMenu;
    [SerializeField] GameObject leftRightControlsTouch;

    const string key = "ControlType";
    public const string TouchControlType = "Touch";
    public const string KeyboardControlType = "Keyboard";

    public static string ControlType;

    [DllImport("__Internal")]
    private static extern bool IsMobile();
    static bool isMobile;
    void Start()
    {
        isMobile = SetIsMobile();
        InitControls();
        Application.targetFrameRate = 144;
        options.Init();
    }

    bool SetIsMobile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return IsMobile();
#endif
        return false;
    }

    void InitControls()
    {
        if (ControlType != TouchControlType || ControlType != KeyboardControlType)
        {
            if (isMobile)
            {
                ControlType = TouchControlType;
            }
            else
            {
                ControlType = KeyboardControlType;
                if (leftRightControlsTouch != null)
                    leftRightControlsTouch.SetActive(false);
            }
        }
        options.SetImage(ControlType);
    }

    public void TryToggleControls()
    {
        if (isMobile)
        {
            options.SetImage(ControlType);
            return;
        }

        if (ControlType == TouchControlType)
        {
            ControlType = KeyboardControlType;
            if (leftRightControlsTouch != null)
                leftRightControlsTouch.SetActive(false);
        }
        else
        {
            ControlType = TouchControlType;

            if (leftRightControlsTouch != null)
                leftRightControlsTouch.SetActive(true);
        }
        options.SetImage(ControlType);
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
