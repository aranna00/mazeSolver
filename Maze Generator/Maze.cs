using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Maze_Generator
{
    /// <summary>
    /// Creates, solves and draws mazes
    /// </summary>
    internal class Maze
    {
        /// <summary>
        /// Initializes a maze with a maximum size
        /// </summary>
        /// <param name="totalWidth">The maximum width of the maze</param>
        /// <param name="totalHeight">The maximum height of the maze</param>
        public Maze(int totalWidth, int totalHeight)
        {
            _maze = new Cell[totalHeight, totalWidth];
        }

        /// <summary>
        /// Indicates the current sleeping time (used to slow operation)
        /// </summary>
        public int Sleep { get; set; }

        private const int SleepPeriod = 1000;

        /// <summary>
        /// Indicates the currnet width the user selects
        /// </summary>
        private int _width;

        /// <summary>
        /// /// Indicates the currnet height the user selects
        /// </summary>
        private int _height;

        /// <summary>
        /// The array that carries all the Cell instances in the maze
        /// </summary>
        private readonly Cell[,] _maze;

        /// <summary>
        /// Indicates the maze begin
        /// </summary>
        private Cell _begin;

        /// <summary>
        /// Indicates the maze end
        /// </summary>
        private Cell _end;

        /// <summary>
        /// Used to view current position when the maze is being solved
        /// </summary>
        private Point _currentSolvePos;

        /// <summary>
        /// Indicates the pen used to draw the maze walls
        /// </summary>
        private Pen _mazePen = new Pen(Brushes.White, 3);

        /// <summary>
        /// Indicates the width of one rectangle on the maze
        /// </summary>
        private int UnitX => _maze.GetLength(1) / _width;

        /// <summary>
        /// Indicates the height of one rectangle on the maze
        /// </summary>
        private int UnitY => _maze.GetLength(0) / _height;

        /// <summary>
        /// Gets a value whether the maze is busy in a job
        /// </summary>
        private bool _working;

        /// <summary>
        /// Used to draw the found path
        /// </summary>
        private readonly List<Cell> _foundPath = new List<Cell>();

        /// <summary>
        /// Gets a value indicates whether the Maze is busy in solving
        /// </summary>
        private bool _solving;

        /// <summary>
        /// Used to generate maze
        /// </summary>
        private readonly Random _random = new Random();

        /// <summary>
        /// Generates a maze with the specific size
        /// </summary>
        /// <param name="width">Number of squares in width</param>
        /// <param name="height">Number of squares in height</param>
        /// <param name="method">indicates the method used to generate the maze</param>
        public void Generate(int width, int height, int method)
        {
            _working = true;

            Initailze(_maze, width, height);

            _mazePen.Dispose();
            _mazePen = UnitX < 5 ? new Pen(Brushes.WhiteSmoke, 1) : new Pen(Brushes.WhiteSmoke, 3);
            BreadthFirstSearchMazeGeneration(_maze, _width, _height);
            _working = false;
        }

        /// <summary>
        /// Resets a maze array
        /// </summary>
        /// <param name="arr">The maze array</param>
        /// <param name="width">Number of squares in width</param>
        /// <param name="height">Number of squares in height</param>
        private void Initailze(Cell[,] arr, int width, int height)
        {
            _width = width;
            _height = height;

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    arr[i, j] = new Cell(new Point(j * UnitX, i * UnitY), new Point(j, i));
                }
            }
        }

        /// <summary>
        /// Generate a maze with the Breadth-First Search approach
        /// </summary>
        /// <param name="arr">the array of cells</param>
        /// <param name="width">A width for the maze</param>
        /// <param name="height">A height for the maze</param>
        private void BreadthFirstSearchMazeGeneration(Cell[,] arr, int width, int height)
        {
            Queue<Cell> queue = new Queue<Cell>();
            Random random = new Random();

            Cell location = arr[_random.Next(height), _random.Next(width)];
            queue.Enqueue(location);

            while (queue.Count > 0)
            {
                List<Point> neighbours = GetNeighbours(arr, location, width, height);
                if (neighbours.Count > 0)
                {
                    Point temp = neighbours[random.Next(neighbours.Count)];

                    KnockWall(arr, ref location, ref arr[temp.Y, temp.X]);

                    queue.Enqueue(location);
                    location = arr[temp.Y, temp.X];
                }
                else
                {
                    location = queue.Dequeue();
                }

                Thread.SpinWait(Sleep * SleepPeriod);
            }

            MakeMazeBeginEnd(_maze);
        }

        /// <summary>
        /// Used to create a begin and end for a maze
        /// </summary>
        /// <param name="arr">The array of the maze</param>
        private void MakeMazeBeginEnd(Cell[,] arr)
        {
            Point temp = new Point {Y = _random.Next(_height), X = _random.Next(_width)};

            _begin = arr[temp.Y, temp.X];
            temp.Y = _random.Next(_height);
            temp.X = _random.Next(_width);
            _end = arr[temp.Y, temp.X];
        }

        /// <summary>
        /// Knocks wall between two neighbor cellls
        /// </summary>
        /// <param name="maze">The maze array</param>
        /// <param name="current">the current cell</param>
        /// <param name="next">the next neighbor cell</param>
        private static void KnockWall(Cell[,] maze, ref Cell current, ref Cell next)
        {
            // The next is down
            if (current.Position.X == next.Position.X && current.Position.Y > next.Position.Y)
            {
                maze[current.Position.Y, current.Position.X].UpWall = false;
                maze[next.Position.Y, next.Position.X].DownWall = false;
            }
            // the next is up
            else if (current.Position.X == next.Position.X)
            {
                maze[current.Position.Y, current.Position.X].DownWall = false;
                maze[next.Position.Y, next.Position.X].UpWall = false;
            }
            // the next is right
            else if (current.Position.X > next.Position.X)
            {
                maze[current.Position.Y, current.Position.X].LeftWall = false;
                maze[next.Position.Y, next.Position.X].RightWall = false;
            }
            // the next is left
            else
            {
                maze[current.Position.Y, current.Position.X].RightWall = false;
                maze[next.Position.Y, next.Position.X].LeftWall = false;
            }
        }

        /// <summary>
        /// Determines whether a particular cell has all its walls intact
        /// </summary>
        /// <param name="arr">the maze array</param>
        /// <param name="cell">The cell to check</param>
        /// <returns></returns>
        private static bool AllWallsIntact(Cell[,] arr, Cell cell)
        {
            for (var i = 0; i < 4; i++)
            {
                if (!arr[cell.Position.Y, cell.Position.X][i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets all neighbor cells to a specific cell, 
        /// where those neighbors exist and not visited already
        /// </summary>
        /// <param name="arr">The maze array</param>
        /// <param name="cell">The current cell to get neighbors</param>
        /// <param name="width">The width of the maze</param>
        /// <param name="height">The height of the maze</param>
        /// <returns></returns>
        private static List<Point> GetNeighbours(Cell[,] arr, Cell cell, int width, int height)
        {
            Point temp = cell.Position;
            List<Point> availablePlaces = new List<Point>();

            // Left
            temp.X = cell.Position.X - 1;
            if (temp.X >= 0 && AllWallsIntact(arr, arr[temp.Y, temp.X]))
            {
                availablePlaces.Add(temp);
            }
            // Right
            temp.X = cell.Position.X + 1;
            if (temp.X < width && AllWallsIntact(arr, arr[temp.Y, temp.X]))
            {
                availablePlaces.Add(temp);
            }

            // Up
            temp.X = cell.Position.X;
            temp.Y = cell.Position.Y - 1;
            if (temp.Y >= 0 && AllWallsIntact(arr, arr[temp.Y, temp.X]))
            {
                availablePlaces.Add(temp);
            }
            // Down
            temp.Y = cell.Position.Y + 1;
            if (temp.Y < height && AllWallsIntact(arr, arr[temp.Y, temp.X]))
            {
                availablePlaces.Add(temp);
            }
            return availablePlaces;
        }

        /// <summary>
        /// Draws the maze on a specific surface
        /// </summary>
        /// <param name="g">a surface to draw on</param>
        public void Draw(Graphics g)
        {
            g.Clear(Color.Black);

            // in case Generate() have not been called yet
            if (_width == 0) return;

            // draws begin
            g.FillRectangle(Brushes.Red, new Rectangle(_begin.Location, new Size(UnitX, UnitY)));

            // loop on every cell in the bounds
            for (var i = 0; i < _height; i++)
            {
                for (var j = 0; j < _width; j++)
                {
                    // Visited cell: fill green square
                    if (_maze[i, j].Visited)
                    {
                        g.FillRectangle(
                            _maze[i, j].Path == Cell.Paths.None ? Brushes.DarkRed : Brushes.Green,
                            new Rectangle(_maze[i, j].Location, new Size(UnitX, UnitY)));
                    }

                    if (_end.Position.X == j && _end.Position.Y == i)
                    {
                        g.FillRectangle(Brushes.Blue, new Rectangle(_maze[i, j].Location, new Size(UnitX, UnitY)));
                    }

                    // fills the current square in the solving process
                    if (_solving && _currentSolvePos.X == j && _currentSolvePos.Y == i)
                    {
                        if (_currentSolvePos.X != _begin.Position.Y && _currentSolvePos.Y != _begin.Position.X)
                            g.FillRectangle(
                                Brushes.LimeGreen,
                                new Rectangle(_maze[i, j].Location, new Size(UnitX, UnitY)));
                    }

                    // Draw the intact walls
                    _maze[i, j].Draw(g, _mazePen, new Size(UnitX, UnitY));
                }
            }
        }

        /// <summary>
        /// Draws the found path on a specific surface
        /// </summary>
        /// <param name="g">a surface to draw on</param>
        public void DrawPath(Graphics g)
        {
            // maze-begin square
            g.FillRectangle(Brushes.Red, new Rectangle(_begin.Location, new Size(UnitX, UnitY)));
        }

        /// <summary>
        /// Used to reset all cells
        /// </summary>
        /// <param name="arr">The maze array to reset elements</param>
        private void UnvisitAll(Cell[,] arr)
        {
            for (int i = 0; i < _maze.GetLength(0); i++)
            {
                for (int j = 0; j < _maze.GetLength(1); j++)
                {
                    arr[i, j].Visited = false;
                    arr[i, j].Path = Cell.Paths.None;
                }
            }
        }

        /// <summary>
        /// Solves the current maze using a specific method
        /// </summary>
        public unsafe void Solve()
        {
            _solving = true;
            // initialize
            _foundPath.Clear();
            UnvisitAll(_maze);

            fixed (Cell* ptr = &_begin) Solve(ptr, ref _end);

            _solving = false;
        }

        /// <summary>
        /// Solves a maze with recursive backtracking
        /// </summary>
        /// <param name="current">The current maze cell</param>
        /// <param name="end">The end of the maze cell</param>
        /// <returns>returrns true if the path is found</returns>
        private unsafe bool Solve(Cell*current, ref Cell end)
        {
            int currentY = current->Position.Y;
            int currentX = current->Position.X;
            if (current->Position == end.Position)
            {
                // make it visited for it to be drawn with green
                _maze[currentY, currentX].Visited = true;
                // add end point to the foundPath
                _foundPath.Add(*current);
                return true;
            }

            // has been visited already so it doesnt reverse
            if (_maze[currentY, currentX].Visited) return false;

            _currentSolvePos = current->Position;
            Thread.SpinWait(Sleep * SleepPeriod);

            // mark as visited
            _maze[currentY, currentX].Visited = true;

            // Left
            if (currentX - 1 >= 0 && !current->LeftWall)
            {
                _maze[currentY, currentX].Path = Cell.Paths.Left;
                fixed (Cell* ptr = &_maze[currentY, currentX - 1])
                {
                    if (Solve(ptr, ref end))
                    {
                        _foundPath.Add(*current);

                        return true;
                    }
                }
            }
            // Right
            if (currentX + 1 < _width && !current->RightWall)
            {
                _maze[currentY, currentX].Path = Cell.Paths.Right;
                fixed (Cell* ptr = &_maze[currentY, currentX + 1])
                {
                    if (Solve(ptr, ref end))
                    {
                        _foundPath.Add(*current);

                        return true;
                    }
                }
            }
            // Up
            if (currentY - 1 >= 0 && !current->UpWall)
            {
                _maze[currentY, currentX].Path = Cell.Paths.Up;
                fixed (Cell* ptr = &_maze[currentY - 1, currentX])
                {
                    if (Solve(ptr, ref end))
                    {
                        _foundPath.Add(*current);

                        return true;
                    }
                }
            }
            // Down
            if (currentY + 1 < _height && !current->DownWall)
            {
                _maze[currentY, currentX].Path = Cell.Paths.Down;
                fixed (Cell* ptr = &_maze[currentY + 1, currentX])
                {
                    if (Solve(ptr, ref end))
                    {
                        _foundPath.Add(*current);

                        return true;
                    }
                }
            }
            _maze[currentY, currentX].Path = Cell.Paths.None;

            return false;
        }
    }
}