using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public GUIText altitudeText;
	public GUIText speedText;
	public GUIText throttleText;
	private PlanePhysicsController physicsController;
	private GameObject cameraTether;
	//private Vector3 cameraOffset;
	
	void Start () {
		physicsController = gameObject.GetComponent<PlanePhysicsController> ();
		if (physicsController == null) Debug.LogError ("Missing physics controller for player aircraft " + gameObject.name);
		cameraTether = GameObject.Find ("Camera Tether");
		if (cameraTether == null) Debug.LogError ("Missing camera tether object");
		//cameraOffset = new Vector3 (0, 4, -10);
	}
	
	void Update()
	{
		float dt = Time.deltaTime;
		float pitch = Input.GetAxis ("Pitch");
		float yaw = Input.GetAxis ("Yaw");
		float roll = -Input.GetAxis ("Roll");	// note roll is reversed!
		float throttle = Input.GetAxis ("Throttle") * 0.1f;
		
		if (pitch != 0) physicsController.ModifyPitch (pitch);
		if (yaw != 0) physicsController.ModifyYaw (yaw);
		if (roll != 0) physicsController.ModifyRoll (roll);
		if (throttle != 0) physicsController.ModifyThrottle (throttle);
	}

	void LateUpdate () {
		Vector3 pos = transform.position;
		Vector3 vel = rigidbody.velocity;
		float speed = Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2) + Mathf.Pow(vel.z, 2);
		speed = Mathf.Sqrt (speed)*3.6f;
		altitudeText.text = Mathf.RoundToInt(pos.y).ToString();
		speedText.text = Mathf.RoundToInt(speed).ToString();
		//speedText.text = Mathf.RoundToInt(vel.z).ToString();
		throttleText.text = Mathf.Round(physicsController.GetThrottle()*100) + "%";

		Quaternion rot = transform.rotation;
		cameraTether.transform.position = pos;
		cameraTether.transform.rotation = rot;
	}

	void OnDestroy()
	{
		/*
		GameObject camera = GameObject.Find ("Global Camera");
		if (camera != null) 
		{
			Vector3 pos = transform.position;
			Vector3 vel = rigidbody.velocity;
			Vector3 cameraOffset = new Vector3 (0, 4, -10);
			camera.transform.position = pos + cameraOffset;
			camera.transform.LookAt(pos);
			AudioListener listener = camera.GetComponent<AudioListener>();
		}*/
		altitudeText.text = "";
		speedText.text = "";
		throttleText.text = "";
	}
}
