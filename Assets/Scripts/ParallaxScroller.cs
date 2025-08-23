using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer; //objeto da camada
        public float speed; //velocidade do scroll
    }

    public ParallaxLayer[] layers;
    public float journeyLength = 100f; //quanto "dura" a viagem
    private float traveled = 0f;

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        foreach (var l in layers)
        {
            l.layer.position += Vector3.down * l.speed * delta;
        }

        traveled += delta;

        if (traveled >= journeyLength)
        {
            //Chegou ao destino
            Debug.Log("Destino alcançado!");
            //Carregar uma nova scene, mostrar um planeta, etc.
        }
    }
}
