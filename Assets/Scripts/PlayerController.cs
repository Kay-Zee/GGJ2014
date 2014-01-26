﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	private bool gameStarted = false;
	private bool gameEnded = false;
	private bool winLevel = false;
	private float gameTimeScale;
	private System.TimeSpan timeLeft;

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
	[HideInInspector]
	public bool green = false;				// Is green activated
	[HideInInspector]
	public bool red = false;				// Is red activated
	[HideInInspector]
	public bool blue = false;				// Is blue activated

	public Texture2D redTex;
	public Texture2D greenTex; 
	public Texture2D blueTex;

	private int redLayer, greenLayer, blueLayer, grayLayer;
	private GameObject[] redLayerObjects,greenLayerObjects,blueLayerObjects,grayLayerObjects;
	

	private float activationEnergy = 10f;
	private float energyDrainRate = 0.1f;
	private float maxEnergy = 100f;
	[HideInInspector]
	public float greenEnergy = 50f;				// Amount of green energy
	[HideInInspector]
	public float redEnergy = 50f;				// Amount of red energy
	[HideInInspector]
	public float blueEnergy = 50f;				// Amount of blue energy

	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	//public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 300f;			// Amount of force added when the player jumps.
	public float climbSpeed = 1f;			// Ladder Climbing Speed
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private Transform ladderCheck;			// A position marking where to check if the player is at a ladder.

	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.


	// Standardized units, useful for making game compatible with multiple resolutions
	private int horizontalUnit = Screen.width/16;
	private int verticalUnit = Screen.height/9;
	private int spacingUnit = Screen.width/32;

	void Awake()
	{
		gameTimeScale = 1;
		timeLeft = System.TimeSpan.FromSeconds (90);

		greenEnergy = 50;
		redEnergy = 50;
		blueEnergy = 50;

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


		// Setting up booleans
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
		
		Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer ("Ground"), true);

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
			RaycastHit2D hitBlue, hitGreen, hitRed;
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


			if (consolidateBoolArray(ladder)) {
				rigidbody2D.gravityScale=0f;

			} else {
				rigidbody2D.gravityScale=1f;
			}

			// If the jump button is pressed and the player is grounded then the player should jump.
			if(Input.GetButtonDown("Jump") && grounded)
				jump = true;
			if (Input.GetButtonDown ("Green")) {
				if (!green && greenEnergy-activationEnergy>0){
					green = true;
					ActivateGameObjects(greenLayerObjects, true);
				} else {
					green = false;
					ActivateGameObjects(greenLayerObjects, false);
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
					red = true;

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
					blue = true;
					ActivateGameObjects(blueLayerObjects, true);
				} else {
					blue = false;
					ActivateGameObjects(blueLayerObjects, false);
				}
			}
			if (blue) {
				anim.SetBool("Blue",true);
				ActivateGameObjects(blueLayerObjects, true);
			} else {
				anim.SetBool("Blue",false);
				ActivateGameObjects(blueLayerObjects, false);
			}

			if (blue || red || green){
				anim.SetBool("Gray",false);
				ActivateGameObjects(grayLayerObjects, false);

			} else {
				anim.SetBool("Gray",true);
				ActivateGameObjects(grayLayerObjects, true);
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
			// Cache the horizontal input.
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis ("Vertical");

			if (consolidateBoolArray(ladder)){
				anim.SetBool("Climb",true);
				if (v > 0) {
					rigidbody2D.velocity = new Vector2( rigidbody2D.velocity.x , 5  );
				} else if (v < 0) {
					rigidbody2D.velocity = new Vector2( rigidbody2D.velocity.x , -5  );
				} else {
					rigidbody2D.velocity = new Vector2( rigidbody2D.velocity.x , 0  );
				}
			} else {
				anim.SetBool("Climb",false);
			}
			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat("Speed", Mathf.Abs(h));

			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			if(h * rigidbody2D.velocity.x < maxSpeed)
				// ... add a force to the player.
				rigidbody2D.AddForce(Vector2.right * h * moveForce);

			// If the player's horizontal velocity is greater than the maxSpeed...
			if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
				// ... set the player's velocity to the maxSpeed in the x axis.
				rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

			// If the input is moving the player right and the player is facing left...
			if(h > 0 && !facingRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if(h < 0 && facingRight)
				// ... flip the player.
				Flip();
			if (h == 0) {
				rigidbody2D.velocity = new Vector2( 0 , rigidbody2D.velocity.y  );
			}
			// If the player should jump...
			if(jump)
			{
				// Set the Jump animator trigger parameter.
				anim.SetTrigger("Jump");
				// Play a random jump audio clip.
				//int i = Random.Range(0, jumpClips.Length);
				//AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

				// Add a vertical force to the player.
				rigidbody2D.AddForce(new Vector2(0f, jumpForce));

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
		}
		if (gameEnded)
			Time.timeScale = 0;

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

	void allowSpring()
	{
		springing = false;
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
	
	void OnGUI() {
		GUI.skin.box.fontSize = spacingUnit;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		if (!gameStarted)
			GUI.Box (new Rect (Screen.width / 2 - horizontalUnit*3, spacingUnit, horizontalUnit * 6, verticalUnit), "Press Space to Start");
		else if (gameEnded){
			if (winLevel){
				GUI.Box (new Rect (Screen.width / 4 , Screen.height / 4, Screen.width / 2, Screen.height / 2), "You Win! \nPress Q to Restart");

			} else {
			GUI.Box (new Rect (Screen.width / 4 , Screen.height / 4, Screen.width / 2, Screen.height / 2), "Game Over! \nPress Q to Restart");
			}
		}
		GUI.Box (new Rect (Screen.width - 2 * horizontalUnit, spacingUnit, horizontalUnit * 2, verticalUnit), string.Format("{0}:{1}:{2}",
		                                                                                                                     timeLeft.Minutes,
		                                                                                                                     timeLeft.Seconds,timeLeft.Milliseconds));

		GUI.DrawTexture(new Rect ((int) (spacingUnit/2), (spacingUnit/2)+(verticalUnit*2)*(1-(float)redEnergy/maxEnergy),horizontalUnit/2,(verticalUnit*2)*((float)redEnergy/maxEnergy)),redTex);		
		GUI.DrawTexture(new Rect ((int) (spacingUnit)+horizontalUnit/2, (spacingUnit/2)+(verticalUnit*2)*(1 - (float)greenEnergy/maxEnergy), horizontalUnit/2,(verticalUnit*2)*((float)greenEnergy/maxEnergy)),greenTex);		
		GUI.DrawTexture(new Rect ((int) (spacingUnit*3/2)+horizontalUnit, (spacingUnit/2)+(verticalUnit*2)*(1 - (float)blueEnergy/maxEnergy),horizontalUnit/2,(verticalUnit*2)*((float)blueEnergy/maxEnergy)),blueTex);		


	}
	
	void fireDamage(float decrement) {
		greenEnergy -= decrement;
		redEnergy -= decrement;
		blueEnergy -= decrement;

		if (isAllEnergyDrained ()) {
			gameEnded = true;
		}

	}
	bool isAllEnergyDrained() {
		return greenEnergy <= 0 && redEnergy <= 0 && blueEnergy <= 0;
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
	
	void touchedMonster(string color){
		if ((color.Equals ("red") && red)) {
			redEnergy += 50;
		} else if (color.Equals ("blue") && blue) {
			blueEnergy += 50;
		} else if (color.Equals ("green") && green) {
			greenEnergy += 50;
		} else {
			gameEnded = true;
		}
			
	}
	
	void ActivateGameObjects (GameObject[] Objects, bool activate){
		if (Objects!=null)
		for(int i = 0; i<Objects.Length; ++i){
			if (Objects[i]!=null)
				Objects[i].active = activate;
		}
	}
}
