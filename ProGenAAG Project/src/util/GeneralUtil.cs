using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNoise.Primitive;
using LibNoise;

namespace ProGenAAG_Project
{
    public struct Coords
    {
        public Coords(int x, int y)
        {
            xCoord = x;
            yCoord = y;
        }
        public int xCoord { get; set; }
        public int yCoord { get; set; }
        public override string ToString() => $"({xCoord}, {yCoord})";
    }
    public class GeneralUtil
    {
        //  This Class contains functions that automates general or miscellaneous tasks.
       
        public static void SortList(string path)
        {

            string[] strings = FileUtil.ReadAllLines(path);

            for (int i = 0; i < strings.Length; i++)
            {
                Console.WriteLine("[" + i + "] " + strings[i]);

            }
        }

        public static T[,] SplitIntoThree<T>(T[] sourceArray)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException(nameof(sourceArray), "Source array cannot be null.");
            }

            if (sourceArray.Length == 0)
            {
                throw new ArgumentException("Source array cannot be empty.", nameof(sourceArray));
            }

            int totalElements = sourceArray.Length;

            // Calculate sizes for the first, middle, and last arrays
            int sizePerEdge = totalElements / 3; // Base size for first and last
            int remainder = totalElements % 3;   // Extra elements to be placed in the middle

            int middleSize = sizePerEdge + remainder; // Middle array gets the remainder

            // Create the 2D array
            T[,] result = new T[3, Math.Max(sizePerEdge, middleSize)];

            // Fill the 2D array
            int currentIndex = 0;

            // Fill the first array
            for (int col = 0; col < sizePerEdge; col++)
            {
                result[0, col] = sourceArray[currentIndex++];
            }

            // Fill the middle array
            for (int col = 0; col < middleSize; col++)
            {
                result[1, col] = sourceArray[currentIndex++];
            }

            // Fill the last array
            for (int col = 0; col < sizePerEdge; col++)
            {
                result[2, col] = sourceArray[currentIndex++];
            }

