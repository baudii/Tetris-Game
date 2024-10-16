using UnityEngine;
using TMPro;

public class ScoringSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI levelText;
    int score = 0, level = 1;
    int totalLines = 0;
    public int AdditionalLines = 0;
    bool shouldSpawnGreyRows;

    void Awake()
    {
        SetTextBoxes();
    }

    public int GetGreyRowsAmount()
    {
        if (shouldSpawnGreyRows)
        {
            shouldSpawnGreyRows = false;
            return AdditionalLines;
        }
        return 0;
    }

    public void OnClearLines(int linesCleared)
    {
        if (linesCleared < 1)
            return;

        SetScore(linesCleared);
        SetTextBoxes();
    }

    void SetScore(int linesCleared)
    {
        var linesMultiplier = 4;
        if (linesCleared == 2)
        {
            linesMultiplier = 10;
        }
        else if (linesCleared == 3)
        {
            linesMultiplier = 30;
        }
        else if (linesCleared == 4)
        {
            linesMultiplier = 120;
        }
        totalLines += linesCleared;

        score += linesMultiplier * (level + 1);

        if (totalLines > 8)
        {
            LevelUp();
            totalLines = 0;
        }
    }

    void SetTextBoxes()
    {
        levelText.text = level.ToString();
        scoreText.text = score.ToString("#,##0");
    }

    void LevelUp()
    {
        level += 1;
        if (level % 5 == 0)
        {
            AudioCrossFade.Instance.Next();
        }
        if ((level-1) % 2 == 0)
        {
            AdditionalLines += 1;
            shouldSpawnGreyRows = true;
        }
    }

    public float GetFallSpeed()
    {
        return Mathf.Pow((float)(0.8f-(level - 1)*0.007), level-1);
    }

    public int GetScore() => score;

    public int GetLevel() => level;
}
