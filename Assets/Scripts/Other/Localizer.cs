using UnityEngine;
using System.Collections;

public class Localizer {

	public enum Language { Japanese, English }

	/*************************************************************
	 * 引数のキーから言語を特定して該当テキストを返す
	 *************************************************************/
	public static string GetLocalizedText(string key) {
		return "";
	}

	/*************************************************************
	 * マスターデータ('#'を挟んで複数言語分を羅列したテキスト)から言語を特定して該当テキストを返す
	 *************************************************************/
	public static string GetLocalizedMasterData(string data) {
		string[] dataArr = data.Split('#');
		string localizedText = "";
		if (Application.systemLanguage == SystemLanguage.Japanese) {
			localizedText = dataArr[0];
		} else if (Application.systemLanguage == SystemLanguage.English) {
			localizedText = dataArr[1];
		}
		return localizedText;
	}
}
