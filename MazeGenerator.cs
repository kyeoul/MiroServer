using System.Collections.Generic;


public class MazeGenerator
{
    public int h;
    public int w;
    public int l;
    public int randomX, randomY;
    private int[][] mazeGraph;
    Random rand = new Random();

    public MazeGenerator(int h, int w, int l)
    {
        this.h = h;
        this.w = w;
        this.l = l;
    }

    public int W { get { return w; } } // Property for width
    public int H { get { return h; } } // Property for height

    public int[][] GenerateMaze()
    {
        int[][] maze = create2DMazeArray(h, w, l);
        return maze;
    }
    //void SelectRandomPathBlock()
    //{

    //    List<Vector2Int> pathBlocks = new List<Vector2Int>();

    //    // Find all the path blocks
    //    for (int x = 0; x < mazeGraph.Length; x++)
    //    {
    //        for (int y = 0; y < mazeGraph[0].Length; y++)
    //        {
    //            if (mazeGraph[x][y] == 0)
    //            {
    //                pathBlocks.Add(new Vector2Int(x, y));
    //            }
    //        }
    //    }

    //    // Select a random path block
    //    Vector2Int randomBlock = pathBlocks[Random.Range(0, pathBlocks.Count)];
    //    randomX = randomBlock.x;
    //    randomY = randomBlock.y;
    //}
    public int[][] create2DMazeArray(int width, int height, int pathLength)
    {
        int[][] Generate2DArray()
        {
            int[][] array = new int[height][];
            for (int i = 0; i < height; i++)
            {
                array[i] = new int[width];
                for (int j = 0; j < width; j++)
                {
                    array[i][j] = 1;
                }
            }
            return array;
        }

        int shortestPathLength(int[][] maze)
        {
            int start_x = -1, start_y = -1;
            int end_x = -1, end_y = -1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (maze[y][x] == -1)
                    {
                        start_x = x;
                        start_y = y;
                    }
                    else if (maze[y][x] == 2)
                    {
                        end_x = x;
                        end_y = y;
                    }
                }
            }

            if (start_x == -1 || start_y == -1 || end_x == -1 || end_y == -1)
            {
                return -1; // Invalid maze
            }

            Queue<(int, int, int)> queue = new Queue<(int, int, int)>();
            queue.Enqueue((start_x, start_y, 0));

            HashSet<(int, int)> visited = new HashSet<(int, int)>();

            while (queue.Count > 0)
            {
                var (x, y, length) = queue.Dequeue();

                if (x == end_x && y == end_y)
                {
                    return length;
                }

                int[][] directions = { new int[] { 0, -1 }, new int[] { 1, 0 }, new int[] { 0, 1 }, new int[] { -1, 0 } };

                foreach (var dir in directions)
                {
                    int nx = x + dir[0];
                    int ny = y + dir[1];

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && maze[ny][nx] != 1 && !visited.Contains((nx, ny)))
                    {
                        queue.Enqueue((nx, ny, length + 1));
                        visited.Add((nx, ny));
                    }
                }
            }

            return -1; // No valid path found
        }

        int maxAttempts = 1000;
        int attempt = 0;
        int[][] maze2DArray = null;

        while (attempt < maxAttempts)
        {
            int[][] mazeArray = Generate2DArray();

            int start_x = rand.Next(0, width);
            int start_y = rand.Next(0, height);

            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((start_x, start_y));

            int[][] directions = { new int[] { 0, -1 }, new int[] { 1, 0 }, new int[] { 0, 1 }, new int[] { -1, 0 } };

            while (stack.Count > 0)
            {
                var (x, y) = stack.Peek();
                mazeArray[y][x] = 0;

                List<(int, int)> neighbors = new List<(int, int)>();

                foreach (var dir in directions)
                {
                    int nx = x + dir[0];
                    int ny = y + dir[1];

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height && mazeArray[ny][nx] == 1)
                    {
                        int count = 0;

                        foreach (var d in directions)
                        {
                            int nnx = nx + d[0];
                            int nny = ny + d[1];

                            if (nnx >= 0 && nnx < width && nny >= 0 && nny < height && mazeArray[nny][nnx] == 0)
                            {
                                count++;
                            }
                        }

                        if (count <= 1)
                        {
                            neighbors.Add((nx, ny));
                        }
                    }
                }

                if (neighbors.Count > 0)
                {
                    var (nx, ny) = neighbors[rand.Next(0, neighbors.Count)];
                    mazeArray[ny][nx] = 0;
                    mazeArray[y + (ny - y) / 2][x + (nx - x) / 2] = 0;
                    stack.Push((nx, ny));
                }
                else
                {
                    if (stack.Count > 1)
                    {
                        stack.Pop();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            mazeArray[start_y][start_x] = -1;
            int end_x = start_x, end_y = start_y;

            while (end_x == start_x && end_y == start_y)
            {
                end_x = rand.Next(0, width);
                end_y = rand.Next(0, height);
            }

            mazeArray[end_y][end_x] = 2;

            if (shortestPathLength(mazeArray) == pathLength)
            {
                maze2DArray = mazeArray;
                break;
            }

            attempt++;
        }

        if (attempt == maxAttempts)
        {
            Console.WriteLine("Couldn't generate a valid maze within the specified attempts.");
        }

        return maze2DArray;
    }

}
