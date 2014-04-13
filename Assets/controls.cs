using UnityEngine;
using System.Collections;

public class controls : MonoBehaviour {

	// Standardized units, useful for making game compatible with multiple resolutions
	private int horizontalUnit = Screen.width/16;
	private int verticalUnit = Screen.height/9;
	private int spacingUnit = Screen.width/32;

	public Texture2D texJoyBase;
	public Texture2D texJoyStick;

	private int threshHold = Screen.width/20;
	private float vertical;
	private float horizontal;

	private Vector2 movementStartPos;
	private Vector2 movementCurrentPos;

	private bool moving = false, colouring = false;


	private Rect[] colourButtons;

	private Rect movementContainer;
	private Rect joyBaseContainer;
	private Rect joyStickContainer;

	private PlayerController player;
	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerController>();
		colourButtons = new Rect[3];
		for (int i = 0; i < colourButtons.Length; i++){
			colourButtons[i] = 	new Rect(Screen.width - Screen.height/3, Screen.height * i / 3, Screen.height/3, Screen.height/3);
		}
		movementContainer = new Rect(0,0,Screen.width/2, Screen.height);
		joyBaseContainer = new Rect(0,0,verticalUnit*2, verticalUnit*2);
		joyStickContainer = new Rect(0,0,verticalUnit, verticalUnit);
	}
	
	// Update is called once per frame
	void Update () {
		colouring = false;
		if ((Input.touchCount == 1)  )
		{
			HandleTouch(Input.GetTouch(0));
		} else if (Input.touchCount==2){
			HandleTouch(Input.GetTouch(0));
			HandleTouch(Input.GetTouch(1));
		}

		if (!colouring) {
			player.ActivateColour(player.colourStrings[0]);
			
		}
	}

	void HandleTouch (Touch t){
		if (player == null) {
			player = GetComponent<PlayerController>();
			Debug.Log ("PLAYER NULL",gameObject);
				}
		player.StartGame ();


		if (t.phase == TouchPhase.Began){
			if (IsInside(t.position, movementContainer)) {
				movementStartPos = new Vector2(t.position.x, Screen.height - t.position.y);
				movementCurrentPos = movementStartPos;
				moving = true;
				vertical = 0;
				horizontal = 0;
			} else {
				for (int i = 0; i < colourButtons.Length; i++){
					if (IsInside(t.position, colourButtons[i])){
						player.ActivateColour(player.colourStrings[i+1]);
						colouring = true;

					}
				}
			}
		} else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) {
			if (IsInside(t.position, movementContainer)) {
				moving = true;
				movementCurrentPos = new Vector2(t.position.x, Screen.height - t.position.y);
				WhichDirection(t.position);
			} else {
				for (int i = 0; i < colourButtons.Length; i++){
					if (IsInside(t.position, colourButtons[i])){
						player.ActivateColour(player.colourStrings[i+1]);
						colouring = true;

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
		float deltaY = (movementStartPos.y - screenPos.y);
		// Delta X, positive is right
		float deltaX = screenPos.x - movementStartPos.x;
		//Debug.Log ("start Y:" + movementStartPos.y + ",start X:" + movementStartPos.y );
		//Debug.Log ("new Y:" + screenPos.y + ",new X:" + screenPos.y );
		//Debug.Log ("Y:" + deltaY + ",X:" + deltaX + ", Threshold:"+threshHold);
		if (deltaY > 0 && deltaY > threshHold){
			vertical = Mathf.Clamp(deltaY/(Screen.width/4), -1, 1);
		} else {
			vertical = 0;
		}
		if (deltaX > threshHold){
			horizontal = Mathf.Clamp(deltaX/(Screen.width/4), -1, 1);
		} else if (deltaX < -threshHold){
			horizontal = Mathf.Clamp(deltaX/(Screen.width/4), -1, 1);
		} else {
			horizontal = 0;
		}
		
	}

	void OnGUI () {
		if (moving) {
			print ("DISPLAY JOY");
			joyBaseContainer.x = movementStartPos.x-joyBaseContainer.width/2;
			joyBaseContainer.y = movementStartPos.y-joyBaseContainer.height/2;
			joyStickContainer.x = movementCurrentPos.x-joyStickContainer.width/2;

			if (joyStickContainer.x<joyBaseContainer.x)
				joyStickContainer.x = joyBaseContainer.x;
			else if (joyStickContainer.x>joyBaseContainer.x+joyBaseContainer.width)
				joyStickContainer.x = joyBaseContainer.x + joyBaseContainer.width;

			joyStickContainer.y = movementCurrentPos.y-joyStickContainer.height/2;
			if (joyStickContainer.y<joyBaseContainer.y)
				joyStickContainer.y = joyBaseContainer.y;
			else if (joyStickContainer.y>joyBaseContainer.y+joyBaseContainer.height)
				joyStickContainer.y = joyBaseContainer.y + joyBaseContainer.height;

			GUI.Label(joyBaseContainer, texJoyBase);
			GUI.Label(joyStickContainer, texJoyStick);
		}
	}
}
