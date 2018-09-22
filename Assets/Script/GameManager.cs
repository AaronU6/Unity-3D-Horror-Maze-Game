using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Maze mazePrefab;

    // public GameObject charPrefab;
    private Dictionary<string, Maze> mazeInstance = new Dictionary<string, Maze>();
   // private GameObject Character;
    
    // Use this for initialization
    void Start () {
        BeginLevel();
	}
    
    private void BeginLevel () {
        //for (int i = -1; i <= 1; i++) {
        //for (int j = -1; j <= 1; j++) {
        //Character = Instantiate(charPrefab) as GameObject;
        string key = "Maze: [0][0]";
        mazeInstance.Add(key, Instantiate(mazePrefab) as Maze);
        mazeInstance[key].Generate(0, 0, true, true, true, true , key);
        //}
        //}
    }

    public void GenerateMaze(float posX, float posY) {
        if (!mazeInstance.ContainsKey("Maze: [" + posX + "][" + posY + "]")) {
            string key = "Maze: [" + posX + "][" + posY + "]";
            mazeInstance.Add(key, Instantiate(mazePrefab) as Maze);
            if (mazeInstance.Count == 2)
                StartCoroutine(mazeInstance[key].GenerateNext(posX, posY, true, true, true, true, true, key));
            else
                StartCoroutine(mazeInstance[key].GenerateNext(posX, posY, true, true, true, true, false, key));
        }
    }

    void Update () {
        if (mazeInstance.Count > 0) {
            List<string> keys = new List<string>(mazeInstance.Keys);
            foreach (var mazeKey in keys) {
                string keyN = "Maze: [" + (mazeInstance[mazeKey].shiftX) + "][" + (mazeInstance[mazeKey].shiftY + 1) + "]";
                string keyE = "Maze: [" + (mazeInstance[mazeKey].shiftX + 1) + "][" + (mazeInstance[mazeKey].shiftY) + "]";
                string keyS = "Maze: [" + (mazeInstance[mazeKey].shiftX) + "][" + (mazeInstance[mazeKey].shiftY - 1) + "]";
                string keyW = "Maze: [" + (mazeInstance[mazeKey].shiftX - 1) + "][" + (mazeInstance[mazeKey].shiftY) + "]";

                if (mazeInstance[mazeKey].GetIntersectN() && !mazeInstance.ContainsKey(keyN)) {
                    GenerateMaze((mazeInstance[mazeKey].shiftX), (mazeInstance[mazeKey].shiftY + 1));
                }
                if (mazeInstance[mazeKey].GetIntersectE() && !mazeInstance.ContainsKey(keyE)) {
                    GenerateMaze((mazeInstance[mazeKey].shiftX + 1), (mazeInstance[mazeKey].shiftY));
                }
                if (mazeInstance[mazeKey].GetIntersectS() && !mazeInstance.ContainsKey(keyS)) {
                    GenerateMaze((mazeInstance[mazeKey].shiftX), (mazeInstance[mazeKey].shiftY - 1));
                }
                if (mazeInstance[mazeKey].GetIntersectW() && !mazeInstance.ContainsKey(keyW)) {
                    GenerateMaze((mazeInstance[mazeKey].shiftX - 1), (mazeInstance[mazeKey].shiftY));
                }
            }
        }
    }
}
