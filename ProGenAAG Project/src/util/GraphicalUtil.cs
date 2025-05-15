using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProGenAAG_Project;
using static System.Net.Mime.MediaTypeNames;

namespace ProGenAAG_Project
{
    public class GraphicalUtil
    {
        public static readonly int worldTileSize = 32;

        /** Functions which deal with creating and manipulating bitmaps                          **/


        //  Section:    Image Manipulation
        public static Bitmap getBitmap(string path) {
            if (path==null) {
                throw new ArgumentNullException(nameof(path) , "The path cannot be null.");
            }


            try {
                Bitmap bitmap = new Bitmap(FileUtil.ConvertToDir(path));
                return bitmap;
            }
            catch {
                Console.WriteLine("ERROR: Bitmap "+path+" NO RETURN");
                return new Bitmap(FileUtil.ConvertToDir("\\debug\\res\\worldtiles_tilesets\\error.png"));

            }


        }


        // Given a string of filepaths, load the Bitmaps at each filepath and return the array
        public static Bitmap[] getBitmapArray(String[] paths) {
            Bitmap[] bitmaps = new Bitmap[paths.Length];

            // Test if the Path is null
            if (paths == null) {
                throw new ArgumentNullException(nameof(paths), "The Paths array cannot be null.");
            }
            // Test each string in Paths for nullinity
            foreach (string path in paths) {
                if (path is null)
                {
                    throw new ArgumentNullException(nameof(paths), "The Paths array cannot be null.");
                }
            }

            // Create a temporary path
            String[] temp_paths = paths;

            // Iterates through bitmaps, set bitmap at each interval to a bitmap at path location
            for (int i = 0; i < temp_paths.Length; i++) {
                // Set
                bitmaps[i] = new Bitmap(FileUtil.ConvertToDir(temp_paths[i]));
            }


            return bitmaps;
        }



            //  Save the bitmap as a .png file at the specificed path
        public static void saveImage(Bitmap image, String path)
        {
            if (path == null) {
                Console.WriteLine("Error: provided path is null");
                return;
            }

            Console.WriteLine("Saving Image: ");

            String filepath = FileUtil.ConvertToDir(path);
            Console.WriteLine(filepath);

            try { image.Save(filepath, System.Drawing.Imaging.ImageFormat.Png); } catch (Exception e) {
                Console.WriteLine("Error: filepath " + filepath + "is not valid");
            }
            
        }


            //  Given an array of Bitmaps, combine them in order and return the combined bitmap
        public static Bitmap CombineBitmaps(Bitmap[] images) {
            if (images == null || images.Length == 0)
            {
                throw new ArgumentException("The array of images cannot be null or empty.");
            }

            // Determine the dimensions of the output bitmap based on the first image
            int width = images[0].Width;
            int height = images[0].Height;

            // Ensure all images are the same size
            foreach (var image in images)
            {
                if (image.Width != width || image.Height != height)
                {
                    throw new ArgumentException("All images must have the same dimensions.");
                }
            }

            // Create a new bitmap to hold the combined image
            Bitmap combinedImage = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(combinedImage)) {
                // Set the background of the combined image to transparent
                g.Clear(Color.Transparent);

                // Draw each image in order
                foreach (var image in images)
                {
                    g.DrawImage(image, new Rectangle(0, 0, width, height));
                }
            }

            return combinedImage;
        }

            //  Get rotated idiot
        public static Bitmap RotateBitmap(Bitmap source, int amount)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // Normalize rotations (every 4 is a full rotation)
            int normalizedRotations = (amount % 4 + 4) % 4;

            if (normalizedRotations == 0)
                return (Bitmap)source.Clone(); // No rotation needed

            Bitmap rotated = (Bitmap)source.Clone();

            // Apply the rotation based on the normalized rotation count
            switch (normalizedRotations)
            {
                case 1:
                    rotated.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 2:
                    rotated.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    rotated.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            return rotated;
        }


        


