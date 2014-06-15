using UnityEngine;
using System.Collections;

public class Gun: Weapon {
	public float heatPerShot = 1;
	public float cooldown = 0.2f;
	public float maxHeat = 100;

	protected float heat;

	protected new void Start()
	{
		base.Start ();
		heat = 0;
		flexAngle = 10;
	}

	public override bool FireWeapon(GameObject target)
	{
		if (heat + heatPerShot > maxHeat) return false;
		bool success = base.FireWeapon(target);
		if (success) heat += heatPerShot;
		return success;
	}

	protected new void Update()
	{
		base.Update ();
		if(heat > 0) heat = Mathf.Max(heat - cooldown * Time.deltaTime, 0);
	}
}
