using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUIController : MonoBehaviour
{
    [Header("==== Background ====")]
    [SerializeField] Image background;
    [SerializeField] Sprite[] backgroundImages;
    [Header("==== Scoring Screen ====")]
    [SerializeField] Canvas scoringScreenCanvas;
    [SerializeField] Text playerScoreText;
    [SerializeField] Button buttonMainMenu;
    [SerializeField] Transform highScoreLeaderboardContainer;

    void Start()
    {
        ShowRandomBackground();
        ShowScoringScreen();
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonMainMenu.gameObject.name, OnButtonMainMenuClicked);
        GameManager.GameState = GameState.Scoring;
    }
    void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }
    void ShowRandomBackground()
    {
        background.sprite = backgroundImages[Random.Range(0, backgroundImages.Length)];
    }
    void ShowScoringScreen()
    {
        scoringScreenCanvas.enabled = true;
        playerScoreText.text = ScoreManager.Instance.Score.ToString();
        UIInput.Instance.SelectUI(buttonMainMenu);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        UpdateHighScoreLeaderboard();
    }
    void UpdateHighScoreLeaderboard()
    {
        var playerScoreList = ScoreManager.Instance.LoadPlayerScoreData().list;
        for (int i = 0; i < highScoreLeaderboardContainer.childCount; i++)
        {
            var child = highScoreLeaderboardContainer.GetChild(i);
            child.Find("Rank").GetComponent<Text>().text = (i + 1).ToString();
            child.Find("Score").GetComponent<Text>().text = playerScoreList[i].score.ToString();
            child.Find("Name").GetComponent<Text>().text = playerScoreList[i].playerName;
        }
    }
    void OnButtonMainMenuClicked()
    {
        scoringScreenCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }
}
