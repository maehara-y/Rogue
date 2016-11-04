using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DungeonInitializer : MonoBehaviour {

	public Text floorLabel;

	// Use this for initialization
	void Start () {
		int floor = PlayerPrefs.GetInt("NextFloor");
		if (floor < 1) floor = 1;
		this.floorLabel.text = floor.ToString() + "階";
	}
}
