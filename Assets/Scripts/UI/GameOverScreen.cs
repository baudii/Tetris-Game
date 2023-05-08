using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreTextObj;
    [SerializeField] TextMeshProUGUI bestScoreTextObj;
    public void SetScores(int newScore)
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (newScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", newScore);
            bestScoreTextObj.text = newScore.ToString();
        }
        else
        {
            bestScoreTextObj.text = highScore.ToString();
        }
        scoreTextObj.text = newScore.ToString();
    }
}
