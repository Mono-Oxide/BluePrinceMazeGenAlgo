using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

public class Program
{
    public static void Main()
    {
        Visualizer vis = new Visualizer();
        MainAlgorithm ma = new MainAlgorithm();
        Console.Write("Starting now");
        ma.Execute();
        vis.Show(ma.maze);
    }
}

public class Maze
{
    public int size;
    public Node[,] grid;
}


public class MainAlgorithm
{
    public Maze maze;

    public void Execute()
    {
        // go to random location on grid

        // loop start
        // loop start
        // pick direction

        // get pathway options (poll)

        // choose one of the pathway types

        // R O T A T E pathway type

        // place in node
        // loop end

        // go to random unvisited location near walls next to a visited cell 
        
        // break open wall between prev cells

        // loop end
    }
}


public class Node
{
    public Wall[] walls;

    public override string ToString()
    {
        string s = "";
        switch (walls)
        {
            case [Wall.North]: s = ""; break;
        }

        return s;
    }
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

    public Wall[][] pathwayTypes2 = new Wall[8][]
    {
        new Wall[3] {Wall.North, Wall.East, Wall.South},
        new Wall[0] {},
        new Wall[2] {Wall.East, Wall.West},
        new Wall[2] {Wall.North, Wall.East},
        new Wall[2] {Wall.North, Wall.West},
        new Wall[1] {Wall.West},
        new Wall[1] {Wall.North},
        new Wall[1] {Wall.East}
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

    public Wall[][] Poll()
    {
        Random rng = new Random();
        Wall[][] result = new Wall[3][];
        result = rng.GetItems<Wall[]>(pathwayTypes2, 3);
        return result;
    }
}

public class Visualizer
{
    public void Show(Maze maze)
    {
        Node[,] grid2 = maze.grid;
        for (int x = 0; x < grid2.GetLength(0); x++)
        {
            string s = "";
            for (int y = 0; y < grid2.GetLength(1); y++)
            {
                s += grid2[x, y].ToString();   
            }
            Console.WriteLine(s);
        }

    }    
}