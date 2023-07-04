using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuArea;

    [SerializeField] private GameObject board;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject chooseTeamPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject offerDrawPanel;
    [SerializeField] private GameObject offerDrawResultPanel;
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private Image background;
    [Header("Defeat")] [SerializeField] private GameObject defeatFlag;

    [SerializeField] private Sprite defeatBackground;
    [Header("Win")] [SerializeField] private GameObject winFlag;
    [SerializeField] private Sprite winBackground;

    [Header("Draw")] [SerializeField] private GameObject drawFlag;
    [SerializeField] private Sprite drawBackground;

    [SerializeField] private GameObject lastScreen;

    [SerializeField] private GameObject[] objectsToHideAfterGameEnd;
    [SerializeField] private string sceneName = "SCENE_5_URP_V2";
    [SerializeField] private cgChessBoardScript cgChessBoard;

    private int ing = 1;
    private void Start()
    {
        if (sceneName != "")
        {
            OpenEnvironmentAdditiveScene(sceneName);
        }

        if (board)
            board.SetActive(true);
    }

    public void CloseChooseTeamArea()
    {
        gameOverPanel.SetActive(false);
        chooseTeamPanel.SetActive(false);
        gameUI.SetActive(true);
    }

    public void ChangeScreen(GameObject newScreen)
    {
        if (lastScreen)
            lastScreen.SetActive(false);
        newScreen.SetActive(true);
        lastScreen = newScreen;
    }

    public void OpenCharacterScreen()
    {
        gameUI.SetActive(false);
        characterPanel.SetActive(true);
    }

    public void CloseCharacterScreen()
    {
        gameUI.SetActive(true);
        characterPanel.SetActive(false);

    }

    public void OpenMenuArea()
    {
        GameManager.Pause = true;
        menuArea.SetActive(true);
    }

    public void OpenMultiplayerOfferDrawConcede()
    {
        offerDrawPanel.SetActive(true);
    }

    public void MultiplayerOfferDrawYes()
    {
        cgChessBoard.Multiplayer_OfferDrawShowScreen();
        offerDrawPanel.SetActive(false);
    }

    public void MultiplayerOfferDrawNo()
    {
        cgChessBoard.Multiplayer_OfferDrawRefused();
        offerDrawPanel.SetActive(false);
    }

    public void OpenMultiplayerDrawRefusedScreen()
    {
        offerDrawResultPanel.SetActive(true);
    }

    public void CloseMultiplayerDrawRefusedScreen()
    {
        offerDrawResultPanel.SetActive(false);
    }

    public void MultiplayerOpenDrawScreen()
    {
        Back();
        drawFlag.SetActive(true);
        gameOverPanel.SetActive(true);
        //gameUI.SetActive(false);
        foreach (var objectToHideAfterGameEnd in objectsToHideAfterGameEnd)
        {
            objectToHideAfterGameEnd.SetActive(false);
        }

        background.sprite = drawBackground;
    }

    public void Back()
    {
        GameManager.Pause = false;

        menuArea.SetActive(false);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenEnvironmentScene()
    {
        board.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenEnvironmentAdditiveScene(string sceneName)
    {
        //board.SetActive(false);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void OpenChessScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenDefeatScreenByConcede()
    {
        OpenDefeatScreen();
        if (cgChessBoard.Mode == cgChessBoardScript.BoardMode.Multiplayer)
        {
            cgChessBoard.Multiplayer_OpenVictoryScreen();
        }
    }
    public void OpenDefeatScreen()
    {
        Back();
        defeatFlag.SetActive(true);
        gameOverPanel.SetActive(true);
        //gameUI.SetActive(false);
        foreach (var objectToHideAfterGameEnd in objectsToHideAfterGameEnd)
        {
            objectToHideAfterGameEnd.SetActive(false);
        }

        background.sprite = defeatBackground;

    }

    public void OfferDraw()
    {
        if (cgChessBoard.Mode == cgChessBoardScript.BoardMode.Multiplayer)
        {
            cgChessBoard.Multiplayer_OfferDraw();
        }
        else
        {
            OpenDrawScreen();
        }
    }
    public void OpenDrawScreen()
    {
        Back();
    
            drawFlag.SetActive(true);
            gameOverPanel.SetActive(true);
            //gameUI.SetActive(false);
            foreach (var objectToHideAfterGameEnd in objectsToHideAfterGameEnd)
            {
                objectToHideAfterGameEnd.SetActive(false);
            }

            background.sprite = drawBackground;
        
    }

    public void OpenWinScreen()
    {
        Back();
        winFlag.SetActive(true);
        gameOverPanel.SetActive(true);
        //gameUI.SetActive(false);
        foreach (var objectToHideAfterGameEnd in objectsToHideAfterGameEnd)
        {
            objectToHideAfterGameEnd.SetActive(false);
        }

        background.sprite = winBackground;
    }
}