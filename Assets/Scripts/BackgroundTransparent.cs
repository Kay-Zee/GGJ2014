using UnityEngine;
using System.Collections;

public class BackgroundTransparent : MonoBehaviour {


	//Ideally hook this up to "green_transparent filter" (a translucent PNG), "red_transparent_filter" etc
	//To change background tint without needing new images

	//BUG: if background starts monochromic, adding another monochromic filter
	//may give unexpected color --> e.g background yellow + filter_green -> ?
	
	Shader shader_transparent;

	// Use this for initialization
	void Start () {

		shader_transparent = Shader.Find( "Transparent/Diffuse" );
	}
	
	// Update is called once per frame
	void Update () {
	
		renderer.material.shader = shader_transparent;

	}
}
