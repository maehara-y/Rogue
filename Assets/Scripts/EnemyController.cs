using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : MonoBehaviour {

	// 物理制御系
	public float speed = 3.0f;
	private GameObject target;	// プレイヤーのこと
	//private NavMeshAgent agent;
	private CharacterController controller;
	private Animator animator;

	// UI系
	public Text nameLabel;
	public Slider hpSlider;
	public GameObject battleObjectRoot;

	// パラメータ系
	private EnemyModel enemyModel;
	private PlayerController playerController;

	// 状態制御系
	private bool isDead = false;
	private bool isAttack = false;

	/*************************************************************
	 * 初期処理
	 *************************************************************/
	public void Initialize () {
		//agent = GetComponent<NavMeshAgent>();
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		enemyModel = new EnemyModel();
		enemyModel.Initialize();

		// TODO:レベル算出と表示
		string lv = "15";
		nameLabel.text = "Lv" + lv + " " + enemyModel.enemyName;
		hpSlider.value = 1.0f;

		// TODO:Findは遅いので変える
		GameObject enemyRoot = GameObject.Find("Enemy");
		transform.SetParent(enemyRoot.transform);
	}

	/*************************************************************
	 * 更新処理
	 *************************************************************/
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

	/*************************************************************
	 * 攻撃
	 *************************************************************/
	IEnumerator Attack() {
		isAttack = true;
		animator.SetBool("Run", false);
		animator.SetTrigger("Attack");
		yield return new WaitForSeconds(1.5f);

		// 攻撃エフェクトの発生
		Vector3 hitEffectPosition = transform.position + transform.forward * 2;
		hitEffectPosition.y += 1;
		GameObject attackPrefab = (GameObject)Resources.Load(enemyModel.attackParticleName);
		// TODO:InstantiateはここではなくInitialize時に行ってプールしておく
		GameObject attackParticle = Instantiate(attackPrefab, hitEffectPosition, Quaternion.identity) as GameObject;
		attackParticle.GetComponent<EnemyAttack>().enemyModel = enemyModel;
		attackParticle.transform.SetParent(battleObjectRoot.transform);

		yield return new WaitForSeconds(1f);
		isAttack = false;
	}

	/*************************************************************
	 * 索敵用の衝突判定 (範囲内に入ったらプレイヤーを追跡対象とする)
	 *************************************************************/
	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "PlayerTag") {
			target = col.gameObject;
			playerController = target.GetComponent<PlayerController>();
		}
	}

	/*************************************************************
	 * ダメージ用の衝突判定
	 *************************************************************/
	void OnCollisionEnter(Collision col) {
		GameObject magic = col.gameObject;
		if (isDead || magic.tag != "PlayerAttackTag") return;
		MagicController magicController = magic.GetComponent<MagicController>();
		if (!magicController) return;

		// 魔法がヒットしたらダメージ計算をする
		GameObject playerObj = GameObject.FindWithTag("PlayerTag");
		PlayerController playerController = playerObj.GetComponent<PlayerController>();
		PlayerModel playerModel = playerController.playerModel;
		if (playerController.isDead) return;

		int damage = 0;
		if (BattleCalculator.IsHitEnemy(playerModel, magicController.magicModel, enemyModel)) {
			damage = BattleCalculator.GetEnemyDamage(playerModel, magicController.magicModel, enemyModel);
		}

		// ダメージを受ける。HPが0以下になったら死ぬ。
		if (enemyModel.hp < damage) {
			enemyModel.hp = 0;
			hpSlider.value = 0f;
			isDead = true;

			// 経験値を獲得
			playerModel.AddExp(enemyModel.exp);
			playerController.DisplayStatus();
			animator.SetBool("Die", true);
			StartCoroutine(Die());
		} else {
			enemyModel.hp -= damage;
			hpSlider.value = (float)enemyModel.hp / enemyModel.maxHp;
			animator.SetBool("Damage", true);
			// TODO:被弾後、waitの時間を少し作りたい
		}
		magic.SetActive(false);

		// TODO:違うタイプの攻撃はここに実装していく？
	}

	/*************************************************************
	 * 死んだ処理
	 *************************************************************/
	IEnumerator Die() {
		yield return new WaitForSeconds(3f);
		// TODO:消滅エフェクトをいれる
		this.gameObject.SetActive(false);
	}
}
