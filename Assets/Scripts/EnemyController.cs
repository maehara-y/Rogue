using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	private GameObject target;
	private NavMeshAgent agent;
	private Animator animator;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		// TODO:一定時間おきにやったほうがよい？
		if (target)	{
			agent.destination = target.transform.position;
			animator.SetBool("Run", true);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "PlayerTag") target = other.gameObject;
	}
}
