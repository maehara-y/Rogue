using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DungeonInitializer : MonoBehaviour {

	public GameObject enemyRoot;
	public Text floorLabel;

	/*void Start() {
		Initialize ();
	}*/

	// Use this for initialization
	public void Initialize () {
		SettingFloor();
		GeneratePlayer();
		GenerateEnemies();
	}

	// 現在のフロア情報を設定する
	void SettingFloor() {
		int floor = PlayerPrefs.GetInt("NextFloor");
		if (floor < 1) floor = 1;
		this.floorLabel.text = floor.ToString() + "階";
	}

	// プレイヤー関連情報の生成
	void GeneratePlayer() {
		GameObject player = GameObject.FindWithTag("PlayerTag");
		player.GetComponent<PlayerController>().Initialize();
	}

	// モンスターの生成
	void GenerateEnemies() {
		// 出現数をランダム算出する (コクのあるランダムに)
		// TODO:実際は部屋数とフロアマスターから割り出す
		int enemyCount = (Random.Range(3,8) + Random.Range(3,8) + Random.Range(3,8)) / 3;
		GameObject[] roomArr = GameObject.FindGameObjectsWithTag("RoomTag");
		//Debug.Log ("roomArr.Length:" + roomArr.Length);
		List<GameObject> rooms = new List<GameObject>();
		rooms.AddRange (roomArr);
		for (int i=0; i < enemyCount; i++) {
			if (rooms.Count < i+1) break;
			int randomIndex = Random.Range(0, rooms.Count);
			GameObject room = rooms[randomIndex];
			rooms.RemoveAt(randomIndex);
			//Debug.Log ("room ramdomindex:" + randomIndex + ", x:" + room.transform.position.x + ", y:" + room.transform.position.y + ", z:" + room.transform.position.z);

			// TODO:フロアマスターにあったモンスター情報をマスターから取得する
			// TODO:Resourcesは重いので、全モンスタープレハブのリストをpublic変数にinspector上でセットしておく
			string prefabName = "Enemy/Skeleton";
			GameObject enemyPrefab = (GameObject)Resources.Load(prefabName);
			GameObject enemyObj = Instantiate(enemyPrefab, room.transform.position, room.transform.rotation) as GameObject;
			enemyObj.GetComponent<EnemyController>().Initialize();
			enemyObj.transform.parent = enemyRoot.transform;
		}
	}
}
