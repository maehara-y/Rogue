using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public float speed = 3.0f;

	private GameObject target;	// プレイヤーのこと
	//private NavMeshAgent agent;
	private CharacterController controller;
	private Animator animator;
	private EnemyModel enemyModel;
	private PlayerController playerController;

	private bool isDead = false;
	private bool isAttack = false;

	public void Initialize () {
		//agent = GetComponent<NavMeshAgent>();
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		enemyModel = GetComponent<EnemyModel>();
		enemyModel.Initialize();

		// TODO:Findは遅いので変える
		GameObject enemyRoot = GameObject.Find("Enemy");
		transform.parent = enemyRoot.transform;
	}

	// Update is called once per frame
	void Update () {
		if (isDead) return;
		
		// TODO:パフォーマンス的に一定時間おきにやったほうがよさそう
		if (target)	{
			if (playerController.isDead) return;

			transform.LookAt(target.transform);
			float distance = Vector3.Distance(transform.position, target.transform.position);
			if (distance < 3f) {
				if (!isAttack) StartCoroutine(Attack());
			} else {
				//agent.destination = target.transform.position;
				controller.SimpleMove(transform.forward * this.speed);
				animator.SetBool("Run", true);
			}
		}
	}

	// 死んだ処理
	IEnumerator Die() {
		yield return new WaitForSeconds(3f);
		// TODO:消滅エフェクトをいれる
		this.gameObject.SetActive(false);
	}

	// 攻撃
	IEnumerator Attack() {
		isAttack = true;
		animator.SetBool("Run", false);
		animator.SetTrigger("Attack");
		yield return new WaitForSeconds(1.5f);

		// 攻撃エフェクトの発生
		Vector3 hitEffectPosition = transform.position + transform.forward * 2;
		hitEffectPosition.y += 1;
		GameObject attackPrefab = (GameObject)Resources.Load(enemyModel.attackParticleName);
		GameObject attackParticle = Instantiate(attackPrefab, hitEffectPosition, Quaternion.identity) as GameObject;
		attackParticle.GetComponent<EnemyAttack>().enemyModel = enemyModel;

		yield return new WaitForSeconds(1f);
		isAttack = false;
	}

	// 索敵用の衝突判定 (範囲内に入ったらプレイヤーを追跡対象とする)
	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "PlayerTag") {
			target = col.gameObject;
			playerController = target.GetComponent<PlayerController>();
		}
	}

	// ダメージ用の衝突判定
	void OnCollisionEnter(Collision col) {
		GameObject magic = col.gameObject;
		if (isDead || magic.tag != "PlayerAttackTag") return;
		MagicController magicController = magic.GetComponent<MagicController>();
		if (!magicController) return;

		// 魔法がヒットしたらダメージ計算をする
		GameObject playerObj = GameObject.FindWithTag("PlayerTag");
		PlayerModel playerModel = playerObj.GetComponent<PlayerModel>();
		PlayerController playerController = playerObj.GetComponent<PlayerController>();
		if (playerController.isDead) return;

		int damage = 0;
		if (BattleCalculator.IsHitEnemy(playerModel, magicController.magicModel, enemyModel)) {
			damage = BattleCalculator.GetEnemyDamage(playerModel, magicController.magicModel, enemyModel);
		}

		// ダメージを受ける。HPが0以下になったら死ぬ。
		if (enemyModel.hp < damage) {
			enemyModel.hp = 0;
			isDead = true;
			// 経験値を獲得
			playerModel.AddExp(enemyModel.exp);
			playerController.DisplayStatus();
			animator.SetBool("Die", true);
			StartCoroutine(Die());
		} else {
			enemyModel.hp -= damage;
			animator.SetBool("Damage", true);
			// TODO:被弾後、waitの時間を少し作りたい
		}

		magic.SetActive(false);

		// TODO:違うタイプの攻撃はここに実装していく？
	}
}
