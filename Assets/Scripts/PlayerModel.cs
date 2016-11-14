using UnityEngine;
using System.Collections;

public class PlayerModel : MonoBehaviour {

	[SerializeField] public int id { get; set; }
	[SerializeField] public int avatarColor { get; set; }
	[SerializeField] public float moveSpeed { get; set; }
	[SerializeField] public int growthType { get; set; }
	[SerializeField] public int lv { get; set; }
	[SerializeField] public int exp { get; set; }
	[SerializeField] public int maxHp { get; set; }
	[SerializeField] public int maxMp { get; set; }
	[SerializeField] public int hp { get; set; }
	[SerializeField] public int mp { get; set; }
	[SerializeField] public int str { get; set; }
	[SerializeField] public int vit { get; set; }
	[SerializeField] public int dex { get; set; }
	[SerializeField] public int agi { get; set; }
	[SerializeField] public int weaponId { get; set; }
	[SerializeField] public int shieldId { get; set; }
	[SerializeField] public int armorId { get; set; }
	[SerializeField] public int accessaryId { get; set; }
	[SerializeField] public int currentMagicId { get; set; }
	[SerializeField] public int currentItemId { get; set; }
	[SerializeField] public int itemSlotCount { get; set; }
	[SerializeField] public int money { get; set; }

	public void Initialize() {
		// TODO:実際は初期値はマスターデータから取得し、以降はPlayerPrefsから取得する
		// オブジェクトのまま突っ込めるPlayerPrefs的な仕組みがあったら便利
		SetTestData();
	}

	public void AddExp(int getExp) {
		int[] lvAndExp = GameCalculator.AddExp(lv, exp, getExp);
		if (lvAndExp[0] > lv) {
			lv = lvAndExp[0];
			// HP/MPは全回復
			hp = maxHp;
			mp = maxMp;
			// TODO:レベルアップ演出を追加する
		}
		exp = lvAndExp[1];
	}

	void SetTestData() {
		avatarColor = 1;	// TODO:未実装
		moveSpeed = 3f;		// TODO:未実装
		lv = 1;
		exp = 0;
		maxHp = 100;
		hp = 100;
		maxMp = 60;
		mp = 60;
		str = 30;
		vit = 25;
		dex = 20;
		agi = 15;
		weaponId = 123;
		shieldId = 234;
		armorId = 345;
		accessaryId = 456;
		currentMagicId = 111;
		currentItemId = 222;
	}
}
