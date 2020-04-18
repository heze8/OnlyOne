using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public int hp = 1;
    protected Transform target;
    public Collider2D ignoreCollider1, ignoreCollider2, ignoreCollider3;
    public Transform enemyFirePoint;
    public Transform head;
    public GameObject bulletPrefab, deathEffect;
    public Animator animator;
    protected float rotateSpeed = 30f;
    public float shootDelay;
    public float shootDistance;
    public float minDistanceFromPlayer;
    public float speed;
    public float searchDistance;
    public float randomNoise = 0.5f;
    public float shootSpeed = 16f;
    protected bool canShoot = true;
    private bool faceRight = true;
    public bool isBulletSmall = true;
    public LayerMask layerMask;

    protected virtual void Start()
    {
        layerMask = LayerMask.GetMask("Default");
        target = GameManager.Instance.player.transform;
    }

    public void Randomize()
    {

    }
    
    public bool GetDamaged(int hpLoss)
    {
        hp -= hpLoss;
        if (hp <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    protected virtual void Die()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        GameManager.Instance.KilledEnemy();
        if (Random.Range(0.0f, 1.0f) < GameManager.Instance.chanceOfPowerUp) 
        {          
            GameManager.Instance.SpawnPowerUp(transform);
        }
        Destroy(gameObject);
    }

    //to draw a square in scene view to show distance until AI reacts to player
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchDistance);
    }

    protected virtual void RotateHead(Vector3 enemyShootDirection)
    {
        float enemyAngle = Mathf.Atan2(enemyShootDirection.y, enemyShootDirection.x) * Mathf.Rad2Deg;
        Quaternion enemyRotation = Quaternion.AngleAxis(enemyAngle, Vector3.forward);
        head.rotation = Quaternion.Slerp(head.rotation, enemyRotation, rotateSpeed * Time.deltaTime);
        float angleZ = head.rotation.eulerAngles.z;
        // flips the head for a more natural look.
        if (faceRight && angleZ > 90 && angleZ < 270)
        {
            faceRight = !faceRight;
            head.localScale = new Vector3(head.localScale.x, -head.localScale.y, head.localScale.z);
        }
        if (!faceRight && (angleZ >= 270 || angleZ <= 90))
        {
            faceRight = !faceRight;
            head.localScale = new Vector3(head.localScale.x, -head.localScale.y, head.localScale.z);
        }
    }

    protected virtual void FlipSprite(Vector3 distanceBetween)
    {
        // Flips the main sprite
        // so that the enemy is walking in the direction its facing
        if (distanceBetween.x <= 0.01f)
        {
            transform.eulerAngles = new Vector2(0, 180);
        }
        else if (distanceBetween.x >= 0.01f)
        {
            transform.eulerAngles = new Vector2(0, 0);
        }
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        Vector3 enemyShootDirection = target.position - enemyFirePoint.position;    
        
        RotateHead(enemyShootDirection);
        
        ShootingBehaviour(enemyShootDirection);

        Vector3 distanceBetween = target.position - transform.position;
        FlipSprite(distanceBetween);
        MovingBehaviour(distanceBetween);
    }

    protected virtual void MovingBehaviour(Vector3 distanceBetween)
    {
        // Moves towards the player if within a searchDistance boundary
        if (WithinRange(distanceBetween, searchDistance))
        {
            animator.SetBool("Running", true);
            transform.position = 
                Vector2.MoveTowards
                    (transform.position, !jumping? target.position: 0.2f * target.position + transform.position + Vector3.up * 2, (speed + randomNoise * speed * Random.Range(-0.5f, 0.5f)) * Time.deltaTime);
        }
        else {
            animator.SetBool("Running", false);
        }
    }

    protected bool jumping;
    protected void Jump()
    {
        jumping = true;
        StartCoroutine(StopJump());
    }
    IEnumerator StopJump()
    {
        yield return new WaitForSeconds(1.5f);
        jumping = false;
    }

    protected bool WithinRange(Vector3 distanceBetween, float searchD)
    {
        return distanceBetween.sqrMagnitude < searchD * searchD &&
            distanceBetween.sqrMagnitude > minDistanceFromPlayer * minDistanceFromPlayer;
    }

    protected int shotsFired = 0;
    protected virtual void ShootingBehaviour(Vector3 enemyShootDirection)
    {
        if (canShoot && WithinRange(enemyShootDirection, shootDistance))
        {
            if (shotsFired++ > 3) 
            {
                shotsFired = 0;
                RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, layerMask);
                if (hit.collider != null)
                {
                    shootDelay *= 0.9f;
                    speed *= 1.1f;
                    
                    canShoot = false;
                    //after finding a wall, jump, pause and find another path
                    Jump();
                    StartCoroutine(ReShoot(5f));
                    return;
                }
                else 
                {
                    shootDelay *= 0.75f;
                    speed *= 0.75f;
                }
            }
            
            canShoot = false;
            Shoot();
            StartCoroutine(ReShoot(shootDelay));
        }
    }

    protected virtual void Shoot()
    {
        Vector3 distanceBetween = target.position - transform.position;
        float offset = (-0.2f * distanceBetween.y + distanceBetween.x) * (16 / shootSpeed) * (16/ shootSpeed);
        GameObject bullet = Instantiate(bulletPrefab, enemyFirePoint.position, enemyFirePoint.rotation * Quaternion.Euler(0 , 0, offset + rotateSpeed * randomNoise * Random.Range(-1, 1)));
        Bullet bt = bullet.GetComponent<Bullet>();
        bt.speed = shootSpeed;
        Collider2D bulletCollider = bullet.GetComponentInChildren<Collider2D>();
        Physics2D.IgnoreCollision(bulletCollider, ignoreCollider1);
        Physics2D.IgnoreCollision(bulletCollider, ignoreCollider2);
        Physics2D.IgnoreCollision(bulletCollider, ignoreCollider3);
        bullet.GetComponentInChildren<SpriteRenderer>().color = GetComponentInChildren<SpriteRenderer>().color;
        bt.LaunchSmall(isBulletSmall);
    }

    protected IEnumerator ReShoot(float shootDelay)
    { 
        yield return new WaitForSeconds(shootDelay + randomNoise * Random.Range(-1, 1) * shootDelay);
        canShoot = true;
    }
}
