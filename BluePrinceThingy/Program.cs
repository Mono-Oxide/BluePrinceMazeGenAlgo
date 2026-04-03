using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime;
using System.Data;
using System.ComponentModel.DataAnnotations;
public class Program
{
    static List<int> intersectsList = new List<int>();
    static List<int> deadendList = new List<int>(); 

    public static void Main()
    {
        deadendList = new List<int>(); 
        intersectsList = new List<int>();
        MainAlgorithm ma = new MainAlgorithm();

        for (int i = 0; i < 1; i++) {
        Visualizer vis = new Visualizer();
        ma = new MainAlgorithm();
        Console.WriteLine("A new maze is now generating.");
        ma.Execute(8);
        vis.Show(ma.maze);

        int intersects = 0;
        int deadends = 0;
        foreach (Node node in ma.maze)
        {
            if (node.walls.Length < 2) intersects++;
            if (node.walls.Length == 3) deadends++;
        }

            Console.WriteLine($"Maze {i+1}/1000 is completed - DE: {deadends}, I:{intersects}.");
            intersectsList.Add(intersects);
            deadendList.Add(deadends);
        }

        Console.WriteLine("Done!");

        Agents agent = new Agents();
        agent.maze = ma.maze;
        //int ranWalk = agent.RandomWalk();
        int dfs = agent.DepthFirst();
        //Console.WriteLine("RandomWalk solved in " + ranWalk + " steps");
        Console.WriteLine("RandomWalk solved in " + dfs + " steps");

        Console.WriteLine($"Avg DE: {deadendList.Average()}, Avg I: {intersectsList.Average()}");
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
        HashSet<Node> visited = new HashSet<Node>();

        int[] modifier = new int[8] {1, 0, 0, 1, -1, 0, 0, -1};
        Node nullNode = new Node(-1, -1, null);

        maze = new Node[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                maze[x, y] = new Node(x, y, nullNode);
            }
        }

        visitedGrid = new bool[size, size];
        int curX = 0;
        int curY = 0;
        int tarX = 0;
        int tarY = 0;

        // go to random location on grid

        curX = rng.Next(size);
        curY = rng.Next(size);
        movementOptions.Add(new Node(curX, curY, nullNode));

        bool allVisited = visitedGrid.Cast<bool>().All(x => x == true);

