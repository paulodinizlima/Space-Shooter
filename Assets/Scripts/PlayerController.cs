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

	private Rigidbody rb;
	private Vector3 touchStartPos;

	void Start()
	{
		gameController = FindObjectOfType<GameController>();
		rb = GetComponent<Rigidbody>();
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
		}
	}

	void FixedUpdate ()
	{
		Vector3 movement = Vector3.zero;
#if UNITY_STANDALONE || UNITY_EDITOR
		//CONTROLE NO PC
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

#elif UNITY_IOS || UNITY_ANDROID
		//CONTROLE NO CELULAR
		if(Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if(touch.phase == TouchPhase.Moved)
			{
				Vector2 delta = touch.deltaPosition;
				movement = new Vector3(delta.x, 0.0f, delta.y) * 0.05f; //0.05f = sensibilidade
			}
		}
#endif
		
		rb.velocity = movement * speed;		
		rb.position = new Vector3
		(
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
		);
		
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}
}
