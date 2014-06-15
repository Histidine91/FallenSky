using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiG21 : Plane {

	protected new void Start()
	{
		base.Start();
		List<GameObject> firepoints1 = new List<GameObject> ();
		firepoints1.Add (transform.Find ("Geometry/hull/gsh-23/barrels/muzzle1").gameObject);
		firepoints1.Add (transform.Find ("Geometry/hull/gsh-23/barrels/muzzle2").gameObject);
		weaponSet.AddWeapon(Resources.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/GSh-23.prefab"), firepoints1);
	}
}
