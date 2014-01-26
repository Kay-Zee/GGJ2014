﻿using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour {

	[HideInInspector]
	public bool facingRight = true;			// For determining which way the monster is currently facing.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag == "Player"){
			col.gameObject.SendMessage("touchedMonster", "red");
			if (((PlayerController) col.gameObject.GetComponent("PlayerController")).red){
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
