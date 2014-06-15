using UnityEngine;
using System.Collections;

public class Plane :  Unit {
	public string callsign;
	public AudioClip jetSFX;
	protected AudioSource jetSFXSource;
	protected PlanePhysicsController physicsController;
	
	protected new void Start () {
		base.Start ();
		physicsController = gameObject.GetComponent<PlanePhysicsController> ();
		if (physicsController == null) Debug.LogError ("Missing physics controller for player aircraft " + gameObject.name);
		jetSFXSource = gameObject.AddComponent<AudioSource> ();
		jetSFXSource.clip = jetSFX;
		jetSFXSource.loop = true;
		jetSFXSource.minDistance = 3;
		jetSFXSource.Play ();
		jetSFXSource.rolloffMode = AudioRolloffMode.Linear;
	}

	public void FireGuns(GameObject target)
	{
		weaponSet.FireGuns(target);
	}

	void Update()
	{
		jetSFXSource.volume = physicsController.GetThrottle ()*0.5f;
	}
}
