using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Hazards")]
    public GameObject[] hazards;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;
    public int totalWaves = 5; //número total de waves antes do boss

    [Header("Boss")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    private bool bossSpawned = false;

    [Header("UI")]
    public HUDController hud;
    public Text scoreText;
    public Text restartText;
    public Text gameOverText;

    [Header("Player")]
    public static GameObject SelectedShip;
    public GameObject playerExplosion;
    private GameObject playerInstance;

    [Header("Vida do Player")]
    public int health = 100;
    public int maxHealth = 100;

    [Header("Energia da Nave (Laser)")]
    public float maxBattery = 100.0f;
    public float currentBattery;
    public float batteryDrainPerShot = 5f;
    public float batteryRechargeRate = 20f; //unidades por segundo

    public Image barraDeBateria; // Barra de bateria no HUD

    private bool gameOver;
    private bool restart;
    private int score;

    void Start()
    {
        hud = FindObjectOfType<HUDController>();

        gameOver = false;
        restart = false;
        restartText.text = "";
        gameOverText.text = "";
        score = 0;

        health = maxHealth;
        currentBattery = maxBattery;

        if (SelectedShip != null)
        {
            playerInstance = Instantiate(SelectedShip, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("SelectedShip is not assigned in the GameController.");
        }

        //atualiza o HUD inicial
        hud.SetHealth(health, maxHealth);
        hud.SetBattery(currentBattery, maxBattery);
        hud.AddScore(0);

        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        //lógica de recarga da bateria
        if (!gameOver && currentBattery < maxBattery)
        {
            currentBattery += batteryRechargeRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);
            barraDeBateria.fillAmount = currentBattery / maxBattery;
            hud.SetBattery(currentBattery, maxBattery); //atualiza o HUD
        }

        if (restart && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // método para gastar bateria
    public bool UseBattery(float amount)
    {
        if (currentBattery >= amount)
        {
            currentBattery -= amount;
            hud.SetBattery(currentBattery, maxBattery); //atualiza o HUD
            return true;
        }
        return false; //sem bateria, não dispara
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        for (int wave = 0; wave < totalWaves; wave++)
        {
            Debug.Log("Wave " + wave);
            for (int i = 0; i < hazardCount; i++)
            {
                GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = hazard.transform.rotation;
                Instantiate(hazard, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);

            if (gameOver)
            {
                restartText.text = "Press 'R' to Restart";
                restart = true;
                break;
            }
        }
        //terminou as waves -> chama o Boss
        if (!bossSpawned)
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
            bossSpawned = true;
            Debug.Log("Boss entrou na arena!");
        }
        else
        {
            Debug.Log("BossPrefab ou BossSpawnPoint não foram configurados no Inspector");
        }
    }
    
    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        hud.AddScore(newScoreValue);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        hud.SetHealth(health, maxHealth);
        if (health <= 0)
        {
            Instantiate(playerExplosion, playerInstance.transform.position, playerInstance.transform.rotation);
            Destroy(playerInstance);
            GameOver();
        }
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        gameOver = true;
    }
}