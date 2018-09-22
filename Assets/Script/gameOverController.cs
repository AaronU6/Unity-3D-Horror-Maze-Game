using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameOverController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioSource audio = GetComponent<AudioSource>();
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Texture gameOverTexture;

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),gameOverTexture);

	}

}
