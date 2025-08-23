using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
	[SerializeField]
	private float lifetime = 15f;

	void Start()
	{
		Destroy(gameObject, lifetime);
	}
}
