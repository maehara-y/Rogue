using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed = 3.0f;
	public float rotateSpeed = 3.0f;
	public float gravity = 20.0f;

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;	
	private Animator animator;

	// Use this for initialization
	void Start () {
		this.controller = GetComponent<CharacterController>();
		this.animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		if (controller.isGrounded) {
			this.moveDirection.z = (Input.GetAxis("Vertical") > 0.0f) ? 
				Input.GetAxis("Vertical") * this.speed : 0;
		}
		transform.Rotate(0, Input.GetAxis("Horizontal") * this.rotateSpeed, 0);
		Vector3 globalDirection = transform.TransformDirection(moveDirection);
		this.controller.Move(globalDirection * this.speed * Time.deltaTime);
		this.animator.SetBool("Run", this.moveDirection.z > 0.0f);
	}
}
