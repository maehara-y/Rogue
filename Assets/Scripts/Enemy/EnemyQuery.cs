using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyQuery {

	/*************************************************************
	 * IDから1件のモデルを取得する
	 *************************************************************/
	public static EnemyModel FindById(int id) {
		// TODO:テスト用
		EnemyModel model = new EnemyModel();
		return model;
	}

	/*************************************************************
	 * ID配列から複数件のモデルを取得する
	 *************************************************************/
	public static List<EnemyModel> FindByIds(int[] ids) {
		// TODO:テスト用
		EnemyModel model1 = new EnemyModel ();
		EnemyModel model2 = new EnemyModel ();
		EnemyModel model3 = new EnemyModel ();

		List<EnemyModel> models = new List<EnemyModel> ();
		models.Add (model1);
		models.Add (model2);
		models.Add (model3);
		return models;
	}

	/*************************************************************
	 * グループIDから1体のモンスターを抽出して取得する
	 *************************************************************/
	public static EnemyModel ChooseByGroupId(int enemyGroupId) {
		// TODO:テスト用
		// TODO:2の倍数、3の倍数、それ以外でモンスターの種類を分けてみる
		EnemyModel model = new EnemyModel();
		model.enemyName = "スケルトン";
		model.prefabName = "Enemy/Skeleton";
		model.attackParticleName = "EnemyAttack/double-slash";
		model.enemyType = 1;
		model.attribute = 0;
		model.hp = 200;
		model.baseMaxHp = 200;
		model.baseAtk = 100;
		model.baseDef = 80;
		model.baseDex = 15;
		model.baseAgi = 10;
		model.baseExp = 30;
		model.chaseRange = 3f;
		return model;
	}
}
