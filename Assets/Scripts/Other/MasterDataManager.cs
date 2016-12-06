using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CsvHelper;

public class MasterDataManager {
	private int culomnIndex = 0;
	private int dataTypeIndex = 1;
	private int dataIndex = 2;

	private string masterDataBaseUrl = "https://docs.google.com/spreadsheets/d/1emJVHTWLZKncwG2zH1HeGVwfy_-HlRJ7xAPhsaCxMMc/export?format=csv&gid=";
	private Dictionary<string, string> tableGIds = new Dictionary<string, string> ();

	public void CreateTableGIds () {
		// TODO:ここにテーブル情報を追加
		tableGIds.Add ("m_enemy_action_group", "937516204");
	}

	public void GetMasterDataAll () {
		CreateTableGIds();

		// マスター単位でループする
		List<string> keyList = new List<string>(tableGIds.Keys);
		for (int i=0; i < keyList.Count; i++) {
			string tableName = keyList[i];

			//Debug.Log("persistentDataPath: " + Application.persistentDataPath);
			// TODO:一時処理
			/*string selectQuery2 = "select * from " + tableName;
			DataTable dataTable2 = SqliteDatabase.Instance.ExecuteQuery(selectQuery2);
			string deleteQuery = "delete from " + tableName;
			SqliteDatabase.Instance.ExecuteNonQuery(deleteQuery);*/

			// SQLiteのテーブルのデータをチェック
			string selectQuery = "select * from " + tableName;
			DataTable dataTable = SqliteDatabase.Instance.ExecuteQuery(selectQuery);
			if (dataTable.Rows.Count > 0) {
				//Debug.Log ("データあり tableName:" + tableName);
				continue;
			}
			// なければスプレッドシートから取得
			WWW masterData = new WWW(masterDataBaseUrl + tableGIds[tableName]);
			while (!masterData.isDone) { 
				// ダウンロードの進捗を表示
				//Debug.Log(Mathf.CeilToInt(masterData.progress * 100));
			}

			if (!string.IsNullOrEmpty(masterData.error)) { 
				// ダウンロードでエラーが発生した場合
				Debug.Log(masterData.error);

			} else { 
				// ダウンロードが正常に完了した場合
				StringReader stringReader = new StringReader(masterData.text);
				CsvReader csvReader = new CsvReader(stringReader);
				int currentRowIndex = 0;
				string culomns = "";
				List<string> dataTypes = new List<string>();
				int culomnCount = 0;
				int dataTypeCount = 0;

				// CSVファイルの行単位でループする
				while (csvReader.Read()) {
					string[] records = csvReader.CurrentRecord;
					// カラム名の場合
					if (currentRowIndex == culomnIndex) {
						culomnCount = records.Length;
						for (int j=0; j < records.Length; j++) {
							culomns += (culomns.Equals("")) ? records[j] : "," + records[j];
						}
					}
					// データ型の場合
					if (currentRowIndex == dataTypeIndex) {
						dataTypeCount = records.Length;
						for (int j=0; j < records.Length; j++) {
							dataTypes.Add(records[j]);
						}
					}
					// データの場合
					if (currentRowIndex >= dataIndex) {
						int valueCount = records.Length;
						// カラム名とデータ型とデータの列数が合わなければ不整合データとして無視する
						if (culomnCount != valueCount || dataTypeCount != valueCount) continue;

						string values = "";
						for (int j=0; j < records.Length; j++) {
							string value = records[j];
							if (value.Equals("")) {
								value = "NULL";
							} else if (dataTypes[j] == "text") {
								value = "'" + value + "'";
							}
							values += (values.Equals("")) ? value : "," + value;
						}

						// SQLiteに挿入する
						string query = "INSERT INTO " + tableName + "(" + culomns + ") VALUES(" + values + ")";
						//Debug.Log ("tableName:" + tableName + ", rowIndex:" + currentRowIndex + ", query:" + query);
						SqliteDatabase.Instance.ExecuteNonQuery(query);
					}
					currentRowIndex++;
				}
			}
		}
	}
}
