using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class WarpController : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		// 階数を保存する
		int nextFloor = PlayerPrefs.GetInt("NextFloor");
		if (nextFloor < 1) nextFloor = 1;
		PlayerPrefs.SetInt("NextFloor", nextFloor + 1);

		// ダンジョンシーン読込
		SceneManager.LoadScene("Dungeon");
	}
}