        // loop start
        while (!allVisited) {
        
            // loop start
            while (movementOptions.Count > 0) {
                // pick direction
                //(Node, Direction) res = pa.pathFind(curX, curY, movementOptions.ToList<Node>()); // random walk
                // Node res = pa.bfs(maze, maze[curX, curY]).ToList<Node>()[0];
                Node res = movementOptions.ToList<Node>()[0];
                movementOptions.Remove(res);

                //var blih = pa.bfs(maze, maze[curX, curY]);
                // (int, int, Direction) res = pa.pathFind(curX, curY); // bfs

                tarX = res.x;
                tarY = res.y;

                Node next = res;
                Node prev = next.prev;
                Direction dir;
                if (prev.x - next.x == 0)
                {
                    if (prev.y - next.y < 0) dir = (Direction)2; // DOWN
                    else dir = (Direction)0;                     // UP
                } else
                {
                    if (prev.x - next.x < 0) dir = (Direction)1; // LEFT
                    else dir = (Direction)3;                     // RIGHT
                }
                
                //Console.WriteLine(dir);

                // get pathway options (poll)
                Wall[][] options = pathway.Poll(curX, curY, size, (Wall)dir);
        
                // choose one of the pathway types
                //Wall[] pathwayType = ca.BChoose(options, visited);
                Wall[] pathwayType = ca.Choose(options);
                //foreach (Wall w in pathwayType) Console.Write($"{w} ");
                //Console.WriteLine();

                // R O T A T E pathway type
                pathwayType = pathway.Rotate(pathwayType, (int)dir);
        
                // place in node
                Node target = new Node(tarX, tarY, prev);
                target.walls = pathwayType;
                target.visited = true;
                maze[tarX, tarY] = target;
                visited.Add(target);

                visitedGrid[tarX, tarY] = true;
                curX = tarX;
                curY = tarY;

                // generate new nodes
                // for (int i = 0; i < 4; i++)
                // {
                //     int posNewX = tarX + modifier[2*i];
                //     int posNewY = tarY + modifier[2*i + 1];
                //     if (!(posNewX < 0 || posNewX >= size || posNewY < 0 || posNewY >= size))
                //     {
                //         if (!maze[posNewX, posNewY].visited && !maze[tarX, tarY].walls.Contains((Wall)i))//!movementOptions.Contains(maze[posNewX, posNewY]) && !maze[posNewX, posNewY]) 
                //         {
                //             movementOptions.Add(new Node(posNewX, posNewY, target));
                //         }   
                //     }
                // }

                for (int i = 0; i < 4; i++)
{
    int posNewX = tarX + modifier[2*i];
    int posNewY = tarY + modifier[2*i + 1];

    if (posNewX < 0 || posNewX >= size || posNewY < 0 || posNewY >= size)
        continue;

    if (maze[tarX, tarY].walls != null &&
        maze[tarX, tarY].walls.Contains((Wall)i))
        continue;

    if (!maze[posNewX, posNewY].visited)
    {
        movementOptions.Add(new Node(posNewX, posNewY, target));
    }
}
            Visualizer v = new Visualizer();

            //v.Show(maze);

            // loop end
            }

            // go to random unvisited location near walls next to a visited cell 
            List<(int, int)> unvisitedCells = new List<(int, int)>();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (visitedGrid[x, y])
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            int tempX = x + modifier[2*i];
                            int tempY = y + modifier[2*i+1];

                            if (tempX < 0 || tempX >= size || tempY < 0 || tempY >= size) continue;

                            if (!visitedGrid[tempX, tempY]) unvisitedCells.Add((tempX, tempY));
                        }
                    }
                }
            }
            
            if (unvisitedCells.Count > 0) {
            (int, int) newLoc = unvisitedCells[rng.Next(unvisitedCells.Count)];
            movementOptions.Add(new Node(newLoc.Item1, newLoc.Item2, nullNode));
        
            // break open wall between prev cells
            List<(int, int, int)> walledNeighbours = new List<(int, int, int)>();
            for (int i = 0; i < 4; i++)
            {
                int tempX = newLoc.Item1 + modifier[2*i];
                int tempY = newLoc.Item2 + modifier[2*i+1];

                if (tempX < 0 || tempX >= size || tempY < 0 || tempY >= size) continue;


                if (visitedGrid[tempX, tempY]) walledNeighbours.Add((tempX, tempY, i));
            }
            (int, int, int) wallToBreak = walledNeighbours[rng.Next(walledNeighbours.Count)];
            List<Wall> temp = maze[wallToBreak.Item1, wallToBreak.Item2].walls.ToList<Wall>();
            temp.Remove((Wall)wallToBreak.Item3);
            maze[wallToBreak.Item1, wallToBreak.Item2].walls = temp.ToArray();
            }

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
        Node prev = next.prev;
        if (prev.x - next.x == 0)
        {
            if (prev.y - next.y < 0) dir = 2; // DOWN
            else dir = 0;                     // UP
        } else
        {
            if (prev.x - next.x < 0) dir = 1; // LEFT
            else dir = 3;                     // RIGHT
        }

        while (tarX < 0 || tarX >= size || tarY < 0 || tarY >= size)
        {
            dir = rng.Next(4);
            tarX = curX + modifier[2 * dir];
            tarY = curY + modifier[2 * dir + 1];
        }

        return (next, (Direction)dir);
    }

    public HashSet<Node> bfs(Node[,] maze, Node toExpand)
    {
        List<Point> toExpNew = new List<Point>();
        HashSet<Node> unfilled = new HashSet<Node>();
        for(int i = 0; i++ < 4; i++)
        {
            if(!toExpand.walls.Contains((Wall)i))
                toExpNew.Add(move(new Point(toExpand.x, toExpand.y), i));
        }
        foreach(Point to in toExpNew)
        {
            Node node = maze[to.X, to.Y];
            if (node != toExpand.prev)
            {
                if(node.visited)
                    unfilled.UnionWith(bfs(maze, node));
                else
                    unfilled.Add(node);
            }
        }
        return unfilled;
    }
    private Point move(Point start, int dir)
    {
        switch (dir)
        {
            case 0:  return (new Point (start.X, start.Y + 1));
            case 1:  return (new Point (start.X + 1, start.Y));
            case 2:  return (new Point (start.X, start.Y - 1));
            default: return (new Point (start.X - 1, start.Y));
        };
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

    public Wall[] BChoose(Wall[][] options, HashSet<Node> bleeh)
    {
        int amountOfOptions = 0;
        foreach (Node node in bleeh)
        {
            if (node.walls.Length < 2)
            {
                amountOfOptions++;
            }
        }

        List<int> pool = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            if (options[i].Length != 3)
            {
                for (int j = 0; j < amountOfOptions; j++) 
                pool.Add(i);
            } else pool.Add(i);
        }

        if (pool.Count <= 0)
        {
            return Choose(options);
        }

        Random rng = new Random();
        return options[pool[rng.Next(pool.Count)]];
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
            case [Wall.North]: res = ("┌─┐", "   ", "└ ┘"); break;
            case [Wall.East]: res = ("┌ ┐", "  │", "└ ┘"); break;
            case [Wall.South]: res = ("┌ ┐", "   ", "└─┘"); break;
            case [Wall.West]: res = ("┌ ┐", "│  ", "└ ┘"); break;

            case [Wall.East, Wall.North]: res = ("┌─┐", "  │", "└ ┘"); break;
            case [Wall.North, Wall.East]: res = ("┌─┐", "  │", "└ ┘"); break;

            case [Wall.South, Wall.North]: res = ("┌─┐", "   ", "└─┘"); break;
            case [Wall.North, Wall.South]: res = ("┌─┐", "   ", "└─┘"); break;

            case [Wall.West, Wall.North]: res = ("┌─┐", "│  ", "└ ┘"); break;
            case [Wall.North, Wall.West]: res = ("┌─┐", "│  ", "└ ┘"); break;

            case [Wall.South, Wall.East]: res = ("┌ ┐", "  │", "└─┘"); break;
            case [Wall.East, Wall.South]: res = ("┌ ┐", "  │", "└─┘"); break;

            case [Wall.West, Wall.East]: res = ("┌ ┐", "│ │", "└ ┘"); break;
            case [Wall.East, Wall.West]: res = ("┌ ┐", "│ │", "└ ┘"); break;

            case [Wall.West, Wall.South]: res = ("┌ ┐", "│  ", "└─┘"); break;
            case [Wall.South, Wall.West]: res = ("┌ ┐", "│  ", "└─┘"); break;

            case [Wall.North, Wall.East, Wall.South]: res = ("┌─┐", "  │", "└─┘"); break;
            case [Wall.East, Wall.South, Wall.West]: res = ("┌ ┐", "│ │", "└─┘"); break;
            case [Wall.South, Wall.West, Wall.North]: res = ("┌─┐", "│  ", "└─┘"); break;
            case [Wall.West, Wall.North, Wall.East]: res = ("┌─┐", "│ │", "└ ┘"); break;

            default: res = ("┌ ┐", "   ", "└ ┘"); break;
        }

        return res;
    }
}

