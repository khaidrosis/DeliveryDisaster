using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int targetScore = 3;
    public int startingAttempts = 3;

    public TMP_Text deliveriesText;
    public TMP_Text attemptsText;
    public TMP_Text statusText;

    private int currentScore = 0;
    private int currentAttempts;

    public bool isPlaying = true;

    void Start()
    {
        currentAttempts = startingAttempts;

        UpdateUI();

        statusText.text = "";
    }

    public void AddScore()
    {
        if (!isPlaying)
            return;

        currentScore++;

        UpdateUI();

        if (currentScore >= targetScore)
        {
            WinGame();
        }
    }

    public void LoseAttempt()
    {
        if (!isPlaying)
            return;

        currentAttempts--;

        UpdateUI();

        if (currentAttempts <= 0)
        {
            LoseGame();
        }
    }

    void UpdateUI()
    {
        deliveriesText.text =
            "Deliveries: " +
            currentScore +
            " / " +
            targetScore;

        attemptsText.text =
             "Attempts: " +
             currentAttempts;
    }

    void WinGame()
    {
        isPlaying = false;

        statusText.text =
             "YOU WIN!";
    }

    void LoseGame()
    {
        isPlaying = false;

        statusText.text =
             "GAME OVER";
    }
}