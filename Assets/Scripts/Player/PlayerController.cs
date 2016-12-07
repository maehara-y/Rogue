using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

	// 物理制御系
	public float speed = 3.0f;
	public float rotateSpeed = 6.0f;
	public float gravity = 20.0f;
	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;	
	private Animator animator;
	private float guardValidTime = 0.15f;
	private float guardWaitTime = 0.1f;

	// UI系
	public Camera mainCamera;
	public Text lvText;
	public Text hpMpText;
	public Slider hpBar;
	public Slider mpBar;
	public Button magicSettingButton;
	private Text magicSettingText;
	public Button itemSettingButton;
	private Text itemSettingText;
	public Text gameOver;
	public GameObject damageFlash;
	public GameObject guardFlash;
	public GameObject battleObjectRoot;
	private Image damageFlashImage;
	private Image guardFlashImage;

	// 状態制御系
	public bool isAttack = false;
	public bool isDead = false;
	public bool isDamage = false;
	public bool isGuard = false;
	public bool isGuardWait = false;

	// パラメータ系
	public PlayerModel playerModel;
	private List<List<GameObject>> magicInstances;
	private int maxMagicInstanceIndex = 2;
	private int magicInstanceIndex = 0;

	// TODO:Input一時処理
	private bool isForward = false;
	private bool isBack = false;
	private bool isLeft = false;
	private bool isRight = false;

	/*************************************************************
	 * 初期処理
	 *************************************************************/
	public void Initialize() {
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		damageFlashImage = damageFlash.GetComponent<Image>();
		guardFlashImage = guardFlash.GetComponent<Image>();
		playerModel = new PlayerModel();
		playerModel.Initialize();

		// 魔法のインスタンスプール処理
		magicInstances = new List<List<GameObject>>();
		for (int i = 0; i < playerModel.settableMagics.Count; i++) {
			for (int j = 0; j < maxMagicInstanceIndex; j++) {
				GenerateMagic(i);
			}
		}
		// 画面表示更新
		DisplayStatus();
	}

	/*************************************************************
	 * 魔法の生成 (シーンの初期処理でストックを作っておく)
	 *************************************************************/
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

	/*************************************************************
	 * 更新処理
	 *************************************************************/
	void Update () {
		// 死亡時・被ダメ中・攻撃中は操作不可
		if (isDead || isDamage || isAttack) return;

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

	/*************************************************************
	 * TODO:スマホタップコントローラー用の一時処理
	 *************************************************************/
	public void MoveForward() { isForward = true; }
	public void MoveForwardStop() { isForward = false; }
	public void MoveBack() { isBack = true; }
	public void MoveBackStop() { isBack = false; }
	public void MoveLeft() { isLeft = true; }
	public void MoveLeftStop() { isLeft = false; }
	public void MoveRight() { isRight = true; }
	public void MoveRightStop() { isRight = false; }
	public void TapAttack() { StartCoroutine(Attack()); }
	public void TapGuard() { StartCoroutine(Guard()); }


	/*************************************************************
	 * ステータス表示
	 *************************************************************/
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

	/*************************************************************
	 * 魔法の切り替え (UIから呼び出し)
	 *************************************************************/
	public void SwitchMagic() {
		int currentIndex = playerModel.usableMagicIndex;
		playerModel.usableMagicIndex = (currentIndex + 1 >= playerModel.settableMagics.Count) ? 0 : currentIndex + 1;
		// 画面表示更新
		DisplayStatus();
	}

	/*************************************************************
	 * 攻撃 (魔法の使用)
	 *************************************************************/
	IEnumerator Attack() {
		// 攻撃中やMP不足時は使えない
		MagicModel usableMagic = playerModel.GetUsableMagic();
		if (isAttack || playerModel.mp < usableMagic.useMp) yield break;

		isAttack = true;
		playerModel.mp -= usableMagic.useMp;
		// 画面表示更新
		DisplayStatus();

		// TODO:あとでenumでtype定義
		if (usableMagic.magicType == 1) {
			this.animator.SetBool(usableMagic.animationName, true);
			yield return new WaitForSeconds(1f);

			GameObject magicObj = magicInstances[playerModel.usableMagicIndex][magicInstanceIndex];
			magicInstanceIndex = (magicInstanceIndex < maxMagicInstanceIndex-1) ? magicInstanceIndex+1 : 0;
			magicObj.SetActive(true);

			Vector3 pos = transform.position + transform.forward * 2;
			pos.y += 1.2f;
			magicObj.transform.position = pos;
			magicObj.transform.rotation = transform.rotation;
			magicObj.GetComponent<MagicController>().Shot();
			isAttack = false;
		}
	}

	/*************************************************************
	 * ジャストガード、防御 (タップタイミングの記録)
	 *************************************************************/
	IEnumerator Guard() {
		if (isGuard || isGuardWait) yield break;

		isGuard = true;
		Debug.Log("<color=blue>ジャストガード - タップ！！</color>");

		yield return new WaitForSeconds(guardValidTime);
		isGuard = false;
		isGuardWait = true;

		yield return new WaitForSeconds(guardWaitTime);
		isGuardWait = false;
	}

	/*************************************************************
	 * 被ダメージ用の衝突判定
	 *************************************************************/
	void OnTriggerEnter(Collider col) {
		// TODO:モンスターが死んでから発せられた攻撃は無効にする
		if (col.gameObject.tag != "EnemyAttackTag") return;

		Debug.Log("<color=red>敵のエフェクトが衝突！！</color>");

		// ダメージ計算
		EnemyAttack enemyAttack = col.gameObject.GetComponent<EnemyAttack>();
		int damage = 0;
		if (BattleCalculator.IsHitPlayer(playerModel, enemyAttack.enemyModel)) {
			damage = BattleCalculator.GetPlayerDamage(playerModel, enemyAttack.enemyModel, enemyAttack.actionKey, isGuard);
		}

		StartCoroutine(HideEffect(col.gameObject));

		// HPが0以下になったら死ぬ
		if (playerModel.hp <= damage) {
			StartCoroutine(Die());
		} else {
			StartCoroutine(Damage(damage));
		}
		// 画面表示更新
		DisplayStatus();
	}

	/*************************************************************
	 * 敵の攻撃エフェクトを非表示にする
	 *************************************************************/
	IEnumerator HideEffect(GameObject targetObject) {
		yield return new WaitForSeconds(0.5f);
		// TODO:DestroyではなくSetActive(false)に変更する
		Destroy(targetObject);
	}
	/*************************************************************
	 * 被ダメージ処理
	 *************************************************************/
	IEnumerator Damage(int damage) {
		isDamage = true;
		playerModel.hp -= damage;
		animator.SetTrigger("TakeDamage");

		if (isGuard) {
			Debug.Log("<color=green>ジャストガード - 成功！！</color>");
			guardFlashImage.DOFade(0.7f, 0.2f);
			yield return new WaitForSeconds(0.3f);
			guardFlashImage.DOFade(0.0f, 0.2f);
		} else {
			// TODO:画面を揺らす対応は、VRプレイ時は酔うので無効化しよう
			mainCamera.DOShakePosition(0.5f, 0.5f, 10, 90.0f);
			damageFlashImage.DOFade(0.7f, 0.2f);
			yield return new WaitForSeconds(0.3f);
			damageFlashImage.DOFade(0.0f, 0.2f);
		}
		yield return new WaitForSeconds(0.2f);
		isDamage = false;
	}

	/*************************************************************
	 * 戦闘不能によりゲームオーバー
	 *************************************************************/
	IEnumerator Die() {
		playerModel.hp = 0;
		isDead = true;
		animator.SetTrigger("Die");
		gameOver.text = "Game Over";

		yield return new WaitForSeconds(5f);

		// ダンジョンシーン再読込
		SceneManager.LoadScene("Dungeon");
	}
}