public class Agents
{
    public Node[,] maze;
    
    public int RandomWalk()
    {
        int i = 0;
        Random rng = new Random();
        int curX = rng.Next(0, maze.GetLength(0));
        int curY = rng.Next(0, maze.GetLength(1));
        int endX = rng.Next(0, maze.GetLength(0));
        int endY = rng.Next(0, maze.GetLength(1));
        while (curX != endX || curY != endY)
        { 
            i++;
            Node curNode = maze[curX, curY];
            List<Point> moves = new List<Point>();
            for(int j = 0; j++ < 4; j++)
            {
                if(!curNode.walls.Contains((Wall)j))
                    moves.Add(move(curX, curY, j));
            }
            int take = rng.Next(0, moves.Count);
            if (moves.Count > 0) {
                curX = moves[take].X;
                curY = moves[take].Y;
            }
            while (0 > curX || curX >= maze.GetLength(0) || 0 > curY || curY >= maze.GetLength(1))
            {
                if (moves.Count > 0) {
                    moves.RemoveAt(take);
                    take = rng.Next(0, moves.Count);
                    curX = moves[take].X;
                    curY = moves[take].Y;
                }
                else
                    break;
            }
            Console.WriteLine($"Now at ({curX}, {curY})");
            if (i > 10000000)
                break;
        }
        return i;
        
    }
    public int DepthFirst()
    {
        int i = 0;
        Random rng = new Random();
        int curX = rng.Next(0, maze.GetLength(0));
        int curY = rng.Next(0, maze.GetLength(1));
        int endX = rng.Next(0, maze.GetLength(0));
        int endY = rng.Next(0, maze.GetLength(1));
        List<Node> expanded = new List<Node>();
        List<Node> toExpand = new List<Node>();
        while (curX != endX || curY != endY)
        { 
            i++;
            Node curNode = maze[curX, curY];
            expanded.Add(curNode);
            List<Point> moves = new List<Point>();
            for(int j = 0; j++ < 4; j++)
            {
                if(!curNode.walls.Contains((Wall)j))
                    moves.Add(move(curX, curY, j));
            }
            foreach (Point move in moves)
            {
                if (0 > move.X || move.Y >= maze.GetLength(0) || 0 > move.X || move.Y >= maze.GetLength(1))
                    continue;
                Node toAdd = maze[move.X, move.Y];
                if (!toExpand.Contains(toAdd) && !expanded.Contains(toAdd))
                    toExpand.Add(toAdd);
            }
            if (toExpand.Count == 0)
                break;
            Node toExp = toExpand[toExpand.Count - 1];
            curX = toExp.x;
            curY = toExp.y;
            Console.WriteLine($"Now at ({curX}, {curY})");
            if (i > 10000)
                break;
        }
        return i;
        
    }
    private Point move(int startX, int startY, int dir)
    {
        switch (dir)
        {
            case 0:  return (new Point (startX, startY + 1));
            case 1:  return (new Point (startX + 1, startY));
            case 2:  return (new Point (startX, startY - 1));
            default: return (new Point (startX - 1, startY));
        };
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
        {"dead end", [Wall.North, Wall.East, Wall.West]},
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
        new Wall[3] {Wall.West, Wall.North, Wall.East},
        new Wall[0] {},
        new Wall[2] {Wall.East, Wall.West},
        new Wall[2] {Wall.North, Wall.East},
        new Wall[2] {Wall.North, Wall.West},
        new Wall[1] {Wall.West},
        new Wall[1] {Wall.North},
        new Wall[1] {Wall.East}
    };

