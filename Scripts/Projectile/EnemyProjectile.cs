using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    void Awake()
    {
        //如果子弹不是往左飞，就修正方向
        if (moveDirection != Vector2.left)
        {
            transform.rotation = Quaternion.FromToRotation(Vector2.left, moveDirection);
        }
    }


}
