using UnityEngine;
using UnityEngine.UI;
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
	private bool availableLongAttack = true;		// 遠距離攻撃の判定をしてもいい状態か？
	private Vector3 stepDestination;				// ステップの着地点
	// 予約済バトル行動indexリスト
	private List<EnemyActionGroupModel.ActionKey> reservedActions = new List<EnemyActionGroupModel.ActionKey>();

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
			return;
		}

		// ステップ中は移動処理
		if (isStepping) {
			MoveByStep ();
			return;
		}

		// 攻撃アクション中は何もしない
		// TODO:敵が離れ過ぎた時に予約行動をキャンセルするとしたらこの中に記述
		if (isAttack) return;

		// バトル行動チェーン中なら残りの行動を継続する
		if (state == EnemyState.Battle && reservedActions.Count >= 1) {
			Debug.Log ("<color=yellow>バトル行動継続 残り:" + reservedActions.Count + "回, アクション:" + reservedActions[0].ToString() + "</color>");
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
			float randomVal = Random.value;
			// TODO:一時処理
			randomVal = 0.9f;
			if (enemyModel.longAttackHitRate >= randomVal) {
				Debug.Log ("<color=blue>ロングレンジ攻撃予約 randomVal:" + randomVal + "</color>");
				state = EnemyState.Battle;
				reservedActions = EnemyActionGroupQuery.ChooseActionPatternByGroupId (enemyModel.longRangeActionGroupId);
				DoAction (reservedActions [0]);
				return;
			} else {
				Debug.Log ("<color=blue>ロングレンジ攻撃なし randomVal:" + randomVal + "</color>");
			}
		}

		// プレイヤーとの距離に応じて攻撃・追跡・追跡停止を選択する
		if (distance < enemyModel.chaseRangeMin) {
			// 一定距離に縮まったら攻撃
			if (state != EnemyState.Battle) {
				state = EnemyState.Battle;
				reservedActions = EnemyActionGroupQuery.ChooseActionPatternByGroupId(enemyModel.shortRangeActionGroupId);
				DoAction(reservedActions[0]);
				Debug.Log ("<color=red>ショートレンジ攻撃予約 アクション:" + reservedActions[0].ToString() + "</color>");
			}

		} else if (distance > enemyModel.chaseRangeMax) {
			// 一定距離離れすぎたら追跡をやめる
			Debug.Log ("<color=white>追跡エンド</color>");
			state = EnemyState.Wait;
			animator.SetBool("Run", false);
			animator.SetBool("Idle", true);
		} else {
			// 追跡範囲内なら追跡
			Debug.Log ("<color=green>追跡スタート</color>");
			state = EnemyState.Chase;
			animator.SetBool("Idle", false);
			animator.SetBool("Run", true);
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
		if (EnemyActionGroupModel.IsAttackActionKey(actionKey)) {
			isAttack = true;
			animator.SetBool("Idle", false);
			animator.SetBool("Run", false);
			animator.SetTrigger(actionKey.ToString());
			StartCoroutine(DoAttack(actionKey));
		}
		if (EnemyActionGroupModel.IsStepActionKey(actionKey)) {
			if (!isStepping) DoStepBefore(actionKey);
			StartCoroutine(DoStep(actionKey));
		}
	}

	/*************************************************************
	 * 攻撃アクションの実行
	 *************************************************************/
	IEnumerator DoAttack(EnemyActionGroupModel.ActionKey actionKey) {
		yield return new WaitForSeconds(EnemyActionGroupModel.AttackWaitTimeActionKey(actionKey));
		// 各攻撃に応じたエフェクトの生成
		if (EnemyActionGroupModel.IsShortRangeAttackActionKey(actionKey)) CreateShortRangeAttackEffect((int)actionKey);
		if (EnemyActionGroupModel.IsLongRangeAttackActionKey(actionKey)) CreateLongRangeAttackEffect((int)actionKey);
		if (EnemyActionGroupModel.ActionKey.SkillAttack.Equals(actionKey)) SkillAttackEffect((int)actionKey);

		reservedActions.RemoveAt(0);
		if (reservedActions.Count < 1) {
			yield return new WaitForSeconds(2f);
			state = EnemyState.Wait;
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
		float forwardDistance = (Random.Range(positions[0], positions[1]) + Random.Range(positions[0], positions[1])) / 2f;
		float rightDistance = (Random.Range(positions[2], positions[3]) + Random.Range(positions[2], positions[3])) / 2f;
		//sideStepDistance = sideStepDistance - (sideStepDistance/2);
		Debug.Log ("ステップ座標　forwardDistance : " + forwardDistance + ", rightDistance : " + rightDistance);
		Vector3 destinationTransform = transform.position;
		destinationTransform += transform.forward * forwardDistance;
		destinationTransform += transform.right * rightDistance;
		//float gravity = 10f;
		//tran.y += gravity * Time.deltaTime;
		stepDestination = destinationTransform;
		float distance = Vector3.Distance(transform.position, stepDestination);
		// TODO:スピード要動作確認
		stepSpeed = distance / 0.5f;
	}

	/*************************************************************
	 * ステップ処理
	 *************************************************************/
	IEnumerator DoStep(EnemyActionGroupModel.ActionKey actionKey) {
		MoveByStep();
		yield return new WaitForSeconds(0.5f);
		reservedActions.RemoveAt(0);
		if (reservedActions.Count < 1) {
			state = EnemyState.Wait;
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
		Debug.Log ("OnCollisionEnter スタート");
		GameObject magic = col.gameObject;
		if (state == EnemyState.Die || magic.tag != "PlayerAttackTag") return;
		MagicController magicController = magic.GetComponent<MagicController>();
		if (!magicController || isStepping) return;	// ステップ中は無敵

		// プレイヤー情報の取得、すでに死んでいたら処理終了
		GameObject playerObj = GameObject.FindWithTag("PlayerTag");
		PlayerController playerController = playerObj.GetComponent<PlayerController>();
		PlayerModel playerModel = playerController.playerModel;
		if (playerController.isDead) return;

		// 発射位置との距離によって命中率が変動し、それを元に命中判定を行い、ヒットしてなければ処理終了
		float distance = Vector3.Distance(transform.position, magicController.startPosition);
		bool isHit = BattleCalculator.IsHitEnemy(playerModel, magicController.magicModel, enemyModel, distance);
		if (!isHit) return;
		Debug.Log ("isHit!!!!!");

		// 衝突エフェクトを表示する
		magicController.CallImpact();
		Debug.Log ("衝突パーティクル表示");

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
			hpSlider.value = (float)enemyModel.hp / enemyModel.maxHp;
			// TODO:チャージ攻撃のみ被ダメアニメーションになって行動予約リストをキャンセルする
			animator.SetTrigger("Damage");
			// TODO:被弾後、waitの時間を少し作りたい
		}
		magic.SetActive(false);

		// TODO:違うタイプの攻撃はここに実装していく？
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
