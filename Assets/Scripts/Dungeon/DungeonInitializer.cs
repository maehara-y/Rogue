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
	public MasterDataManager masterDataManager;

	//静的ダンジョンデバッグ用
	/*void Start() {
		Initialize ();
	}*/

	/*************************************************************
	 * シーン読込時の初期処理 (DunGenのRuntimeDungeonスクリプトの処理終了時に呼ばれる)
	 *************************************************************/
	public void Initialize () {
		Application.targetFrameRate = 60;

		masterDataManager = new MasterDataManager();
		masterDataManager.GetMasterDataAll();

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
		// モンスターの出現数をランダム算出する (正規分布に近いランダムさで)
		GameObject[] roomArr = GameObject.FindGameObjectsWithTag("RoomTag");
		int minCount = (int)(roomArr.Length * floorModel.minEnemyRate);
		int maxCount = (int)(roomArr.Length * floorModel.maxEnemyRate);
		int enemyCount = (Random.Range(minCount, maxCount) + Random.Range(minCount, maxCount) + Random.Range(minCount, maxCount)) / 3;
		//Debug.Log ("roomArr.Length:" + roomArr.Length);

		// モンスター出現数分ループし、ランダムに部屋ごとにモンスターを割り当てる
		List<GameObject> rooms = new List<GameObject>();
		rooms.AddRange(roomArr);
		for (int i=0; i < enemyCount; i++) {
			if (rooms.Count < i+1) break;
			int randomIndex = Random.Range(0, rooms.Count);
			GameObject room = rooms[randomIndex];
			rooms.RemoveAt(randomIndex);
			//Debug.Log ("room ramdomindex:" + randomIndex + ", x:" + room.transform.position.x + ", y:" + room.transform.position.y + ", z:" + room.transform.position.z);

			// フロア情報から出現モンスターの種類を特定し、フロア階層にあったLevelとステータスを設定する
			EnemyModel enemyModel = EnemyQuery.ChooseByGroupId(floorModel.enemyGroupId);
			int enemyLevel = GameCalculator.GetEnemyLevel(floorModel.floorNumber);
			enemyModel.level = enemyLevel;
			enemyModel = GameCalculator.GetEnemyStatusByLevel(enemyLevel, enemyModel);
			// TODO:Resourcesは重いので、全モンスタープレハブのリストをpublic変数にinspector上でセットしておく？
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
