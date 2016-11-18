using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCalculator : MonoBehaviour {

	/*************************************************************
	 * 経験値を加算する
	 *************************************************************/
	public static int[] AddExp(int level, int currentExp, int getExp) {
		// TODO:最大レベル時はそのままreturn;

		int[] newInfo = new int[2];
		int needExp = GetLevelUpExp(level);
		if (currentExp + getExp >= needExp) {
			newInfo[0] = level+1;
			newInfo[1] = needExp - (currentExp + getExp);
		} else {
			newInfo[1] = currentExp + getExp;
		}
		return newInfo;
	}

	/*************************************************************
	 * 次のレベルアップに必要な経験値を取得する
	 *************************************************************/
	public static int GetLevelUpExp(int currentlevel) {
		// TODO:計算式記述　以下は一時処理
		int exp = currentlevel * 30;
		return exp;
	}

	/*************************************************************
	 * フロア階層からモンスターのレベルを算出する
	 *************************************************************/
	public static int GetEnemyLevel(int floorNumber) {
		int minLevel = (floorNumber * 3 - 3 < 1) ? 1 : floorNumber * 3 - 3;
		int maxLevel = floorNumber * 3 + 3;
		int enemyLevel = (Random.Range(minLevel, maxLevel) + Random.Range(minLevel, maxLevel) + Random.Range(minLevel, maxLevel)) / 3;
		return enemyLevel;
	}

	/*************************************************************
	 * モンスターのレベルからステータスを算出する
	 *************************************************************/
	public static EnemyModel GetEnemyStatusByLevel(int level, EnemyModel model) {
		// TODO:計算式は仮
		model.maxHp = model.baseMaxHp;
		model.atk = model.baseAtk;
		model.def = model.baseDef;
		model.dex = model.baseDex;
		model.agi = model.baseAgi;
		model.exp = model.baseExp;
		return model;
	}
}
