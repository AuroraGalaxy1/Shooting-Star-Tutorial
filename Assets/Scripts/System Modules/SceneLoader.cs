using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentSsingleton<SceneLoader>
{
    [SerializeField] UnityEngine.UI.Image transitionImage;
    [SerializeField] float fadeTime = 3.5f;

    Color color;

    const string GAMEPLAY = "Gameplay";
    const string MAIN_MENU = "MainMenu";
    const string SCORING = "Scoring";
    void Load(string sceneNanme)
    {
        SceneManager.LoadScene(sceneNanme);
    }

    IEnumerator LoadingCoroutine(string sceneName)
    {
        // Load new scene in background and
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        // Set this scene inactive
        loadingOperation.allowSceneActivation = false;

        // Fade out
        transitionImage.gameObject.SetActive(true);

        while (color.a < 1f)
        {
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);
        // Activate the new scene
        loadingOperation.allowSceneActivation = true;

        // Fade in
        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        transitionImage.gameObject.SetActive(false);
    }

    internal void LoadScoringScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(SCORING));
    }

    public void LoadGamplayScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(GAMEPLAY));
    }
    public void LoadMainMenuScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(MAIN_MENU));
    }
}
