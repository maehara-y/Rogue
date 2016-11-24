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
		// TODO:テスト用　ランダムに3種類ぐらい分けてみる
		EnemyModel model = new EnemyModel();
		model.enemyType = 1;
		model.attribute = 0;
		model.hp = 200;
		model.baseMaxHp = 200;
		model.baseAtk = 100;
		model.baseDef = 80;
		model.baseDex = 15;
		model.baseAgi = 10;
		model.baseExp = 30;
		model.chaseRangeMin = 5f;
		model.chaseRangeMax = 20f;
		model.shortRangeActionGroupId = 1;
		model.longRangeActionGroupId = 2;
		model.longAttackHitRate = 0.2f;
		model.longAttackRangeMin = 6.0f;
		model.longAttackRangeMax = 18.0f;
		model.attackDamageRates = "1,0.5,2,1.6,0.8,3.2,5";
		int index = Random.Range(1,4);
		switch(index) {
		case 1 :
			model.enemyName = "スケルトン";
			model.prefabName = "Enemy/Skeleton";
			model.prefabName = "Enemy/Demon";
			model.attackParticleNames = "EnemyAttack/double-slash,EnemyAttack/double-slash,EnemyAttack/double-slash,EnemyAttack/DeathMissile,EnemyAttack/SlimeMissile,EnemyAttack/BloodMissile";
			break;
		case 2:
			model.enemyName = "アークデーモン";
			model.prefabName = "Enemy/Demon";
			model.attackParticleNames = "EnemyAttack/double-slash,EnemyAttack/double-slash,EnemyAttack/double-slash,EnemyAttack/DeathMissile,EnemyAttack/SlimeMissile,EnemyAttack/BloodMissile";
			break;
		case 3 :
			model.enemyName = "炎獣イフリート";
			model.prefabName = "Enemy/Ifrit";
			model.prefabName = "Enemy/Demon";
			model.attackParticleNames = "EnemyAttack/double-slash,EnemyAttack/double-slash,EnemyAttack/double-slash,EnemyAttack/DeathMissile,EnemyAttack/SlimeMissile,EnemyAttack/BloodMissile";
			break;
		}
		return model;
	}
}
