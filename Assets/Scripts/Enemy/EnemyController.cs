using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class EnemyController : MonoBehaviour {

	// 物理制御系
	public float runSpeed = 3.0f;
	private float stepSpeed = 1.0f;
	private float longRangeAttackSpeed = 300.0f;
	private GameObject target;	// プレイヤーのこと
	private CharacterController enemyController;
	private Animator animator;

	// UI系
	public Text nameLabel;
	public Slider hpSlider;
	public GameObject battleObjectRoot;

	// パラメータ系
	public EnemyModel enemyModel;
	private PlayerController playerController;

	// 状態制御系
	private enum EnemyState { Wait, Chase, Battle, Die }
	private EnemyState state = EnemyState.Wait;		// 状態遷移
	private bool isAttack = false;					// 攻撃中か
	public bool isStepping = false;					// ステップ中か(プレイヤー攻撃側の衝突判定に使うためpublicに)
	private bool isDamage = false;					// 被ダメージ中か
	private bool availableLongAttack = true;		// 遠距離攻撃の判定をしてもいい状態か
	private Vector3 stepDestination;				// ステップの着地点
	// 予約済バトル行動indexリスト
	private List<EnemyActionGroupModel.ActionKey> reservedActions = new List<EnemyActionGroupModel.ActionKey>();
	private EnemyActionGroupModel.ActionKey currentActionKey;

	private float debugTime = 0;

	/*************************************************************
	 * 初期処理
	 *************************************************************/
	public void Initialize () {
		enemyController = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		animator.SetBool("Idle", true);

		// レベル算出と表示
		nameLabel.text = "Lv" + enemyModel.level + " " + enemyModel.enemyName;
		hpSlider.value = 1.0f;

		// TODO:Findは遅いので変える
		GameObject enemyRoot = GameObject.Find("Enemy");
		transform.SetParent(enemyRoot.transform);
	}

	/*************************************************************
	 * 更新処理
	 *************************************************************/
	void Update () {
		debugTime += Time.deltaTime;
		if (!target || state == EnemyState.Die) return;

		// 状態遷移を行う
		TransitState();
	}

	/*************************************************************
	 * 敵の状態遷移 (Update時)
	 *************************************************************/
	private void TransitState() {
		// プレイヤーが死んでいたら待ち状態に遷移
		if (playerController.isDead) {
			state = EnemyState.Wait;
			animator.SetBool("Run", false);
			currentActionKey = EnemyActionGroupModel.ActionKey.Null;
			return;
		}

		// 被ダメアニメーション中は何もしない
		if (isDamage) return; 

		// ステップ中は移動処理
		if (isStepping) {
			//Debug.Log ("<color=yellow>ステップ中</color>");
			MoveByStep ();
			return;
		}

		// 攻撃アクション中は何もしない
		// TODO:敵が離れ過ぎた時に予約行動をキャンセルするとしたらこの中に記述
		if (isAttack) return;

		// バトル行動チェーン中なら残りの行動を継続する
		if (state == EnemyState.Battle && reservedActions.Count >= 1) {
			Debug.Log (debugTime + " <color=yellow>バトル行動継続 残り:" + reservedActions.Count + "回, アクション:" + reservedActions[0].ToString() + "</color>");
			DoAction(reservedActions[0]);
			return;
		}

		// プレイヤーとの距離から適切な状態に遷移する
		transform.LookAt(target.transform);
		float distance = Vector3.Distance(transform.position, target.transform.position);

		// 遠距離攻撃の判定
		if (distance >= enemyModel.longAttackRangeMin && distance <= enemyModel.longAttackRangeMax
			&& availableLongAttack && state != EnemyState.Battle) {
			availableLongAttack = false;
			StartCoroutine(ControlLongAttackCheck());
			float randomVal = UnityEngine.Random.value;
			// TODO:一時処理
			randomVal = 0.9f;
			if (enemyModel.longAttackHitRate >= randomVal) {
				//Debug.Log ("<color=blue>ロングレンジ攻撃予約 randomVal:" + randomVal + "</color>");
				state = EnemyState.Battle;
				reservedActions = EnemyActionGroupQuery.ChooseActionPatternByGroupId (enemyModel.longRangeActionGroupId);
				DoAction(reservedActions[0]);
				return;
			}
		}

		// プレイヤーとの距離に応じて攻撃・追跡・追跡停止を選択する
		if (distance < enemyModel.chaseRangeMin) {
			// 一定距離に縮まったら攻撃
			if (state != EnemyState.Battle) {
				state = EnemyState.Battle;
				reservedActions = EnemyActionGroupQuery.ChooseActionPatternByGroupId(enemyModel.shortRangeActionGroupId);
				Debug.Log (debugTime + " <color=red>ショートレンジ攻撃予約 アクション:" + reservedActions[0].ToString() + "</color>");
				DoAction(reservedActions[0]);
			}

		} else if (distance > enemyModel.chaseRangeMax) {
			// 一定距離離れすぎたら追跡をやめる
			//Debug.Log ("<color=white>追跡エンド</color>");
			state = EnemyState.Wait;
			animator.SetBool("Run", false);
			animator.SetBool("Idle", true);
			currentActionKey = EnemyActionGroupModel.ActionKey.Null;
		} else {
			// 追跡範囲内なら追跡
			//Debug.Log ("<color=green>追跡スタート</color>");
			state = EnemyState.Chase;
			animator.SetBool("Idle", false);
			animator.SetBool("Run", true);
			currentActionKey = EnemyActionGroupModel.ActionKey.Null;
			enemyController.SimpleMove(transform.forward * runSpeed);
		}
	}

	/*************************************************************
	 * 遠距離攻撃判定の管理 (0.5秒に1回だけ判定チェックする)
	 *************************************************************/
	IEnumerator ControlLongAttackCheck() {
		yield return new WaitForSeconds(0.5f);
		availableLongAttack = true;
	}

	/*************************************************************
	 * アクションが攻撃かステップか、分岐して処理する
	 *************************************************************/
	private void DoAction(EnemyActionGroupModel.ActionKey actionKey) {
		if (reservedActions.Count < 1) return;		// 行動キャンセル時は終了
		currentActionKey = actionKey;
		if (EnemyActionGroupModel.IsAttackActionKey(actionKey)) {
			isAttack = true;
			animator.SetBool("Idle", false);
			animator.SetBool("Run", false);
			animator.SetTrigger(actionKey.ToString());
			Debug.Log (debugTime + " <color=red>DoAction 攻撃:" + actionKey.ToString() + "</color>");
		}
		if (EnemyActionGroupModel.IsStepActionKey(actionKey)) {
			if (isStepping) return;
			DoStepBefore(actionKey);
			StartCoroutine("DoStep", actionKey);
		}
	}

	/*************************************************************
	 * 攻撃エフェクトの再生 (アニメーション再生中イベントから呼び出される)
	 *************************************************************/
	public void PlayAttackEffect() {
		if (reservedActions.Count < 1) return;	// 行動キャンセル時は終了

		Debug.Log ("PlayAttackEffect start");
		// 各攻撃に応じたエフェクトの生成
		Debug.Log (debugTime + " <color=blue>DoAttack エフェクト再生:" + currentActionKey.ToString() + "</color>");
		if (EnemyActionGroupModel.IsShortRangeAttackActionKey(currentActionKey)) CreateShortRangeAttackEffect((int)currentActionKey);
		if (EnemyActionGroupModel.IsLongRangeAttackActionKey(currentActionKey)) CreateLongRangeAttackEffect((int)currentActionKey);
		if (EnemyActionGroupModel.ActionKey.SkillAttack.Equals(currentActionKey)) SkillAttackEffect((int)currentActionKey);
	}

	/*************************************************************
	 * アニメーション再生終了イベント
	 *************************************************************/
	public void EndAttackMotion() {
		Debug.Log ("EndAttackMotion");
		StartCoroutine(EndAttackMotionCoroutine());
	}

	/*************************************************************
	 * アニメーション再生終了イベント - コルーチン処理
	 *************************************************************/
	IEnumerator EndAttackMotionCoroutine() {
		if (reservedActions.Count < 1) yield break;	// 行動キャンセル時は終了

		Debug.Log ("EndAttackMotionCoroutine");
		reservedActions.RemoveAt(0);
		if (reservedActions.Count < 1) {
			yield return new WaitForSeconds (2f);
			state = EnemyState.Wait;
		} else {
			yield return new WaitForSeconds(0.2f);
		} 
		isAttack = false;
	}

	/*************************************************************
	 * 近距離攻撃エフェクトの生成
	 *************************************************************/
	private void CreateShortRangeAttackEffect(int actionKeyIndex) {
		Vector3 hitEffectPosition = transform.position + transform.forward * 2;
		hitEffectPosition.y += 1;
		GameObject attackPrefab = (GameObject)Resources.Load(enemyModel.attackParticleNames.Split(',')[actionKeyIndex]);
		// TODO:InstantiateはここではなくInitialize時に行ってプールしておく
		GameObject attackParticle = Instantiate(attackPrefab, hitEffectPosition, Quaternion.identity) as GameObject;
		attackParticle.transform.SetParent(battleObjectRoot.transform);

		EnemyAttack enemyAttack = attackParticle.GetComponent<EnemyAttack>();
		enemyAttack.actionKey = (EnemyActionGroupModel.ActionKey)actionKeyIndex;
		enemyAttack.enemyModel = enemyModel;
	}

	/*************************************************************
	 * 遠距離攻撃エフェクトの生成
	 *************************************************************/
	private void CreateLongRangeAttackEffect(int actionKeyIndex) {
		Vector3 hitEffectPosition = transform.position + transform.forward * 2;
		hitEffectPosition.y += 1.5f;
		GameObject attackPrefab = (GameObject)Resources.Load(enemyModel.attackParticleNames.Split(',')[actionKeyIndex]);
		// TODO:InstantiateはここではなくInitialize時に行ってプールしておく
		GameObject attackParticle = Instantiate(attackPrefab, hitEffectPosition, transform.rotation) as GameObject;
		attackParticle.transform.SetParent(battleObjectRoot.transform);

		EnemyAttack enemyAttack = attackParticle.GetComponent<EnemyAttack>();
		enemyAttack.actionKey = (EnemyActionGroupModel.ActionKey)actionKeyIndex;
		enemyAttack.enemyModel = enemyModel;

		// エフェクトを動かす
		transform.LookAt(target.transform);
		attackParticle.GetComponent<Rigidbody>().AddForce(transform.forward * longRangeAttackSpeed);
	}

	/*************************************************************
	 * スキル攻撃エフェクトの生成
	 *************************************************************/
	private void SkillAttackEffect(int actionKeyIndex) {
		// TODO:スキル攻撃エフェクトの設定　とりあえず衝撃波を飛ばすだけ
	}

	/*************************************************************
	 * ステップの共通事前処理
	 *************************************************************/
	private void DoStepBefore(EnemyActionGroupModel.ActionKey actionKey) {
		isStepping = true;
		animator.SetBool("Run", false);
		// TODO:ステップアニメーションの差し替え
		animator.SetTrigger("Step");

		// 着地点と移動速度を算出
		float[] positions = EnemyActionGroupModel.StepRangeByActionKey(actionKey);
		float forwardDistance = (UnityEngine.Random.Range(positions[0], positions[1]) + UnityEngine.Random.Range(positions[0], positions[1])) / 2f;
		float rightDistance = (UnityEngine.Random.Range(positions[2], positions[3]) + UnityEngine.Random.Range(positions[2], positions[3])) / 2f;
		//sideStepDistance = sideStepDistance - (sideStepDistance/2);
		//Debug.Log ("ステップ座標　forwardDistance : " + forwardDistance + ", rightDistance : " + rightDistance);
		Vector3 destinationTransform = transform.position;
		destinationTransform += transform.forward * forwardDistance;
		destinationTransform += transform.right * rightDistance;
		//float gravity = 10f;
		//tran.y += gravity * Time.deltaTime;
		stepDestination = destinationTransform;
		float distance = Vector3.Distance(transform.position, stepDestination);
		// TODO:スピード要動作確認
		stepSpeed = distance / 0.3f;
	}

	/*************************************************************
	 * ステップ処理
	 *************************************************************/
	IEnumerator DoStep(EnemyActionGroupModel.ActionKey actionKey) {
		if (reservedActions.Count < 1) yield break;	// 行動キャンセル時は終了
		MoveByStep();
		yield return new WaitForSeconds(0.5f);
		if (reservedActions.Count > 0) {
			reservedActions.RemoveAt(0);
			if (reservedActions.Count < 1) {
				state = EnemyState.Wait;
			}
		}
		isStepping = false;
	}

	/*************************************************************
	 * ステップによる移動
	 *************************************************************/
	private void MoveByStep() {
		// TODO:ジャンプの軌道を加える
		Vector3 direction = (stepDestination - transform.position).normalized * stepSpeed;
		stepDestination.y = transform.position.y;
		direction += Vector3.down * 9.81f * Time.deltaTime;
		float distance = Vector3.Distance(transform.position, stepDestination);
		if (distance > 1.0f) {
			enemyController.Move(direction * Time.deltaTime);
		}
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
		//Debug.Log (debugTime + " <color=blue>OnCollisionEnter スタート</color>");
		GameObject magic = col.gameObject;
		MagicController magicController = magic.GetComponent<MagicController>();
		// ステップ中は無敵、ダメージ中は重ねてダメージを受けない
		if (state == EnemyState.Die || magic.tag != "PlayerAttackTag" || 
			!magicController || isStepping || isDamage) {
			magic.SetActive(false);
			return;
		}

		// プレイヤー情報の取得、すでに死んでいたら処理終了
		GameObject playerObj = GameObject.FindWithTag("PlayerTag");
		PlayerController playerController = playerObj.GetComponent<PlayerController>();
		PlayerModel playerModel = playerController.playerModel;
		if (playerController.isDead) {
			magic.SetActive(false);
			return;
		}

		// 発射位置との距離によって命中率が変動し、それを元に命中判定を行い、ヒットしてなければ処理終了
		float distance = Vector3.Distance(transform.position, magicController.startPosition);
		bool isHit = BattleCalculator.IsHitEnemy(playerModel, magicController.magicModel, enemyModel, distance);
		if (!isHit) {
			// TODO:miss表記を出す
			magic.SetActive(false);
			return;
		}

		// 衝突エフェクトを表示する
		magicController.CallImpact();

		// ダメージ計算をする
		int damage = 0;
		if (BattleCalculator.IsHitEnemy(playerModel, magicController.magicModel, enemyModel, distance)) {
			damage = BattleCalculator.GetEnemyDamage(playerModel, magicController.magicModel, enemyModel);
		}

		// ダメージを受ける。HPが0以下になったら死ぬ。
		if (enemyModel.hp < damage) {
			StartCoroutine(Die(playerModel));
		} else {
			enemyModel.hp -= damage;
			// TODO:ダメージ表記を出す
			hpSlider.value = (float)enemyModel.hp / enemyModel.maxHp;

			// バトルアクション中の場合は、チャージ攻撃のみ被ダメアニメーションになって行動予約リストをキャンセルする
			bool isChargeAttack = false;
			if (!currentActionKey.Equals(EnemyActionGroupModel.ActionKey.Null)) {
				isChargeAttack = EnemyActionGroupModel.IsChargeAttackActionKey(currentActionKey);
			}
			if (state == EnemyState.Wait || state == EnemyState.Chase || (isAttack && isChargeAttack)) {
				Debug.Log (debugTime + " <color=green>被ダメモーション開始</color>");
				StartCoroutine(Damaging());
			}
		}
		magic.SetActive(false);
	}

	/*************************************************************
	 * ダメージ中状態
	 *************************************************************/
	IEnumerator Damaging() {
		animator.SetTrigger ("Damage");
		isDamage = true;
		isAttack = false;
		isStepping = false;
		StopCoroutine("DoAttack");
		StopCoroutine("DoStep");
		reservedActions = new List<EnemyActionGroupModel.ActionKey>();
		state = EnemyState.Wait;

		yield return new WaitForSeconds(0.5f);
		isDamage = false;
		Debug.Log (debugTime + " <color=green>被ダメモーション終了</color>");
	}

	/*************************************************************
	 * 死んだ処理
	 *************************************************************/
	IEnumerator Die(PlayerModel playerModel) {
		// 状態遷移
		state = EnemyState.Die;
		animator.SetTrigger("Die");

		// パラメータ変更・経験値を獲得
		enemyModel.hp = 0;
		hpSlider.value = 0f;
		playerModel.AddExp(enemyModel.exp);
		playerController.DisplayStatus();

		yield return new WaitForSeconds(3f);
		// TODO:消滅エフェクトをいれる
		this.gameObject.SetActive(false);
	}
}
