using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [System.Serializable]
    public class PlarallaxLayer
    {
        public Transform layer;
        public float speed;
    }
    public PlarallaxLayer[] layers;

    public float stageLength = 60f; //duração da viagem em segundos
    private float timer = 0f;
    private bool bossStarted = false;

    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        //mover fundos enquanto ainda não chegou no boss
        if (!bossStarted)
        {
            foreach (var l in layers)
            {
                l.layer.position += Vector3.forward * l.speed * delta;
            }

            timer += delta;
            if (timer >= stageLength)
            {
                StartBossFight();
            }
        }
    }

    void StartBossFight()
    {
        bossStarted = true;
        Debug.Log("Boss time!");

        //spawnar o boss
        Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        //opcional: reduzir velocidade dos fundos ou trocar cenário
        foreach (var l in layers)
        {
            l.speed *= 0.3f; //deixa quase parado
        }
    }
}
