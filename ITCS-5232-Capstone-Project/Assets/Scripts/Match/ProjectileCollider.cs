using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollider : MonoBehaviour
{
    public PlayerProjectile playerProjectile;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            MatchEnemy matchEnemy = other.gameObject.GetComponent<MatchEnemy>();
            if (matchEnemy != null) playerProjectile.TriggerCollision(matchEnemy);
        }
    }

    public List<MatchEnemy> EnemiesInRange()
    {
        List<MatchEnemy> enemies = new List<MatchEnemy>();
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Collider2D[] results = new Collider2D[32];
            int numResults = collider.OverlapCollider(new ContactFilter2D(), results);
            for (int i = 0; i < numResults; i++)
            {
                if (results[i].tag == "Enemy")
                {
                    MatchEnemy enemy = results[i].GetComponent<MatchEnemy>();
                    if (enemy != null)
                    {
                        enemies.Add(enemy);
                    }
                }
            }
        }
        return enemies;
    }
}
