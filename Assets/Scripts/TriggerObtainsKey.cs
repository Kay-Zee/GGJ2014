using UnityEngine;
using System.Collections;

public class TriggerObtainsKey : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			col.gameObject.SendMessage ("obtainsKey");
			Destroy (this.gameObject);
		}
	}

			




}


