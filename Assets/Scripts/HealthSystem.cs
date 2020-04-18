using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int startHp = 3;
    private int currentHp;
    private int maxHp = 12;
    public Image[] hearts;

    void Awake() 
    {
        if (hearts.Length > maxHp) 
        {
            throw new System.Exception("hearts more then max hp");
        }
        currentHp = startHp;
        UpdateGUI();
    }

    public bool loseHp(int amt)
    {
        currentHp -= amt;
        UpdateGUI();
        if (currentHp <= 0) {
            return true;
        }
        return false;
    }

    public void gainHp(int amt)
    {
        currentHp += amt;
        UpdateGUI();
    }

    void UpdateGUI()
    {
        if (currentHp <= 0) 
        {
            currentHp = 0;
        }
        if (currentHp > maxHp) 
        {
            currentHp = maxHp;
        }

        for (int i = 0; i < hearts.Length; i++) 
        {
            Image heart = hearts[i];
            if (i < currentHp)
            {
                heart.enabled = true;
            }
            else 
            {
                heart.enabled = false;
            }
        }
    }
}