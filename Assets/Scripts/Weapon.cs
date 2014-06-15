using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

	public string weaponName;
	public string shortName;
	public string fireType;	// defines which key is pressed to fire this weapon
	public bool requireSelected = false;
	public float damage = 0;
	public float areaOfEffect = 0;
	public float fireInterval = 1;	// trigger can be pulled no more often than this
	public float preFireDelay = 0;	// TODO
	public float range = 1000;
	public float spread = 0;
	public float armorPenetration = 100;

	public float inheritVelocity = 1;	// 0.5 means half parent velocity is added to projectile velocity

	// the following settings all do something slightly different
	// burst fires in a, well, burst
	public float burst = 1;
	public float burstInterval = 0;
	// this many projectiles are fired per burst	
	public float projectiles = 1;
	// barrels reload independently and will not allow firing if still reloading even if fireInterval is over
	public int numBarrels = 1;
	public float barrelReloadTime = 0;
		
	public Vector3 aimVector = Vector3.forward;
	public float maxAngleDiff = 360;
	public float flexAngle = 0;
	public GameObject projectile;
	public GameObject impactGFX;
	public AudioClip fireSound;
	public float fireSoundVolume = 1;
	//public AudioClip triggerSound;
	public AudioClip hitSound;
	public float hitSoundVolume = 1;

	//FIXME implement GUI icons

	protected List<GameObject> firepoints;	// this is where the bullets come out from
	protected int firepointIndex;

	protected float fireIntervalRemaining;
	protected float[] barrels;	// [barrelIndex] = reload time remaining
	protected int preferredBarrel;

	protected GameObject unit;
	protected GameObject target;

	protected AudioSource fireSoundSource;
	
	protected void Start () {
		fireIntervalRemaining = -0;
		barrels = new float[numBarrels];
		for (int i = 0; i < numBarrels; i++) {
			barrels[i] = 0;		
		}
		preferredBarrel = 0;
		firepointIndex = 0;
		fireSoundSource = gameObject.GetComponent<AudioSource>();
	}

	protected void Update () {
		// handle cooldown
		float dt = Time.deltaTime;
		if (fireIntervalRemaining > 0) fireIntervalRemaining = Mathf.Max (fireIntervalRemaining - dt, 0);
		for (int i = 0; i < numBarrels; i++) {
			if(barrels[i] > 0) barrels[i] = Mathf.Max (barrels[i] - dt, 0);
		}
	}

	public void SetFirepoints(List<GameObject> points)
	{
		firepoints = points;
	}

	public void SetUnit(GameObject unit)
	{
		this.unit = unit;
	}

	public virtual bool FireWeapon(GameObject target) {
		if (fireIntervalRemaining > 0) return false;

		int barrel = preferredBarrel;
		if (barrels [barrel] <= 0) {	// our currently selected barrel can fire, use it
			StartCoroutine(Fire(barrel, target));
			return true;
		} else {
			// first see if the barrels ahead of us can fire
			for (int i = barrel; i < numBarrels; i++) {
				if(barrels[i] <= 0) {
					preferredBarrel = i;
					StartCoroutine(Fire(i, target));
					return true;
				}
			}
			// now check the ones behind
			for (int i = 0; i < barrel; i++) {
				if(barrels[i] <= 0) {
					preferredBarrel = i;
					StartCoroutine(Fire(i, target));
					return true;
				}
			}
		}
		return false;	// give up
	}

	protected virtual GameObject CreateProjectile(GameObject target) {

		Transform firepoint = firepoints [firepointIndex].transform;
		//AudioSource.PlayClipAtPoint(fireSound, firepoint.position, fireSoundVolume);
		if (fireSoundSource != null) fireSoundSource.Play();
		GameObject newProjectile = (GameObject)GameObject.Instantiate (projectile, firepoint.position, firepoint.rotation);
		Projectile projComponent = newProjectile.GetComponent<Projectile> ();
		projComponent.InitializeProjectile (unit, this, target);

		newProjectile.transform.Rotate (projComponent.rotation);
		newProjectile.rigidbody.velocity += unit.rigidbody.velocity * inheritVelocity;

		Physics.IgnoreCollision (newProjectile.collider, unit.collider);

		firepointIndex++;
		if (firepointIndex == firepoints.Count)	firepointIndex = 0;
		return newProjectile;
	}

	protected IEnumerator Fire(int barrel, GameObject target) {
		yield return new WaitForSeconds (preFireDelay);
		preferredBarrel = preferredBarrel + 1;
		if (preferredBarrel == numBarrels) preferredBarrel = 0;
		fireIntervalRemaining = fireInterval;

		barrels[barrel] = barrelReloadTime;
		for (int bNum=0; bNum<burst; bNum++) {
			for (int pNum = 0; pNum<projectiles; pNum++) {
				CreateProjectile (target);
				yield return new WaitForSeconds (burstInterval);
			}
		}
	}

	public void WeaponHit(GameObject projectile, Collider other)
	{
		// TODO apply damage bla bla
		Vector3 pos = projectile.transform.position;
		Instantiate(impactGFX, pos, Quaternion.identity);
		if(hitSound != null) AudioSource.PlayClipAtPoint(hitSound, pos, hitSoundVolume);
		Destroy (projectile);
	}
}