            return result;
        }
        public static T[] GetRow<T>(T[,] sourceArray, int row)
        {
            T[,] sArray = sourceArray;
            T[] array = new T[row];
            for (int i = 0; i < sourceArray.GetLength(row) - 1; i++)
            {
                Console.WriteLine("DEBUG:   Row index = " + i);
                array[i] = sArray[row, i];
            }
            return array;
        }




            //  These functions have to do with list manipulation of any value Type
        public static void RemoveLargestList<T>(List<List<T>> listOfLists)
        {
            if (listOfLists == null || listOfLists.Count == 0)
                return;

            int maxIndex = 0;
            int maxCount = listOfLists[0].Count;

            for (int i = 1; i < listOfLists.Count; i++)
            {
                if (listOfLists[i].Count > maxCount)
                {
                    maxIndex = i;
                    maxCount = listOfLists[i].Count;
                }
            }

            listOfLists.RemoveAt(maxIndex);
        }

        public static void RemoveListAboveSize<T>(List<List<T>> listOfLists, int minSize) {
            if (listOfLists == null || listOfLists.Count == 0) {
                return;
            }

            List<int> largeIndexes = new List<int>();
            int maxCount = listOfLists[0].Count;

            for (int i = 1; i < listOfLists.Count; i++) {
                if (listOfLists[i].Count > minSize)
                {
                    largeIndexes.Add(i);
                    maxCount = listOfLists[i].Count;
                }
            }

            for (int i = listOfLists.Count; i > 0; i--) {
                if (largeIndexes.Contains(i))
                {
                    listOfLists.RemoveAt(i);
                }

            }

        }

        

        public static void RemoveListBelowSize<T>(List<List<T>> listOfLists, int maxSize)
        {
            if (listOfLists == null) throw new ArgumentNullException(nameof(listOfLists));

            listOfLists.RemoveAll(subList => subList.Count < maxSize);
        }



        public static List<Coords> GetCoordsWithinCircle<T>(T[,] array, Coords center, int radius)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            List<Coords> coordsList = new List<Coords>();

            for (int r = Math.Max(0, center.xCoord - radius); r <= Math.Min(rows - 1, center.xCoord + radius); r++) {
                for (int c = Math.Max(0, center.yCoord - radius); c <= Math.Min(cols - 1, center.yCoord + radius); c++) {
                    int deltaX = r - center.xCoord;
                    int deltaY = c - center.yCoord;
                    if (deltaX * deltaX + deltaY * deltaY <= radius * radius) {
                        coordsList.Add(new Coords(r, c));
                    }
                }
            }

            return coordsList;
        }


        //  These functions deal with island finding in a 2D array of integers
            //  Given a start range, end range and an integer array, find all islands within the range
        public static List<List<Coords>> FindIslandsInRange(int[,] array, int targetRangeStart, int targetRangeEnd, bool hWrapping, bool vWrapping)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            bool[,] visited = new bool[rows, cols];
            List<List<Coords>> islands = new List<List<Coords>>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (!visited[r, c] && array[r, c] >= targetRangeStart && array[r, c] <= targetRangeEnd)
                    {
                        List<Coords> island = new List<Coords>();
                        ExploreIsland(array, r, c, targetRangeStart, targetRangeEnd, visited, island, hWrapping, vWrapping);
                        islands.Add(island);
                    }
                }
            }

            return islands;
        }
        private static void ExploreIsland(int[,] array, int startRow, int startCol, int targetRangeStart, int targetRangeEnd, bool[,] visited, List<Coords> island, bool hWrapping, bool vWrapping)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            Queue<Coords> queue = new Queue<Coords>();
            queue.Enqueue(new Coords(startRow, startCol));

            while (queue.Count > 0)
            {
                Coords current = queue.Dequeue();
                int r = current.xCoord;
                int c = current.yCoord;

                if (visited[r, c]) continue;

                visited[r, c] = true;
                island.Add(new Coords(r, c));

                int[] dr = { -1, 1, 0, 0 };
                int[] dc = { 0, 0, -1, 1 };

                for (int i = 0; i < 4; i++)
                {
                    int newRow = r + dr[i];
                    int newCol = c + dc[i];

                    if (hWrapping)
                    {
                        newCol = (newCol + cols) % cols;
                    }

                    if (vWrapping)
                    {
                        newRow = (newRow + rows) % rows;
                    }

                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && !visited[newRow, newCol]
                        && array[newRow, newCol] >= targetRangeStart && array[newRow, newCol] <= targetRangeEnd)
                    {
                        queue.Enqueue(new Coords(newRow, newCol));
                    }
                }
            }
        }

        

            //  Return the borders within range thickness of all islands
        public static List<List<Coords>> FindIslandBordersInRange(int[,] array, int targetRangeStart, int targetRangeEnd, bool hWrapping, bool vWrapping, int thickness) {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            bool[,] visited = new bool[rows, cols];
            List<List<Coords>> borders = new List<List<Coords>>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (!visited[r, c] && array[r, c] >= targetRangeStart && array[r, c] <= targetRangeEnd)
                    {
                        List<Coords> border = new List<Coords>();
                        ExploreBorder(array, r, c, targetRangeStart, targetRangeEnd, visited, border, hWrapping, vWrapping, thickness);
                        borders.Add(border);
                    }
                }
            }

            return borders;
        }
        private static void ExploreBorder(int[,] array, int startRow, int startCol, int targetRangeStart, int targetRangeEnd, bool[,] visited, List<Coords> border, bool hWrapping, bool vWrapping, int thickness) {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            Queue<Coords> queue = new Queue<Coords>();
            queue.Enqueue(new Coords(startRow, startCol));

            while (queue.Count > 0)
            {
                Coords current = queue.Dequeue();
                int r = current.xCoord;
                int c = current.yCoord;

                if (visited[r, c]) continue;

                visited[r, c] = true;

                if (IsOnBorder(array, r, c, targetRangeStart, targetRangeEnd, hWrapping, vWrapping, thickness))
                {
                    border.Add(new Coords(r, c));
                }

                int[] dr = { -1, 1, 0, 0 };
                int[] dc = { 0, 0, -1, 1 };

                for (int i = 0; i < 4; i++)
                {
                    int newRow = r + dr[i];
                    int newCol = c + dc[i];

                    if (hWrapping)
                    {
                        newCol = (newCol + cols) % cols;
                    }

                    if (vWrapping)
                    {
                        newRow = (newRow + rows) % rows;
                    }

                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && !visited[newRow, newCol]
                        && array[newRow, newCol] >= targetRangeStart && array[newRow, newCol] <= targetRangeEnd)
                    {
                        queue.Enqueue(new Coords(newRow, newCol));
                    }
                }
            }
        }

        private static bool IsOnBorder(int[,] array, int row, int col, int targetRangeStart, int targetRangeEnd, bool hWrapping, bool vWrapping, int thickness) {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int t = 1; t <= thickness; t++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int newRow = row + dr[i] * t;
                    int newCol = col + dc[i] * t;

                    if (hWrapping)
                    {
                        newCol = (newCol + cols) % cols;
                    }

                    if (vWrapping)
                    {
                        newRow = (newRow + rows) % rows;
                    }

                    if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols || array[newRow, newCol] < targetRangeStart || array[newRow, newCol] > targetRangeEnd)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public static int[,] generatePerlinNoise(int width, int height, double gradient, int lowestValue, int highestValue)
        {
            //double gradient = 1.00;
            Random randomPerlin = new Random();
            int randomSeed = randomPerlin.Next(0, 32767);

            int[,] array = new int[width, height];
            ImprovedPerlin perlin = new ImprovedPerlin(randomSeed, NoiseQuality.Best);

            for (int r = 0; r < array.GetLength(0); r++)
            {
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    double noiseValue = perlin.GetValue((float)(r * 0.1), (float)(c * 0.1), 0) * 0.5 + 0.5; // Normalize Perlin noise to [0,1]
                    int mappedValue = (int)(highestValue * Math.Pow(noiseValue, gradient));
                    array[r, c] = Math.Clamp(mappedValue, lowestValue, highestValue);
                }
            }
            return array;
        }








        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }


    }
}
