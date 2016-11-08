using UnityEngine;
using System.Collections;

public class EnemyModel : MonoBehaviour {

	public int id { get; set; }
	public string enemyName { get; set; }
	public string prefabName { get; set; }
	public string attackParticleName { get; set; }
	public int type { get; set; }
	public int attribute { get; set; }
	public int maxHp { get; set; }
	public int hp { get; set; }
	public int atk { get; set; }
	public int def { get; set; }
	public int dex { get; set; }
	public int agi { get; set; }
	public int skillGroupId { get; set; }	// TODO:どうする？
	public int pattern_id { get; set; }	// TODO:どうする？
	public float moveSpeed { get; set; }
	public float chaseRange { get; set; }
	public int exp { get; set; }
	public int dropItemGroupId { get; set; }

	void Start() {
		// TODO:実際はフロア階層(マスター)から対象のモンスター(マスター)とLvを出し、それに基づいたステータスをセットする
		SetTestData();
	}

	void SetTestData() {
		enemyName = "スケルトン";
		prefabName = "Skelton";
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
