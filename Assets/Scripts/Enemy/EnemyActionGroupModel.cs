using UnityEngine;
using System.Collections;
using System;

public class EnemyActionGroupModel {

	public static string[] columNames = {"id", "group_id", "action_keys", "rate"};
	[SerializeField] public int id { get; set; }
	[SerializeField] public int groupId { get; set; }
	[SerializeField] public string actionKeys { get; set; }
	[SerializeField] public float rate { get; set; }

	public enum ActionKey { 
		Null, ShortRangeNormalAttack, ShortRangeQuickAttack, ShortRangeChargeAttack,
		LongRangeNormalAttack, LongRangeQuickAttack, LongRangeChargeAttack,
		SkillAttack, BackStep, BackLeftStep, BackCenterStep, BackRightStep, 
		FrontStep, FrontLeftStep, FrontCenterStep, FrontRightStep, 
		SideStep, LeftStep, RightStep
	}

	/*************************************************************
	 * 全てのフィールドにマスターデータをセットする
	 *************************************************************/
	public void setFieldsFromDataRow(DataRow dataRow) {
		if (dataRow.Count < 1) return;

		if (dataRow["id"] != null) id = (int)dataRow["id"];
		if (dataRow["group_id"] != null) groupId = (int)dataRow["group_id"];
		if (dataRow["action_keys"] != null) actionKeys = (string)dataRow["action_keys"];
		if (dataRow["rate"] != null) rate = (float)Convert.ToDouble(dataRow["rate"]);
	}

	/*************************************************************
	 * アクションキーが攻撃かどうか
	 *************************************************************/
	public static bool IsAttackActionKey(ActionKey actionKey) {
		if (ActionKey.ShortRangeNormalAttack.Equals(actionKey) 
			|| ActionKey.ShortRangeQuickAttack.Equals(actionKey)
			|| ActionKey.ShortRangeChargeAttack.Equals(actionKey) 
			|| ActionKey.LongRangeNormalAttack.Equals(actionKey)
			|| ActionKey.LongRangeQuickAttack.Equals(actionKey) 
			|| ActionKey.LongRangeChargeAttack.Equals(actionKey)
			|| ActionKey.SkillAttack.Equals(actionKey)) {
			return true;
		}
		return false;
	}

	/*************************************************************
	 * アクションキーがステップかどうか
	 *************************************************************/
	public static bool IsStepActionKey(ActionKey actionKey) {
		if (ActionKey.BackStep.Equals(actionKey) || ActionKey.BackLeftStep.Equals(actionKey) 
			|| ActionKey.BackCenterStep.Equals(actionKey) || ActionKey.BackRightStep.Equals(actionKey)
			|| ActionKey.FrontStep.Equals(actionKey) || ActionKey.FrontLeftStep.Equals(actionKey)
			|| ActionKey.FrontCenterStep.Equals(actionKey) || ActionKey.FrontRightStep.Equals(actionKey)
			|| ActionKey.SideStep.Equals(actionKey) || ActionKey.LeftStep.Equals(actionKey)
			|| ActionKey.RightStep.Equals(actionKey)) {
			return true;
		}
		return false;
	}

	/*************************************************************
	 * アクションキーが近距離攻撃かどうか
	 *************************************************************/
	public static bool IsShortRangeAttackActionKey(ActionKey actionKey) {
		if (ActionKey.ShortRangeNormalAttack.Equals(actionKey) 
			|| ActionKey.ShortRangeQuickAttack.Equals(actionKey)
			|| ActionKey.ShortRangeChargeAttack.Equals(actionKey)) {
			return true;
		}
		return false;
	}

	/*************************************************************
	 * アクションキーが遠距離攻撃かどうか
	 *************************************************************/
	public static bool IsLongRangeAttackActionKey(ActionKey actionKey) {
		if (ActionKey.LongRangeNormalAttack.Equals(actionKey)
			|| ActionKey.LongRangeQuickAttack.Equals(actionKey) 
			|| ActionKey.LongRangeChargeAttack.Equals(actionKey)) {
			return true;
		}
		return false;
	}

	/*************************************************************
	 * アクションキーが遠距離攻撃かどうか
	 *************************************************************/
	public static bool IsChargeAttackActionKey(ActionKey actionKey) {
		if (ActionKey.ShortRangeChargeAttack.Equals(actionKey)
			|| ActionKey.LongRangeChargeAttack.Equals(actionKey)) {
			return true;
		}
		return false;
	}

	/*************************************************************
	 * actionKeyからステップの移動範囲の各座標配列を取得する
	 * 0:forwardの最小値、1:forwardの最大値、2:rightの最小値、3:rightの最大値
	 *************************************************************/
	public static float[] StepRangeByActionKey(ActionKey actionKey) {
		float[] positions = null;
		if (ActionKey.BackStep.Equals(actionKey)) {
			positions = new float[] { -6f, -2f, -6f,  6f };
		} else if (ActionKey.BackLeftStep.Equals(actionKey)) {
			positions = new float[] { -6f, -2f, -6f, -2f };
		} else if (ActionKey.BackCenterStep.Equals(actionKey)) {
			positions = new float[] { -6f, -2f, -2f,  2f };
		} else if (ActionKey.BackRightStep.Equals(actionKey)) {
			positions = new float[] { -6f, -2f,  2f,  6f };
		} else if (ActionKey.FrontStep.Equals(actionKey)) {
			positions = new float[] {  2f,  6f, -6f,  6f };
		} else if (ActionKey.FrontLeftStep.Equals(actionKey)) {
			positions = new float[] {  2f,  6f, -6f, -2f };
		} else if (ActionKey.FrontCenterStep.Equals(actionKey)) {
			positions = new float[] {  2f,  6f, -2f,  2f };
		} else if (ActionKey.FrontRightStep.Equals(actionKey)) {
			positions = new float[] {  2f,  6f,  2f,  6f };
		} else if (ActionKey.SideStep.Equals(actionKey)) {
			positions = new float[] { -2f,  2f, -6f,  6f };
		} else if (ActionKey.LeftStep.Equals(actionKey)) {
			positions = new float[] { -2f,  2f, -6f, -2f };
		} else if (ActionKey.RightStep.Equals(actionKey)) {
			positions = new float[] { -2f,  2f,  2f,  6f };
		}
		return positions;
	}
}
