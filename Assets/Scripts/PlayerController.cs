using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{


	private bool gameStarted = false;
	private bool gameEnded = false;
	private bool winLevel = false;
	private float gameTimeScale;
	private System.TimeSpan timeLeft;
	private System.DateTime jumpAllowedAfter = System.DateTime.Now;

	private bool hasKey = false;

	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	[HideInInspector]
	public bool[] spring = {false, false, false};				// Condition for whether the player should spring.
	private bool springing = false;
	private bool[] ladder = {false, false, false};
	private bool[] hit = {false, false, false};
	
	// Energy Controls
	private int numColours = 3;
	[HideInInspector]
	public string[] colourStrings;
	[HideInInspector]
	public bool[] activeColour;			// Is green activated
	public bool[] activeColourLastFrame;			// Is green activated


	public Texture2D[] colourTex;

	private int[] colourLayers;
	private GameObject[][] layerObjects;
	

	private float activationEnergy = 10f;
	private float energyDrainRate = 0.1f;
	private float energyGainFromEat = 25f;
	private float maxEnergy = 100f;
	[HideInInspector]
	public float[] colourEnergy;

	public float horizontalTouch;
	public float verticalTouch;
	private float horizVel = 0f;
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	//public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 300f;			// Amount of force added when the player jumps.
	public float climbSpeed = 1f;			// Ladder Climbing Speed
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private Transform ladderCheck;			// A position marking where to check if the player is at a ladder.

	private bool hasParachute = false;
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.
	private Animation animSplatter;

	// Standardized units, useful for making game compatible with multiple resolutions
	private int horizontalUnit = Screen.width/16;
	private int verticalUnit = Screen.height/9;
	private int spacingUnit = Screen.width/32;

	void Awake()
	{
		gameTimeScale = 1;
		timeLeft = System.TimeSpan.FromSeconds (90);
		rigidbody2D.gravityScale=1f;
		colourStrings = new string[numColours+1];
		colourStrings[0] = "Gray";
		colourStrings[1] = "Red";
		colourStrings[2] = "Green";
		colourStrings[3] = "Blue";

		// Initialize Energy levels
		//colourTex=new Texture2D[numColours];
		colourEnergy = new float[numColours];
		for (int i = 0; i < colourEnergy.Length; ++i)
			colourEnergy[i]=50;

		for (int i = 0; i < colourEnergy.Length; ++i)
			colourEnergy[i]=50;

		// Initialize layer objects
		colourLayers = new int[numColours+1];
		layerObjects = new GameObject[numColours+1][];
		for (int i = 0; i < colourLayers.Length; ++i){
			colourLayers[i] = LayerMask.NameToLayer(colourStrings[i]);
			layerObjects[i] = FindGameObjectsWithLayer (colourLayers[i]);
			if (i>0)
				ActivateGameObjects(layerObjects[i], false);
		}

		/*
		redLayer = LayerMask.NameToLayer("Red");
		greenLayer = LayerMask.NameToLayer ("Green");
		blueLayer = LayerMask.NameToLayer ("Blue");
		grayLayer = LayerMask.NameToLayer ("Gray");
		redLayerObjects = FindGameObjectsWithLayer (redLayer);
		greenLayerObjects = FindGameObjectsWithLayer (greenLayer);
		blueLayerObjects = FindGameObjectsWithLayer (blueLayer);
		grayLayerObjects = FindGameObjectsWithLayer (grayLayer);
		ActivateGameObjects(redLayerObjects, false);
		ActivateGameObjects(greenLayerObjects, false);
		ActivateGameObjects(blueLayerObjects, false);
		*/

		// Setting up booleans
		activeColour = new bool[3];
		activeColourLastFrame = new bool[3];
		Populate(activeColour, false);
		Populate(activeColourLastFrame, false);
		spring = new bool[3];	
		Populate(spring, false);
		springing = false;
		Populate(ladder, false);
		ladder = new bool[3];	
		Populate(hit, false);
		hit = new bool[3];	
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		ladderCheck = transform.Find("ladderCheck");
		anim = GetComponent<Animator>();

		animSplatter = transform.FindChild("splatter_of_paint").GetComponent<Animation>();
		Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer ("Ground"), false);

	}


	void Update()
	{
		if (!gameStarted) {
			if(Input.GetButtonDown("Jump")){
				print (gameTimeScale);
				Time.timeScale=gameTimeScale;
				gameStarted = true;

			}
		} else {
			// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
			grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
			Populate (ladder, false);
			RaycastHit2D[] colourHit = new RaycastHit2D[numColours];
			for (int i = 0; i < colourHit.Length; ++i){
				if (activeColour[i]){
					hit[i] = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer(colourStrings[i+1]));  
					colourHit[i] = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer(colourStrings[i+1]));  
					if (hit[i]) 
						performAction(colourHit[i],i);
				}
			}

			/* Refactored into a for loop
			if (red){
				hit[0] = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Red"));  
				hitRed = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Red"));  
				if (hit[0]) 
					performAction(hitRed,0);
			}
			if (green){
				hit[1] = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Green"));  
				hitGreen = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Green"));  
				if (hit[1])
					performAction (hitGreen,1);
			}
			if (blue){
				hit[2] = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Blue"));  
				hitBlue = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Blue"));  
				if (hit[2]) 
					performAction (hitBlue, 2);
			}

			*/


			if(consolidateBoolArray(spring))
			{
				springing = true;
				Invoke("allowSpring",0.1f);
				// Set the Jump animator trigger parameter.
				anim.SetTrigger("Jump");
				
				// Play a random jump audio clip.
				//int i = Random.Range(0, jumpClips.Length);
				//AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);
				
				// Add a vertical force to the player.
				rigidbody2D.AddForce(new Vector2(0f, jumpForce*2));
				
				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				Populate(spring,false);
				
			}


			// Change Gravity according to current state of the game
			if (consolidateBoolArray(ladder)) {
				rigidbody2D.gravityScale=0f;
			} else if (hasParachute){
				rigidbody2D.gravityScale=0.5f;
			} else
			{
				rigidbody2D.gravityScale=1f;
			}

			// If the jump button is pressed and the player is grounded then the player should jump.
			if(Input.GetButtonDown("Jump") && grounded)
				jump = true;


			for (int i = 0; i < activeColour.Length; ++i){
				if (Input.GetButtonDown(colourStrings[i+1])){
					activeColour[i] = true;
				}
				if ((activeColour[i] && !activeColourLastFrame[i])&& colourEnergy[i]-activationEnergy>0){
					activeColourLastFrame[i] = activeColour[i];
					colourEnergy[i] -= activationEnergy;
					print ("paint_splatter_"+colourStrings[i+1].ToLower());
					animSplatter.Play("paint_splatter_"+colourStrings[i+1].ToLower());
					//ActivateGameObjects(greenLayerObjects, true);
				}
				
				if (activeColour[i]) {
					anim.SetBool(colourStrings[i+1],true);
					ActivateGameObjects(layerObjects[i+1], true);
				} else {
					anim.SetBool(colourStrings[i+1],false);
					ActivateGameObjects(layerObjects[i+1], false);
				}
			}


			/* Refactored to for loop
			if (Input.GetButtonDown ("Green")) {
				if (!green && greenEnergy-activationEnergy>0){
					greenEnergy = greenEnergy-activationEnergy;
					green = true;
					red = false;
					blue = false;
					//ActivateGameObjects(greenLayerObjects, true);
				} else {
					green = false;
					//ActivateGameObjects(greenLayerObjects, false);
				}
			}
			if (green) {
				anim.SetBool("Green",true);
				ActivateGameObjects(greenLayerObjects, true);
			} else {
				anim.SetBool("Green",false);
				ActivateGameObjects(greenLayerObjects, false);
			}
			if (Input.GetButtonDown ("Red")) {
				if (!red && redEnergy-activationEnergy>0){
					redEnergy = redEnergy-activationEnergy;
					red = true;
					green = false;
					blue = false;
				} else {
					red = false;

				}
			}
			if (red) {
				anim.SetBool("Red",true);
				ActivateGameObjects(redLayerObjects, true);
			} else {
				anim.SetBool("Red",false);
				ActivateGameObjects(redLayerObjects, false);
			}
			if (Input.GetButtonDown ("Blue")) {
				if (!blue && blueEnergy-activationEnergy>0){
					blueEnergy = blueEnergy-activationEnergy;
					blue = true;
					red = false;
					green = false;
					//ActivateGameObjects(blueLayerObjects, true);
				} else {
					blue = false;
					//ActivateGameObjects(blueLayerObjects, false);
				}
			}
			if (blue) {
				anim.SetBool("Blue",true);
				ActivateGameObjects(blueLayerObjects, true);
			} else {
				anim.SetBool("Blue",false);
				ActivateGameObjects(blueLayerObjects, false);
			}
			*/
			if (consolidateBoolArray(activeColour)){
				anim.SetBool(colourStrings[0],false);
				ActivateGameObjects(layerObjects[0], false);

			} else {
				anim.SetBool(colourStrings[0],true);
				ActivateGameObjects(layerObjects[0], true);
			}

			if (Input.GetButtonDown ("Reset")) {
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}


	void FixedUpdate ()
	{
		if (gameStarted && !gameEnded){
			timeLeft = timeLeft.Subtract (System.TimeSpan.FromSeconds(Time.fixedDeltaTime));
			if (timeLeft.TotalMilliseconds<0){
				gameEnded = true;
			}

			/*
			 * The input for the pc 
			 */
			// Cache the horizontal input.
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis ("Vertical");

			if (consolidateBoolArray(ladder)){
				anim.SetBool("Climb",true);
				if (v > 0 || verticalTouch > 0) {
					jumpAllowedAfter = System.DateTime.Now.AddMilliseconds(1000);
					print (jumpAllowedAfter);
					rigidbody2D.velocity = new Vector2( rigidbody2D.velocity.x , 5  );
				} else if (v < 0 || verticalTouch < 0) {
					rigidbody2D.velocity = new Vector2( rigidbody2D.velocity.x , -5  );
				} else {
					rigidbody2D.velocity = new Vector2( rigidbody2D.velocity.x , 0  );
				}
			} else {
				if ((v > 0 || verticalTouch > 0) && grounded && System.DateTime.Now.CompareTo(jumpAllowedAfter)>0) {

					jump = true;
				} 
				anim.SetBool("Climb",false);
			}
			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(h), Mathf.Abs(horizontalTouch)));


			// If the input is moving the player right and the player is facing left...
			if((h > 0.01f || horizontalTouch > 0.01f) && !facingRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if((h < -0.01f || horizontalTouch < -0.01f)&& facingRight)
				// ... flip the player.
				Flip();

			// calculate adding the force ourselves: acceleration = Force/mass ... and clamp the velocity 
			horizVel = Mathf.Clamp(horizVel + (h+horizontalTouch)*(moveForce/rigidbody2D.mass)*Time.deltaTime, -maxSpeed, maxSpeed);
			
			Vector3 vel = rigidbody2D.velocity;
			if (h == 0 && horizontalTouch == 0) {
				horizVel = 0;
			}
			vel.x = horizVel;



			/*
			// If the player's horizontal velocity is greater than the maxSpeed...
			if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
				// ... set the player's velocity to the maxSpeed in the x axis.
				rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
*/



			rigidbody2D.velocity = vel;
			// If the player should jump...
			if(jump)
			{
				// Set the Jump animator trigger parameter.
				anim.SetTrigger("Jump");
				// Play a random jump audio clip.
				//int i = Random.Range(0, jumpClips.Length);
				//AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

				// Add a vertical force to the player.
				if (System.DateTime.Now.CompareTo(jumpAllowedAfter)>0){
					rigidbody2D.AddForce(new Vector2(0f, jumpForce));
					jumpAllowedAfter = System.DateTime.Now.AddMilliseconds(50);

				}

				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				jump = false;

			} 

			//if we are in the state jump and i am ground than go to idle

			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);			
			
			if (stateInfo.IsName("player_green_jump")||
			    stateInfo.IsName("player_gray_jump")||
			    stateInfo.IsName("player_blue_jump")||
			    stateInfo.IsName("player_red_jump"))

			{

				if(grounded)
				{
					print ("touching ground");
					anim.SetTrigger("Touchground");
				}
				             
			}

			for (int i = 0; i < activeColour.Length; ++i){
				if (activeColour[i]){
					colourEnergy[i] -= energyDrainRate;
					if (colourEnergy[i] < 0) {
						colourEnergy[i]=0;
						activeColour[i] = false;
					}
				}
			}
				
			/* Refactored to for loop
			if (green){
				greenEnergy -= energyDrainRate;
				if (greenEnergy < 0) {
					greenEnergy=0;
					green = false;
				}
			}
			if (red) {
				redEnergy -= energyDrainRate;
				if (redEnergy < 0) {
					redEnergy=0;
					red = false;
				}
			}
			if (blue){
				blueEnergy -= energyDrainRate;
				if (blueEnergy < 0) {
					blueEnergy=0;
					blue = false;
				}
			}
			*/
		}
		if (gameEnded)
			Time.timeScale = 0;

	}

	void OnGUI() {
		GUI.skin.box.fontSize = spacingUnit;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		if (!gameStarted)
			GUI.Box (new Rect (Screen.width / 2 - horizontalUnit*3, spacingUnit, horizontalUnit * 6, verticalUnit), "Press Space to Start");
		else if (gameEnded){
			if (winLevel){
				GUI.Box (new Rect (Screen.width / 4 , Screen.height / 4, Screen.width / 2, Screen.height / 2), "You Win! \nPress Q to Restart \nPress N to go to next level");
				if (GUI.Button(new Rect (Screen.width *3 / 4 - Screen.width / 6, Screen.height *3 / 4 - verticalUnit, Screen.width / 6, verticalUnit), "Next") || Input.GetButtonDown ("NextLevel")) {
					if (Application.loadedLevel < 3) {
						Application.LoadLevel(Application.loadedLevel + 1);
					}
				}
			} else {
				GUI.Box (new Rect (Screen.width / 4 , Screen.height / 4, Screen.width / 2, Screen.height / 2), "Game Over! \nPress Q to Restart");
			}
			if (GUI.Button(new Rect (Screen.width / 4 , Screen.height *3 / 4 - verticalUnit, Screen.width / 6, verticalUnit), "Reset")) {
				if (Application.loadedLevel < 3) {
					Application.LoadLevel(Application.loadedLevel);
				}
			}
			if (GUI.Button(new Rect (Screen.width / 4 + Screen.width/6 , Screen.height *3 / 4 - verticalUnit, Screen.width / 6, verticalUnit), "Menu")) {
				if (Application.loadedLevel < 3) {
					Application.LoadLevel(0);
				}
			}


		}
		GUI.Box (new Rect (Screen.width - 2 * horizontalUnit, spacingUnit, horizontalUnit * 2, verticalUnit), string.Format("{0}:{1}:{2}",
		                                                                                                                    timeLeft.Minutes,
		                                                                                                                    timeLeft.Seconds,timeLeft.Milliseconds));

		for (int i = 0; i< colourEnergy.Length; ++i){
			GUI.DrawTexture(new Rect ((spacingUnit*(i+1)/2)+horizontalUnit*i/2, (spacingUnit/2)+(verticalUnit*2)*(1-(float)colourEnergy[i]/maxEnergy),horizontalUnit/2,(verticalUnit*2)*((float)colourEnergy[i]/maxEnergy)),colourTex[i]);		
		}
		/* Refactored into for loop
		GUI.DrawTexture(new Rect ((int) (spacingUnit/2), (spacingUnit/2)+(verticalUnit*2)*(1-(float)redEnergy/maxEnergy),horizontalUnit/2,(verticalUnit*2)*((float)redEnergy/maxEnergy)),redTex);		
		GUI.DrawTexture(new Rect ((int) (spacingUnit)+horizontalUnit/2, (spacingUnit/2)+(verticalUnit*2)*(1 - (float)greenEnergy/maxEnergy), horizontalUnit/2,(verticalUnit*2)*((float)greenEnergy/maxEnergy)),greenTex);		
		GUI.DrawTexture(new Rect ((int) (spacingUnit*3/2)+horizontalUnit, (spacingUnit/2)+(verticalUnit*2)*(1 - (float)blueEnergy/maxEnergy),horizontalUnit/2,(verticalUnit*2)*((float)blueEnergy/maxEnergy)),blueTex);		
		*/
		
	}
	
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}


	bool consolidateBoolArray(bool[] array){
		bool returnBool = false;
		for (int i = 0; i< array.Length; i++) {
			returnBool = returnBool || array[i];	
		}
		return returnBool;
	}

	public void Populate(bool[] arr, bool value ){
		for ( int i = 0; i < arr.Length;i++ ) {
			arr[i] = value;
		}
	}

	void performAction(RaycastHit2D hit, int index){
		if (hit.collider.gameObject.tag == "Trampoline" && !springing){
			spring[index] = true;
		} else if (hit.collider.gameObject.tag == "Ladder"){
			ladder[index] = true;
		} else if (hit.collider.gameObject.tag == "Key"){
			
		}
	}

	GameObject[] FindGameObjectsWithLayer (int layer) {
		GameObject[] goArray = FindObjectsOfType<GameObject>();
		System.Collections.Generic.List<GameObject> goList = new System.Collections.Generic.List<GameObject>();
		for (var i = 0; i < goArray.Length; i++) {
			if (goArray[i].layer == layer) {
				goList.Add(goArray[i]);
			}
		}
		if (goList.Count == 0) {
			return null;
		}
		return goList.ToArray();
	}
	
	/**
	 * Enviromental effects
	 **/
	
	void fireDamage(float decrement) {
		for (int i = 0; i<colourEnergy.Length; ++i){
			colourEnergy[i] -= decrement;
			if (colourEnergy[i]<0){
				colourEnergy[i] = 0;
			}
		}
		
		if (isAllEnergyDrained ()) {
			gameEnded = true;
		}

	}
	bool isAllEnergyDrained() {
		bool isEmpty = true;
		for (int i = 0; i<colourEnergy.Length; ++i){
			isEmpty = isEmpty && (colourEnergy[i]<=0);
		}
		return isEmpty;
	}

	void obtainsKey() {
		hasKey = true;
	}

	void touchedDoor() {
		if (hasKey) {
				gameEnded = true;
			winLevel = true;
		}

	}

	void death() {
		gameEnded = true;

	}

	void allowSpring()
	{
		springing = false;
	}

	void obtainsParachute(){
		hasParachute = true;
	}
	
	void touchedMonster(string color){
		bool eatMonster = false;
		for (int i = 0; i<activeColour.Length; ++i){
			if (color.Equals(colourStrings[i+1]) && activeColour[i]){
				colourEnergy[i] +=energyGainFromEat;
				eatMonster = true;
			}
		}

		if (!eatMonster){
			gameEnded = true;
		}
		

		/* Refactored into for loop
		if ((color.Equals ("red") && red)) {
			redEnergy += 50;
		} else if (color.Equals ("blue") && blue) {
			blueEnergy += 50;
		} else if (color.Equals ("green") && green) {
			greenEnergy += 50;
		} else {
			gameEnded = true;
		}
		*/
			
	}

	
	public void ActivateColour(string colour){
		if (colour.Equals(colourStrings[0])){
			Populate(activeColour, false);
			Populate(activeColourLastFrame, false);
		} else {
			for (int i = 1; i < colourStrings.Length; i++){
				if (colour.Equals(colourStrings[i]) && colourEnergy[i-1]-activationEnergy>0){
					Populate(activeColour, false);
					activeColour[i-1] = true;
				}
			}
		}
	}

	public void StartGame (){
		if (!gameStarted){
			print (gameTimeScale);
			Time.timeScale=gameTimeScale;
			gameStarted = true;
		}
	}

	void ActivateGameObjects (GameObject[] Objects, bool activate){
		if (Objects!=null)
		for(int i = 0; i<Objects.Length; ++i){
			if (Objects[i]!=null)
				//Objects[i].active = activate;
				Objects[i].SetActive(activate);
		}
	}
}
