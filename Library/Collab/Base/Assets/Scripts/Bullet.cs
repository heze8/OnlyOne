using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public GameObject impactEffect;
    public Transform child;

    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        Vector3 hitPosition = Vector3.zero;
        if (collision.gameObject.tag == "Tile")
        {
            foreach (ContactPoint2D hit in collision.contacts)
            {
                hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                hitPosition.y = hit.point.y - 0.01f * hit.normal.y;

                if (GameManager.Instance.HandleTileCollision(hitPosition))
                {
                    Destroy(gameObject);
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
    }

    // void handle(string name)
    // {
    //     if (name == null)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }

    //     switch (name)
    //     {
    //         case "stick":
    //             Stick();
    //             break;
    //     }
    // }

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

    //     }
    //     Instantiate(impactEffect, transform.position, transform.rotation);
    //     // Destroy(hitInfo.gameObject);
    //     Destroy(gameObject);
    // }
}
