using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DungeonInitializer : MonoBehaviour {

	public int floorId;
	public Text floorLabel;
	public GameObject enemyRoot;
	public GameObject battleObjectRoot;
	private FloorModel floorModel;

	//静的ダンジョンデバッグ用
	/*void Start() {
		Initialize ();
	}*/

	/*************************************************************
	* シーン読込時の初期処理 (DunGenのRuntimeDungeonスクリプトの処理終了時に呼ばれる)
	*************************************************************/
	public void Initialize () {
		SettingFloor();
		GeneratePlayer();
		GenerateEnemies();
	}

	/*************************************************************
	 * 現在のフロア情報を設定する
	 *************************************************************/
	void SettingFloor() {
		floorId = PlayerPrefs.GetInt("NextFloorId");
		if (floorId < 1) floorId = 1;
		floorModel = FloorQuery.FindById(floorId);
		floorLabel.text = floorModel.floorNumber + "階";
	}

	/*************************************************************
	 * プレイヤー関連情報を生成する
	 *************************************************************/
	void GeneratePlayer() {
		GameObject player = GameObject.FindWithTag("PlayerTag");
		player.GetComponent<PlayerController>().Initialize();
	}

	/*************************************************************
	 * モンスターを生成する
	 *************************************************************/
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

			// TODO:Resourcesは重いので、全モンスタープレハブのリストをpublic変数にinspector上でセットしておく？
			// フロア情報から出現モンスターの種類を特定し、フロア階層にあったLevelとステータスを設定する
			EnemyModel enemyModel = EnemyQuery.ChooseByGroupId(floorModel.enemyGroupId);
			int enemyLevel = GameCalculator.GetEnemyLevel(floorModel.floorNumber);
			enemyModel.level = enemyLevel;
			enemyModel = GameCalculator.GetEnemyStatusByLevel(enemyLevel, enemyModel);
			GameObject enemyPrefab = (GameObject)Resources.Load(enemyModel.prefabName);
			GameObject enemyObj = Instantiate(enemyPrefab, room.transform.position, room.transform.rotation) as GameObject;
			enemyObj.transform.SetParent(enemyRoot.transform);
			EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
			enemyController.battleObjectRoot = battleObjectRoot;
			enemyController.enemyModel = enemyModel;
			enemyController.Initialize();
		}
	}
}
