using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 300;
    private int currentHealth;

    [Header("Movimento")]
    public float moveSpeed = 2f;
    public float moveRange = 6f;

    [Header("Ataques")]
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab; //opcional (míssil/laser)
    public Transform[] firePoints;
    public float fireRate = 2f;
    private float nextFire;

    [Header("UI")]
    public Slider bossHealthBar;

    private int currentPhase = 1;

    // Start is called before the first frame update
    private void Start()
    {
        currentHealth = maxHealth;

        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = maxHealth;
            bossHealthBar.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //Movimento lateral simples
        float x = Mathf.PingPong(Time.time * moveSpeed, moveRange) - (moveRange / 2);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        //Disparo conforme a fase
        if (Time.time > nextFire)
        {
            AttackPattern();
            nextFire = Time.time + fireRate;
        }
    }

    void AttackPattern()
    {
        switch (currentPhase)
        {
            case 1:
                //Tiros simples
                foreach (Transform firePoint in firePoints)
                    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                break;

            case 2:
                //Rajada tripla
                for (int i = 0; i < 3; i++)
                {
                    foreach (Transform firePoint in firePoints)
                        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                }
                break;

            case 3:
                //Rajada tripla
                for (int i = 0; i < 5; i++)
                {
                    foreach (Transform firePoint in firePoints)
                        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                }
                if (specialBulletPrefab != null)
                {
                    //Dispara míssil / laser do centro
                    Instantiate(specialBulletPrefab, transform.position, Quaternion.identity);
                }
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (bossHealthBar != null)
            bossHealthBar.value = currentHealth;

        CheckPhase();

        if (currentHealth <= 0)
            Die();
    }

    void CheckPhase()
    {
        float hpPercent = (float)currentHealth / maxHealth;

        if (hpPercent > 0.5f)
        {
            currentPhase = 1;
            moveSpeed = 2f;
            fireRate = 2f;
        }
        else if (hpPercent > 0.2f)
        {
            currentPhase = 2;
            moveSpeed = 3f;
            fireRate = 1.5f;
        }
        else
        {
            currentPhase = 3;
            moveSpeed = 4f;
            fireRate = 1;
        }
    }

    void Die()
    {
        if (bossHealthBar != null)
            bossHealthBar.gameObject.SetActive(false);

        Destroy(gameObject);
        Debug.Log("Boss derrotado! Fim da fase!");
    }
}
