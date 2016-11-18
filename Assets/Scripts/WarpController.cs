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
		int nextFloor = PlayerPrefs.GetInt("NextFloor");
		if (nextFloor < 1) nextFloor = 1;
		PlayerPrefs.SetInt("NextFloor", nextFloor + 1);

		// ダンジョンシーン読込
		SceneManager.LoadScene("Dungeon");
	}
}
