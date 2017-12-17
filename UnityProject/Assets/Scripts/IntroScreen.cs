using JetBrains.Annotations;
using Rewired;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScreen : MonoBehaviour
{
    [UsedImplicitly] public CanvasGroup introCanvasGroup;

    [UsedImplicitly] public CanvasGroup characterSelectCanvasGroup;

    [UsedImplicitly] public float transitionTime;

    [UsedImplicitly] public CanvasGroup player0;
    [UsedImplicitly] public GameObject player0Left;
    [UsedImplicitly] public GameObject player0Middle;
    [UsedImplicitly] public GameObject player0Right;
    [UsedImplicitly] public GameObject player0Instructions;

    [UsedImplicitly] public CanvasGroup player1;
    [UsedImplicitly] public GameObject player1Left;
    [UsedImplicitly] public GameObject player1Middle;
    [UsedImplicitly] public GameObject player1Right;
    [UsedImplicitly] public GameObject player1Instructions;

    [UsedImplicitly] public GameObject pressStartToPlay;

    private bool m_toCharacterSelectScreen;

    private readonly int[] m_controllerPlayerIndices = new int[2];

    [UsedImplicitly]
    public void Awake()
    {
        introCanvasGroup.gameObject.SetActive(true);
        introCanvasGroup.alpha = 1f;

        characterSelectCanvasGroup.gameObject.SetActive(true);
        characterSelectCanvasGroup.alpha = 0f;

        m_controllerPlayerIndices[0] = -1;
        m_controllerPlayerIndices[1] = -1;

        player0.alpha = 0f;
        player1.alpha = 0f;

        player0Left.SetActive(false);
        player0Middle.SetActive(true);
        player0Right.SetActive(false);
        player0Instructions.SetActive(true);

        player1Left.SetActive(false);
        player1Middle.SetActive(true);
        player1Right.SetActive(false);
        player1Instructions.SetActive(true);

        PlayerAssignation.Clear();
    }

    [UsedImplicitly]
    public void Update()
    {
        ReInput.PlayerHelper playerHelper = ReInput.players;
        int playerCount = playerHelper.playerCount;

        if (m_toCharacterSelectScreen)
        {
            const float buttonThreshold = 1f / 3f;

            for (int i = 0; i < playerCount; ++i)
            {
                Player player = playerHelper.GetPlayer(i);
                if (player.GetButtonDown("AddController"))
                {
                    if (m_controllerPlayerIndices[0] != i && m_controllerPlayerIndices[1] != i)
                    {
                        if (m_controllerPlayerIndices[0] < 0)
                        {
                            m_controllerPlayerIndices[0] = i;

                            player0.alpha = 1f;
                            player0Instructions.SetActive(false);
                        }
                        else 
                        if (m_controllerPlayerIndices[1] < 0)
                        {
                            m_controllerPlayerIndices[1] = i;

                            player1.alpha = 1f;
                            player1Instructions.SetActive(false);
                        }
                    }
                }
                else
                if(player.GetButtonDown("RemoveController"))
                {
                    if (m_controllerPlayerIndices[0] == i)
                    {
                        m_controllerPlayerIndices[0] = -1;

                        player0.alpha = 0f;
                        player0Left.SetActive(false);
                        player0Middle.SetActive(true);
                        player0Right.SetActive(false);
                        player0Instructions.SetActive(true);

                        PlayerAssignation.Remove(i);
                    }
                    else
                    if (m_controllerPlayerIndices[1] == i)
                    {
                        m_controllerPlayerIndices[1] = -1;

                        player1.alpha = 0f;
                        player1Left.SetActive(false);
                        player1Middle.SetActive(true);
                        player1Right.SetActive(false);
                        player1Instructions.SetActive(true);

                        PlayerAssignation.Remove(i);
                    }
                }
                else
                if (player.GetButtonDown("StartGame"))
                {
                    if (PlayerAssignation.isValid)
                    {
                        SceneManager.LoadScene("main", LoadSceneMode.Single);
                    }
                }
                else
                {
                    if (m_controllerPlayerIndices[0] == i)
                    {
                        float hAxis = player.GetAxis("MoveHorizontal");
                        if (hAxis < -buttonThreshold)
                        {
                            if (!PlayerAssignation.playerSet[0])
                            {
                                PlayerAssignation.Set(0, i);

                                player0Left.SetActive(true);
                                player0Middle.SetActive(false);
                                player0Right.SetActive(false);
                            }
                            else if (PlayerAssignation.playerControllerIndices[0] != i)
                            {
                                PlayerAssignation.Remove(i);

                                player0Left.SetActive(false);
                                player0Middle.SetActive(true);
                                player0Right.SetActive(false);
                            }
                        }
                        else 
                        if (hAxis > buttonThreshold)
                        {
                            if (!PlayerAssignation.playerSet[1])
                            {
                                PlayerAssignation.Set(1, i);

                                player0Left.SetActive(false);
                                player0Middle.SetActive(false);
                                player0Right.SetActive(true);
                            }
                            else 
                            if (PlayerAssignation.playerControllerIndices[1] != i)
                            {
                                PlayerAssignation.Remove(i);

                                player0Left.SetActive(false);
                                player0Middle.SetActive(true);
                                player0Right.SetActive(false);
                            }
                        }
                    }
                    else 
                    if (m_controllerPlayerIndices[1] == i)
                    {
                        float hAxis = player.GetAxis("MoveHorizontal");
                        if (hAxis < -buttonThreshold)
                        {
                            if (!PlayerAssignation.playerSet[0])
                            {
                                PlayerAssignation.Set(0, i);

                                player1Left.SetActive(true);
                                player1Middle.SetActive(false);
                                player1Right.SetActive(false);
                            }
                            else 
                            if (PlayerAssignation.playerControllerIndices[0] != i)
                            {
                                PlayerAssignation.Remove(i);

                                player1Left.SetActive(false);
                                player1Middle.SetActive(true);
                                player1Right.SetActive(false);
                            }
                        }
                        else
                        if (hAxis > buttonThreshold)
                        {
                            if (!PlayerAssignation.playerSet[1])
                            {
                                PlayerAssignation.Set(1, i);

                                player1Left.SetActive(false);
                                player1Middle.SetActive(false);
                                player1Right.SetActive(true);
                            }
                            else 
                            if (PlayerAssignation.playerControllerIndices[1] != i)
                            {
                                PlayerAssignation.Remove(i);

                                player1Left.SetActive(false);
                                player1Middle.SetActive(true);
                                player1Right.SetActive(false);
                            }
                        }
                    }
                }
            }

            pressStartToPlay.SetActive(PlayerAssignation.isValid);
        }
        else
        {
            for (int i = 0; i < playerCount; ++i)
            {
                Player player = playerHelper.GetPlayer(i);
                if (player.GetButtonDown("StartGame"))
                {
                    m_toCharacterSelectScreen = true;
                    StartCoroutine(GoToCharacterSelectScreen());
                    break;
                }
            }
        }
    }

    private IEnumerator GoToCharacterSelectScreen()
    {
        float transitionTimer = 0f;
        do
        {
            yield return null;

            transitionTimer += Time.deltaTime;
            introCanvasGroup.alpha = 1f - transitionTimer / transitionTime;
        }
        while (transitionTimer < transitionTime);

        transitionTimer = 0f;
        do
        {
            yield return null;

            transitionTimer += Time.deltaTime;
            characterSelectCanvasGroup.alpha = transitionTimer / transitionTime;
        }
        while (transitionTimer < transitionTime);
    }
}