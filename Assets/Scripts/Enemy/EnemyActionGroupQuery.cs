using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionGroupQuery {

	/*************************************************************
	 * グループIDから該当するモデルのリストを取得する
	 *************************************************************/
	public static List<EnemyActionGroupModel> FindByGroupId(int groupId) {
		// TODO:テスト用
		List<EnemyActionGroupModel> list = new List<EnemyActionGroupModel>();
		EnemyActionGroupModel model1 = new EnemyActionGroupModel();
		EnemyActionGroupModel model2 = new EnemyActionGroupModel();
		EnemyActionGroupModel model3 = new EnemyActionGroupModel();
		EnemyActionGroupModel model4 = new EnemyActionGroupModel();
		EnemyActionGroupModel model5 = new EnemyActionGroupModel();
		list.Add(model1);
		list.Add(model2);
		list.Add(model3);
		list.Add(model4);
		list.Add(model5);
		return list;
	}

	/*************************************************************
	 * グループIDから該当するモデルのリストを取得し、その中から行動パターンを選定する
	 *************************************************************/
	public static List<EnemyActionGroupModel.ActionKey> ChooseActionPatternByGroupId(int groupId) {
		List<EnemyActionGroupModel> group = FindByGroupId(groupId);
		for (int i=0; i < group.Count; i++) {
			// TODO:ランダム選出する
			EnemyActionGroupModel model = group[i];
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
			result.Add(EnemyActionGroupModel.ActionKey.BackStep);
			result.Add(EnemyActionGroupModel.ActionKey.BackStep);
			result.Add(EnemyActionGroupModel.ActionKey.LongRangeNormalAttack);
		} else if (groupId == 2) {
			result.Add(EnemyActionGroupModel.ActionKey.LongRangeQuickAttack);
			result.Add(EnemyActionGroupModel.ActionKey.LongRangeQuickAttack);
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
