using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] Image borderImage;
    [SerializeField] Color gameOverBorderColor;
    [SerializeField] UI_MenuAnimationSlide slider;
    [SerializeField] GameObject resumeButtonGO;
    [SerializeField] GameObject pauseHeader, gameOverHeader;
    [SerializeField] HeaderScores headerScores;

    public void OnPause()
    {
        slider.Slide();
    }

    public void OnGameOver(int score)
    {
        headerScores.SetScores(score);
        borderImage.color = gameOverBorderColor;
        resumeButtonGO.SetActive(false);
        pauseHeader.SetActive(false);
        gameOverHeader.SetActive(true);
    }
}
