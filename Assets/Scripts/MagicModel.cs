using UnityEngine;
using System.Collections;

public class MagicModel : MonoBehaviour {

	public int id { get; set; }
	public string magicName { get; set; }
	public string prefabName { get; set; }
	public string animationName { get; set; }
	public string description { get; set; }
	public float masterRate { get; set; }
	public int type { get; set; }
	public int useMp { get; set; }
	public int attribute { get; set; }
	public int effectValue { get; set; }
	public int targetStatus { get; set; }
	public float firingRange { get; set; }
	public float speed { get; set; }
	public float strikeRate { get; set; }
	public int duration { get; set; }

	void Start() {
	}

	// TODO:実際はマスターデータから取得する
	public void SetMagicId(int id) {
		this.id = id;
		magicName = "ファイアーボール";
		prefabName = "Magic/FireMissileOBJ";
		animationName = "MeleeAttack01";
		description = "炎の弾を放ちます";
		masterRate = 0.25f;
		type = 1;
		useMp = 5;
		attribute = 1;
		effectValue = 100;
		targetStatus = 0;
		firingRange = 12f;		// TODO:未実装
		speed = 300f;
		strikeRate = 0.8f;
		duration = 0;
	}
}
