using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : MonoBehaviour {

    public MazeCell cell1, cell2;

    public void Initialize(MazeCell cell1, MazeCell cell2, Quaternion rotation)
    {
        this.cell1 = cell1;
        this.cell2 = cell2;
        transform.parent = cell1.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = rotation;
    }

    void OnCollisionEnter(Collision obj)
    {
        GameObject.Destroy(this.gameObject);
    }

    public void delete() {
        transform.parent = null;
    }

}
