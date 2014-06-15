using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeaponSet {
	private GameObject unit;
	private List<Weapon> weapons;
	private List<Weapon> guns;
	private List<Weapon> selectableWeapons;
	private Weapon selectedWeapon;

	public WeaponSet (GameObject unit) {
		this.unit = unit;
		weapons = new List<Weapon> ();
		guns = new List<Weapon> ();
		selectableWeapons = weapons.Where (x => x.GetComponent<Weapon> ().requireSelected).ToList();
		if (selectableWeapons.Count > 0) selectedWeapon = selectableWeapons [0];
	}

	public void AddWeapon(GameObject weapon, List<GameObject> firepointSet)
	{
		GameObject newWeapon = (GameObject)GameObject.Instantiate(weapon, unit.transform.position, Quaternion.identity);
		newWeapon.transform.parent = unit.transform;
		Weapon weaponComponent = newWeapon.GetComponent<Weapon> ();
		weaponComponent.SetUnit (unit);
		weaponComponent.SetFirepoints(firepointSet);
		weapons.Add(weaponComponent);
		if (weaponComponent.requireSelected) selectableWeapons.Add (weaponComponent);
		if (weaponComponent.fireType == "Gun") guns.Add (weaponComponent);
	}

	public void FireGuns(GameObject target)
	{
		foreach (Weapon gun in guns) 
		{
			if(!gun.requireSelected || gun == selectedWeapon) 
			{
				gun.FireWeapon(target);
			}
		}
	}

	// TODO
	public void FireWeapon(int? index, GameObject target = null)
	{
		if (index != null) {
				
		}
	}

	public void SelectWeapon(int num)
	{
		// TODO
	}
}
