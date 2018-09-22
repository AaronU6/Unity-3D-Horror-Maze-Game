using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public MazeCell current;
    private List<MazeCell> path = new List<MazeCell>();
	public GameObject playerObject;

    public void Generate (MazeCell current, Transform transform) {
        this.current = current;

        this.transform.parent = transform;
        this.transform.localPosition = new Vector3(this.current.coordX, 2.0f, this.current.coordY);
    }

    public int GetPathSize() {
        return path.Count;
    }

    public void AddToPath(MazeCell c) {
        path.Add(c);
    }

    public bool IsMoving() {
        return inMotion;
    }

    public bool NoticedPlayer () {
        Debug.Log(playerPos != new Vector3(0, 0, 0));
        return (playerPos != new Vector3(0, 0, 0));
    }

    public Vector3 playerPos = new Vector3(0, 0, 0);
    public void LocationOfPlayer (RaycastHit hit) {
        /* Call ray casting return value
         If found playerPos = location otherwise playerpos = null*/
        if (hit.transform.tag == "Player") {
        Debug.Log(hit.transform.tag);
            playerObject = GameObject.FindGameObjectWithTag ("Player");
            this.playerPos.x = playerObject.transform.position.x;
            this.playerPos.y = playerObject.transform.position.y;
            this.playerPos.z = playerObject.transform.position.z;
		}
    }

    public Vector3 GetLocationOfPlayer () { 
        return playerPos;
    }

    //check our fov 
    void raycastOut () {
		RaycastHit hit;
		Ray left = new Ray (transform.position, Vector3.left);
		Ray right = new Ray (transform.position, Vector3.right);
		Ray up = new Ray (transform.position, Vector3.forward);
		Ray down = new Ray (transform.position, Vector3.back);
        bool hitPlayer = false;
		if (Physics.Raycast (left, out hit, 10f) && hit.transform.tag == "Player") {
			LocationOfPlayer (hit);
            hitPlayer = true;
        }
        if (Physics.Raycast (right, out hit, 10f) && hit.transform.tag == "Player") {
			LocationOfPlayer (hit);
            hitPlayer = true;
        }
		if (Physics.Raycast (up, out hit, 10f) && hit.transform.tag == "Player") {
			LocationOfPlayer (hit);
            hitPlayer = true;
        }
		if (Physics.Raycast (down, out hit, 10f) && hit.transform.tag == "Player") {
			LocationOfPlayer (hit);
            hitPlayer = true;
        }
        if (!hitPlayer) {
            this.playerPos.x = playerPos.y = playerPos.z = 0;
        }
	}

    void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Player") {
            Application.LoadLevel("gameOver");
        }
    }
    
    public bool inMotion = false;
    public MazeCell next;
    public Vector3 direction;
    void Update() {
        raycastOut();
        if (inMotion) {
            if (Mathf.Abs(this.transform.position.x - next.transform.position.x) < 0.1f && Mathf.Abs(this.transform.position.z - next.transform.position.z) < 0.1f) {
                inMotion = false;
                this.current = next;
                this.current.probability = 0;
            } else {
                this.transform.Translate(direction);
                //direction = new Vector3(Time.deltaTime * (next.transform.position.x - this.transform.position.x), 0f, Time.deltaTime * (next.transform.position.z - this.transform.position.z));
            }
        } else if (!inMotion && this.current.NextCell() != -1) {
            inMotion = true;
            next = this.current.neighbor[this.current.NextCell()];
            next.DiffuseProb();
            direction = new Vector3(Time.deltaTime * (next.transform.position.x - this.transform.position.x), 0f, Time.deltaTime * (next.transform.position.z - this.transform.position.z));
        } 
    }
}
