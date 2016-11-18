using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicQuery {

	public static MagicModel findById(int id) {
		// TODO:テスト用
		MagicModel model = new MagicModel();
		switch (id) {
		case 1:
			model.id = 1;
			model.magicName = "ファイアーボール";
			model.prefabName = "Magic/FireMissileOBJ";
			model.animationName = "MeleeAttack01";
			model.description = "炎の弾を放ちます";
			model.masterRate = 0.25f;
			model.magicType = 1;
			model.useMp = 5;
			model.attribute = 1;
			model.effectValue = 100;
			model.targetStatus = 0;
			model.firingRange = 12f;		// TODO:未実装
			model.speed = 300f;
			model.strikeRate = 0.8f;
			model.duration = 0;
			break;
		case 2:
			model.id = 2;
			model.magicName = "バブルボール";
			model.prefabName = "Magic/BubbleMissileOBJ";
			model.animationName = "MeleeAttack01";
			model.description = "泡の弾を放ちます";
			model.masterRate = 0.25f;
			model.magicType = 1;
			model.useMp = 8;
			model.attribute = 1;
			model.effectValue = 100;
			model.targetStatus = 0;
			model.firingRange = 12f;		// TODO:未実装
			model.speed = 300f;
			model.strikeRate = 0.8f;
			model.duration = 0;
			break;
		case 3:
			model.id = 3;
			model.magicName = "コスミックボール";
			model.prefabName = "Magic/CosmicMissileOBJ";
			model.animationName = "MeleeAttack01";
			model.description = "異次元の弾を放ちます";
			model.masterRate = 0.25f;
			model.magicType = 1;
			model.useMp = 10;
			model.attribute = 1;
			model.effectValue = 100;
			model.targetStatus = 0;
			model.firingRange = 12f;		// TODO:未実装
			model.speed = 300f;
			model.strikeRate = 0.8f;
			model.duration = 0;
			break;
		}
		return model;
	}

	public static List<MagicModel> findByIds(int[] ids) {
		// TODO:テスト用
		MagicModel model1 = new MagicModel();
		model1.id = 1;
		model1.magicName = "ファイアーボール";
		model1.prefabName = "Magic/FireMissileOBJ";
		model1.animationName = "MeleeAttack01";
		model1.description = "炎の弾を放ちます";
		model1.masterRate = 0.25f;
		model1.magicType = 1;
		model1.useMp = 5;
		model1.attribute = 1;
		model1.effectValue = 100;
		model1.targetStatus = 0;
		model1.firingRange = 12f;		// TODO:未実装
		model1.speed = 300f;
		model1.strikeRate = 0.8f;
		model1.duration = 0;

		MagicModel model2 = new MagicModel();
		model2.id = 2;
		model2.magicName = "バブルボール";
		model2.prefabName = "Magic/BubbleMissileOBJ";
		model2.animationName = "MeleeAttack01";
		model2.description = "泡の弾を放ちます";
		model2.masterRate = 0.25f;
		model2.magicType = 1;
		model2.useMp = 8;
		model2.attribute = 1;
		model2.effectValue = 100;
		model2.targetStatus = 0;
		model2.firingRange = 12f;		// TODO:未実装
		model2.speed = 300f;
		model2.strikeRate = 0.8f;
		model2.duration = 0;

		MagicModel model3 = new MagicModel();
		model3.id = 3;
		model3.magicName = "コスミックボール";
		model3.prefabName = "Magic/CosmicMissileOBJ";
		model3.animationName = "MeleeAttack01";
		model3.description = "異次元の弾を放ちます";
		model3.masterRate = 0.25f;
		model3.magicType = 1;
		model3.useMp = 10;
		model3.attribute = 1;
		model3.effectValue = 100;
		model3.targetStatus = 0;
		model3.firingRange = 12f;		// TODO:未実装
		model3.speed = 300f;
		model3.strikeRate = 0.8f;
		model3.duration = 0;

		List<MagicModel> models = new List<MagicModel>();
		models.Add(model1);
		models.Add(model2);
		models.Add(model3);
		return models;
	}
}
