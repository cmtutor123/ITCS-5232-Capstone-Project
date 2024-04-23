using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollider : MonoBehaviour
{
    public PlayerProjectile playerProjectile;

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            playerProjectile.TriggerCollision(other.gameObject.GetComponent<MatchEnemy>());
        }
    }
}
