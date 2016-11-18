using UnityEngine;
using System.Collections;

public class FloorQuery : MonoBehaviour {

	/*************************************************************
	 * IDから1件のモデルを取得する
	 *************************************************************/
	public static FloorModel FindById(int id) {
		// TODO:テスト用
		FloorModel model = new FloorModel();
		model.id = id;
		model.floorNumber = id;
		model.floorType = 1;
		model.isStairs = true;
		model.roomLength = 5;
		model.branchCount = 4;
		model.branchLength = 2;
		model.enemyGroupId = 1;
		model.minEnemyRate = 0.5f;
		model.maxEnemyRate = 0.8f;
		return model;
	}
}
