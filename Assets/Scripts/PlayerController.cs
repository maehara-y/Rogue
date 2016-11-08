using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed = 3.0f;
	public float rotateSpeed = 3.0f;
	public float gravity = 20.0f;
	public Text playerStatus;
	public Text gameOver;

	private Vector3 moveDirection = Vector3.zero;

	private CharacterController controller;	
	private Animator animator;
	private PlayerModel model;
	private MagicModel magic;

	public bool isDead = false;
	public bool isDamage = false;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		model = GetComponent<PlayerModel>();
		magic = GetComponent<MagicModel>();
		magic.SetMagicId(model.currentMagicId);
	}

	// Update is called once per frame
	void Update () {
		if (isDead || isDamage) return;
		
		// TODO:Start関数内だと反映されず。とはいえUpdate内はよくないのであとで修正
		DisplayStatus();

		// 移動
		if (controller.isGrounded) {
			this.moveDirection.z = (Input.GetAxis("Vertical") > 0.0f) ? 
				Input.GetAxis("Vertical") * this.speed : 0;
		}
		transform.Rotate(0, Input.GetAxis("Horizontal") * this.rotateSpeed, 0);
		Vector3 globalDirection = transform.TransformDirection(moveDirection);
		this.controller.Move(globalDirection * this.speed * Time.deltaTime);
		this.animator.SetBool("Run", this.moveDirection.z > 0.0f);

		// 魔法の使用
		if (Input.GetKeyDown(KeyCode.Space)) {
			StartCoroutine(Attack());
		}
	}

	public void DisplayStatus() {
		playerStatus.text = "Lv:" + model.lv + "   exp:" + model.exp + "   HP:" + model.hp + "/" + model.maxHp + "   MP:" + model.mp + "/" + model.maxMp + "   魔法:" + magic.magicName; 
	}

	// ダメージを受ける
	IEnumerator Damage(int damage) {
		isDamage = true;
		model.hp -= damage;
		animator.SetTrigger("TakeDamage");
		yield return new WaitForSeconds(1f);
		isDamage = false;
	}

	// 戦闘不能によりゲームオーバー
	IEnumerator Die() {
		model.hp = 0;
		isDead = true;
		animator.SetTrigger("Die");
		gameOver.text = "Game Over";

		yield return new WaitForSeconds(5f);

		// ダンジョンシーン再読込
		SceneManager.LoadScene("Dungeon");
	}

	// 攻撃 (魔法の使用)
	IEnumerator Attack() {
		magic.SetMagicId(model.currentMagicId);
		// MPが足りなければ使えない
		if (model.mp < magic.useMp) yield break;
		model.mp -= magic.useMp;
		DisplayStatus();

		// TODO:あとでenumでtype定義
		if (magic.type == 1) {
			this.animator.SetBool(magic.animationName, true);

			yield return new WaitForSeconds(1f);
			// TODO:以下はショット系魔法の場合。他の魔法には他のクラスを適用する。
			// モンスター側のcollisionイベントで発動する場合もあれば、効果がここで発動するものもある。
			GameObject magicPrefab = (GameObject)Resources.Load(magic.prefabName);
			Vector3 pos = transform.position + transform.forward * 2;
			pos.y += 2;
			GameObject magicObj = Instantiate(magicPrefab, pos, transform.rotation) as GameObject;
			magicObj.GetComponent<Rigidbody>().AddForce(magicObj.transform.forward * magic.speed);
			magicObj.GetComponent<ProjectileScript>().impactNormal = pos;
			magicObj.GetComponent<ProjectileScript>().magicModel = magic;
		}
	}
	// ダメージ用の衝突判定
	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag != "EnemyAttackTag") return;

		EnemyAttack enemyAttack = col.gameObject.GetComponent<EnemyAttack>();
		int damage = 0;
		if (BattleCalculator.IsHitPlayer(model, magic, enemyAttack.enemyModel)) {
			damage = BattleCalculator.GetPlayerDamage(model, magic, enemyAttack.enemyModel);
		}

		// HPが0以下になったら死ぬ
		if (model.hp <= damage) {
			StartCoroutine(Die());
		} else {
			StartCoroutine(Damage(damage));
		}
		DisplayStatus();
	}
}
