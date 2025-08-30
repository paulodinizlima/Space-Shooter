using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    public GameObject explosion;
    public GameObject boltExplosion;
    public GameObject enemyBoltCollision;

    public GameObject[] bonusPrefabs; // Array de prefabs de bônus
    [Range(0f, 1f)]
    public float bonusDropChance = 0.2f; // Chance de drop de bônus (20% por padrão)

    public int scoreValue = 1;
    public int damageValue = 10; // Valor de dano ao jogador
    public int enemyHealth = 3; // Vida do inimigo

    private GameController gameController;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
        if (gameObject.tag != "Asteroid")
        {
            //se não for asteroide, desabilitar drop de bonus
            bonusDropChance = 0f;
            bonusPrefabs = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary") || other.CompareTag("Enemy") || other.CompareTag("Asteroid"))
            return;

        // ignora projéteis inimigos batendo em inimigos
        if ((CompareTag("Enemy") || CompareTag("BossEnemy")) && other.CompareTag("BoltEnemy"))
            return;

        // explosão genérica para inimigo ou asteroide
        if (explosion != null && CompareTag("Asteroid"))
        {
            Instantiate(explosion, transform.position, transform.rotation);
            gameController.AddScore(scoreValue); //score ao destruir o asteroide
            Destroy(gameObject);
        }

        // dano no jogador
        if (other.CompareTag("Player"))
        {
            gameController.TakeDamage(damageValue);

            // explosão específica de BoltEnemy
            if (CompareTag("BoltEnemy") && boltExplosion != null)
            {
                Instantiate(boltExplosion, transform.position, transform.rotation);
            }
        }

        // se for projétil do jogador, sempre destruímos o projétil
        if (other.CompareTag("Bolt"))
        {
            Destroy(other.gameObject);

            //inimigo normal
            if (CompareTag("Enemy"))
            {
                enemyHealth--;
                if (enemyHealth <= 0)
                {
                    if (explosion != null)
                        Instantiate(explosion, transform.position, transform.rotation);

                    gameController.AddScore(scoreValue);
                    Destroy(gameObject);
                }
                else if (enemyBoltCollision != null)
                {
                    Instantiate(enemyBoltCollision, transform.position, transform.rotation);
                }
            }

            // boss -> usa o BossController
            if (CompareTag("BossEnemy"))
            {
                BossController boss = GetComponent<BossController>();
                if (boss != null)
                {
                    boss.TakeDamage(2); //aqui define quanto de dano o tiro do player causa
                }
                if (enemyBoltCollision != null)
                {
                    Instantiate(enemyBoltCollision, transform.position, transform.rotation);
                }
            }

            // drop de bônus só em asteroide
            if (CompareTag("Asteroid") && bonusPrefabs != null && bonusPrefabs.Length > 0 && Random.value < bonusDropChance)
            {
                Instantiate(bonusPrefabs[Random.Range(0, bonusPrefabs.Length)], transform.position, Quaternion.identity);
            }
        }
    }
}
