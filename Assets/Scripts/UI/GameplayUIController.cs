using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [Header("==== Player Input ====")]
    [SerializeField] PlayerInput playerInput;
    [Header("==== Audio Data ====")]
    [SerializeField] AudioData pauseSFX;
    [SerializeField] AudioData unpauseSFX;
    [Header("==== Canvas ====")]
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] Canvas menuCanvas;
    [Header("==== Buttons ====")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;

    int buttonPressedParameterID = Animator.StringToHash("Pressed");

    void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnpause += Unpause;

        ButtonPressedBehavior.buttonFunctionTable.Add(resumeButton.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnMainMenuButtonClick);
    }
    void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnpause -= Unpause;
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }
    void Pause()
    {
        //hUDCanvas.enabled = false;
        menuCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        TimeController.Instance.Pause();
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
        AudioManager.Instance.PlaySFX(pauseSFX);
    }
    void Unpause()
    {
        resumeButton.Select();
        resumeButton.animator.SetTrigger(buttonPressedParameterID);
        AudioManager.Instance.PlaySFX(unpauseSFX);
    }
    void OnResumeButtonClick()
    {
        hUDCanvas.enabled = true;
        menuCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;
        TimeController.Instance.Unpause();
        playerInput.EnableGameplayInput();
        playerInput.SwitchToFixedUpdateMode();
    }
    void OnOptionsButtonClick()
    {
        UIInput.Instance.SelectUI(optionsButton);
        playerInput.EnablePauseMenuInput();
    }
    void OnMainMenuButtonClick()
    {
        menuCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }
}
