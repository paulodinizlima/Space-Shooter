using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class BonusItem : MonoBehaviour
{
    public enum BonusType { ExtraLife, CannonBuster, Shield, Ammo }
    public BonusType bonusType;

    public float fallSpeed = -1.0f; // Velocidade de movimento do bônus
    public int extraLifeAmount = 20; // Quantidade de vida extra
    public float cannonFireRate = 0.3f; // Velocidade do canhão (quanto menor, mais rápido)
    public int shieldAmount = 5; // Quantidade de escudo
    public int ammoAmount = 5; // Quantidade de munição

    public GameObject explosion;

    private GameController gameController;
    private HUDController hudController;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        hudController = FindObjectOfType<HUDController>();
        playerController = FindObjectOfType<PlayerController>();

        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene.");
        }
        if (hudController == null)
        {
            Debug.LogError("HUDController not found in the scene.");
        }
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene.");
        }
        GetComponent<Rigidbody>().velocity = transform.forward * fallSpeed;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameController != null && hudController != null)
        {
            switch (bonusType)
            {
                case BonusType.ExtraLife:
                    gameController.health = Mathf.Min(gameController.health + extraLifeAmount, gameController.maxHealth);
                    hudController.SetHealth(gameController.health, 100);
                    break;
                //case BonusType.Ammo:
                //    playerController.fireRate = Mathf.Min(playerController.fireRate - cannonFireRate, playerController.minFireRate);
                //    break;
            }
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
