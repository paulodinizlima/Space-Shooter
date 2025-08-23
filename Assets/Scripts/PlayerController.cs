using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Boundary
{
	// limites de movimento do player
	public float xMin = -6.0f;
	public float xMax = 6.0f;
	public float zMin = -20.0f;
	public float zMax = 20.0f;
}

public class PlayerController : MonoBehaviour
{
    public float speed;
	public float tilt;
	public Boundary boundary;

	[Header("Disparo")]
	public GameObject shot;
	public Transform shotSpawn1;
	public Transform shotSpawn2;
	public float fireRate;
	public float minFireRate = 0.1f; // taxa mínima de disparo
	 
	private float nextFire;
	private bool useFirstSpawn = true; // alternador

	private GameController gameController;

	void Start()
	{
		gameController = FindObjectOfType<GameController>();
	}
	
	void Update ()
	{
		if (Input.GetButton("Fire1") && Time.time > nextFire)
		{
			if (gameController.UseBattery(gameController.batteryDrainPerShot))
			{
				nextFire = Time.time + fireRate;
				// controla qual spawn usar
				if (useFirstSpawn)
					Instantiate(shot, shotSpawn1.position, shotSpawn1.rotation);
				else
					Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation);

				useFirstSpawn = !useFirstSpawn; // inverte para o próximo disparo

				GetComponent<AudioSource>().Play();
			}
			else
			{
				//sem bateria, não dispara
				Debug.Log("Sem bateria!");
			}
			
		}
	}

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		GetComponent<Rigidbody>().velocity = movement * speed;
		
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);
		
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
}
