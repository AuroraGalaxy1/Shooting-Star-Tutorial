using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeactiveate : MonoBehaviour
{
    [SerializeField] bool destroyGameObject;
    [SerializeField] float lifetime = 3f;
    WaitForSeconds waitLifetime;
    public GameObject aaa;
    void Awake()
    {
        waitLifetime = new WaitForSeconds(lifetime);
    }
    void OnEnable()
    {
        StartCoroutine(nameof(DeactivateCoroutine));
    }
    IEnumerator DeactivateCoroutine()
    {
        yield return waitLifetime;
        if (destroyGameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            //aaa.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
