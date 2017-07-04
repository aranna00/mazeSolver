using System;
using System.Drawing;

namespace Maze_Generator
{
    /// <summary>
    /// Represents a maze cell
    /// </summary>
    internal struct Cell
    {
        /// <summary>
        /// Initializes a new instance of maze cell with locations
        /// </summary>
        /// <param name="location">The location on the graphics surface</param>
        /// <param name="position">The location on the 2d array</param>
        public unsafe Cell(Point location, Point position)
        {
            this.location = location;
            this.position = position;

            // initially, all walls are intact
            LeftWall = true;
            RightWall = true;
            UpWall = true;
            DownWall = true;
            Path = Paths.None;

            // must be initialized, since it is a member of a struct
            Visited = false;
            Previous = null;
        }

        /// <summary>
        /// Gets or sets a value whether the cell has an intact left wall
        /// </summary>
        public bool LeftWall;

        /// <summary>
        /// /// Gets or sets a value whether the cell has an intact right wall
        /// </summary>
        public bool RightWall;

        /// <summary>
        /// Gets or sets a value whether the cell has an intact up wall
        /// </summary>
        public bool UpWall;

        /// <summary>
        /// Gets or sets a value whether the cell has an intact down wall
        /// </summary>
        public bool DownWall;

        /// <summary>
        /// Gets or sets a value whether the cell has been visited already
        /// </summary>
        public bool Visited;

        public enum Paths
        {
            Up, Down, Right, Left, None
        }

        public Paths Path;

        /// <summary>
        /// Gets or sets a pointer to the previous Cell in the found path chain
        /// </summary>
        public unsafe Cell* Previous;

        /// <summary>
        /// Provides indexing to the boolean fields in the cell
        /// </summary>
        /// <param name="index">0 leftW, 1 rightW, 2 UpW, 3 downW, 4 visited</param>
        /// <returns></returns>
        public bool this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return LeftWall;
                    case 1:
                        return RightWall;
                    case 2:
                        return UpWall;
                    case 3:
                        return DownWall;
                    case 4:
                        return Visited;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        LeftWall = value;
                        break;
                    case 1:
                        RightWall = value;
                        break;
                    case 2:
                        UpWall = value;
                        break;
                    case 3:
                        DownWall = value;
                        break;
                    case 4:
                        Visited = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private Point location;
        /// <summary>
        /// The current location on the graphics surface
        /// </summary>
        public Point Location => location;

        private Point position;
        /// <summary>
        /// The current location on the two-dimensional container
        /// </summary>
        public Point Position => position;

        /// <summary>
        /// Reset a cell so that all walls are intact and not visited
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < 4; i++)
            {
                this[i] = true;
            }
            Visited = false;
        }

        /// <summary>
        /// Draws a cell onto a graphics surface
        /// </summary>
        /// <param name="g">the graphics surface to draw on</param>
        /// <param name="pen">a pen to draw walls</param>
        /// <param name="size">The width of horizontal wall and height of the vertical walls</param>
        public void Draw(Graphics g, Pen pen, Size size)
        {
            if (LeftWall)
            {
                g.DrawLine(pen,
                    location,
                    new Point(location.X, location.Y + size.Height));
            }
            if (RightWall)
            {
                g.DrawLine(pen,
                    new Point(location.X + size.Width, location.Y),
                    new Point(location.X + size.Width, location.Y + size.Height));
            }
            if (UpWall)
            {
                g.DrawLine(pen,
                    location,
                    new Point(location.X + size.Width, location.Y));
            }
            if (DownWall)
            {
                g.DrawLine(pen,
                    new Point(location.X, location.Y + size.Height),
                    new Point(location.X + size.Width, location.Y + size.Height));
            }
        }
    }
}