        //  Section: Debug for WorldGen
        public static Bitmap DEBUGgetBitmap_TerrainType(WorldTile[,] worldMap) {
            bool DEBUG = false;
            int size = worldMap.GetLength(0);
            int canvasSize = worldTileSize * worldMap.GetLength(0);

            Bitmap globalBitmap = new Bitmap(canvasSize + (worldTileSize / 2), canvasSize + (worldTileSize / 2));

            using (Graphics graphics = Graphics.FromImage(globalBitmap)) {
                graphics.Clear(Color.Black); // Optional: Clear the background


                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        int drawX = i * worldTileSize;
                        int drawY = j * worldTileSize;

                        //Bitmap worldTileBaseBitmap = worldMap[i, j].terrainTypeBitmap;
                        String debugterrainpath = "";
                        switch (worldMap[i,j].landType) {
                            case "OPENWATER":
                                switch (worldMap[i,j].terrainType)
                                {
                                    case "COLDFRESHWATER":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\openwater\\coldFreshwater.png";
                                        break;
                                    case "COLDOCEANIC":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\openwater\\coldSeawater.png";
                                        break;
                                    case "FRESHWATER":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\openwater\\Freshwater.png";
                                        break;
                                    case "OCEANIC":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\openwater\\Seawater.png";
                                        break;
                                    default:
                                        break;


                                }
                                break;
                            case "LAND":
                                switch (worldMap[i, j].terrainType) {
                                    case "PLAINS":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\plains.png";
                                        break;
                                    case "GRASSLAND":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\grassland.png";
                                        break;
                                    case "DESERT":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\desert.png";
                                        break;
                                    case "TUNDRA":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\tundra.png";
                                        break;
                                    default:
                                        Console.WriteLine(worldMap[i, j].toString1());
                                        break;


                                }
                                break;
                            case "MOUNTAIN":
                                switch (worldMap[i, j].terrainType)
                                {
                                    case "PLAINS":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\plains.png";
                                        break;
                                    case "GRASSLAND":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\grassland.png";
                                        break;
                                    case "DESERT":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\desert.png";
                                        break;
                                    case "TUNDRA":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\tundra.png";
                                        break;
                                    default:
                                        Console.WriteLine(worldMap[i,j].toString1());
                                        break;

                                }
                                break;
                        }

                        Bitmap worldTileBaseBitmap = getBitmap(debugterrainpath);
                        

                        if (worldMap[i, j].landType == "MOUNTAIN") {
                            worldTileBaseBitmap = CombineBitmaps(new Bitmap[] { worldTileBaseBitmap , getBitmap("\\debug\\res\\worldtiles_tilesets\\mountain\\plains.png")  } );
                        }


                        graphics.DrawImage(worldTileBaseBitmap, drawY, drawX);
                    }

                }
            }

            return globalBitmap;
        }

        public static Bitmap DEBUGgetBitmap_Rivers(WorldTile[,] worldMap)
        {
            bool DEBUG = false;
            int size = worldMap.GetLength(0);
            int canvasSize = worldTileSize * worldMap.GetLength(0);

            Bitmap globalBitmap = new Bitmap(canvasSize + (worldTileSize / 2), canvasSize + (worldTileSize / 2));

            using (Graphics graphics = Graphics.FromImage(globalBitmap))
            {
                graphics.Clear(Color.Black); // Optional: Clear the background


                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        int drawX = i * worldTileSize;
                        int drawY = j * worldTileSize;

                        //Bitmap worldTileBaseBitmap = worldMap[i, j].terrainTypeBitmap;
                        String debugterrainpath = "";
                        switch (worldMap[i, j].landType)
                        {
                            case "OPENWATER":
                                switch (worldMap[i, j].terrainType)
                                {
                                    case "COLDFRESHWATER":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\openwater\\coldFreshwater.png";
                                        break;
                                    case "COLDOCEANIC":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\openwater\\coldSeawater.png";
                                        break;
                                    case "FRESHWATER":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\openwater\\Freshwater.png";
                                        break;
                                    case "OCEANIC":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\openwater\\Seawater.png";
                                        break;
                                    default:
                                        break;


                                }
                                break;
                            case "LAND":
                                switch (worldMap[i, j].terrainType)
                                {
                                    case "PLAINS":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\plains.png";
                                        break;
                                    case "GRASSLAND":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\grassland.png";
                                        break;
                                    case "DESERT":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\desert.png";
                                        break;
                                    case "TUNDRA":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\tundra.png";
                                        break;
                                    default:
                                        Console.WriteLine(worldMap[i, j].toString1());
                                        break;


                                }
                                break;
                            case "MOUNTAIN":
                                switch (worldMap[i, j].terrainType)
                                {
                                    case "PLAINS":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\plains.png";
                                        break;
                                    case "GRASSLAND":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\grassland.png";
                                        break;
                                    case "DESERT":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\desert.png";
                                        break;
                                    case "TUNDRA":
                                        debugterrainpath = "\\debug\\res\\worldtiles_tilesets\\land\\terrainType\\tundra.png";
                                        break;
                                    default:
                                        Console.WriteLine(worldMap[i, j].toString1());
                                        break;

                                }
                                break;
                        }

                        Bitmap worldTileBaseBitmap = getBitmap(debugterrainpath);


                        if (worldMap[i, j].landType == "MOUNTAIN")
                        {
                            worldTileBaseBitmap = CombineBitmaps(new Bitmap[] { worldTileBaseBitmap, getBitmap("\\debug\\res\\worldtiles_tilesets\\mountain\\plains.png") });
                        }


                        graphics.DrawImage(worldTileBaseBitmap, drawY, drawX);
                    }

                }
            }

            return globalBitmap;
        }


    }
}
