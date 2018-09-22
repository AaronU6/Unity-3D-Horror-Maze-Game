using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityCell {
   
    public PriorityCell prev;
    public MazeCell cell;
    public int priority;

    public PriorityCell(MazeCell cell, int priority, PriorityCell prev) {
        this.cell = cell;
        this.prev = prev;

        this.priority = priority;
    }

    public int GetPriority() {
        return this.priority;
    }

    public MazeCell GetCell() {
        return this.cell;
    }
}