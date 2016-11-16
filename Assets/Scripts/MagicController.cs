using UnityEngine;
using System.Collections;

public class MagicController : MonoBehaviour {

	public GameObject shotParticle;
	public GameObject impactParticle;
	public MagicModel magicModel { get; set;}	// ヒット後のダメージ計算に利用？
	private Rigidbody rigidbody;

	// 初期処理
	public void Initialize() {
		rigidbody = gameObject.GetComponent<Rigidbody>();

		shotParticle = Instantiate(shotParticle, transform.position, transform.rotation) as GameObject;
		shotParticle.transform.parent = transform;
		shotParticle.SetActive(false);

		impactParticle = Instantiate (impactParticle, transform.position, Quaternion.FromToRotation (Vector3.up, transform.position)) as GameObject;
		impactParticle.transform.parent = transform;
		impactParticle.SetActive(false);
	}

	// 発射時のエフェクト
	public void Shot() {
		// パーティクルの再生
		shotParticle.SetActive(true);
		shotParticle.transform.position = transform.position;
		shotParticle.transform.rotation = transform.rotation;
		shotParticle.transform.parent = transform;
		foreach (Transform shotChild in shotParticle.transform) {
			shotChild.gameObject.SetActive(true);
			ParticleSystem childParticle = shotChild.gameObject.GetComponent<ParticleSystem>();
			if (childParticle) childParticle.Play();
		}
		// エフェクトを動かす
		rigidbody.AddForce(transform.forward * magicModel.speed);
	}
	
	void OnCollisionEnter(Collision hit) {
		StartCoroutine(impact());
	}

	// 衝突時のエフェクト
	IEnumerator impact() {
		shotParticle.SetActive(false);
		foreach (Transform shotChild in shotParticle.transform) {
			shotChild.gameObject.SetActive(false);
		}
		impactParticle.SetActive(true);
		foreach (Transform impactChild in impactParticle.transform) {
			impactChild.gameObject.SetActive(true);
			ParticleSystem childParticle = impactChild.gameObject.GetComponent<ParticleSystem>();
			if (childParticle) childParticle.Play();
		}

		yield return new WaitForSeconds(1f);
		foreach (Transform impactChild in impactParticle.transform) {
			impactChild.gameObject.SetActive(false);
		}
		impactParticle.SetActive(false);
		gameObject.SetActive(false);
	}
}
