using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpammer : EnemyAI
{
    protected float initSpeed;
    protected bool isPaused;
    public float pauseDuration = 1f;
    protected override void Start()
    {
        initSpeed = speed;
        layerMask = LayerMask.GetMask("Default");
        target = GameManager.Instance.player.transform;
    }

    protected override void ShootingBehaviour(Vector3 enemyShootDirection)
    {
        if (canShoot && !isPaused && WithinRange(enemyShootDirection, shootDistance))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, layerMask);
            if (hit.collider != null)
            {
                speed = initSpeed;
                canShoot = false;
                //after finding a wall, jump, pause and find another path
                Jump();
                StartCoroutine(ReShoot(2f));
                return;
            }
            else 
            {
                speed = 3f;
            }
            canShoot = false;
            Shoot();
            StartCoroutine(ReShoot(shootDelay));
            if (shotsFired > Random.Range(2, 7))
            {
                shotsFired = 0;
                StartCoroutine(PauseShooting());
            }
        }
    }

    protected override void FixedUpdate()
    {
        Vector3 enemyShootDirection = target.position - enemyFirePoint.position;    
        
        RotateHead(enemyShootDirection);
        
        ShootingBehaviour(enemyShootDirection);

        Vector3 distanceBetween = target.position - transform.position;
        MovingBehaviour(distanceBetween);
    }

    protected override void MovingBehaviour(Vector3 distanceBetween)
    {
        bool isClose = distanceBetween.sqrMagnitude < minDistanceFromPlayer * minDistanceFromPlayer;
        
        //if the target is nearer than the min distance, the target position to move is set to be the inverse
            //equivalent to running away
        
        // Moves towards the player if within a searchDistance boundary
        if (WithinRange(distanceBetween, searchDistance) || isClose)
        {
            Vector2 targetPos = target.position;
            Vector2 myPos = (Vector2) transform.position;
            if(isClose) 
            {
               targetPos = myPos + new Vector2(-distanceBetween.x * 10 , Mathf.Abs(distanceBetween.y * 10));  
            }
            FlipSprite(targetPos - myPos);
            animator.SetBool("Running", true);
            transform.position = 
                Vector2.MoveTowards
                    (myPos, !jumping? targetPos: 0.2f * targetPos + myPos + Vector2.up * 2, (speed + randomNoise * speed * Random.Range(-0.5f, 0.5f)) * Time.deltaTime);
        }
        else {
            animator.SetBool("Running", false);
        }
    }
    
    IEnumerator PauseShooting()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;
    }
}
