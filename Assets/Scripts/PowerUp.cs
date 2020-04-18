using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public static int healthBuffHp = 2; 
    public Sprite healthSprite;
    private Buff[] powerUpBuffs = {
        Buff.SpeedBuff, Buff.ShootDelayBuff, Buff.JumpBuff, Buff.HealthBuff, Buff.BulletBuff
    };

    public ParticleSystem levelUpParticles;
    public GameObject levelUpText; 
    public float rndMovementSpeedScale = 100f, jumpBuffScale = 1.5f, speedBuffScale = 0.8f, buffChangeDelay = 2f;
    private Buff currentBuff;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem particles;
    private int buffIndex;
    private Sprite normalSprite;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;
        particles = GetComponent<ParticleSystem>();
        StartCoroutine(ChangeBuff());
    }

    private int RandomExcept (int min, int max, int except ) {
        int randomNr = except;
    
        while ( randomNr == except ) {
            randomNr = UnityEngine.Random.Range(min, max);
        }
        return randomNr;
    }

    IEnumerator ChangeBuff()
    {
        buffIndex = RandomExcept(0, powerUpBuffs.Length, buffIndex);
        currentBuff = powerUpBuffs[buffIndex];
        ChangeProperties(currentBuff);
        yield return new WaitForSeconds(buffChangeDelay);
        StartCoroutine(ChangeBuff());
    }

    private void ChangeProperties(Buff buff)
    {
        ResetProperties();
        switch (buff)
        {
            case Buff.SpeedBuff:
            SpeedProperties();
            break;
            
            case Buff.ShootDelayBuff:
            ShootDelayProperties();
            break;

            case Buff.JumpBuff:
            JumpProperties();
            break;

            case Buff.HealthBuff:
            HealthProperties();
            break;

            case Buff.BulletBuff:
            BulletProperties();
            break;

            default:
            Glitch();
            break;
        }
    }

    private void BulletProperties()
    {
        spriteRenderer.color = Color.gray;
        transform.eulerAngles = new Vector3(30, 70, 0);
        ParticleSystem.EmissionModule em = particles.emission;
        em.rateOverTimeMultiplier = 2000f;
        rb.AddForce(Vector2.up * 50, ForceMode2D.Impulse);
        rb.mass = 4.0f;
    }

    private void Glitch()
    {
    }

    private void HealthProperties()
    {
        spriteRenderer.sprite = healthSprite;
        spriteRenderer.color = Color.red;
        transform.eulerAngles = Vector3.zero;
    }

    private void ResetProperties()
    {
        ParticleSystem.EmissionModule em = particles.emission;
        em.rateOverTimeMultiplier = 100f;
        spriteRenderer.sprite = normalSprite;
        rb.gravityScale = 1f;
        rb.mass = 2f;
        particles.Play();
        var main = particles.main;
        main.startSpeedMultiplier = 1f;
        transform.eulerAngles = new Vector3(0, 0, 90);
        isRandomMovementOn = false;
        rb.transform.localScale =  new Vector3(1, 1, 1);
    }
    private void JumpProperties()
    {
        var main = particles.main;
        main.startSpeedMultiplier = 0.5f;
        spriteRenderer.color = new Color(0.0f, 0.95f, 0.92f);
        transform.eulerAngles = new Vector3(-20, 180, 90);
        rb.gravityScale = 0.2f;
        rb.mass = 5f;
        rb.transform.localScale =  rb.transform.localScale * jumpBuffScale;
    }

    private void ShootDelayProperties()
    {
        particles.Stop();
        spriteRenderer.color = new Color(1.0f, 0.05f, 0.786f);
        transform.eulerAngles = new Vector3(-20, 180, -90);
    }

    private bool isRandomMovementOn = false;
    void SpeedProperties() 
    {
        var main = particles.main;
        main.startSpeedMultiplier = 2f;
        spriteRenderer.color = new Color(0.4f, 1.0f, 0.3f);
        isRandomMovementOn = true;
        rb.transform.localScale =  rb.transform.localScale * speedBuffScale;
        StartCoroutine(randomMovement());
    }

    IEnumerator randomMovement()
    {
        float noise = Mathf.PerlinNoise(transform.position.x, transform.position.y);
        rb.AddForce(rndMovementSpeedScale * noise * new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        if (isRandomMovementOn) {
            StartCoroutine(randomMovement());
        }
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.PowerUpPlayer(currentBuff);
            GameObject.Instantiate(levelUpParticles, collision.transform.position, collision.transform.rotation);
            initLevelUpText(GameObject.Instantiate(levelUpText, 
                collision.transform.position + Vector3.up * 5, levelUpText.transform.rotation));
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            StopAllCoroutines();
            StartCoroutine(ChangeBuff());
        }
    }

    private void initLevelUpText(GameObject textObject)
    {
        TempText tt = textObject.GetComponent<TempText>();
        string s;
        switch (currentBuff)
        {
            case Buff.SpeedBuff:
            s = "Speed ↑";
            break;
            case Buff.JumpBuff:
            s = "Jump ↑";
            break;
            case Buff.ShootDelayBuff:
            s = "Shooting ↑";
            break;
            case Buff.HealthBuff:
            s = "Health ↑";
            break;
            case Buff.BulletBuff:
            s = "Bullet ↑";
            break;
            default:
            s = currentBuff.ToString();
            break;
        }
        tt.Init(s);
    }
}
