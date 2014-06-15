using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
	public GameObject deathExplosion;
	public AudioClip deathSound;
	public float deathSoundVol = 1;
	public float maxHealth = 100;
	public int armor = 100;
	public string unitName;
	public string hudName;
	public string shortName;
	public int side;
	public List<string> categories;

	protected float health;
	protected WeaponSet weaponSet;

	private bool isQuitting = false;

	protected void Start()
	{
		health = maxHealth;
		weaponSet = new WeaponSet(gameObject);
	}

	void OnApplicationQuit () {
		isQuitting = true;
	}

	public void FireWeapon(int? index, GameObject target)
	{
		weaponSet.FireWeapon(index, target);
	}
	
	void OnDestroy() {
		if (isQuitting)	return;
		Vector3 pos = transform.position;
		Instantiate(deathExplosion, pos, Quaternion.identity);
		if(deathSound != null) AudioSource.PlayClipAtPoint (deathSound, pos, deathSoundVol);
	}
}
