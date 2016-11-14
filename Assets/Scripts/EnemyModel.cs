using UnityEngine;
using System.Collections;

public class EnemyModel : MonoBehaviour {

	[SerializeField] public int id { get; set; }
	[SerializeField] public string enemyName { get; set; }
	[SerializeField] public string prefabName { get; set; }
	[SerializeField] public string attackParticleName { get; set; }
	[SerializeField] public int type { get; set; }
	[SerializeField] public int attribute { get; set; }
	[SerializeField] public int maxHp { get; set; }
	[SerializeField] public int hp { get; set; }
	[SerializeField] public int atk { get; set; }
	[SerializeField] public int def { get; set; }
	[SerializeField] public int dex { get; set; }
	[SerializeField] public int agi { get; set; }
	[SerializeField] public int skillGroupId { get; set; }	// TODO:どうする？
	[SerializeField] public int pattern_id { get; set; }	// TODO:どうする？
	[SerializeField] public float moveSpeed { get; set; }
	[SerializeField] public float chaseRange { get; set; }
	[SerializeField] public int exp { get; set; }
	[SerializeField] public int dropItemGroupId { get; set; }

	public void Initialize() {
		// TODO:実際はフロア階層(マスター)から対象のモンスター(マスター)とLvを出し、それに基づいたステータスをセットする
		SetTestData();
	}

	void SetTestData() {
		enemyName = "スケルトン";
		prefabName = "Skeleton";
		attackParticleName = "EnemyAttack/double-slash";
		type = 1;
		attribute = 0;
		maxHp = 200;
		hp = 200;
		atk = 100;
		def = 80;
		dex = 15;
		agi = 10;
		exp = 30;
	}
}
