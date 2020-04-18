using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public GameObject impactEffectTile, impactEffectEnemy;
    public Transform child;
    public int damage = 1, bigDamage = 3, hp = 1, bigHp = 2;
    public float bigScale = 0.75f;
    // smallScale = 0.45f,
    private bool activated = false;

    public void LaunchSmall(bool small)
    {
        if (!small)
        {
            damage = bigDamage;
            hp = bigHp;
            rb.transform.localScale = new Vector3(bigScale, bigScale, 1);
        }
        Launch();
    }

    private void Launch()
    {
        if (rb == null)
        {
            Debug.Log(this + " bullet is null error");
        }
        activated = true;
        rb.gravityScale = 1.0f;
        rb.velocity = transform.right * speed;
    }

    public void PlayerLaunchWithParam(int damage, int hp)
    {
        this.damage = damage;
        this.hp = hp;
        Launch();
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.tag == "BulletTile" && activated)
        {
            Stick();
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            if (!collision.gameObject.GetComponent<EnemyAI>().GetDamaged(damage))
            {
                Destroy(gameObject);
            }
            else
            {
                hp--;
                if (hp <= 0) 
                { 
                    Destroy(gameObject);
                } 
            }   
        }
        else if (collision.gameObject.tag == "Player")
        {
            Instantiate(impactEffectEnemy, transform.position, transform.rotation);
            GameManager.Instance.damagePlayer(damage);
            Destroy(gameObject);
        }
    }

    void OnCollisionStay2D (Collision2D collision)
    {
        if (collision.gameObject.tag == "Tile" && activated)
        {
            Vector3 hitPosition = Vector3.zero; 
            foreach (ContactPoint2D hit in collision.contacts)
            {
                hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
                
                if (GameManager.Instance.HandleTileCollision(hitPosition))
                {
                    Instantiate(impactEffectTile, transform.position, transform.rotation);
                    hp--;
                    if (hp <= 0) {
                        Destroy(gameObject);
                    }
                
                } 
                else 
                {
                    Stick();
                }
            }
            
        }
    }

    void Stick()
    {
        Destroy(GetComponent<Rigidbody2D>());
        gameObject.tag = "BulletTile";
        child.tag = "BulletTile";
        child.gameObject.layer = 10; //BulletTile layer
    }
}