    public Wall[][] pathwayTypes3 = new Wall[7][]
    {
        new Wall[3] {Wall.West, Wall.North, Wall.East},
        new Wall[2] {Wall.East, Wall.West},
        new Wall[2] {Wall.North, Wall.East},
        new Wall[2] {Wall.North, Wall.West},
        new Wall[1] {Wall.West},
        new Wall[1] {Wall.North},
        new Wall[1] {Wall.East}
    };

    public Wall[][] pathwayTypes4 = new Wall[3][]
    {
        new Wall[3] {Wall.West, Wall.North, Wall.East},
        new Wall[2] {Wall.North, Wall.East},
        new Wall[2] {Wall.North, Wall.West},
    };

    public Wall[] Rotate(Wall[] original, int turnAmount)
    {
        Wall[] result = new Wall[original.Length];
        int x = 2;
        for (int i = 0; i < original.Length; i++)
        {
            result[i] = (Wall)(((int)original[i] + turnAmount) % 4);
        }
        return result;
    }

    public Wall[][] Poll(int x, int y, int size, Wall dir)
    {
        Random rng = new Random();
        Wall[][] result = new Wall[3][];
        List<Wall[]> cellKnowledge = new List<Wall[]>();
        if ((x == 0 || x == size - 1) && (y == 0 || y == size - 1)) cellKnowledge = pathwayTypes4.ToList<Wall[]>();
        else if ((x == 0 || x == size - 1) || (y == 0 || y == size - 1)) cellKnowledge = pathwayTypes3.ToList<Wall[]>();
        else cellKnowledge = pathwayTypes2.ToList<Wall[]>();

        for (int i = 0; i < 0; i++)
        {
            if (cellKnowledge[i].Contains(dir)) cellKnowledge.RemoveAt(i); 
        }

        result = rng.GetItems<Wall[]>(cellKnowledge.ToArray<Wall[]>(), 3);
        return result;
    }
}

public class Visualizer
{
    public void Show(Node[,] maze)
    {
        Node[,] grid2 = maze;
        for (int y = 0; y < grid2.GetLength(1); y++)
        {
            string row1 = "";
            string row2 = "";
            string row3 = "";
            for (int x = 0; x < grid2.GetLength(0); x++)
            {
                if (grid2[x, y] != null) {
                (string, string, string) s = grid2[x, y].Stringer();
                row1 += s.Item1;
                row2 += s.Item2;
                row3 += s.Item3;   
                } else 
                {
                    row1 += "   ";
                    row2 += "   ";
                    row3 += "   ";
                }
            }
            Console.WriteLine(row1);
            Console.WriteLine(row2);
            Console.WriteLine(row3);
        }
    }    
}