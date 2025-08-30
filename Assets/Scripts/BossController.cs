using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 300;
    private int currentHealth;

    [Header("Entrada em Cena")]
    public Vector3 targetPosition = new Vector3(0, 8, 15); //posição final
    public float entrySpeed = 2f;
    private bool hasEntered = false;

    [Header("Movimento durante a luta")]
    public float moveSpeed = 2f;
    public float moveRange = 6f;
    public int rotationMesh = 0;

    [Header("Ataques")]
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab; //opcional (míssil/laser)
    public Transform[] firePoints;
    public float fireRate = 2f;
    private float nextFire;

    [Header("UI")]
    public Slider bossHealthBar;

    private int currentPhase = 1;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.rotation = Quaternion.Euler(0, rotationMesh, 0);
    }

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

        if (firePoints.Length >= 4)
        {
            firePoints[2].gameObject.SetActive(false);
            firePoints[3].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!hasEntered)
        {
            //Boss desce até a posição final
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, entrySpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                hasEntered = true; //começa a lutar
            }
            return; //ainda não entrou, não faz nada além de descer
        }
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
        //Dispara apenas dos firePoints ativos no Hierarchy
        void ShootOnceFromActivePoints()
        {
            foreach (Transform fp in firePoints)
            {
                if (fp != null && fp.gameObject.activeInHierarchy)
                {
                    Instantiate(bulletPrefab, fp.position, fp.rotation);
                }
            }
        }
        switch (currentPhase)
        {
            case 1:
                {
                    ShootOnceFromActivePoints();
                    GetComponent<AudioSource>().Play();
                }
                break;

            case 2:
                {
                    for (int i = 0; i < 2; i++)
                    {
                        ShootOnceFromActivePoints();
                    }
                    GetComponent<AudioSource>().Play();
                }
                break;

            case 3:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        ShootOnceFromActivePoints();
                    }
                    GetComponent<AudioSource>().Play();
                }
                break;
        }
    }
    /*void AttackPattern()
    {
        switch (currentPhase)
        {
            case 1:
                //Tiros simples
                foreach (Transform firePoint in firePoints)
                    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                    GetComponent<AudioSource>().Play();
                break;

            case 2:
                //Rajada tripla
                for (int i = 0; i < 2; i++)
                {
                    foreach (Transform firePoint in firePoints)
                        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                        GetComponent<AudioSource>().Play();
                }
                break;

            case 3:
                //Rajada tripla
                for (int i = 0; i < 3; i++)
                {
                    foreach (Transform firePoint in firePoints)
                        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                        GetComponent<AudioSource>().Play();
                }
                if (specialBulletPrefab != null)
                {
                    //Dispara míssil / laser do centro
                    Instantiate(specialBulletPrefab, transform.position, Quaternion.identity);
                }
                break;
        }
    }*/

    public void TakeDamage(int damage)
    {
        Debug.Log("Tomou dano");
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
            // Apenas 2 canhões ativos
            if (firePoints.Length >= 4)
            {
                firePoints[0].gameObject.SetActive(true);
                firePoints[1].gameObject.SetActive(true);
                firePoints[2].gameObject.SetActive(false);
                firePoints[3].gameObject.SetActive(false);
            }
        }
        else if (hpPercent > 0.2f)
        {
            currentPhase = 2;
            moveSpeed = 3f;
            fireRate = 1.5f;
            // Ativa mais canhões
            if (firePoints.Length >= 4)
            {
                firePoints[0].gameObject.SetActive(true);
                firePoints[1].gameObject.SetActive(true);
                firePoints[2].gameObject.SetActive(true);
                firePoints[3].gameObject.SetActive(true);
            }
        }
        else
        {
            currentPhase = 3;
            moveSpeed = 4f;
            fireRate = 1;
            // Ativa mais canhões
            if (firePoints.Length >= 4)
            {
                firePoints[0].gameObject.SetActive(true);
                firePoints[1].gameObject.SetActive(true);
                firePoints[2].gameObject.SetActive(true);
                firePoints[3].gameObject.SetActive(true);
            }
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
