using UnityEngine;
using System.Collections;

public class PlaneController :  UnitController {
	public string callsign;
	public AudioClip jetSFX;
	private AudioSource jetSFXSource;
	private PlanePhysicsController physicsController;
	
	void Start () {
		physicsController = gameObject.GetComponent<PlanePhysicsController> ();
		if (physicsController == null) Debug.LogError ("Missing physics controller for player aircraft " + gameObject.name);
		jetSFXSource = gameObject.AddComponent<AudioSource> ();
		jetSFXSource.clip = jetSFX;
		jetSFXSource.loop = true;
		jetSFXSource.Play ();
		jetSFXSource.rolloffMode = AudioRolloffMode.Linear;
	}

	void Update()
	{
		jetSFXSource.volume = physicsController.GetThrottle ()*0.5f;
	}
}
