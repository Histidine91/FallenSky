using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public string name;
	public string shortName;
	public string fireType;	// defines which key is pressed to fire this weapon
	public bool requireSelected = false;
	public float damage = 0;
	public float areaOfEffect = 0;
	public float fireInterval = 1;	// trigger can be pulled no more often than this
	public float preFireDelay = 0;
	public float range = 1000;
	public float spread = 0;

	// the following settings all do something slightly different
	// burst fires in a, well, burst
	public float burst = 1;
	public float burstInterval = 0;
	// this many projectiles are fired per burst	
	public float projectiles = 1;
	// barrels reload independently and will not allow firing if still reloading even if fireInterval is over
	public int numBarrels = 1;
	public float barrelReloadTime = 0;
	public float armorPenetration = 100;

	//FIXME implement icons

	public GameObject[] firepoints;	// this is where the bullets come out from

	protected float fireIntervalRemaining;
	protected float[] barrels;	// [barrelIndex] = reload time remaining
	protected int preferredBarrel;
	
	void Start () {
		fireIntervalRemaining = -0;
		barrels = new float[numBarrels];
		for (int i = 0; i < numBarrels; i++) {
			barrels[i] = 0;		
		}
		preferredBarrel = 0;
	}

	protected void Update () {
		// handle cooldown
		float dt = Time.deltaTime;
		if (fireIntervalRemaining > 0) fireIntervalRemaining = Mathf.Max (fireIntervalRemaining - dt, 0);
		for (int i = 0; i < numBarrels; i++) {
			if(barrels[i] > 0) barrels[i] = Mathf.Max (barrels[i] - dt, 0);
		}
	}

	public bool FireWeapon() {
		if (fireIntervalRemaining > 0) return false;

		int barrel = preferredBarrel;
		if (barrels [barrel] <= 0) {	// our currently selected barrel can fire, use it
			StartCoroutine(Fire(barrel));
			return true;
		} else {
			// first see if the barrels ahead of us can fire
			for (int i = barrel; i < numBarrels; i++) {
				if(barrels[i] <= 0) {
					preferredBarrel = i;
					StartCoroutine(Fire(i));
					return true;
				}
			}
			// now check the ones behind
			for (int i = 0; i < barrel; i++) {
				if(barrels[i] <= 0) {
					preferredBarrel = i;
					StartCoroutine(Fire(i));
					return true;
				}
			}
		}
		return false;	// give up
	}

	// override me!
	protected void CreateProjectile() {
	}

	IEnumerator Fire(int barrel) {
		preferredBarrel = preferredBarrel + 1;
		if(preferredBarrel == numBarrels) preferredBarrel = 0;
		fireIntervalRemaining = fireInterval;

		barrels[barrel] = barrelReloadTime;
		for (int bNum=0; bNum<burst; bNum++) {
			for (int pNum = 0; pNum<projectiles; pNum++) {
				CreateProjectile ();
				yield return new WaitForSeconds (burstInterval);
			}
		}
	}
}
