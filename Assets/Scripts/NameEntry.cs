using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NameEntry : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI[] names;
    [SerializeField] public TextMeshProUGUI[] scores;
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
            if (scoreManager.scoreBoard.ContainsKey(nameText.text))
            {
                scoreManager.scoreBoard[nameText.text] = scoreManager.playerScore;
            }
            else
            {
                scoreManager.scoreBoard.Add(nameText.text, scoreManager.playerScore);
            }
            List<(string, int)> playerScores = scoreManager.getSortedPlayers();
            int count = 0;
            for (int i = 0; i < playerScores.Count; i++)
            {
                if (count >= 5)
                {
                    break;
                }
                names[i].text = playerScores[i].Item1;
                scores[i].text = playerScores[i].Item2.ToString();
                count++;
            }
            gameObject.SetActive(false);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void continueGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}