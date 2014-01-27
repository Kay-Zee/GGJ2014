using UnityEngine;
using System.Collections;

public class TriggerPlatform : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		print (col.gameObject.tag);
		Transform platform = this.transform.parent;
		if (platform == null){
			print ("platform is null");
		}
		if(col.gameObject.tag == "Player"){
			Physics2D.IgnoreLayerCollision(col.gameObject.layer, platform.gameObject.layer, true);

		}
	}

	void OnTriggerExit2D(Collider2D col){
		Transform platform = transform.parent;
		if(col.gameObject.tag == "Player"){
			Physics2D.IgnoreLayerCollision(col.gameObject.layer, platform.gameObject.layer, false);
		}
	}
}
