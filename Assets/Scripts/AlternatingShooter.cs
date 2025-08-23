using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatingShooter : MonoBehaviour
{
    public Transform[] shotSpawns;  //Arraste todos os pontos de spawn no Inspector
    public GameObject shot;         // O prefab do tiro
    public float fireRate = 0.25f;  // Taxa de disparo

    private float nextFire;
    private int currentSpawnIndex = 0; // Índice do spawn atual

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            // Instancia o tiro no spawn atual
            Transform spawn = shotSpawns[currentSpawnIndex];
            Instantiate(shot, spawn.position, spawn.rotation);

            // Passa para o próximo spawn (circularmente)
            currentSpawnIndex = (currentSpawnIndex + 1) % shotSpawns.Length; // Alterna entre os spawns
            // Desativa o spawn atual para evitar múltiplos disparos
            for (int i = 0; i < shotSpawns.Length; i++)
            {
                shotSpawns[i].gameObject.SetActive(i == currentSpawnIndex);
            }
            // Reativa o spawn atual para o próximo disparo
            shotSpawns[currentSpawnIndex].gameObject.SetActive(true);

            // Toca o som do tiro
            if (GetComponent<AudioSource>() != null)    
            {
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
