using UnityEngine;
using System.Collections;

public class TriggerParachute : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		print ("touching parachute");
		if (col.gameObject.tag == "Player") {
			col.gameObject.SendMessage ("obtainsParachute");
			
			//player will now emit a burst
			col.gameObject.particleSystem.Emit(50);
			
			Destroy (this.gameObject);
		}
		
	}
}
