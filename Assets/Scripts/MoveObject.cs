using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float speed = 20.0f;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

}
