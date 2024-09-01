using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayProjectile : Projectile
{
    TrailRenderer trail;
    protected virtual void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        //如果子弹不是往右飞，就修正方向
        if (moveDirection != Vector2.right)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirection);
        }
    }
    void OnDisable()
    {
        trail.Clear();    
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PlayerEnergy.Instance.Obtain(PlayerEnergy.PERCENT);
    }
}
