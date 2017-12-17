using JetBrains.Annotations;
using Rewired;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    [UsedImplicitly] public int targetScore = 10;

    [UsedImplicitly] public GameObject[] playerCrowns;
    [UsedImplicitly] public Text[] playerScores;

    [UsedImplicitly] public GameObject pressStartEn;
    [UsedImplicitly] public GameObject pressStartFr;

    private int m_winningPlayer = -1;

    [UsedImplicitly]
    private void Awake()
    {
        gameObject.SetActive(false);

        playerScores[0].text = "0";
        playerScores[1].text = "0";

        playerCrowns[0].SetActive(false);
        playerCrowns[1].SetActive(false);

        switch (ChoosenLanguage.currentLanguage)
        {
            case Language.English:
                pressStartEn.SetActive(true);
                pressStartFr.SetActive(false);
                break;
            case Language.French:
                pressStartEn.SetActive(false);
                pressStartFr.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [UsedImplicitly]
    private void Update()
    {
        if (m_winningPlayer < 0)
        {
            return;
        }
        
        int winningPlayerControllerIndex = PlayerAssignation.playerControllerIndices[m_winningPlayer];
        Player player = ReInput.players.GetPlayer(winningPlayerControllerIndex);
        if (player.GetButtonDown("Start"))
        {
            switch (ChoosenLanguage.currentLanguage)
            {
                case Language.English:
                    SceneManager.LoadScene("intro_en", LoadSceneMode.Single);
                    break;
                case Language.French:
                    SceneManager.LoadScene("intro_fr", LoadSceneMode.Single);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void SetPlayerScore(int playerId, int score)
    {
        if (m_winningPlayer >= 0)
        {
            return;
        }

        playerScores[playerId].text = score.ToString();

        if (score == targetScore)
        {
            m_winningPlayer = playerId;

            playerCrowns[playerId].SetActive(true);
            gameObject.SetActive(true);
        }
    }
}
