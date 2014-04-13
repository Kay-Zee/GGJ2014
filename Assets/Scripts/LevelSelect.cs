using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {

	// Standardized units, useful for making game compatible with multiple resolutions
	private int horizontalUnit = Screen.width/16;
	private int verticalUnit = Screen.height/9;
	private int spacingUnit = Screen.width/32;

	private int totalLevels = 3;
	private int dimension = 0;
	private string[] levels;
	private Rect rLevels, rTitle, rSelect;
	public GUIStyle style;

	// Level Selecting int
	private int levelSelect = 0;

	// Game Name
	private string gameName = "Tinted";

	// Show Levels
	private bool showLevels = false;

	// Use this for initialization
	void Start () {
		levels = new string[totalLevels];
		for (int i = 0; i< totalLevels; i++) {
			levels[i] = (i+1).ToString();
		}

		Invoke("Wait", 5);

		dimension = Screen.height - spacingUnit * 6 - verticalUnit;
		rLevels = new Rect (spacingUnit*10, spacingUnit*3, dimension, dimension/3);
		rSelect = new Rect (Screen.width / 2 - horizontalUnit*2, Screen.height-spacingUnit*7-verticalUnit, horizontalUnit * 4, verticalUnit);
	}

	void Wait () {
		showLevels = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		if (showLevels) {
			levelSelect = GUI.SelectionGrid (rLevels, levelSelect, levels, 3, style);
			if (GUI.Button (rSelect, "Select Level")) {
				print ("Player selected " + levels [levelSelect]);
				Application.LoadLevel ("Level_0" + (levelSelect + 1).ToString ());
			}
		}
	}

}
