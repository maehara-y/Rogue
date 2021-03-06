﻿using UnityEngine;
using System.Collections;

public class ItemModel {

	[SerializeField] public int id { get; set; }
	[SerializeField] public string itemName { get; set; }
	[SerializeField] public string prefabName { get; set; }
	[SerializeField] public string description { get; set; }
	[SerializeField] public int itemType { get; set; }
	[SerializeField] public int effectValue { get; set; }
	[SerializeField] public int magicId { get; set; }
	[SerializeField] public int attribute { get; set; }
	[SerializeField] public int targetStatus { get; set; }
	[SerializeField] public int usableCount { get; set; }
	[SerializeField] public string memo { get; set; }

	/*************************************************************
	 * 初期処理
	 *************************************************************/
	public void Initialize() {
		// TODO:実際はcurrentFloorIdをキーにマスターから取得する
		SetTestData();
	}

	/*************************************************************
	 * デバッグ用のデータ設定
	 *************************************************************/
	void SetTestData() {
		itemName = "ポーション";
		description = "HPを10回復します";
		effectValue = 10;
	}
}
