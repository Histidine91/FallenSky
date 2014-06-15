using UnityEngine;
using System.Collections;

public class PlanePhysicsController : MonoBehaviour {
	public float maxThrust;
	//public float lift;	// autoset
	public float initialVelocity;
	public float pitchRate;
	public float yawRate;
	public float rollRate;
	//public float speedDrag;
	//public float normalDrag;
	//public float brakeDrag;

	private float throttle;		// zero to one
	//private int wantedPitch, wantedYaw, wantedRoll;
	private float deltaPitch, deltaYaw, deltaRoll;
	private float lift;
	
	void Start () {
		throttle = 0.5f;
		lift = rigidbody.mass * -Physics.gravity.y * 50;
		rigidbody.velocity = new Vector3 (0, 0, initialVelocity);
		//wantedPitch = 0; wantedYaw = 0; wantedRoll = 0;
		deltaPitch = 0; deltaYaw = 0; deltaRoll = 0;
	}

	void Update()
	{

	}

	void FixedUpdate () {
		float dt = Time.fixedDeltaTime;

		// this seems like a totally boneheaded way to do it
		/*
		if (pitch < wantedPitch) {
			pitch += pitchRate*dt;
		}
		else if (pitch > wantedPitch) {
			pitch -= pitchRate*dt;
		}
		if (yaw < wantedYaw) {
			yaw += yawRate*dt;
		}
		else if (yaw > wantedYaw) {
			yaw -= yawRate*dt;
		}
		if (roll < wantedPitch) {
			roll += rollhRate*dt;
		}
		else if (roll > wantedRoll) {
			roll -= rollRate*dt;
		}
		*/
		if (deltaPitch != 0 || deltaYaw != 0 || deltaRoll != 0) 
		{
			float pitch = deltaPitch * pitchRate * dt;
			float yaw = deltaYaw * yawRate * dt;
			float roll = deltaRoll * rollRate * dt;

			deltaPitch = 0;
			deltaYaw = 0;
			deltaRoll = 0;

			transform.Rotate(pitch, yaw, roll);
		}

		float currentThrust = throttle * maxThrust;
		float currentLift = lift;	// * rigidbody.GetRelativePointVelocity;
		Vector3 force = new Vector3 (0, currentLift, currentThrust) * dt;
		rigidbody.AddRelativeForce(force);
	}

	public void ModifyPitch(float delta)
	{
		if (delta < 0) delta = delta * 0.5f; // harder to pitch down than up
		//wantedPitch += delta;
		deltaPitch += delta;
	}
	public void ModifyYaw(float delta)
	{
		//wantedYaw += delta;
		deltaYaw += delta;
	}
	public void ModifyRoll(float delta)
	{
		//wantedRoll += delta;
		deltaRoll += delta;
	}

	public void ModifyThrottle(float delta)
	{
		throttle = Mathf.Clamp(throttle + delta, 0.3f, 1f);
		//if (delta > 0) rigidbody.drag = speedDrag;
		//else if (delta < 0) rigidbody.drag = brakeDrag;
		//else rigidbody.drag = normalDrag;
	}

	public float GetThrottle()
	{
		return throttle;
	}

	void OnCollisionEnter(Collision collision) {
		GameObject other = collision.gameObject;
		if (other.tag == "Ground") {
			Destroy (gameObject);
		}
	}
}
