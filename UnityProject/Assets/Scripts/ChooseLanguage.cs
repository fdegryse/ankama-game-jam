using JetBrains.Annotations;
using Rewired;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseLanguage : MonoBehaviour
{
    public enum Language
    {
        English,
        French
    };

    public static Language currentLanguage;

    public Image en;
    public Image fr;

    public GameObject instructionsEn;
    public GameObject instructionsFr;

    [UsedImplicitly]
    private void Awake()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.French:
                currentLanguage = Language.French;
                break;

            default:
                currentLanguage = Language.English;
                break;
        }

        switch (currentLanguage)
        {
            case Language.English:
                en.color = new Color(1f, 1f, 1f, 1f);
                fr.color = new Color(1f, 1f, 1f, 0.5f);
                instructionsEn.SetActive(true);
                instructionsFr.SetActive(false);
                break;
            case Language.French:
                en.color = new Color(1f, 1f, 1f, 0.5f);
                fr.color = new Color(1f, 1f, 1f, 1f);
                instructionsEn.SetActive(false);
                instructionsFr.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [UsedImplicitly]
    private void Update()
    {
        const float buttonThreshold = 1f / 3f;

        ReInput.PlayerHelper playerHelper = ReInput.players;
        int playerCount = playerHelper.playerCount;

        for (int i = 0; i < playerCount; ++i)
        {
            Player player = playerHelper.GetPlayer(i);

            float hAxis = player.GetAxis("MoveHorizontal");
            if (hAxis < -buttonThreshold)
            {
                currentLanguage = Language.English;
                en.color = new Color(1f, 1f, 1f, 1f);
                fr.color = new Color(1f, 1f, 1f, 0.5f);
                instructionsEn.SetActive(true);
                instructionsFr.SetActive(false);
            }
            else 
            if (hAxis > buttonThreshold)
            {
                currentLanguage = Language.French;
                en.color = new Color(1f, 1f, 1f, 0.5f);
                fr.color = new Color(1f, 1f, 1f, 1f);
                instructionsEn.SetActive(false);
                instructionsFr.SetActive(true);
            }
            else
            if (player.GetButtonDown("ChooseLanguage"))
            {
                switch (currentLanguage)
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
    }
}
