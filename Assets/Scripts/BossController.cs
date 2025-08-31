using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{

    [Header("VFX")]
    public GameObject explosionPrefab; //Prefab da explosão

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

    private float PingPongOffset = 0f;//usado para suavizar mudança de fase
    private float lastMoveSpeed;

    [Header("Ataques")]
    public GameObject bulletPrefab;
    public GameObject specialBulletPrefab; //opcional (míssil/laser)
    public Transform[] firePoints;
    private bool[] canShoot;
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

        canShoot = new bool[firePoints.Length];
        for(int i = 0; i < canShoot.Length; i++) {
            canShoot[i] = (i < 2); //só os dois primeiros atiram no começo
        }
        
        currentHealth = maxHealth;
        lastMoveSpeed = moveSpeed;

        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = maxHealth;
            bossHealthBar.value = maxHealth;
            bossHealthBar.gameObject.SetActive(true);
        }

        if (firePoints.Length >= 4)
        {
            canShoot[2] = false;
            canShoot[3] = false;
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
                                   //inicializa pingPongOffset para não pular ao começaer o movimento lateral
                PingPongOffset = transform.position.x + (moveRange / 2) - Time.time * moveSpeed;
            }
            return; //ainda não entrou, não faz nada além de descer
        }
        //Movimento lateral simples
        float x = Mathf.PingPong(Time.time * moveSpeed + PingPongOffset, moveRange) - (moveRange / 2);
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
                int index = System.Array.IndexOf(firePoints, fp);
                if (fp != null && canShoot[index])
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
        int previousPhase = currentPhase;

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
            // Ativa mais canhões
            canShoot[2] = true;
            canShoot[3] = true;
        }
        else
        {
            currentPhase = 3;
            moveSpeed = 4f;
            fireRate = 1;
            // Ativa mais canhões
            canShoot[2] = true;
            canShoot[3] = true;
        }
        //ajusta o offset para evitar "pulo" ao mudar de fase
        if (currentPhase != previousPhase)
        {
            PingPongOffset += Time.time * (lastMoveSpeed - moveSpeed);
            lastMoveSpeed = moveSpeed;
        }
    }

    void Die()
    {
        Debug.Log("Boss.Die() chamado");
        if (bossHealthBar != null)
            bossHealthBar.gameObject.SetActive(false);

        //Instancia a explosão na posição do boss
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        GameController gc = FindObjectOfType<GameController>();
        if (gc != null)
        {
            Debug.Log("Chamando GameController.Victory()");
            gc.Victory();
        }

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            Debug.Log("Chamando PlayerController.OnVictory()");
            player.OnVictory();
        }

        Destroy(gameObject);
        Debug.Log("Boss destruído");
    }
}
