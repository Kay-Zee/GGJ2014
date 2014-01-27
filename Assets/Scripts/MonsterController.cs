using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour {

	[HideInInspector]
	public bool facingRight = true;			// For determining which way the monster is currently facing.
	[HideInInspector]
	public string[] colours;
	public int color = 0;
	// Use this for initialization
	void Start () {
	
	}

	void Awake () {
		colours = new string[3];
		colours [0] = "Red";
		colours [1] = "Green";
		colours [2] = "Blue";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag == "Player"){
			col.gameObject.SendMessage("touchedMonster", colours[color]);
			if (((PlayerController) col.gameObject.GetComponent("PlayerController")).activeColour[color]){
				Destroy (this.gameObject);
			}
		}
	}
	
	void OnTriggerStay2D(Collider2D col){
		if(col.gameObject.tag == "Player"){			
			col.gameObject.SendMessage("fireDamage", 0.2f);	
		}
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
}
