using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
    public Node[,] maze;

    public void Execute(int size)
    {
        Random rng = new Random();
        Pathway pathway = new Pathway();
        PathAlgorithm pa = new PathAlgorithm(size);
        ChooseAlgorithm ca = new ChooseAlgorithm(); 

        int[] modifier = new int[8] {1, 0, 0, 1, -1, 0, 0, -1};

        maze = new Node[size, size];
        int curX = 0;
        int curY = 0;
        int tarX = 0;
        int tarY = 0;

        // go to random location on grid

        curX = rng.Next(size);
        curY = rng.Next(size);

        // loop start
        // loop start
        // pick direction
        Direction dir = pa.pathFind();
        tarX = curX + modifier[2 * (int)dir];
        tarY = curY + modifier[2 * (int)dir + 1];

        // get pathway options (poll)
        Wall[][] options = pathway.Poll();
        
        // choose one of the pathway types


        // R O T A T E pathway type

        // place in node
        // loop end

        // go to random unvisited location near walls next to a visited cell 
        
        // break open wall between prev cells

        // loop end
    }
}



public class PathAlgorithm
{
    int size;
    public PathAlgorithm(int size_)
    {
        size = size_;
    }

    public Direction pathFind() 
    {
        Random rng = new Random();
        return (Direction)rng.Next(4);
    }
}

public enum Direction
{
    Up,
    Right,
    Down,
    Left
};

public class ChooseAlgorithm
{
    public Wall[] Choose(Wall[][] options) 
    {
        return options[0];
    }
}



public class Node
{
    public Wall[] walls;
    public bool visited = false;

    public (string, string, string) Stringer()
    {
        (string, string, string) res = ("", "", "");
        switch (walls)
        {
            case [Wall.North]: res = ("###", "   ", "# #"); break;
            case [Wall.East]: res = ("# #", "  #", "# #"); break;
            case [Wall.South]: res = ("# #", "   ", "# #"); break;
            case [Wall.West]: res = ("# #", "#  ", "# #"); break;

            case [Wall.North, Wall.East]: res = ("###", "  #", "# #"); break;
            case [Wall.North, Wall.South]: res = ("###", "   ", "###"); break;
            case [Wall.North, Wall.West]: res = ("###", "#  ", "###"); break;
            case [Wall.East, Wall.South]: res = ("# #", "  #", "###"); break;
            case [Wall.East, Wall.West]: res = ("# #", "# #", "# #"); break;
            case [Wall.South, Wall.West]: res = ("# #", "#  ", "###"); break;

            case [Wall.North, Wall.East, Wall.South]: res = ("###", "  #", "###"); break;
            case [Wall.East, Wall.South, Wall.West]: res = ("# #", "# #", "###"); break;
            case [Wall.South, Wall.West, Wall.North]: res = ("###", "#  ", "###"); break;
            case [Wall.West, Wall.North, Wall.East]: res = ("###", "# #", "# #"); break;

            default: res = ("# #", "   ", "# #"); break;
        }

        return res;
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
            string row1 = "";
            string row2 = "";
            string row3 = "";
            for (int y = 0; y < grid2.GetLength(1); y++)
            {
                (string, string, string) s = grid2[x, y].Stringer();
                row1 += s.Item1;
                row2 += s.Item2;
                row3 += s.Item3;   
            }
            Console.WriteLine(row1);
            Console.WriteLine(row2);
            Console.WriteLine(row3);
        }
    }    
}