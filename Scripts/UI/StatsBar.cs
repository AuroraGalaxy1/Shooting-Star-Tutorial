using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [SerializeField] Image fillImageBack;
    [SerializeField] protected Image fillImageFront;
    [SerializeField] bool delayFill = true;
    [SerializeField] float fillDelay = 0.5f;
    [SerializeField] float fillSpeed = 0.1f;
    float currentFillAmout;
    protected float targetFillAmount;
    float previousFillAmount;
    float t;
    Canvas canvas;
    Coroutine bufferedFillingCoroutine;
    WaitForSeconds waitForDelayFill;
    void Awake()
    {
        if (TryGetComponent<Canvas>(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }
        waitForDelayFill = new WaitForSeconds(fillDelay);
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    public virtual void Initialize(float currentValue, float maxValue)
    {
        currentFillAmout = currentValue / maxValue;
        targetFillAmount = currentFillAmout;
        fillImageBack.fillAmount = currentFillAmout;
        fillImageFront.fillAmount = currentFillAmout;
    }
    public void UpdateStats(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;
        if (bufferedFillingCoroutine != null)
        {
            StopCoroutine(bufferedFillingCoroutine);
        }

        if (currentFillAmout > targetFillAmount)
        {
            fillImageFront.fillAmount = targetFillAmount;
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
            return;
        }

        if (currentFillAmout < targetFillAmount)
        {
            fillImageBack.fillAmount = targetFillAmount;
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
    }

    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {
        if (delayFill)
        {
            yield return waitForDelayFill;
        }
        previousFillAmount = currentFillAmout;
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmout = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmout;
            yield return null;
        }

    }
}
