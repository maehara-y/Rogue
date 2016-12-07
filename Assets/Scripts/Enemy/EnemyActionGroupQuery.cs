using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionGroupQuery {

	private static string tableName = "m_enemy_action_group";

	/*************************************************************
	 * グループIDから該当するモデルのリストを取得する
	 *************************************************************/
	public static List<EnemyActionGroupModel> FindByGroupId(int groupId) {

		string selectQuery = "select * from " + tableName + " where group_id = " + groupId;
		DataTable dataTable = SqliteDatabase.Instance.ExecuteQuery(selectQuery);

		List<EnemyActionGroupModel> enemyActionGroupModels = new List<EnemyActionGroupModel>();
		foreach(DataRow dataRow in dataTable.Rows){
			EnemyActionGroupModel model = new EnemyActionGroupModel();
			model.setFieldsFromDataRow(dataRow);
			enemyActionGroupModels.Add(model);
		}
		return enemyActionGroupModels;
	}

	/*************************************************************
	 * グループIDから該当するモデルのリストを取得し、その中から行動パターンを選定する
	 *************************************************************/
	public static List<EnemyActionGroupModel.ActionKey> ChooseActionPatternByGroupId(int groupId) {
		List<EnemyActionGroupModel> group = FindByGroupId(groupId);
		for (int i=0; i < group.Count; i++) {
			// TODO:ランダム選出する
			//EnemyActionGroupModel model = group[i];
			//model.rate;
		}
		/*
		EnemyActionGroupModel choicedModel = group[0];
		string[] indexes = choicedModel.actionKeys.Split(',');
		List<string> result = indexisToKeys(indexes);
		*/

		// TODO:テスト実装
		List<EnemyActionGroupModel.ActionKey> result = new List<EnemyActionGroupModel.ActionKey>();
		if (groupId == 1) {
			result.Add(EnemyActionGroupModel.ActionKey.ShortRangeQuickAttack);
			result.Add(EnemyActionGroupModel.ActionKey.ShortRangeQuickAttack);
			result.Add(EnemyActionGroupModel.ActionKey.ShortRangeChargeAttack);
			result.Add(EnemyActionGroupModel.ActionKey.BackCenterStep);
			result.Add(EnemyActionGroupModel.ActionKey.FrontCenterStep);
			result.Add(EnemyActionGroupModel.ActionKey.BackCenterStep);
			result.Add(EnemyActionGroupModel.ActionKey.LongRangeNormalAttack);

			/*ステップテスト
			result.Add(EnemyActionGroupModel.ActionKey.LeftStep);
			result.Add(EnemyActionGroupModel.ActionKey.BackLeftStep);
			result.Add(EnemyActionGroupModel.ActionKey.BackRightStep);
			result.Add(EnemyActionGroupModel.ActionKey.RightStep);
			result.Add(EnemyActionGroupModel.ActionKey.RightStep);
			result.Add(EnemyActionGroupModel.ActionKey.FrontRightStep);
			result.Add(EnemyActionGroupModel.ActionKey.FrontLeftStep);
			result.Add(EnemyActionGroupModel.ActionKey.LeftStep);*/
		} else if (groupId == 2) {
			result.Add(EnemyActionGroupModel.ActionKey.LongRangeQuickAttack);
			result.Add(EnemyActionGroupModel.ActionKey.LongRangeQuickAttack);
			result.Add(EnemyActionGroupModel.ActionKey.BackCenterStep);
			result.Add(EnemyActionGroupModel.ActionKey.LongRangeChargeAttack);
		}
		return result;
	}

	/*************************************************************
	 * グループIDから該当するモデルのリストを取得し、その中から行動パターンを選定する
	 *************************************************************/
	public static List<EnemyActionGroupModel.ActionKey> indexisToKeys(string[] indexes) {
		List<EnemyActionGroupModel.ActionKey> actionKeys = new List<EnemyActionGroupModel.ActionKey>();
		for (int i=0; i < indexes.Length; i++) {
			EnemyActionGroupModel.ActionKey actionKey = (EnemyActionGroupModel.ActionKey)Enum.ToObject(typeof(EnemyActionGroupModel.ActionKey), indexes[i]);
			actionKeys.Add(actionKey);
		}
		return actionKeys;
	}
}
