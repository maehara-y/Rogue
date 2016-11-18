using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	// 物理制御系
	public float speed = 3.0f;
	public float rotateSpeed = 6.0f;
	public float gravity = 20.0f;
	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;	
	private Animator animator;

	// UI系
	public Text lvText;
	public Text hpMpText;
	public Slider hpBar;
	public Slider mpBar;
	public Button magicSettingButton;
	private Text magicSettingText;
	public Button itemSettingButton;
	private Text itemSettingText;
	public Text gameOver;
	public GameObject battleObjectRoot;

	// 状態制御系
	public bool isDead = false;
	public bool isDamage = false;

	// パラメータ系
	public PlayerModel playerModel;
	private List<List<GameObject>> magicInstances;
	private int maxMagicIndex = 5;
	private int magicInstanceIndex = 0;

	// TODO:Input一時処理
	private bool isForward = false;
	private bool isBack = false;
	private bool isLeft = false;
	private bool isRight = false;

	// 初期処理
	public void Initialize() {
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		playerModel = new PlayerModel();
		playerModel.Initialize();

		magicInstances = new List<List<GameObject>>();
		for (int i = 0; i < playerModel.settableMagics.Count; i++) {
			for (int j = 0; j < maxMagicIndex; j++) {
				GenerateMagic(i);
			}
		}
		DisplayStatus();
	}

	// 魔法の生成 (シーンの初期処理でストックを作っておく)
	void GenerateMagic(int magicIndex) {
		// TODO:Resourcesは重いので、全魔法プレハブのリストをpublic変数にinspector上でセットしておく
		// TODO:以下はショット系魔法の場合。他の魔法には他のクラスを適用する。
		// モンスター側のcollisionイベントで発動する場合もあれば、効果がここで発動するものもある。
		MagicModel targetMagic = playerModel.settableMagics[magicIndex];
		GameObject magicPrefab = (GameObject)Resources.Load(targetMagic.prefabName);
		GameObject magicObj = Instantiate(magicPrefab, transform.position, transform.rotation) as GameObject;
		magicObj.transform.SetParent(battleObjectRoot.transform);
		MagicController magicController = magicObj.GetComponent<MagicController>();
		magicController.Initialize();
		magicController.magicModel = targetMagic;
		magicObj.SetActive(false);

		// インスタンスプールに入れる
		if (magicInstances.Count > magicIndex) {
			magicInstances[magicIndex].Add(magicObj);
		} else {
			List<GameObject> magicList = new List<GameObject>();
			magicList.Add(magicObj);
			magicInstances.Add(magicList);
		}
	}

	// Update is called once per frame
	void Update () {
		if (isDead || isDamage) return;

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
		lvText.text = "Lv." + playerModel.lv + "　　　　Exp " + playerModel.exp + "\nHP\nMP";
		hpMpText.text = playerModel.hp + "/" + playerModel.maxHp + "\n" + playerModel.mp + "/" + playerModel.maxMp;
		hpBar.value = (float)playerModel.hp / playerModel.maxHp;
		mpBar.value = (float)playerModel.mp / playerModel.maxMp;
		MagicModel usableMagic = playerModel.GetUsableMagic();
		magicSettingText = magicSettingButton.GetComponentInChildren<Text>();
		magicSettingText.text = (usableMagic != null) ? usableMagic.magicName : "";
		/* TODO:
		ItemModel usableItem = playerModel.GetUsableItem();
		itemSettingText = itemSettingButton.GetComponentInChildren<Text>();
		itemSettingText.text = (usableItem != null) ? usableItem.itemName : "アイテム切り替え";*/
	}

	// ダメージを受ける
	IEnumerator Damage(int damage) {
		isDamage = true;
		playerModel.hp -= damage;
		animator.SetTrigger("TakeDamage");
		yield return new WaitForSeconds(1f);
		isDamage = false;
	}

	// 戦闘不能によりゲームオーバー
	IEnumerator Die() {
		playerModel.hp = 0;
		isDead = true;
		animator.SetTrigger("Die");
		gameOver.text = "Game Over";

		yield return new WaitForSeconds(5f);

		// ダンジョンシーン再読込
		SceneManager.LoadScene("Dungeon");
	}

	// 魔法の切り替え
	public void SwitchMagic() {
		int currentIndex = playerModel.usableMagicIndex;
		playerModel.usableMagicIndex = (currentIndex + 1 >= playerModel.settableMagics.Count) ? 0 : currentIndex + 1;
		DisplayStatus();
	}

	// 攻撃 (魔法の使用)
	IEnumerator Attack() {
		// MPが足りなければ使えない
		MagicModel usableMagic = playerModel.GetUsableMagic();
		if (playerModel.mp < usableMagic.useMp) yield break;
		playerModel.mp -= usableMagic.useMp;
		DisplayStatus();

		// TODO:あとでenumでtype定義
		if (usableMagic.magicType == 1) {
			this.animator.SetBool(usableMagic.animationName, true);
			yield return new WaitForSeconds(1f);

			GameObject magicObj = magicInstances[playerModel.usableMagicIndex][magicInstanceIndex];
			magicInstanceIndex = (magicInstanceIndex < maxMagicIndex-1) ? magicInstanceIndex+1 : 0;
			magicObj.SetActive(true);

			Vector3 pos = transform.position + transform.forward * 2;
			pos.y += 1.2f;
			magicObj.transform.position = pos;
			magicObj.transform.rotation = transform.rotation;
			magicObj.GetComponent<MagicController>().Shot();
		}
	}

	// 被ダメージ用の衝突判定
	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag != "EnemyAttackTag") return;

		// ダメージ計算
		EnemyAttack enemyAttack = col.gameObject.GetComponent<EnemyAttack>();
		int damage = 0;
		MagicModel usableMagic = playerModel.GetUsableMagic();
		if (BattleCalculator.IsHitPlayer(playerModel, usableMagic, enemyAttack.enemyModel)) {
			damage = BattleCalculator.GetPlayerDamage(playerModel, usableMagic, enemyAttack.enemyModel);
		}

		// HPが0以下になったら死ぬ
		if (playerModel.hp <= damage) {
			StartCoroutine(Die());
		} else {
			StartCoroutine(Damage(damage));
		}
		DisplayStatus();
	}
}
