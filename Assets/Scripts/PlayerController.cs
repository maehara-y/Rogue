using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public float speed = 3.0f;
	public float rotateSpeed = 6.0f;
	public float gravity = 20.0f;
	public Text playerStatus;
	public Text gameOver;
	public bool isDead = false;
	public bool isDamage = false;
	public GameObject battleObjectRoot;
	public List<GameObject> magicList;

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;	
	private Animator animator;
	private PlayerModel model;
	private MagicModel magic;
	private int maxMagicIndex = 5;
	private int currentMagicIndex = 0;
	// TODO:Input一時処理
	private bool isForward = false;
	private bool isBack = false;
	private bool isLeft = false;
	private bool isRight = false;

	public void Initialize() {
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		model = GetComponent<PlayerModel>();
		model.Initialize();
		magicList = new List<GameObject>();
		magic = GetComponent<MagicModel>();
		for (int i = 0; i < maxMagicIndex; i++) {
			GenerateMagic();
		}
	}

	// 魔法の生成 (シーンの初期処理でストックを作っておく)
	void GenerateMagic() {
		// TODO:以下はショット系魔法の場合。他の魔法には他のクラスを適用する。
		// モンスター側のcollisionイベントで発動する場合もあれば、効果がここで発動するものもある。
		// TODO:Resourcesは重いので、全魔法プレハブのリストをpublic変数にinspector上でセットしておく
		magic.SetMagicId(model.currentMagicId);
		GameObject magicPrefab = (GameObject)Resources.Load(magic.prefabName);
		GameObject magicObj = Instantiate(magicPrefab, transform.position, transform.rotation) as GameObject;
		magicObj.transform.parent = battleObjectRoot.transform;
		MagicController magicController = magicObj.GetComponent<MagicController>();
		magicController.Initialize();
		magicController.magicModel = magic;
		magicObj.SetActive(false);
		magicList.Add(magicObj);
	}

	// Update is called once per frame
	void Update () {
		if (isDead || isDamage) return;
		
		// TODO:Start関数内だと反映されず。とはいえUpdate内はよくないのであとで修正
		DisplayStatus();

		// 移動
		if (controller.isGrounded) {
			moveDirection.z = (Input.GetAxis("Vertical") > 0.0f) ? 
				Input.GetAxis("Vertical") * speed : 0;
			// TODO:Input一時処理
			if (isForward) { moveDirection.z = speed; }
			if (isBack) { moveDirection.z = -1 * speed; }
		}
		transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
		// TODO:Input一時処理
		if (isLeft) { transform.Rotate(0, -1 * rotateSpeed, 0); }
		if (isRight) { transform.Rotate(0, 1 * rotateSpeed, 0); }

		Vector3 globalDirection = transform.TransformDirection(moveDirection);
		controller.Move(globalDirection * speed * Time.deltaTime);
		animator.SetBool("Run", moveDirection.z > 0.0f);

		// 攻撃
		if (Input.GetKeyDown(KeyCode.Space)) {
			StartCoroutine(Attack());
		}
	}

	// TODO:スマホタップコントローラー用の一時処理
	public void MoveForward() { isForward = true; }
	public void MoveForwardStop() { isForward = false; }
	public void MoveBack() { isBack = true; }
	public void MoveBackStop() { isBack = false; }
	public void MoveLeft() { isLeft = true; }
	public void MoveLeftStop() { isLeft = false; }
	public void MoveRight() { isRight = true; }
	public void MoveRightStop() { isRight = false; }
	public void TapAttack() { StartCoroutine(Attack()); }

	// ステータス表示
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
		// MPが足りなければ使えない
		if (model.mp < magic.useMp) yield break;
		model.mp -= magic.useMp;
		DisplayStatus();

		// TODO:あとでenumでtype定義
		if (magic.magicType == 1) {
			this.animator.SetBool(magic.animationName, true);
			yield return new WaitForSeconds(1f);

			GameObject magicObj = magicList[currentMagicIndex];
			currentMagicIndex = (currentMagicIndex < maxMagicIndex-1) ? currentMagicIndex+1 : 0;
			magicObj.SetActive(true);

			Vector3 pos = transform.position + transform.forward * 2;
			pos.y += 1.5f;
			magicObj.transform.position = pos;
			magicObj.transform.rotation = transform.rotation;
			magicObj.GetComponent<MagicController>().Shot();
		}
	}

	// 被ダメージ用の衝突判定
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
