using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{
    [SerializeField, Range(0f, 1f)] float bullerTimeScale = 0.1f;
    float defaultFixedDeltaTime;
    float timeScaleBeforePause;
    float t;
    protected override void Awake()
    {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }
    public void Pause()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
    }
    public void Unpause()
    {
        Time.timeScale = timeScaleBeforePause;
    }
    public void BulletTime(float duration)
    {
        Time.timeScale = bullerTimeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }
    public void BulletTime(float inDuration, float outDuration)
    {
        StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
    }
    public void BulletTime(float inDuration, float keepingDuration, float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutDuration(inDuration, keepingDuration, outDuration));
    }
    public void SlowIn(float duration)
    {
        StartCoroutine(SlowInCoroutine(duration));
    }
    public void SlowOut(float duration)
    {
        StartCoroutine(SlowOutCoroutine(duration));
    }
    IEnumerator SlowInKeepAndOutDuration(float inDuration, float keepingDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        yield return new WaitForSecondsRealtime(keepingDuration);
        StartCoroutine(SlowOutCoroutine(outDuration));

    }
    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        StartCoroutine(SlowOutCoroutine(outDuration));

    }
    IEnumerator SlowInCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(1f, bullerTimeScale, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }

            yield return null;
        }
    }
    IEnumerator SlowOutCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(bullerTimeScale, 1f, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }

            yield return null;
        }
    }
}
