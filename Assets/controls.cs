using UnityEngine;
using System.Collections;

public class controls : MonoBehaviour {

	// Standardized units, useful for making game compatible with multiple resolutions
	private int horizontalUnit = Screen.width/16;
	private int verticalUnit = Screen.height/9;
	private int spacingUnit = Screen.width/32;

	private int threshHold = Screen.width/100;
	private float vertical;
	private float horizontal;

	private Vector2 movementStartPos;

	private bool moving = false, colouring = false;

	private Rect[] colourButtons;

	private Rect movementContainer;

	private PlayerController player;
	// Use this for initialization
	void Start () {
		player = gameObject.GetComponent<PlayerController>();
		colourButtons = new Rect[3];
		for (int i = 0; i < colourButtons.Length; i++){
			colourButtons[i] = 	new Rect(Screen.width - Screen.height/3, Screen.height * i / 3, Screen.height/3, Screen.height/3);
		}
		movementContainer = new Rect(0,0,Screen.width/2, Screen.height);
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
		player.StartGame();
		if (t.phase == TouchPhase.Began){
			if (IsInside(t.position, movementContainer)) {
				movementStartPos = new Vector2(t.position.x, Screen.height - t.position.y);
				moving = true;
			} else {
				for (int i = 0; i < colourButtons.Length; i++){
					if (IsInside(t.position, colourButtons[i])){
						player.ActivateColour(player.colourStrings[i]);

					}
				}
			}
		} else if (t.phase == TouchPhase.Moved) {
			if (IsInside(t.position, movementContainer)) {
				moving = true;
				WhichDirection(t.position);
			} else {
				for (int i = 0; i < colourButtons.Length; i++){
					if (IsInside(t.position, colourButtons[i])){
						player.ActivateColour(player.colourStrings[i]);

					}
				}
			}
		} else if (t.phase == TouchPhase.Ended) {
			if (IsInside(t.position, movementContainer)) {
				moving = false;
				vertical = 0;
				horizontal = 0;
			} else {
				for (int i = 0; i < colourButtons.Length; i++){
					if (IsInside(t.position, colourButtons[i])){
						player.ActivateColour(player.colourStrings[i]);
						
					}
				}
			}
		}

		player.verticalTouch = vertical;
		player.horizontalTouch = horizontal;


	}

    bool IsInside (Vector2 position, Rect bounds){
		Vector2 screenPos = new Vector2(position.x, Screen.height - position.y);	// Invert Y coordinate
		return bounds.Contains(screenPos);
    
    }

	void WhichDirection (Vector2 position){
		Vector2 screenPos = new Vector2(position.x, Screen.height - position.y);	// Invert Y coordinate
		// Delta Y, positive is up
		float deltaY = (movementStartPos.y - position.y);
		// Delta X, positive is right
		float deltaX = position.x - movementStartPos.x;
		if (deltaY > 0 && deltaY > threshHold){
			vertical = Mathf.Clamp(deltaY/(Screen.width/4), -1, 1);
		}
		if (deltaX > threshHold){
			horizontal = Mathf.Clamp(deltaX/(Screen.width/4), -1, 1);
		} else if (deltaX < -threshHold){
			horizontal = Mathf.Clamp(deltaX/(Screen.width/4), -1, 1);
		}
		
	}

	void OnGUI () {

	}
}
