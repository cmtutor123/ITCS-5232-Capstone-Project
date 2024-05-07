using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int index;

    public ProjectileShape shape;
    public int bounces, pierce;
    public float duration, sizeX, sizeY, sizeXGrow, sizeYGrow, growDuration, moveSpeed, homingStrength, rotateSpeed, periodLength;
    public bool grow, followPlayer, move, homing, rotate, returning, bouncing, piercing, periodic, chargeDuration;
    public Vector2 initialMoveDirection, moveDirection;
    public AbilityType abilityType;
    public bool chargeActive;
    public bool active;
    public float activeTime;
    public float periodTime;
    public bool isReturning;

    public float chargedUp;

    public Sprite sprite;

    public Rigidbody2D rb;
    public ProjectileCollider projectileCollider;

    public MatchPlayer matchPlayer => GameManager.instance.matchPlayer;


    void FixedUpdate()
    {
        if (active)
        {
            if (rotate)
            {
                transform.Rotate(transform.forward, rotateSpeed * Time.fixedDeltaTime);
            }
            if (homing)
            {
                float closestDistance = float.PositiveInfinity;
                MatchEnemy closestEnemy = null;
                foreach (MatchEnemy enemy in GameManager.instance.EnemiesInRange(transform.position, moveSpeed * (duration - activeTime)))
                {
                    if (enemy != null)
                    {
                        float currentDistance = GameManager.instance.HomingDistance(moveSpeed, homingStrength, transform.up, transform.position, enemy.transform.position);
                        if (currentDistance < moveSpeed * (duration - activeTime) && currentDistance < closestDistance)
                        {
                            closestDistance = currentDistance;
                            closestEnemy = enemy;
                        }
                    }
                }
                if (closestDistance < float.PositiveInfinity && closestEnemy != null)
                {
                    float angle = Vector2.SignedAngle(transform.up, closestEnemy.transform.position - transform.position);
                    float rotation = Mathf.Clamp(Mathf.PI * 2 * homingStrength * Time.fixedDeltaTime, 0, Mathf.Abs(angle)) * (angle < 0 ? 1 : -1);
                    moveDirection = GameManager.instance.RotateVector(moveDirection, rotation).normalized;
                }
            }
            if (grow)
            {
                float xSize = Mathf.Lerp(sizeX, sizeXGrow, Mathf.Clamp(activeTime / growDuration, 0, 1));
                float ySize = Mathf.Lerp(sizeY, sizeYGrow, Mathf.Clamp(activeTime / growDuration, 0, 1));
                transform.localScale = new Vector3(xSize, ySize, transform.localScale.z);
            }
            if (isReturning)
            {
                moveDirection = (transform.position - matchPlayer.transform.position).normalized;
            }
            if (move)
            {
                Vector2 moveVelocity = moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
                rb.velocity = moveVelocity;
            }
            if (followPlayer)
            {
                transform.position = matchPlayer.transform.position - new Vector3(0, 0, -1);
            }
            if (periodic)
            {
                periodTime += Time.fixedDeltaTime;
                if (periodTime > periodLength)
                {
                    periodTime -= periodLength;
                    TriggerPeriodic();
                }
            }
            activeTime += Time.fixedDeltaTime;
            if (activeTime > duration)
            {
                if (returning)
                {
                    if (isReturning)
                    {
                        float playerDistance = (matchPlayer.transform.position - transform.position).magnitude;
                        if (playerDistance < moveSpeed * 0.25f)
                        {
                            TriggerDestroy();
                        }
                    }
                    else
                    {
                        isReturning = true;
                    }
                }
                else if (chargeDuration)
                {
                    if (!matchPlayer.chargedActive)
                    {
                        TriggerDestroy();
                    }
                }
                else
                {
                    TriggerDestroy();
                }
            }
        }
    }

    public void PrepareProjectile(ProjectileShape shape, float duration, float sizeX, float sizeY, float sizeXGrow, float sizeYGrow, float growDuration, float moveSpeed, float homingStrength, float rotateSpeed, float periodLength, int pierce, int bounces, bool followPlayer, bool returning, bool chargeDuration, AbilityType abilityType, Vector2 initialMoveDirection, Sprite sprite, bool chargeActive, int index, float chargedUp = 1)
    {
        this.chargedUp = chargedUp;
        this.shape = shape;
        this.duration = duration;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.sizeXGrow = sizeXGrow;
        this.sizeYGrow = sizeYGrow;
        this.growDuration = growDuration;
        grow = (sizeX != sizeXGrow) || (sizeY != sizeYGrow);
        this.moveSpeed = moveSpeed * 50;
        move = moveSpeed != 0;
        this.homingStrength = homingStrength;
        homing = homingStrength != 0;
        this.rotateSpeed = rotateSpeed;
        rotate = rotateSpeed != 0;
        this.periodLength = periodLength;
        periodic = periodLength != 0;
        this.pierce = pierce;
        piercing = pierce != 0;
        this.bounces = bounces;
        bouncing = bounces != 0;
        this.followPlayer = followPlayer;
        this.returning = returning;
        this.chargeDuration = chargeDuration;
        this.abilityType = abilityType;

        this.initialMoveDirection = initialMoveDirection;
        moveDirection = initialMoveDirection;

        GameObject colliderPrefab = GameManager.instance.GetProjectilePrefab(shape);
        GameObject collider = Instantiate(colliderPrefab, transform);
        rb = collider.GetComponent<Rigidbody2D>();
        projectileCollider = collider.GetComponent<ProjectileCollider>();
        collider.transform.localScale = new Vector3(sizeX / collider.transform.localScale.x, sizeY / collider.transform.localScale.y);
        collider.transform.position = matchPlayer.transform.position + new Vector3(0, 0, -1);
        collider.transform.Rotate(matchPlayer.transform.forward, Vector2.SignedAngle(collider.transform.up, initialMoveDirection));

        projectileCollider.playerProjectile = this;

        this.sprite = sprite;

        this.chargeActive = chargeActive;

        this.index = index;
    }

    public void StartProjectile()
    {
        projectileCollider.GetComponent<SpriteRenderer>().sprite = sprite;
        active = true;
    }

    public void TriggerCollision(MatchEnemy enemy)
    {
        if (enemy == null) return;
        if (periodic) return;
        bool invincibilityFrames = matchPlayer.TriggerHit(enemy, abilityType, index, chargeActive, chargedUp);
        if (invincibilityFrames) return;
        if (piercing)
        {
            if (pierce != int.MaxValue)
            {
                pierce -= 1;
                if (pierce < 0)
                {
                    TriggerDestroy();
                }
            }
        }
        else if (bouncing)
        {
            if (--bounces < 0)
            {
                List<MatchEnemy> enemies = GameManager.instance.EnemiesInRange(transform.position, (duration - activeTime) * moveSpeed);
                if (enemies.Count > 0)
                {
                    MatchEnemy randEnemy = enemies[Random.Range(0, enemies.Count)];
                    if (randEnemy != null)
                    {
                        moveDirection = (randEnemy.transform.position - transform.position).normalized;
                    }
                }
            }
        }
        else
        {
            TriggerDestroy();
        }
    }

    public void TriggerPeriodic()
    {
        List<MatchEnemy> enemies = projectileCollider.EnemiesInRange();
        matchPlayer.TriggerPeriodic(enemies, abilityType, index, chargeActive);
    }

    public void TriggerDestroy()
    {
        active = false;
        matchPlayer.TriggerDestroy(transform.position, transform.up, abilityType, index, chargeActive);
        Destroy(gameObject);
    }
}

public enum ProjectileShape
{
    Rectangle,
    Circle,
    Slash
}