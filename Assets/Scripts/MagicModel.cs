using UnityEngine;
using System.Collections;

public class MagicModel : MonoBehaviour {

	[SerializeField] public int id { get; set; }
	[SerializeField] public int magicType { get; set; }
	[SerializeField] public string magicName { get; set; }
	[SerializeField] public string prefabName { get; set; }
	[SerializeField] public string animationName { get; set; }
	[SerializeField] public string description { get; set; }
	[SerializeField] public float masterRate { get; set; }
	[SerializeField] public int useMp { get; set; }
	[SerializeField] public int attribute { get; set; }
	[SerializeField] public int effectValue { get; set; }
	[SerializeField] public int targetStatus { get; set; }
	[SerializeField] public float firingRange { get; set; }
	[SerializeField] public float speed { get; set; }
	[SerializeField] public float strikeRate { get; set; }
	[SerializeField] public int duration { get; set; }

	// TODO:実際はマスターデータから取得する
	public void SetMagicId(int id) {
		this.id = id;
		magicName = "ファイアーボール";
		prefabName = "Magic/FireMissileOBJ";
		animationName = "MeleeAttack01";
		description = "炎の弾を放ちます";
		masterRate = 0.25f;
		magicType = 1;
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
