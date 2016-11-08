using UnityEngine;
using System.Collections;

public class BattleCalculator : MonoBehaviour {

	// 敵攻撃時のプレイヤー側ダメージ計算
	public static int GetPlayerDamage(PlayerModel player, MagicModel magic, EnemyModel enemy) {
		// TODO:計算式記述
		return 30;
	}

	// プレイヤー攻撃時の敵側ダメージ計算
	public static int GetEnemyDamage(PlayerModel player, MagicModel magic, EnemyModel enemy) {
		// TODO:計算式記述
		return 50;
	}

	// プレイヤー攻撃時の命中判定
	public static bool IsHitEnemy(PlayerModel player, MagicModel magic, EnemyModel enemy) {
		// TODO:計算式記述
		return true;
	}

	// 敵攻撃時の命中判定
	public static bool IsHitPlayer(PlayerModel player, MagicModel magic, EnemyModel enemy) {
		// TODO:計算式記述
		return true;
	}
}
