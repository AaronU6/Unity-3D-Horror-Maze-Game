using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour {
    public int x;
    public int y;

    public float coordX;
    public float coordY;

    //Reference:    0 - North, 1 - East, 2 - South, 3 - West
    //              0 - Unassigned, 1 - Open, -1 - Blocked (Wall)  
    public int[] CellEdges = new int[4];
    public MazeCell[] neighbor = new MazeCell[4];

    public bool allEdgesAssigned = false;

    public bool hasBeenVisited = false;

    public float probability;
    public float alpha = 0.5f;

    public void InitializeEdges() {
        for (int i = 0; i < 4; i++) {
            CellEdges[i] = 0;
        }
        this.probability = 0.01f;
    }

    public int GetRandomUnassignedEdge() {
        int i = Random.Range(0, 4);
        while (CellEdges[i] != 0) {
            i++;
            i = i % 4;
        }
        return i;
    }
    public void SetCoord (float x, float y)
    {
        coordX = x;
        coordY = y;
    }

    public void SetEdge (int edge, int value) {
        CellEdges[edge] = value;
        IsAllAssigned();
    }

    private void IsAllAssigned() {
        bool isUnassigned = false;
        for (int i  = 0; i < 4; i++) {
            if (CellEdges[i] == 0) {
                isUnassigned = true;
            }
        }
        if (!isUnassigned) {
            allEdgesAssigned = true;
        }
    }

    public int NextCell() {
        float maxProb = 0.0f;
        int maxIndex = -1;
        for (int i = 0; i < 4; i++) {
            if (neighbor[i] != null && neighbor[i].probability > maxProb) {
                maxIndex = i;
                maxProb = neighbor[i].probability;
                Debug.Log(neighbor[maxIndex].name);
            }
            if (neighbor[i] != null && neighbor[i].probability == maxProb && maxProb != 0.0f && Random.Range(0,4) == 0) {
                maxIndex = i;
                maxProb = neighbor[i].probability;
            }
        }
        
        return maxIndex;
    }

    public void DiffuseProb() {
        for (int i = 0; i < 4; i++) {
            if (neighbor[i] != null) {
                neighbor[i].probability = alpha * this.probability;
            }
        }
    }
}
