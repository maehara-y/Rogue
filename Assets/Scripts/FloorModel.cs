using UnityEngine;
using System.Collections;

public class FloorModel : MonoBehaviour {

	[SerializeField] public int id { get; set; }
	[SerializeField] public int floorNumber { get; set; }
	[SerializeField] public int floorType { get; set; }
	[SerializeField] public int isStairs { get; set; }
	[SerializeField] public int roomLength { get; set; }
	[SerializeField] public int branchCount { get; set; }
	[SerializeField] public int branchLength { get; set; }
	[SerializeField] public int enemyGroupId { get; set; }
	[SerializeField] public float minEnemyRate { get; set; }
	[SerializeField] public float maxEnemyRate { get; set; }
	[SerializeField] public int dropItemPatternId { get; set; }
	[SerializeField] public float minItemRate { get; set; }
	[SerializeField] public float maxItemRate { get; set; }
	[SerializeField] public int gimmickPatternId { get; set; }
	[SerializeField] public float minGimmickRate { get; set; }
	[SerializeField] public float maxGimmickRate { get; set; }
	[SerializeField] public string memo { get; set; }

	public void Initialize() {
		// TODO:実際はcurrentFloorIdをキーにマスターから取得する
		SetTestData();
	}

	void SetTestData() {
	}
}
