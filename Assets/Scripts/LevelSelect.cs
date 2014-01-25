using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {

	// Standardized units, useful for making game compatible with multiple resolutions
	private int horizontalUnit = Screen.width/16;
	private int verticalUnit = Screen.height/9;
	private int spacingUnit = Screen.width/32;

	private int totalLevels = 12;
	private string[] levels;
	private Rect rLevels, rTitle, rSelect;

	// Level Selecting int
	private int levelSelect = 0;

	// Game Name
	private string gameName = "GAME";

	// Use this for initialization
	void Start () {
		levels = new string[totalLevels];
		for (int i = 0; i< totalLevels; i++) {
			levels[i] = "Level " + (i+1).ToString();
		}
		rLevels = new Rect (spacingUnit, spacingUnit*3, Screen.width - spacingUnit * 2, Screen.height - spacingUnit * 5 - verticalUnit);
		rTitle = new Rect (Screen.width / 2 - horizontalUnit, spacingUnit, horizontalUnit * 2, spacingUnit);
		rSelect = new Rect (Screen.width / 2 - horizontalUnit*2, Screen.height-spacingUnit-verticalUnit, horizontalUnit * 4, verticalUnit);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUI.Label (rTitle, gameName);
		levelSelect = GUI.SelectionGrid (rLevels, levelSelect, levels, 4);
		if (GUI.Button(rSelect,"Select Level")) {
			print ("Player selected " + levels[levelSelect]);
		}
		
	}
}
