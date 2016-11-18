using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class WarpController : MonoBehaviour {

	/*************************************************************
	 * 次の階へ移動する
	 *************************************************************/
	void OnTriggerEnter(Collider col) {
		// 階数を保存する
		int nextFloorId = PlayerPrefs.GetInt("NextFloorId");
		if (nextFloorId < 1) nextFloorId = 1;
		PlayerPrefs.SetInt("NextFloorId", nextFloorId + 1);

		// ダンジョンシーン読込
		SceneManager.LoadScene("Dungeon");
	}
}
