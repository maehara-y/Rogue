using UnityEngine;
using System.Collections;

public class EnemyModel {

	[SerializeField] public int id { get; set; }
	[SerializeField] public string enemyName { get; set; }
	[SerializeField] public string prefabName { get; set; }
	[SerializeField] public string attackParticleName { get; set; }
	[SerializeField] public int enemyType { get; set; }
	[SerializeField] public int level { get; set; }
	[SerializeField] public int attribute { get; set; }
	[SerializeField] public int baseMaxHp { get; set; }
	[SerializeField] public int maxHp { get; set; }
	[SerializeField] public int hp { get; set; }
	[SerializeField] public int baseAtk { get; set; }
	[SerializeField] public int baseDef { get; set; }
	[SerializeField] public int baseDex { get; set; }
	[SerializeField] public int baseAgi { get; set; }
	[SerializeField] public int atk { get; set; }
	[SerializeField] public int def { get; set; }
	[SerializeField] public int dex { get; set; }
	[SerializeField] public int agi { get; set; }
	[SerializeField] public int skillGroupId { get; set; }	// TODO:どうする？
	[SerializeField] public int pattern_id { get; set; }	// TODO:どうする？
	[SerializeField] public float moveSpeed { get; set; }
	[SerializeField] public float chaseRange { get; set; }
	[SerializeField] public int baseExp { get; set; }
	[SerializeField] public int exp { get; set; }
	[SerializeField] public int dropItemGroupId { get; set; }

}
