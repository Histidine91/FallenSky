using UnityEngine;
using System.Collections;

public class Cannon: Gun {
	public float muzzleVelocity = 1000;

	protected override GameObject CreateProjectile(GameObject target) {
		GameObject newProjectile = base.CreateProjectile (target);
		Transform firepoint = firepoints [firepointIndex].transform;
		newProjectile.rigidbody.velocity += (firepoint.forward + Random.insideUnitSphere * spread/360) * muzzleVelocity;
		return newProjectile;
	}
}
