using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    public GameObject[] planetPrefabs; //lista de planetas diferentes
    public float spawnInterval = 10f; // tempo entre cada planeta
    public float planetSpeed = 0.5f; //velodidade da descida
    public Vector2 spawnXRange = new Vector2(-6f, 6f); //posição X aleatória

    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPlanet();
            timer = 0f;
        }
    }

    void SpawnPlanet()
    {
        //escolhe um planeta aleatório
        GameObject planet = planetPrefabs[Random.Range(0, planetPrefabs.Length)];

        //define posição inicial fora da tela (em cima)
        Vector3 spawnPos = new Vector3(Random.Range(spawnXRange.x, spawnXRange.y), 0f, 15f);

        //instancia o planeta
        GameObject newPlanet = Instantiate(planet, spawnPos, Quaternion.identity);

        //adiciona movimento
        Rigidbody rb = newPlanet.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = newPlanet.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }

        rb.velocity = new Vector3(0, -planetSpeed, 0);

        //destrói após 60s para não acumular
        Destroy(newPlanet, 60f);
    }
}
