using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour {

    private float x, y;
    private bool hasIntersected = false;
    public void Generate(float x, float y, Transform transform) {
        this.x = x;
        this.y = y;

        this.transform.parent = transform;
        this.transform.Translate(new Vector3(this.x, 0f, this.y));
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Player")
            hasIntersected = true;
    }

    public bool GetIntersected() {
        return hasIntersected;
    }
}
