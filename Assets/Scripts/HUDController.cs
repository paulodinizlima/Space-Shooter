using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Vida")]
    public Slider healthBar; //Barra de vida
    public TextMeshProUGUI healthText; //Texto numérico da vida

    [Header("Munição")]
    public TextMeshProUGUI ammoText; //Texto de munição ex.: 15/30

    [Header("Bateria (para laser)")]
    public Slider batteryBar;
    public TextMeshProUGUI batteryText;

    [Header("Pontuação")]
    public TextMeshProUGUI scoreText; //Texto de pontuação

    private int score;
    private int health;
    private int maxHealth;


    // Start is called before the first frame update
    void Start()
    {
        //Valores iniciais
       // maxHealth = 100;
       // health = maxHealth;
       // maxAmmo = 30;
       // ammo = maxAmmo;
       // battery = 100;
       // maxBattery = 1000;
        score = 0;
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        //Vida
        if (healthBar != null)
        {
            healthBar.value = (float)health / maxHealth;
            healthText.text = health + " / " + maxHealth;
        }

        //Pontuação
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("0000");
        }
    }

    //Funções públicas para outros scripts atualizarem o HUD
    public void SetHealth(int value, int max)
    {
        this.maxHealth = max;
        this.health = Mathf.Clamp(value, 0, maxHealth);
        UpdateHUD();
    }

    public void SetBattery(float current, float max)
    {
        if (batteryBar != null)
        {
            batteryBar.value = current / max;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateHUD();
    }
}
