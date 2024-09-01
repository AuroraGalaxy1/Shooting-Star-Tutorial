using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header("==== Canvas ====")]
    [SerializeField] Canvas mainMenuCanvas;
    [Header("==== Buttons ====")]
    [SerializeField] Button buttonStart;
    [SerializeField] Button buttonOptions;
    [SerializeField] Button buttonQuit;
    [SerializeField] GameObject Option;
    [SerializeField] GameObject player;
    void OnEnable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonStart.gameObject.name, OnStartGameButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonOptions.gameObject.name, OnButtonOptionClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonQuit.gameObject.name, OnButtonQuitClicked);
    }
    void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }
    void Start()
    {
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Playing;
        UIInput.Instance.SelectUI(buttonStart);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Option.activeSelf)
            {
                Option.SetActive(!Option.activeSelf);
                player.SetActive(true);
                mainMenuCanvas.enabled = false;
                SceneLoader.Instance.LoadGamplayScene();
            }
        }
    }

    void OnButtonOptionClicked()
    {

        UIInput.Instance.SelectUI(buttonOptions);
        player.SetActive(false);
        mainMenuCanvas.enabled = false;
        Option.SetActive(true);
        //buttonReturn.gameObject.SetActive(true);
    }


    void OnStartGameButtonClick()
    {
        //mainMenuCanvas.enabled = false;
        //SceneLoader.Instance.LoadGamplayScene();
        player.SetActive(false);
        mainMenuCanvas.enabled = false;
        Option.SetActive(true);
    }
    // void OnButtonOptionsClicked()
    // {
    //     UIInput.Instance.SelectUI(buttonOptions);
    //     Option.SetActive(true);
    // }
    void OnButtonQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
