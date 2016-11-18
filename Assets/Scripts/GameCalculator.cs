using UnityEngine;
using System.Collections;

public class GameCalculator : MonoBehaviour {

	/*************************************************************
	 * 経験値を加算する
	 *************************************************************/
	public static int[] AddExp(int level, int currentExp, int getExp) {
		// TODO:最大レベル時はそのままreturn;

		int[] newInfo = new int[2];
		int needExp = getLevelUpExp(level);
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
	public static int getLevelUpExp(int currentlevel) {
		// TODO:計算式記述　以下は一時処理
		int exp = currentlevel * 30;
		return exp;
	}
}
