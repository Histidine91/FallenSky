using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {
	public GameObject deathExplosion;
	public float maxHealth;
	public int armor;
	public string name;
	public string hudName;
	public string shortName;
	public int side;

	private float health;

	private bool isQuitting = false;

	void Start()
	{
		health = maxHealth;
	}

	void OnApplicationQuit () {
		isQuitting = true;
	}
	
	void OnDestroy() {
		if (isQuitting)	return;
		Vector3 pos = transform.position;
		Instantiate(deathExplosion, pos, Quaternion.identity);
	}
}
