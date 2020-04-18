using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using System;
using UnityEngine.PlayerLoop;

public class GameManager : Singleton<GameManager>
{
    public GameObject losePanel, winPanel;
    public Text scriptedText;
    public Text winText;
    public Tilemap tilemap;
    public ColorToPrefab[] colorMappings;
    private int enemiesKilled = 0;
    public GameObject player, powerUp;
    public float speedPerBuff = 2f;
    public float jumpPerBuff = 1f;
    public float shootDelay = 1f;
    public float shootDelayPerBuff = 0.8f;
    public float bulletScalePerBuff = 1.2f, bulletRecoilPerBuff = 1.2f;
    public int bulletDamageIncreasePerBuff = 1, bulletHpIncreasePerBuff = 1;
    public float sizePerEnemy = 1.1f;
    public CinemachineVirtualCamera vcam;
    public float zoomPerEnemy = 1f;
    public float chanceOfPowerUp = 0.2f;
    private HealthSystem healthSystem;
    private PlayerMovement playerMovement;
    private Weapon playerWeapon;

    static bool initialized { get; set; }
    void Start()
    {
        if (initialized)
        {
            player = GameObject.FindWithTag("Player");
            player.GetComponent<Weapon>().Init();
        }
        else
        {
            initialized = true;
            player = Instantiate(player);
        }
        
        player.transform.position = new Vector2(165, 41.6f);
        vcam.m_Follow = player.transform;
        vcam.m_LookAt = player.transform;
        healthSystem = GetComponent<HealthSystem>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerWeapon = player.GetComponent<Weapon>();
    }

    bool frameBuffer = true;
    public void SpawnPowerUp(Transform t)
    {
        
        if (frameBuffer)
            Instantiate(powerUp, t.position, powerUp.transform.rotation);

        StartCoroutine(frame());
    }

    //prevent multiple calls per frame.
    IEnumerator frame()
    {
        yield return null;
        frameBuffer = true;
    }
    
    public void PowerUpPlayer(Buff buff)
    {
        switch (buff) {
            case Buff.SpeedBuff:
            SpeedBuff();
            break;

            case Buff.JumpBuff:
            JumpBuff();
            break;

            case Buff.ShootDelayBuff:
            ShootDelayBuff();
            break;

            case Buff.HealthBuff:
            HealthBuff();
            break;

            case Buff.BulletBuff:
            BulletBuff();
            break;
        }
    }

    private void BulletBuff()
    {
        playerWeapon.BulletBuff(bulletScalePerBuff, bulletRecoilPerBuff, bulletDamageIncreasePerBuff, bulletHpIncreasePerBuff);
    }

    private void HealthBuff()
    {
        healthSystem.gainHp(PowerUp.healthBuffHp);
    }

    private void ShootDelayBuff()
    {
        shootDelay *= shootDelayPerBuff;
    }

    private void JumpBuff()
    {
        playerMovement.increaseJump(jumpPerBuff);
    }

    private void SpeedBuff()
    {
        playerMovement.increaseSpeed(speedPerBuff);
    }

    public void KilledEnemy()
    {
        enemiesKilled++;
        // playerMovement.increaseSpeed(speedPerEnemy);
        // shootDelay *= shootDelayPerBuff;
        // playerMovement.increaseJump(jumpPerBuff);
        // player.transform.localScale *= sizePerEnemy;
        // vcam.m_Lens.OrthographicSize += zoomPerEnemy;
    }

    public void KilledBoss()
    {
        Debug.Log("KilledBoss");
        StartCoroutine(Delay(5f));
        // playerMovement.increaseSpeed(speedPerEnemy);
        // shootDelay *= shootDelayPerBuff;
        // playerMovement.increaseJump(jumpPerBuff);
        // player.transform.localScale *= sizePerEnemy;
        // vcam.m_Lens.OrthographicSize += zoomPerEnemy;
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Win();
    }

    public bool HandleTileCollision(Vector3 pos)
    {
        Vector3Int intPos = tilemap.WorldToCell(pos);        
        Color color = tilemap.GetColor(intPos);
        string name = null;
        
        foreach(ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(color))
			{
                name = colorMapping.tile.name;
                continue;
            }
        }

        switch(name)
        {
            case "DarkOutline":
                tilemap.SetTile(intPos, null);
                break;

            case "FloorBlue":
                //GameOver(1);
                break;

            case "FloorSticky":
                return false;

            default:
                tilemap.SetTile(intPos, null);
                break;
        }
        return true;
    }

    private int currentTime = 10;
    public void Win()
    {
        this.currentTime = 10;
        Time.timeScale = 0;
        winPanel.SetActive(true);
        enabled = false;
        winText.text += "\n" + "You've Killed " + enemiesKilled + " enemies.";
        winText.text += "\n" + "You've completed the level in " + TimeSystem.Instance.textTime.text;
        
        StartCoroutine(UpdatePerSecond(winText.text));
    }

    IEnumerator UpdatePerSecond(string text)
    {
        if (currentTime <= 0)
        {
            winPanel.SetActive(false);
            StartCoroutine(ZoomOut());
        }
        else
        {
            winText.text = text + "\n" + "Continuing in " + currentTime + " seconds";
            yield return new WaitForSecondsRealtime(1.0f);
            currentTime -= 1;
            StartCoroutine(UpdatePerSecond(text));
        }
    }

    IEnumerator ZoomOut()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        vcam.m_Lens.OrthographicSize += vcam.m_Lens.OrthographicSize * 0.01f;
        if (vcam.m_Lens.OrthographicSize > 1500)
        {
            StartCoroutine(StopCongrats());
        }
        else
        {
            StartCoroutine(ZoomOut());
        }
    }

    IEnumerator StopCongrats()
    {
        yield return new WaitForSecondsRealtime(2f);
        DontDestroyOnLoad(player);
        player.transform.position = new Vector2(165, 41.6f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        enabled = true;
    }

    public void damagePlayer(int damage)
    {
        if (healthSystem.loseHp(damage)) {
            GameOver(2);
        }
    }

    public void GameOver(int index)
    {
        Time.timeScale = 0;
        losePanel.SetActive(true);
        enabled = false;
        string loseText = null;
        switch(index)
        {
            case 1:
            loseText = "Looks like something important got destroyed";
            break;
            case 2:
            loseText = "Looks like you got shot dead";
            break;
            default:
            break;
        }
        loseText += "\n" + "You've Killed " + enemiesKilled + " enemies";
        loseText += "\n" + "You've survived for " + TimeSystem.Instance.textTime.text;
        scriptedText.text = loseText; 
        StartCoroutine("StopRestart");
    }

    IEnumerator StopRestart()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        losePanel.SetActive(false);
        Time.timeScale = 1;
        enabled = true;
    }
}
