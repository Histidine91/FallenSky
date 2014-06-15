using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class F35B : Plane {

	protected new void Start()
	{
		base.Start();
		List<GameObject> firepoints1 = new List<GameObject> ();
		firepoints1.Add (transform.Find ("Geometry/fuselage/gun").gameObject);
		weaponSet.AddWeapon(Resources.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/GAU-22.prefab"), firepoints1);
	}
}
