using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime;

public class Program
{
    public static void Main()
    {
        Visualizer vis = new Visualizer();
        MainAlgorithm ma = new MainAlgorithm();
        Console.Write("Starting now");
        ma.Execute(20);
        vis.Show(ma.maze);
    }
}


public class MainAlgorithm
{
    public Node[,] maze;
    public bool[,] visitedGrid;

    public void Execute(int size)
    {
        Random rng = new Random();
        Pathway pathway = new Pathway();
        PathAlgorithm pa = new PathAlgorithm(size);
        ChooseAlgorithm ca = new ChooseAlgorithm();
        HashSet<Node> movementOptions = new HashSet<Node>(); 

        int[] modifier = new int[8] {1, 0, 0, 1, -1, 0, 0, -1};

        maze = new Node[size, size];
        int curX = 0;
        int curY = 0;
        int tarX = 0;
        int tarY = 0;

        // go to random location on grid

        curX = rng.Next(size);
        curY = rng.Next(size);
        movementOptions.Add(new Node(curX, curY, null));

        bool allVisited = visitedGrid.Cast<bool>().All(x => x == true);

        // loop start
        while (allVisited) {
        
            // loop start
            while (movementOptions.Count > 0) {
                // pick direction
                (Node, Direction) res = pa.pathFind(curX, curY, movementOptions.ToList<Node>()); // random walk
                movementOptions.Remove(res.Item1);
                // (int, int, Direction) res = pa.pathFind(curX, curY); // bfs

                tarX = res.Item1.x;
                tarY = res.Item1.y;
                Direction dir = res.Item2;

                // get pathway options (poll)
                Wall[][] options = pathway.Poll();
        
                // choose one of the pathway types
                Wall[] pathwayType = ca.Choose(options);

                // R O T A T E pathway type
                pathwayType = pathway.Rotate(pathwayType, (int)dir);
        
                // place in node
                Node target = new Node(tarX, tarY, res.Item1);
                target.walls = pathwayType;
                target.visited = true;
                maze[tarX, tarY] = target;

                visitedGrid[tarX, tarY] = true;
                curX = tarX;
                curY = tarY;

                // generate new nodes
                for (int i = 0; i < 4; i++)
                {
                    int posNewX = tarX + modifier[2*i];
                    int posNewY = tarY + modifier[2*i + 1];
                    if (posNewX < 0 || posNewX >= size || posNewY < 0 || posNewY >= size) 
                    {
                        if (!maze[posNewX, posNewY].visited)//!movementOptions.Contains(maze[posNewX, posNewY]) && !maze[posNewX, posNewY]) 
                        {
                            movementOptions.Add(new Node(posNewX, posNewY, target));
                        }   
                    }
                }
            
            // loop end
            }

        // go to random unvisited location near walls next to a visited cell 
        
        // break open wall between prev cells

        allVisited = visitedGrid.Cast<bool>().All(x => x == true);

        // loop end
        }
    }
}



public class PathAlgorithm
{
    int size;
    int[] modifier = new int[8] {1, 0, 0, 1, -1, 0, 0, -1};

    public PathAlgorithm(int size_)
    {
        size = size_;
    }

    public (Node, Direction) pathFind(int curX, int curY, List<Node> options) 
    {
        Random rng = new Random();
        int dir = rng.Next(4);
        int tarX = curX + modifier[2 * dir];
        int tarY = curY + modifier[2 * dir + 1];

        Node next = options[rng.Next(options.Count)];

        while (tarX < 0 || tarX >= size || tarY < 0 || tarY >= size)
        {
            dir = rng.Next(4);
            tarX = curX + modifier[2 * dir];
            tarY = curY + modifier[2 * dir + 1];
        }

        return (next, (Direction)dir);
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
    public int x;
    public int y;
    public Wall[] walls;
    public Node prev;
    public bool visited = false;

    public Node(int x_, int y_, Node prev_)
    {
        x = x_;
        y = y_;
        prev = prev_;
    }

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
    public void Show(Node[,] maze)
    {
        Node[,] grid2 = maze;
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