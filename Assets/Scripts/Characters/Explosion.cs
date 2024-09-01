using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float explosionDamage = 100f;
    [SerializeField] Collider2D explosionCollider;
    WaitForSeconds waitExplosionTime = new WaitForSeconds(0.1f);
    void OnEnable()
    {
        StartCoroutine(nameof(ExplosionCoroutine));
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(explosionDamage);
            Debug.Log("111");
        }
    }
    IEnumerator ExplosionCoroutine()
    {
        explosionCollider.enabled = true;
        yield return waitExplosionTime;
        explosionCollider.enabled = false;
    }
}
