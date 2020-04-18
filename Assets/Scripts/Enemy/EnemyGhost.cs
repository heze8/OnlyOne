using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhost : EnemyAI
{
    public SpriteRenderer spriteRenderer;
    protected override void ShootingBehaviour(Vector3 enemyShootDirection)
    {
        if (canShoot && WithinRange(enemyShootDirection, shootDistance))
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, layerMask);
            if (hit.collider != null)
            {
                GhostMode(true);
            }
            else {
                GhostMode(false);
                if (canShoot)
                    Shoot();
                canShoot = false;
                StartCoroutine(ReShoot(shootDelay));
            }
        }
    }
    
    bool ghostMode = false;
    void GhostMode(bool b)
    {
        if(!ghostMode && b) {
            ghostMode = true;
            speed *= 2;
            Color temp = spriteRenderer.color;
            temp.a = 0.08f;
            spriteRenderer.color = temp;
        } 
        else if (ghostMode && !b) {
            ghostMode = false;
            canShoot = false;
            speed *= 0.5f;
            Color temp = spriteRenderer.color;
            temp.a = 0.2f;
            spriteRenderer.color = temp;
        }
    }
}
