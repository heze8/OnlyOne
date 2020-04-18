using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public Transform head;
    public GameObject smallBulletPrefab, bigBulletPrefab;
    private float rotateSpeed = 30f;
    private Camera mainCamera;
    public float holdDuration = 3f;
    public float scaleHeadColor;
    private bool faceRight = true;
    private bool canShoot = true;
    private float timer;
    private SpriteRenderer headSpriteRenderer;
    private GameObject bullet;
    public float smallBulletScale = 0.4f, bigBulletScale = 0.75f, smallForceStrength = 50f, bigForceStrength = 150f;
    public int bigBulletDamage = 3, bigBulletHp = 2;
    private Rigidbody2D playerRb2d;

    void Start() 
    {
        
        playerRb2d = GetComponent<Rigidbody2D>();
        this.headSpriteRenderer = head.GetComponent<SpriteRenderer>();
        Init();
    }

    public void Init()
    {
        mainCamera = Camera.main;
    }
    
    public void BulletBuff(float scaleSizeIncrease, float forceSizeIncrease, int damageIncrease, int bulletHpIncrease)
    {
        smallBulletScale *= scaleSizeIncrease;
        bigBulletScale *= scaleSizeIncrease;
        smallForceStrength *= forceSizeIncrease;
        bigForceStrength *= forceSizeIncrease;
        bigBulletDamage += damageIncrease;
        bigBulletHp += bulletHpIncrease;
    }

    void Update()
    {
        RotateHead();
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            bullet = Instantiate(smallBulletPrefab, firePoint.position, firePoint.rotation);
            bullet.transform.localScale = new Vector3(smallBulletScale, smallBulletScale, 1);
            timer = Time.time; 
        }
        else if (Input.GetButton("Fire1") && canShoot)
        {
            if (bullet == null) {
                return;
            }
            //maybe can try setting bullet parent as firepoint
            bullet.transform.SetParent(firePoint);
            bullet.transform.position = firePoint.position;
            float durationPassed = Time.time - timer;
            
            float scale = Mathf.Lerp(smallBulletScale, bigBulletScale, (float) ((durationPassed - 0.1) / (holdDuration - 0.1)));
            bullet.transform.localScale = new Vector3(scale, scale, 1);
            
            float colorScale = Mathf.Lerp(1, scaleHeadColor, (float) ((durationPassed - 0.1) / (holdDuration - 0.1)));
            headSpriteRenderer.color = new Color(1, colorScale, colorScale);
        }
        else if(Input.GetButtonUp("Fire1") && canShoot) 
        {
            if (bullet == null) {
                return;
            }
            float durationPassed = Time.time - timer;
            Bullet b = bullet.GetComponent<Bullet>();

            float t = durationPassed > holdDuration? holdDuration: durationPassed;   
            float scale = t / holdDuration;
            int damage = Mathf.FloorToInt(Mathf.Lerp(1, bigBulletDamage, scale));
            int hp = Mathf.FloorToInt(Mathf.Lerp(1, bigBulletHp, scale));
            
            b.PlayerLaunchWithParam(damage, hp);
            Recoil(scale);
            
            bullet.transform.SetParent(null);
            bullet = null; //reset

            if (GameManager.Instance.shootDelay >= 0.01f)
            {
                canShoot = false;
                StartCoroutine(ReShoot(GameManager.Instance.shootDelay));
            }
            headSpriteRenderer.color = new Color(1, 1, 1);
        }
    }

    private void Recoil(float scale)
    {
        float forceStrength = Mathf.Lerp(smallForceStrength, bigForceStrength, scale);
        Vector2 direction = mainCamera.ScreenToWorldPoint(Input.mousePosition) - head.position;
        playerRb2d.AddForce(forceStrength * new Vector2(-direction.x, -direction.y));
    }

    IEnumerator ReShoot(float shootDelay)
    { 
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    void RotateHead()
    {
        Vector2 direction = mainCamera.ScreenToWorldPoint(Input.mousePosition) - head.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        head.rotation = Quaternion.Slerp(head.rotation, rotation, rotateSpeed * Time.deltaTime);

        float angleZ = head.rotation.eulerAngles.z;
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

    void ShootSmall(bool small)
    {
        if (small) {
            Instantiate(smallBulletPrefab, firePoint.position, firePoint.rotation);
        } 
        else 
        {
            Instantiate(bigBulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
