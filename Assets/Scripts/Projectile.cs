using UnityEngine;
using System.Collections;

public class Projectile: MonoBehaviour {
	public Vector3 rotation;
	public float ttl = 1;

	protected GameObject parent;
	protected Weapon weapon;
	protected GameObject target;

	void Start()
	{
		Destroy(gameObject, ttl);
	}

	public void InitializeProjectile(GameObject parent, Weapon weapon, GameObject target)
	{
		this.parent = parent;
		this.weapon = weapon;
		this.target = target;
	}

	public void SetTarget(GameObject target)
	{
		this.target = target;
	}

	public void SetProjectileVelocity(Vector3 velocity)
	{
		rigidbody.velocity = velocity;
	}

	void OnCollisionEnter(Collision collision) {
		GameObject other = collision.gameObject;
		OnTriggerEnter(other.collider);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == parent) return;
		weapon.WeaponHit(gameObject, other);
	}
}
