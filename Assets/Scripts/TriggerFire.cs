using UnityEngine;
using System.Collections;

public class TriggerFire : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag == "Player"){
			col.gameObject.SendMessage("fireDamage", 0.5f);

		}
	}

	void OnTriggerStay2D(Collider2D col){
		if(col.gameObject.tag == "Player"){			
			col.gameObject.SendMessage("fireDamage", 0.2f);	
		}
	}


}
