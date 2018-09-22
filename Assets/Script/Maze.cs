using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {
    public MazeCell mazeCellPrefab;
    public MazeWall mazeWallPrefab;
    
    public Boundary boundaryNorthPrefab;
    public Boundary boundaryEastPrefab;
    public Boundary boundarySouthPrefab;
    public Boundary boundaryWestPrefab;

    public Enemy enemyPrefab;
    public treasureController treasurePrefab;

    private MazeCell[,] cells;
    private MazeWall[,,] walls;
    
    private Boundary boundaryN;
    private Boundary boundaryE;
    private Boundary boundaryS;
    private Boundary boundaryW;

    public bool DebuggingEnemy;
    public bool DebuggingTrasure;

    private Enemy enemy;
    private treasureController treasure;

    private int size_x = 15;
    private int size_y = 15;
    //Reference:    0 - North, 1 - East, 2 - South, 3 - West
    private int[,] Direction = { {0, 1}, { 1, 0 }, { 0, -1 }, { -1, 0 } };
    private string[] DirName = { "North", "East", "South", "West" };
    private int[] inverseDirection = { 2, 3, 0, 1 };

    private Quaternion[] rotations = {Quaternion.identity, Quaternion.Euler(0f, 90f, 0f), Quaternion.Euler(0f, 180f, 0f), Quaternion.Euler(0f, 270f, 0f)};

    public float shiftX, shiftY;

    public string mazeKey = "";

    public void Generate(float shiftX, float shiftY) {
        cells = new MazeCell[size_x, size_y];
        walls = new MazeWall[size_x, size_y, 4];

        this.shiftX = shiftX;
        this.shiftY = shiftY;

        List<MazeCell> initializedCells = new List<MazeCell>();
        initializedCells.Add(InitializeCell(0,7));
        
        while (initializedCells.Count > 0) {
            int currentIndex = initializedCells.Count - 1;
            MazeCell currentCell = initializedCells[currentIndex];

            if (!currentCell.hasBeenVisited) {
                currentCell.hasBeenVisited = true;
            }

            if (currentCell.allEdgesAssigned) {
                initializedCells.RemoveAt(currentIndex);
                continue;
            }

            int dir = currentCell.GetRandomUnassignedEdge();
            int newX = currentCell.x + Direction[dir, 0];
            int newY = currentCell.y + Direction[dir, 1];

            //Outer Wall or Preventing Loop
            if (newX < 0 || newX >= size_x || newY < 0 || newY >= size_y) {
                currentCell.SetEdge(dir, -1);
                if (!(currentCell.x == 0 && currentCell.y == 7) && !(currentCell.x == 14 && currentCell.y == 7))
                {
                    walls[currentCell.x, currentCell.y, dir] = Instantiate(mazeWallPrefab) as MazeWall;
                    walls[currentCell.x, currentCell.y, dir].name = DirName[dir];
                    walls[currentCell.x, currentCell.y, dir].Initialize(currentCell, null, rotations[dir]);
                } else if (currentCell.x == 0 && currentCell.y == 7) {
                }
            } else if (cells[newX, newY] == null) {
                currentCell.SetEdge(dir, 1);
                initializedCells.Add(InitializeCell(newX, newY));
                cells[newX, newY].SetEdge(inverseDirection[dir], 1);
            } else if (cells[newX, newY].hasBeenVisited) {
                currentCell.SetEdge(dir, -1);
                cells[newX, newY].SetEdge(inverseDirection[dir], -1);
            } 
        }   
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                cells[i, j].hasBeenVisited = false;
            }
        }
        Solver(14, 7);
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                cells[i, j].hasBeenVisited = false;
            }
        }
        
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                for (int k = 0; k < 4; k++) {
                    if (cells[i, j].CellEdges[k] == -1 && !(i == 0 && k == 3) && !(i == size_x -1 && k == 1) && !(j == 0 && k == 2) && !(j == size_y - 1 && k == 0)) {
                        walls[i, j, k] = Instantiate(mazeWallPrefab) as MazeWall;
                        walls[i, j, k].name = DirName[k];
                        walls[i, j, k].Initialize(cells[i, j], cells[i+Direction[k, 0], j+Direction[k, 1]], rotations[k]);
                    }
                }
            }
        }

        boundaryN = Instantiate(boundaryNorthPrefab as Boundary);
        boundaryN.Generate((shiftX*size_x*3),(shiftY*size_y*3), this.transform);
        boundaryE = Instantiate(boundaryEastPrefab as Boundary);
        boundaryE.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        boundaryS = Instantiate(boundarySouthPrefab as Boundary);
        boundaryS.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        boundaryW = Instantiate(boundaryWestPrefab as Boundary);
        boundaryW.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
    }

    public void Generate(float shiftX, float shiftY, bool North, bool South, bool East, bool West, string mazeKey) {
        cells = new MazeCell[size_x, size_y];
        walls = new MazeWall[size_x, size_y, 4];

        this.shiftX = shiftX;
        this.shiftY = shiftY;

        this.mazeKey = mazeKey;
        List<MazeCell> initializedCells = new List<MazeCell>();
        initializedCells.Add(InitializeCell(0,7));

        boundaryN = Instantiate(boundaryNorthPrefab as Boundary);
        boundaryN.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        boundaryE = Instantiate(boundaryEastPrefab as Boundary);
        boundaryE.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        boundaryS = Instantiate(boundarySouthPrefab as Boundary);
        boundaryS.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        boundaryW = Instantiate(boundaryWestPrefab as Boundary);
        boundaryW.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        while (initializedCells.Count > 0) {
            int currentIndex = initializedCells.Count - 1;
            MazeCell currentCell = initializedCells[currentIndex];

            if (!currentCell.hasBeenVisited) {
                currentCell.hasBeenVisited = true;
            }

            if (currentCell.allEdgesAssigned) {
                initializedCells.RemoveAt(currentIndex);
                continue;
            }

            int dir = currentCell.GetRandomUnassignedEdge();
            int newX = currentCell.x + Direction[dir, 0];
            int newY = currentCell.y + Direction[dir, 1];

            //Outer Wall or Preventing Loop
            if (newX < 0 || newX >= size_x || newY < 0 || newY >= size_y) {
                currentCell.SetEdge(dir, -1);
                if (!(currentCell.x == 0 && currentCell.y == 7 && East) && !(currentCell.x == 14 && currentCell.y == 7 && West) && !(currentCell.x == 7 && currentCell.y == 0 && South) && !(currentCell.x == 7 && currentCell.y == 14 && North))
                {
                    walls[currentCell.x, currentCell.y, dir] = Instantiate(mazeWallPrefab) as MazeWall;
                    walls[currentCell.x, currentCell.y, dir].name = DirName[dir];
                    walls[currentCell.x, currentCell.y, dir].Initialize(currentCell, null, rotations[dir]);
                }
            } else if (cells[newX, newY] == null) {
                currentCell.SetEdge(dir, 1);
                initializedCells.Add(InitializeCell(newX, newY));
                cells[newX, newY].SetEdge(inverseDirection[dir], 1);
            } else if (cells[newX, newY].hasBeenVisited) {
                currentCell.SetEdge(dir, -1);
                cells[newX, newY].SetEdge(inverseDirection[dir], -1);
            } 
        }   
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                cells[i, j].hasBeenVisited = false;
            }
        }
        Solver(14, 7);
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                cells[i, j].hasBeenVisited = false;
            }
        }
        
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                for (int k = 0; k < 4; k++) {
                    if (cells[i, j].CellEdges[k] == -1 && !(i == 0 && k == 3) && !(i == size_x -1 && k == 1) && !(j == 0 && k == 2) && !(j == size_y - 1 && k == 0)) {
                        walls[i, j, k] = Instantiate(mazeWallPrefab) as MazeWall;
                        walls[i, j, k].name = DirName[k];
                        walls[i, j, k].Initialize(cells[i, j], cells[i+Direction[k, 0], j+Direction[k, 1]], rotations[k]);
                    }
                }
            }
        }
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                for (int k = 0; k < 4; k++) {
                    if (cells[i, j].CellEdges[k] == 1
                        && !(i == 7 && j == 0 && k == 0)
                        && !(i == 14 && j == 7 && k == 1)
                        && !(i == 7 && j == 14 && k == 2)
                        && !(i == 0 && j == 7 && k == 3)) {
                        if (k == 0)
                            cells[i, j].neighbor[k] = cells[i, j+1];
                        else if (k == 1)
                            cells[i, j].neighbor[k] = cells[i + 1, j];
                        else if(k == 2)
                            cells[i, j].neighbor[k] = cells[i, j - 1];
                        else if (k == 3)
                            cells[i, j].neighbor[k] = cells[i - 1, j];
                        
                    }
                }
            }
        }
       GenerateEnemy();
       if (DebuggingTrasure) {
            GenerateTreasure();
       }
    }

    
    public IEnumerator GenerateNext(float shiftX, float shiftY, bool North, bool South, bool East, bool West, bool Treasure, string mazeKey) {
        WaitForSeconds delay = new WaitForSeconds(0f);
        cells = new MazeCell[size_x, size_y];
        walls = new MazeWall[size_x, size_y, 4];

        this.shiftX = shiftX;
        this.shiftY = shiftY;

        this.mazeKey = mazeKey;
        List<MazeCell> initializedCells = new List<MazeCell>();
        initializedCells.Add(InitializeCell(0,7));

        boundaryN = Instantiate(boundaryNorthPrefab as Boundary);
        boundaryN.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        boundaryE = Instantiate(boundaryEastPrefab as Boundary);
        boundaryE.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        boundaryS = Instantiate(boundarySouthPrefab as Boundary);
        boundaryS.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        boundaryW = Instantiate(boundaryWestPrefab as Boundary);
        boundaryW.Generate((shiftX * size_x * 3), (shiftY * size_y * 3), this.transform);
        while (initializedCells.Count > 0) {
            int currentIndex = initializedCells.Count - 1;
            MazeCell currentCell = initializedCells[currentIndex];

            if (!currentCell.hasBeenVisited) {
                currentCell.hasBeenVisited = true;
            }

            if (currentCell.allEdgesAssigned) {
                initializedCells.RemoveAt(currentIndex);
                continue;
            }

            int dir = currentCell.GetRandomUnassignedEdge();
            int newX = currentCell.x + Direction[dir, 0];
            int newY = currentCell.y + Direction[dir, 1];

            //Outer Wall or Preventing Loop
            if (newX < 0 || newX >= size_x || newY < 0 || newY >= size_y) {
                currentCell.SetEdge(dir, -1);
                if (!(currentCell.x == 0 && currentCell.y == 7 && East) && !(currentCell.x == 14 && currentCell.y == 7 && West) && !(currentCell.x == 7 && currentCell.y == 0 && South) && !(currentCell.x == 7 && currentCell.y == 14 && North))
                {
                    walls[currentCell.x, currentCell.y, dir] = Instantiate(mazeWallPrefab) as MazeWall;
                    walls[currentCell.x, currentCell.y, dir].name = DirName[dir];
                    walls[currentCell.x, currentCell.y, dir].Initialize(currentCell, null, rotations[dir]);
                }
            } else if (cells[newX, newY] == null) {
                currentCell.SetEdge(dir, 1);

                yield return delay;
                initializedCells.Add(InitializeCell(newX, newY));
                cells[newX, newY].SetEdge(inverseDirection[dir], 1);
            } else if (cells[newX, newY].hasBeenVisited) {
                currentCell.SetEdge(dir, -1);
                cells[newX, newY].SetEdge(inverseDirection[dir], -1);
            } 
        }   
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                cells[i, j].hasBeenVisited = false;
            }
        }
        Solver(14, 7);
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                cells[i, j].hasBeenVisited = false;
            }
        }
        
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                for (int k = 0; k < 4; k++) {
                    if (cells[i, j].CellEdges[k] == -1 && !(i == 0 && k == 3) && !(i == size_x -1 && k == 1) && !(j == 0 && k == 2) && !(j == size_y - 1 && k == 0)) {
                        walls[i, j, k] = Instantiate(mazeWallPrefab) as MazeWall;
                        walls[i, j, k].name = DirName[k];
                        walls[i, j, k].Initialize(cells[i, j], cells[i+Direction[k, 0], j+Direction[k, 1]], rotations[k]);
                    }
                }
            }
        }
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                for (int k = 0; k < 4; k++) {
                    if (cells[i, j].CellEdges[k] == 1
                        && !(i == 7 && j == 0 && k == 0)
                        && !(i == 14 && j == 7 && k == 1)
                        && !(i == 7 && j == 14 && k == 2)
                        && !(i == 0 && j == 7 && k == 3)) {
                        if (k == 0)
                            cells[i, j].neighbor[k] = cells[i, j+1];
                        else if (k == 1)
                            cells[i, j].neighbor[k] = cells[i + 1, j];
                        else if(k == 2)
                            cells[i, j].neighbor[k] = cells[i, j - 1];
                        else if (k == 3)
                            cells[i, j].neighbor[k] = cells[i - 1, j];
                        
                    }
                }
            }
        }
        GenerateEnemy();
        if (Treasure) {
            GenerateTreasure();
        }
    }

    public void GenerateEnemy() {
        int x = (Random.Range(0, 2) == 0) ? -1 : 1;
        int y = (Random.Range(0, 2) == 0) ? -1 : 1;
        if (!DebuggingEnemy) {
           x = x*Random.Range(3, 6)+7;
           y = y*Random.Range(3, 6)+7;
        } else {
           x = x*Random.Range(1, 3)+7;
           y = y*Random.Range(1, 3)+7;
        }
        enemy = Instantiate(enemyPrefab) as Enemy;
        enemy.Generate(cells[y, x], this.transform);
    }

    public void GenerateTreasure()
    {
        int x = (Random.Range(0, 2) == 0) ? -1 : 1;
        int y = (Random.Range(0, 2) == 0) ? -1 : 1;

        x = x * Random.Range(3, 6) + 7;
        y = y * Random.Range(3, 6) + 7;

        treasure = Instantiate(treasurePrefab) as treasureController;
        treasure.Generate(cells[y, x], this.transform);
    }

    List<MazeCell> path = new List<MazeCell>();
    public bool Solver(int x, int y) {
        path.Add(cells[x, y]);
        cells[x, y].hasBeenVisited = true;
        bool b = false;
        if (x == 0 && y == 7) {
            b = true;
            return b;
        }
        for (int i = 0; i < 4; i++) {
            if (cells[x,y].CellEdges[i] == 1 && !cells[x + Direction[i, 0], y + Direction[i, 1]].hasBeenVisited) {
                b = Solver(x + Direction[i, 0], y + Direction[i, 1]);
                if (b) {
                    return b;
                }
            }
        }
        if (!b) {
            path.RemoveAt(path.Count-1);
        }
        return b;
    }
    
    public MazeCell GetCell(int x, int y) {
        return cells[x, y];
    }

    public MazeCell InitializeCell(int x, int y) {
        MazeCell newCell = Instantiate(mazeCellPrefab) as MazeCell;
        cells[x, y] = newCell;

        newCell.x = x;
        newCell.y = y;
        
        newCell.InitializeEdges();
        newCell.SetCoord((3*(x - size_x * 0.5f+ 0.5f) + shiftX*size_x*3),(3*(y - size_y * 0.5f + 0.5f) + shiftY*size_y*3));
        
        newCell.name = mazeKey + " Cell: [" + x + "][" + y + "]";
        newCell.transform.parent = transform;
        //Initialize the position of the cell (A cell is 1 unit large)
        newCell.transform.localPosition = new Vector3(newCell.coordX, 0f, newCell.coordY);
        
        return newCell;
    }

    private bool boundaryNIntersected = false;
    private bool boundaryEIntersected = false;
    private bool boundarySIntersected = false;
    private bool boundaryWIntersected = false;
    
    public bool GetIntersectN () {
        return boundaryNIntersected;
    }

    public bool GetIntersectE() {
        return boundaryEIntersected;
    }

    public bool GetIntersectS() {
        return boundarySIntersected;
    }

    public bool GetIntersectW() {
        return boundaryWIntersected;
    }

    public void MakePathToPlayer(Vector3 playerPos) {
        int[] pos = PositionToCellCoord(playerPos);
        Debug.Log("HELLOOOOL");
        Debug.Log(pos[0] +", "+ pos[1]);
        if (pos != null)
            MakePath(cells[pos[0], pos[1]]);
    }

    public void MakePath(MazeCell end) {
        List<PriorityCell> queue = new List<PriorityCell>();

        PriorityCell first = new PriorityCell(enemy.current, 0, null);
        queue.Add(first);

        int i = 0;
        bool isAtEnd = (queue[i].cell.x == end.x && queue[i].cell.y == end.y);
        Debug.Log(queue[i].cell.x + ", " + queue[i].cell.y + " " + end.x + ", " + end.y);
        int inc = 1;
        while (!isAtEnd) {
            PriorityCell cur = queue[i];
            //Up
            if (cur.cell.y < size_y - 1
                && cells[cur.cell.x, cur.cell.y].CellEdges[0] == 1) {
                PriorityCell next = new PriorityCell(cells[cur.cell.x, cur.cell.y + 1], (queue[i].GetPriority() + inc), cur);
                queue.Add(next);
                inc = 1;
            }
            //Right
            if (cur.cell.x < size_x - 1
                && cells[cur.cell.x, cur.cell.y].CellEdges[1] == 1) {
                PriorityCell next = new PriorityCell(cells[cur.cell.x + 1, cur.cell.y], (queue[i].GetPriority() + inc), cur);
                queue.Add(next);
                inc = 1;
            }
            //Down
            if (cur.cell.y > 0
                && cells[cur.cell.x, cur.cell.y].CellEdges[2] == 1) {
                PriorityCell next = new PriorityCell(cells[cur.cell.x, cur.cell.y - 1], (queue[i].GetPriority() + inc), cur);
                queue.Add(next);
                inc = 1;
            }
            //Left
            if (cur.cell.x > 0
                && cells[cur.cell.x, cur.cell.y].CellEdges[3] == 1) {
                PriorityCell next = new PriorityCell(cells[cur.cell.x - 1, cur.cell.y], (queue[i].GetPriority() + inc), cur);
                queue.Add(next);
                inc = 1;
            }
            i++;
            isAtEnd = (queue[i].cell.x == end.x && queue[i].cell.y == end.y);
        }

        PriorityCell current = queue[i];
        List<MazeCell> path = new List<MazeCell>();
        while (current.GetPriority() != 0) {
            path.Insert(0, current.GetCell());
            current = current.prev;
        }

        enemy.current.probability = 0;
        if (path.Count > 0)
            path[0].probability = 1;
    }

    public int[] PositionToCellCoord (Vector3 pos) {
        for (int i  = 0;  i < size_y; i++) {
            for (int j = 0; j < size_x; j++) {
                if (Mathf.Abs(cells[i,j].coordX - pos.x) <= 1.5f && Mathf.Abs(cells[i, j].coordY - pos.z) <= 1.5f) {
                    int[] position = new int[2];
                    position[0] = i;
                    position[1] = j;

                    return position;
                }
            }
        }
        return null;
    }

    void Update() {
        if (!boundaryNIntersected && boundaryN.GetIntersected()) {
            boundaryNIntersected = true;
        }
        if(!boundaryEIntersected && boundaryE.GetIntersected()) {
            boundaryEIntersected = true;
        }
        if(!boundarySIntersected && boundaryS.GetIntersected()) {
            boundarySIntersected = true;
        }
        if(!boundaryWIntersected && boundaryW.GetIntersected()) {
            boundaryWIntersected = true;
        }
        if (enemy != null && !enemy.IsMoving()) {
            if (enemy.NoticedPlayer()) {
                MakePathToPlayer(enemy.GetLocationOfPlayer());
            }
        }
    }
}
