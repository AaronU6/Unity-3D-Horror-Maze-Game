using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treasureController : MonoBehaviour {

	
    public void Generate (MazeCell current, Transform transform) {
        this.transform.parent = transform;
        this.transform.localPosition = new Vector3(current.coordX, 0.5f,current.coordY);
    }

    void OnCollisionEnter(Collision collision) {
		if (collision.transform.tag == "Player") {
			Application.LoadLevel("winScreen");
		}
	}
}
