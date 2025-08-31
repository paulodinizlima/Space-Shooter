using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Boundary
{
    public float xMin = -6.0f;
    public float xMax = 6.0f;
    public float zMin = -20.0f;
    public float zMax = 20.0f;
}

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 10f;
    public float tilt = 5f;
    public Boundary boundary;

    [Header("Disparo")]
    public GameObject shot;
    public Transform shotSpawn1;
    public Transform shotSpawn2;
    public float fireRate = 0.25f;

    private float nextFire;
    private bool useFirstSpawn = true;

    private GameController gameController;
    private Rigidbody rb;

    private bool hasControl = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (hasControl)
        {
            HandleShooting();
        }
    }

    void FixedUpdate()
    {
        if (!hasControl) return;

        Vector3 movement = Vector3.zero;

#if UNITY_STANDALONE || UNITY_EDITOR
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector3(moveHorizontal, 0f, moveVertical);

#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                movement = new Vector3(delta.x, 0f, delta.y) * 0.05f; // sensibilidade
            }
        }
#endif

        rb.velocity = movement * speed;

        rb.position = new Vector3(
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            0f,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );

        rb.rotation = Quaternion.Euler(0f, 0f, rb.velocity.x * -tilt);
    }

    private void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            if (gameController.UseBattery(gameController.batteryDrainPerShot))
            {
                nextFire = Time.time + fireRate;

                Transform spawn = useFirstSpawn ? shotSpawn1 : shotSpawn2;
                Instantiate(shot, spawn.position, spawn.rotation);

                useFirstSpawn = !useFirstSpawn;

                GetComponent<AudioSource>()?.Play();
            }
        }
    }

    // chamado pelo GameController quando o player vence
    public void OnVictory()
    {
        hasControl = false;
        rb.velocity = Vector3.zero;
        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        yield return new WaitForSeconds(0.5f); // breve pausa antes de iniciar

        // Move para o centro horizontal
        Vector3 startPos = transform.position;
        Vector3 centerPos = new Vector3(0f, startPos.y, startPos.z);
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, centerPos, elapsed / duration);
            transform.rotation = Quaternion.identity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = centerPos;

        // Move para cima (fora da tela)
        startPos = transform.position;
        Vector3 exitPos = new Vector3(centerPos.x, centerPos.y, 30f);
        elapsed = 0f;
        duration = 2f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, exitPos, elapsed / duration);
            transform.rotation = Quaternion.identity;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = exitPos;
    }
}
