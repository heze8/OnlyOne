using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBoss : EnemyAI
{
    protected bool isPaused;
    public float pauseDuration = 1f;
    public float firstSpeed = 5f;
    private BossStates currentState = BossStates.First;
    private bool isTransitioning = false;
    public float transitionDelay = 4f;
    public float jumpHeight = 5f;
    enum BossStates
    {
        First,
        Second,
        Third
    }

    IEnumerator TransitionState(float delay)
    {
        isPaused = true;
        isTransitioning = true;
        yield return new WaitForSeconds(delay);
        hp = 60;
        isPaused = false;
        isTransitioning = false;
    }

    private void Transitioning()
    {
        gameObject.transform.localScale += new Vector3(0.02f, 0.02f, 0.02f);
    }
    
    private void FirstShootingState(Vector3 enemyShootDirection)
    {
        if (canShoot && !isPaused && WithinRange(enemyShootDirection, shootDistance))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, layerMask);
            if (hit.collider != null)
            {
                canShoot = false;
                //after finding a wall, jump, pause and find another path
                Jump();
                StartCoroutine(ReShoot(2f));
                return;
            }
            
            canShoot = false;
            Shoot();
            StartCoroutine(ReShoot(shootDelay));
            if (shotsFired > Random.Range(2, 7))
            {
                shotsFired = 0;
                StartCoroutine(PauseShooting(pauseDuration));
            }
        }
    }

    protected void ShootEnemies()
    {
        Vector3 distanceBetween = target.position - transform.position;
        float offset = (-0.2f * distanceBetween.y + distanceBetween.x) * (16 / shootSpeed) * (16 / shootSpeed);

        var o = EnemyManager.Instance.enemy[Random.Range(0, EnemyManager.Instance.enemy.Length)];
        GameObject enemies = Instantiate(o, enemyFirePoint.position,
            enemyFirePoint.rotation * Quaternion.Euler(0, 0, offset + rotateSpeed * randomNoise * Random.Range(-1, 1)));

        o.GetComponent<Rigidbody2D>().AddForce(distanceBetween);
        Collider2D col = o.GetComponentInChildren<Collider2D>();
        Physics2D.IgnoreCollision(col, ignoreCollider1);
        Physics2D.IgnoreCollision(col, ignoreCollider2);
        Physics2D.IgnoreCollision(col, ignoreCollider3);

    }

    private void SecondShootingState(Vector3 enemyShootDirection)
    {
        if (canShoot && !isPaused && WithinRange(enemyShootDirection, shootDistance))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, layerMask);
            if (hit.collider != null)
            {
                canShoot = false;
                //after finding a wall, jump, pause and find another path
                Jump();
                StartCoroutine(ReShoot(2f));
                return;
            }

            canShoot = false;
            ShootEnemies();
            StartCoroutine(ReShoot(shootDelay/2));
        }
    }

    private void ThirdShootingState(Vector3 enemyShootDirection)
    {
        if (canShoot && !isPaused && WithinRange(enemyShootDirection, shootDistance))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, layerMask);
            canShoot = false;
            randomNoise = 1f;
            Shoot();
            StartCoroutine(ReShoot(shootDelay / 10));
        }
    }
    
    protected override void ShootingBehaviour(Vector3 enemyShootDirection)
    {
        if (isTransitioning)
        {
            Transitioning();
            return;
        }
        
        if (hp < 60 && currentState == BossStates.First)
        {
            StartCoroutine(TransitionState(transitionDelay));
            speed = firstSpeed * 2;
            currentState = BossStates.Second;
        } 
        else if (hp < 30 && currentState == BossStates.Second)
        {
            speed = speed * 2;
            StartCoroutine(TransitionState(transitionDelay));
            currentState = BossStates.Third;
        }
        
        switch (currentState)
        {
            case BossStates.First:
                FirstShootingState(enemyShootDirection);
                break;
            case BossStates.Second:
                SecondShootingState(enemyShootDirection);
                break;
            case BossStates.Third :
                ThirdShootingState(enemyShootDirection);
                break;
        }
    }
    
    protected override void MovingBehaviour(Vector3 distanceBetween)
    {
        if (isTransitioning)
        {
            return;
        }

        switch (currentState)
        {
            case BossStates.First:
                FirstMovingState(distanceBetween);
                break;
            case BossStates.Third:
                ThirdMovingState(distanceBetween);
                break;
        }
        
    }

    protected override void Die()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        GameManager.Instance.KilledBoss();
        for(int i = 0; i < 3; i++)
            GameManager.Instance.SpawnPowerUp(transform);
        Destroy(gameObject);
    }

    private void FirstMovingState(Vector3 distanceBetween)
    {
        bool isClose = distanceBetween.sqrMagnitude < minDistanceFromPlayer * minDistanceFromPlayer;

        //if the target is nearer than the min distance, the target position to move is set to be the inverse
        //equivalent to running away

        // Moves towards the player if within a searchDistance boundary
        if (WithinRange(distanceBetween, searchDistance) || isClose)
        {
            Vector2 targetPos = target.position;
            Vector2 myPos = (Vector2) transform.position;
            if (isClose)
            {
                targetPos = myPos + new Vector2(-distanceBetween.x * 10, Mathf.Abs(distanceBetween.y * 10));
            }

            FlipSprite(targetPos - myPos);
            animator.SetBool("Running", true);
            transform.position =
                Vector2.MoveTowards
                (myPos, !jumping ? targetPos : 0.2f * targetPos + myPos + Vector2.up * jumpHeight,
                    (speed + randomNoise * speed * Random.Range(-0.5f, 0.5f)) * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Running", false);
        }
    }

    private void ThirdMovingState(Vector3 distanceBetween)
    {
        bool isClose = distanceBetween.sqrMagnitude < minDistanceFromPlayer * minDistanceFromPlayer;

        //if the target is nearer than the min distance, the target position to move is set to be the inverse
        //equivalent to running away
        if (Random.value < 0.001f)
        {
            transform.position += Random.Range(-1, 1) * distanceBetween;
            return;
        }
        // Moves towards the player if within a searchDistance boundary
        if (WithinRange(distanceBetween, searchDistance) || isClose)
        {
            Vector2 targetPos = target.position;
            Vector2 myPos = (Vector2) transform.position;
            if (isClose)
            {
                targetPos = myPos + new Vector2(-distanceBetween.x * 10, Mathf.Abs(distanceBetween.y * 10));
            }

            FlipSprite(targetPos - myPos);
            animator.SetBool("Running", true);
            transform.position =
                Vector2.MoveTowards
                (myPos, !jumping ? targetPos : 0.2f * targetPos + myPos + Vector2.up * jumpHeight,
                    (speed + randomNoise * speed * Random.Range(-0.5f, 0.5f)) * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Running", false);
        }
    }
    
    IEnumerator PauseShooting(float pauseDuration)
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }
}
