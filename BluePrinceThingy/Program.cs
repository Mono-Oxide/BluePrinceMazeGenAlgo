using System;
using System.Collections.Generic;
using System.ComponentModel;

public class Maze
{
    public int size;
    public Node[] grid;
}

public class Node
{
    public Wall[] walls;
}

public enum Wall
{
    North,
    East,
    South,
    West
}

public class Pathway {

    public Dictionary<string, Wall[]> pathwayTypes = new Dictionary<string, Wall[]>
    {
        {"dead end", [Wall.North, Wall.East, Wall.South]},
        {"4-way", []},
        {"straight", [Wall.East, Wall.West]},
        {"turn-L", [Wall.North, Wall.East]},
        {"turn-R", [Wall.North, Wall.West]},
        {"t-ur", [Wall.West]},
        {"t-lr", [Wall.North]},
        {"t-ul", [Wall.East]},
    }; 

    public Wall[] Rotate(Wall[] original, int turnAmount)
    {
        Wall[] result = new Wall[original.Length];
        for (int i = 0; i < original.Length; i++)
        {
            result[i] = (Wall)(((int)original[i] + turnAmount) % 4);
        }
        return result;
    }
    
}