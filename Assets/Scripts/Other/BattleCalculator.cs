using UnityEngine;
using System.Collections;

public class BattleCalculator : MonoBehaviour {

	/*************************************************************
	 * 敵攻撃時のプレイヤー側ダメージ計算を行う
	 *************************************************************/
	public static int GetPlayerDamage(PlayerModel player, EnemyModel enemy, EnemyActionGroupModel.ActionKey actionKey) {
		// TODO:計算式記述
		return 15;
	}

	/*************************************************************
	 * プレイヤー攻撃時の敵側ダメージ計算を行う
	 *************************************************************/
	public static int GetEnemyDamage(PlayerModel player, MagicModel magic, EnemyModel enemy) {
		// TODO:計算式記述
		return 50;
	}

	/*************************************************************
	 * プレイヤー攻撃時の命中判定を行う
	 *************************************************************/
	public static bool IsHitEnemy(PlayerModel player, MagicModel magic, EnemyModel enemy, float distance) {
		// TODO:計算式記述 とりあえず半々の確率で命中
		if (Random.value > 0.5f) return false; 
		return true;
	}

	/*************************************************************
	 * 敵攻撃時の命中判定を行う
	 *************************************************************/
	public static bool IsHitPlayer(PlayerModel player, EnemyModel enemy) {
		// TODO:計算式記述
		return true;
	}
}
