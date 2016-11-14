using UnityEngine;
using System.Collections;

public class FloorModel : MonoBehaviour {

	[SerializeField] public int id { get; set; }
	[SerializeField] public int floor_number { get; set; }
	[SerializeField] public int type { get; set; }
	[SerializeField] public int is_stairs { get; set; }
	[SerializeField] public int room_length { get; set; }
	[SerializeField] public int branch_count { get; set; }
	[SerializeField] public int branch_length { get; set; }
	[SerializeField] public int enemy_group_id { get; set; }
	[SerializeField] public int min_enemy_rate { get; set; }
	[SerializeField] public int max_enemy_rate { get; set; }
	[SerializeField] public int drop_item_pattern_id { get; set; }
	[SerializeField] public int min_item_rate { get; set; }
	[SerializeField] public int max_item_rate { get; set; }
	[SerializeField] public int gimmick_pattern_id { get; set; }
	[SerializeField] public int min_gimmick_rate { get; set; }
	[SerializeField] public int max_gimmick_rate { get; set; }
	[SerializeField] public int memo { get; set; }

	public void Initialize() {
		// TODO:実際はcurrentFloorIdをキーにマスターから取得する
		SetTestData();
	}

	void SetTestData() {
	}
}
