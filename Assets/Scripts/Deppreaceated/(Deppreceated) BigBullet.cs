using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BigBullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public GameObject impactEffectTile, impactEffectEnemy;
    public Transform child;
    public int damage = 3;
    public int hp = 2;

    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.tag == "Tile")
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
        else if (collision.gameObject.tag == "BulletTile")
        {
            Stick();
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            if (!collision.gameObject.GetComponent<EnemyAI>().GetDamaged(damage))
                Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Player")
        {
            Instantiate(impactEffectEnemy, transform.position, transform.rotation);
            GameManager.Instance.GameOver(2);
        }
    }

    

    void Stick()
    {
        Destroy(GetComponent<Rigidbody2D>());
        gameObject.tag = "BulletTile";
        child.tag = "BulletTile";
        child.gameObject.layer = 10; //BulletTile layer
    }

    // void OnTriggerEnter2D (Collider2D hitInfo)
    // {
    //     if (hitInfo.tag == "Player") 
    //     {
    //         return;
    //     }
    //     if (hitInfo.tag == "Tile")
    //     {
    //         var tilePos = tilemap.WorldToCell(hitInfo.gameObject.transform.position);
    //         Debug.Log("location:" + tilePos);
    //         // tilemap.SetTile(tilePos, null);
    //         tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, tilePos.z), null);
    //         // Vector3 hit = transform.position;
    //         // Vector3 hitPosition = Vector3.zero;
    //         // hitPosition.x = hit.x - 0.01f * tra

    //     }       Instantiate(impactEffect, transform.position, transform.rotation);
    //     // Destroy(hitInfo.gameObject);
    //     Destroy(gameObject);
    // }
}
