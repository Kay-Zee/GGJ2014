using UnityEngine;
using System.Collections;

public class controls : MonoBehaviour {

	// Standardized units, useful for making game compatible with multiple resolutions
	private int horizontalUnit = Screen.width/16;
	private int verticalUnit = Screen.height/9;
	private int spacingUnit = Screen.width/32;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ((Input.touchCount == 1)  )
		{
			HandleTouch(Input.GetTouch(0));
		} else if (Input.touchCount==2){
			HandleTouch(Input.GetTouch(0));
			HandleTouch(Input.GetTouch(1));
		}
	}

	void HandleTouch (Touch t){
		if (Input.GetTouch(0).phase == TouchPhase.Began)
		{
			//if (Input.GetTouch(0).position
		}


	}

	void OnGUI () {

	}
}
