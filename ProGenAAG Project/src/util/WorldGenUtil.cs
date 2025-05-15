using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using LibNoise;
using LibNoise.Primitive;
using System;
using System.Reflection.Emit;
using System.Drawing;
using System.Drawing.Drawing2D;
using LibNoise.Renderer;

//  Utilizes the WorldTile Class





namespace ProGenAAG_Project {
    public class WorldGenUtil {
        public static Random random = new Random();

        #region defined
        //  Arguement Values
        #region Indexes of Arguements
        public static readonly int argIndex_arraysize               = 0;    //  World Size
        public static readonly int argIndex_sealevel                = 1;    //  Sea Level
        public static readonly int argIndex_mountainThreshold       = 2;    //  Mountain Threshold
        public static readonly int argIndex_continentAmount         = 3;    //  PlaceHolder Values
        public static readonly int argIndex_placeholder4            = 4;    //  PlaceHolder Values
        public static readonly int argIndex_placeholder5            = 5;    //  PlaceHolder Values
        public static readonly int argIndex_temperatureSmoothness   = 6;    //  Temperature for Smoothness
        public static readonly int argIndex_centerStrength          = 7;    //  CenterStrength for Temperature
        public static readonly int argIndex_placeholder8            = 8;    //  PlaceHolder Values
        public static readonly int argIndex_placeholder9            = 9;    //  PlaceHolder Values
        public static readonly int argIndex_placeholder10           = 10;   //  Placeholder Values
        #endregion
        //  IDs for temperature
        #region Constants of Temperature
        public static readonly int temperatureSetting_PERMAFROST = -70;
        public static readonly int temperatureThreshhold_FREEZING = -32;
        public static readonly int temperatureThreshhold_COLD = 32;
        public static readonly int temperatureThreshhold_WARM = 80;
        public static readonly int temperatureThreshhold_HOT = 90;
        #endregion
        //  IDs for hydration values
        #region Constants of Hydration
        public static readonly int hydrationID_ARID = 1;
        public static readonly int hydrationID_DRY = 2;
        public static readonly int hydrationID_HYDRATED = 3;
        public static readonly int hydrationID_HIGHLYHYDRATED = 4;
        public static readonly int hydrationID_SUPERHYDRATED = 5;
        public static readonly int hydrationID_FRESHWATERED = 6;
        public static readonly int hydrationID_SEAWATERED = 7;
        #endregion
        //  IDs for river values
        #region Constants of River Form
        public static readonly int riverForm_noRiver        = 00;
        public static readonly int riverForm_water          = 01;
        public static readonly int riverForm_riverGeneric   = 02;
        public static readonly int riverForm_poolNorth      = 11;
        public static readonly int riverForm_poolEast       = 12;
        public static readonly int riverForm_poolSouth      = 13;
        public static readonly int riverForm_poolWest       = 14;
        public static readonly int riverForm_straightNS    = 20;
        public static readonly int riverForm_straightEW    = 21;
        public static readonly int riverForm_LjointNW      = 30;
        public static readonly int riverForm_LjointNE      = 31;
        public static readonly int riverForm_LjointSW      = 32;
        public static readonly int riverForm_LjointSE      = 33;
        #endregion

        //  Miscellaneous Values 
        #region Misc Constants
        public static readonly double seaSizeThreshholdModifier = 2;
        #endregion
        #endregion defined

        

        //  SECTION:    Mapset Generators
        /**INFO*/
        #region Base Map Generators




        //  SECTION:    Land/Sea Generation
        #region Land Generation


        //  Creates a water base that we can put continents on
        public static int[,] createWaterBase(int[] mapArgs)
        {
            int size = mapArgs[argIndex_arraysize];
            int sealevel = mapArgs[argIndex_sealevel];
            double gradient = 1.00;

            int randomSeed = random.Next(0, 32767);

            int[,] array = new int[size, size];
            ImprovedPerlin perlin = new ImprovedPerlin(randomSeed, NoiseQuality.Best);

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    double noiseValue = perlin.GetValue((float)(r * 0.1), (float)(c * 0.1), 0) * 0.5 + 0.5; // Normalize Perlin noise to [0,1]
                    int mappedValue = (int)(63 * Math.Pow(noiseValue, gradient));
                    array[r, c] = Math.Clamp(mappedValue, 0, (sealevel / 4));
                }
            }
            return array;
        }
            //  Modifies an integer Noise Map to appear more "Natural"
        public static int[,] naturalizeLandForm(int[] mapArgs, int[,] elevationMap) {
            bool verbose = false;
            int size = mapArgs[argIndex_arraysize];
            int sealevel = mapArgs[argIndex_sealevel];
            int sizePunchout = 2;
            int size_borderPunchoutRange = 4;
            int numLakesRemovedMin = 0;
            int numLakesRemovedMax = size / 4;
            

            //  For any landmass, create a new natural border
            List<List<Coords>> landmassBorders = GeneralUtil.FindIslandBordersInRange(elevationMap, sealevel, 64, false, false, 1);
            foreach (List<Coords> borderCoords in landmassBorders) {
                foreach (Coords coordinates in borderCoords) {
                    int randomChance = random.Next(0, 101);
                    int randomRange = random.Next(1, size_borderPunchoutRange);
                    List<Coords> circleCoords = GeneralUtil.GetCoordsWithinCircle(elevationMap, coordinates, randomRange);
                    if (randomChance <= 25) {
                        foreach (Coords circleCoord in circleCoords) {
                            elevationMap[circleCoord.xCoord, circleCoord.yCoord] = sealevel;
                        }
                    }
                    else {
                        foreach (Coords circleCoord in circleCoords) {
                            elevationMap[circleCoord.xCoord, circleCoord.yCoord] = sealevel - 1;
                        }
                    }
                    
                }
            }

            //  Set all land values near water sources to the value of sealevel (coastal)
            List<List<Coords>> coastalTilesLand = GeneralUtil.FindIslandBordersInRange(elevationMap, sealevel, 999, false, false, 3);
            foreach (List<Coords> listCoasts in coastalTilesLand) {
                foreach (Coords coordinates in listCoasts) {
                    elevationMap[coordinates.xCoord, coordinates.yCoord] = sealevel;
                }
            }





            //  Identify lakes and remove lakes below a certain size (I.E 1 tile lakes)
            //  TODO: This function isn't reliable, improve it

            


            Boolean smallLakesExist = true;
            while (smallLakesExist) {
                List<List<Coords>> lakesList = getLakesList(elevationMap, mapArgs);

                //  Set all lakes below a certain size to sea-level
                foreach (List<Coords> lakeTileList in lakesList)
                {
                    if (lakesList.Count() < (size * seaSizeThreshholdModifier))
                    {
                        //Console.WriteLine("Lake has" + lakeTileList.Count() + " tiles");
                        foreach (Coords lakeTile in lakeTileList)
                        {

                            /** This part creates a wider border around any isolated lake
                            List<Coords> circleCoods = GeneralUtil.GetCoordsWithinCircle(elevationMap, lakeTile, 4);
                            foreach (Coords circleCood in circleCoods)
                            {
                                elevationMap[circleCood.xCoord, circleCood.yCoord] = sealevel;
                            }
                            **/
                            
                            elevationMap[lakeTile.xCoord, lakeTile.yCoord] = sealevel;


                        }
                    }



                }



                smallLakesExist = false;


            }

            //  Shave off sides, and create a boundary between the west/east edges of the array
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    if (j < (size / 32) || j > size - (size / 32)) {
                        if (elevationMap[i, j] >= sealevel)
                        {
                            elevationMap[i, j] = sealevel - 1;
                        }
                    }
                    if (j == (size / 32) || j == size - (size / 32)) {
                        int randomChance = random.Next(0, 101);
                        if (randomChance <= 25) {
                            List<Coords> borderpunchout = GeneralUtil.GetCoordsWithinCircle(elevationMap, new Coords(i, j), sizePunchout);
                            foreach (Coords coordinates in borderpunchout)
                            {

                                elevationMap[coordinates.xCoord, coordinates.yCoord] = sealevel - 1;

                            }
                        }

                        if (elevationMap[i, j] >= sealevel) {
                            elevationMap[i, j] = sealevel - 1;
                        }


                    }
                }
            }


            return elevationMap;
        }
            //  Method which returns an array of lakes (bodies of water below a certain size
        public static List<List<Coords>> getLakesList(int[,] elevationMap, int[] mapArgs)
        {
            int size = mapArgs[argIndex_arraysize];
            int sealevel = mapArgs[argIndex_sealevel];

            //  Get all islands below sea level (I.E all pockets of water) and store in list
            List<List<Coords>> lakesList = GeneralUtil.FindIslandsInRange(elevationMap, 0, sealevel - 1, true, false);


            
            //  Remove the ocean present at the border
            List<Coords> oceanTileCoords;
            
            int indexOfOcean = 0;
            //  Iterate through lakesList: identify the index of the sublist containing ocean tiles
            for (int listCounter = 0; listCounter < lakesList.Count(); listCounter++) {
                List<Coords> lakeTilesList = lakesList[listCounter];

                for (int coordCount = 0; coordCount < lakeTilesList.Count(); coordCount++) {
                    
                    if (lakeTilesList[coordCount].xCoord == 0 && lakeTilesList[coordCount].yCoord == 0) {
                        indexOfOcean = listCounter;
                        
                        
                    }
                }
                
            }
            //  Remove the index
            lakesList.RemoveAt(indexOfOcean);

            oceanRemoved:


            //  Remove all bodies of water above a certain tilecount
            GeneralUtil.RemoveListAboveSize(lakesList, (Convert.ToInt32(size * seaSizeThreshholdModifier)));
            //Console.WriteLine("Counttile" + counttile);


            return lakesList;
        }
        
            //  Returns a single landmass on a water base
        public static List<List<Coords>> getValidContinentCoordsLists(int[] mapArgs, int[,] elevationMap, int[,] continentMap) {
            Boolean verbose = false;
            
            //  Args
            int size = mapArgs[argIndex_arraysize];
            int sealevel = mapArgs[argIndex_sealevel];
            int mountainRange = mapArgs[argIndex_mountainThreshold];

            int minimumSize = (int)((size * 20));
            int maximumSize = (int)((size * 100));
            double maxOverlap = 0.5;

            //  Create Water Base
            int[,] continentCanvas = createWaterBase(mapArgs);
            //  Create continent with natural landforms
            


            //  Now, we must isolate only valid continents
            //  First, we create a list of valid contientns
            List<List<Coords>> continents = GeneralUtil.FindIslandsInRange(continentMap, sealevel, 999999, false, false);
            //  Now, we iterate through our conditions



            //  Condition 1: All Continents must be of sufficient size (i.e between a tile count)
            #region Condition 1
                //  Remove landmass below a certain size
            GeneralUtil.RemoveListBelowSize(continents, minimumSize);
                //  Remove landmass above a certain size
            GeneralUtil.RemoveListAboveSize(continents, maximumSize);

            #endregion Condition 1


            //  Condition 2: No continent overlaps more than maxOverlap% with the elevationMap
            #region Condition 2
                //  Get the amount of land tiles on the elevation map
            int numLandTiles = 0;
            foreach (int tile in elevationMap) {
                if (tile >= sealevel) {
                    numLandTiles++;
                }
            }
                //  Get overlapping and remove them
            int overlapCounter = 0;
            List<int> overlappingIndexes = new List<int>();
            for (int counter = 0; counter < continents.Count(); counter++) {
                List<Coords> continent = continents[counter];
                //  Gets the amount of overlapping tiles
                foreach (Coords coordinates in continent) {
                    //  If a land tile in each continent would overlap with a land tile on the elevation map, increment the counter
                    if (elevationMap[coordinates.xCoord, coordinates.yCoord] >= sealevel) {
                        overlapCounter++;
                    }
                }

                //  Compare the ratio of overlap as a percentage
                double percentageOverlap = (double)overlapCounter / (double)numLandTiles;

                //  If the percentage is too high, add the index of the list to the overlappingIndexes list
                if (percentageOverlap > maxOverlap)
                {
                    overlappingIndexes.Add(counter);
                }
            }

            //  Sort the list so that is starts backwards and in order to avoid falling
            overlappingIndexes.Sort();
            overlappingIndexes.Reverse();
            //  Remove list of continents with indexes inside of overlappingIndexes
            foreach (int index in overlappingIndexes) {
                continents.RemoveAt(index);
            }

            #endregion Condition 2

            //  If debugging, display the eleveation map at this stage
            if (verbose) {
                Console.WriteLine(" Continents remaining after cull: " + continents.Count());
            }





            return continents;
            
        }
        
           
        
        
            //  Function that oversees creating an elevation map       
        public static int[,] generateElevationMap(int[] mapArgs) {
            int failureGeneration = 0;
            restartElevationGen:

            Boolean verbose = false;
            
            int size = mapArgs[argIndex_arraysize];
            int sealevel = mapArgs[argIndex_sealevel];
            int mountainRange = mapArgs[argIndex_mountainThreshold];

            int continentAmount = mapArgs[argIndex_continentAmount];
            //continentAmount = 2;    //Min 1, Max 5?

            

            //  Generate Large Landmasses
            int restartCounter = 0;
            //  Generate the base from which we will generate continents
            int[,] elevationMap = createWaterBase(mapArgs);
            for (int continentCounter = 0; continentCounter < continentAmount; continentCounter++) {
                double maxOverlap = 0.5;
                
                
                regenerateContinent:
                if (verbose) {
                    Console.Write("Amount of failures in continent " + continentCounter + ": " + restartCounter);
                    GeneralUtil.ClearCurrentConsoleLine();
                }

                //  Generate a map of valid continents
                int[,] continentMap = GeneralUtil.generatePerlinNoise(size, size, 1.00, 0, 63);
                naturalizeLandForm(mapArgs, continentMap);

                //  Generate a list of coords of valid continents
                List<List<Coords>> continents = getValidContinentCoordsLists(mapArgs, elevationMap, continentMap);

                //  Verify that there are valid continents left
                if (continents.Count() <= 1) {
                    if (verbose)
                    {
                        Console.WriteLine(" No continents available: regenerating");
                    }
                    restartCounter++;

                    goto regenerateContinent;
                }

                //  Get a random valid continent
                int randomIndex = random.Next(0, continents.Count());
                List<Coords> continentLayer = continents[randomIndex];

                //  Determine if this contiennt will be "fused"
                Boolean fusedContinent = false;
                



                //  Get all overlapping continents to be cut out
                List<Coords> overlappingTiles = new List<Coords>();

                //TODO
                foreach (Coords coordinates in continentLayer) {
                    //  If the land tile would be placed in ocean, place it as you would
                    if (elevationMap[coordinates.xCoord, coordinates.yCoord] < sealevel) {
                        elevationMap[coordinates.xCoord, coordinates.yCoord] = continentMap[coordinates.xCoord, coordinates.yCoord];
                    }
                    //  If the land tile would be placed on another land tile, check
                    else if (elevationMap[coordinates.xCoord, coordinates.yCoord] >= sealevel) {
                        elevationMap[coordinates.xCoord, coordinates.yCoord] = continentMap[coordinates.xCoord, coordinates.yCoord];
                        overlappingTiles.Add(coordinates);
                    }
                }

                foreach (Coords overlappingTile in overlappingTiles) {
                    //elevationMap[overlappingTile.xCoord, overlappingTile.yCoord] = mountainRange;
                }
                



                
                if (verbose) {
                    Console.WriteLine("[DEBUG]  Printing the elevation Map at stage: " + continentCounter);
                    ASCII_printElevationMap(elevationMap, mapArgs); 
                }
            }

            //  Remove borders for shrinkage
            List<List<Coords>> bordertiles = GeneralUtil.FindIslandBordersInRange(elevationMap, sealevel, 666, false, false, (size / 64));
            foreach (List<Coords> borders in bordertiles) { 
                foreach (Coords coords in borders) {
                        //elevationMap[coords.xCoord, coords.yCoord] = mountainRange;

                }
            }



            //TODO: Paint continentCanvas onto elevation map
            //  Possibly make its own method?
            int waterCounter = 0;
            foreach (int value in elevationMap) {
                if (value < sealevel) {
                    waterCounter++;
                }
            }
            //  Verify that map is mostly land
            if ((double)waterCounter / (double)(size*size) >= 0.50) {
                if (verbose)
                {
                    Console.WriteLine("Too much water: regenerating world");
                }
                failureGeneration++;
                if (verbose) {
                    Console.WriteLine(failureGeneration + " failures to generate: restarting");
                }
                //goto restartElevationGen;
            }
            return elevationMap;
        }

        #endregion

        //  SECTION: Temperature Map
        #region Temperature Generation
        public static int[,] generateTemperatureMap(int[] args) {
            // double smoothness, double centerStrength
            int size = args[argIndex_arraysize];
            double smoothness = 100;
            double centerStrength = (double)args[argIndex_temperatureSmoothness] / 10;
            centerStrength = -200.5;

            

            int[,] array = new int[size, size];
            Random rand = new Random();

            for (int r = 0; r < size; r++) {
                double normalizedRow = (double)r / (size - 1); // Normalize row to [0,1]
                double distanceFromCenter = Math.Abs(normalizedRow - 0.5) * 2; // 0 at center, 1 at edges
                double strengthFactor = Math.Pow(distanceFromCenter, smoothness) * centerStrength;
                double gradientValue = 100 - (160 * (distanceFromCenter + strengthFactor));

                for (int c = 0; c < size; c++)
                {
                    int noise = rand.Next(-5, 6); // Small horizontal variation
                    array[r, c] = Math.Clamp((int)gradientValue + noise, -60, 100);

                    if (r <= (size / 32) || r >= size - (size / 32) - 1)
                    {
                        array[r,c] = temperatureSetting_PERMAFROST;
                    }

                }
            }



            //  Identify islands
            List<List<Coords>> mediumisolated = GeneralUtil.FindIslandsInRange(array, -9999999, temperatureThreshhold_COLD, true, true);
            //  Remove the biggest island
            GeneralUtil.RemoveLargestList(mediumisolated);
            foreach (List<Coords> temperlist in mediumisolated) {
                foreach (Coords temper in temperlist) {
                    //  Get the coords of the neighbors of each island
                    List<Coords> circleCoords = GeneralUtil.GetCoordsWithinCircle(array, temper, 1);
                    List<int> averageTemperature = new List<int>();
                    //  Add neighboring array temperature to average
                    foreach (Coords circCoords in circleCoords) {
                        averageTemperature.Add(array[circCoords.xCoord,circCoords.yCoord]);
                    }

                    array[temper.xCoord,temper.yCoord] = (int)averageTemperature.Average();



                }
            }




            //  Identify islands
            List<List<Coords>> warmisolated = GeneralUtil.FindIslandsInRange(array, temperatureThreshhold_COLD + 1, 99999999, true, true);
            //  Remove the biggest island
            GeneralUtil.RemoveLargestList(warmisolated);
            foreach (List<Coords> temperlist in warmisolated)
            {
                foreach (Coords temper in temperlist)
                {
                    //  Get the coords of the neighbors of each island
                    List<Coords> circleCoords = GeneralUtil.GetCoordsWithinCircle(array, temper, 2);
                    List<int> averageTemperature = new List<int>();
                    //  Add neighboring array temperature to average
                    foreach (Coords circCoords in circleCoords)
                    {
                        averageTemperature.Add(array[circCoords.xCoord, circCoords.yCoord]);
                    }

                    array[temper.xCoord, temper.yCoord] = (int)averageTemperature.Average();



                }
            }



            return array;
        }
        #endregion

        //  SECTION:    Hydration Generation
        #region Hydration Generation

        public static int[,] generateHydrationMap(int[] mapArgs,int[,] elevationMap, int[,] temperatureMap, int[,] riversMap) {
            bool verbose = false;
            int size = elevationMap.GetLength(0);
            int sealevel = mapArgs[argIndex_sealevel];
            int mountainThreshold = mapArgs[argIndex_mountainThreshold];
            
            int[,] hydrationMap = new int[size, size];

            int heatedRange = size / 64;
            int temperateRange = size / 32;
            int coldRange = size / 64;

            //  Initialize array
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    if (elevationMap[i, j] < sealevel) {
                        hydrationMap[i, j] = hydrationID_SEAWATERED;
                    } 
                    else {
                        hydrationMap[i, j] = hydrationID_ARID;
                    }

                } 
            }

            //  Identify lakes
            List<List<Coords>> lakesList = getLakesList(elevationMap, mapArgs);


            //  Set all lakes to lake hydrated
            List<Coords> allLakeTiles = new List<Coords>();
            foreach (List<Coords> lakeTiles in lakesList) {
                foreach (Coords lakeCoords in lakeTiles) {
                    hydrationMap[lakeCoords.xCoord, lakeCoords.yCoord] = hydrationID_FRESHWATERED;
                    allLakeTiles.Add(lakeCoords);
                }
            }




            //  Identify regions that are within a hydration range of a lake
            for (int r = 0; r < hydrationMap.GetLength(0); r++) {
                for (int c = 0; c < hydrationMap.GetLength(1); c++) {

                    if (elevationMap[r,c] >= sealevel) {

                        //  Tiles in radius for first "thin" layer
                        List<Coords> tilesInRadius = new List<Coords>();
                        //  Tiles in radius for first "thick" layer
                        List<Coords> tilesInRadiusAux = new List<Coords>();

                        if (temperatureMap[r, c] >= temperatureThreshhold_WARM)
                        {
                            tilesInRadius = GeneralUtil.GetCoordsWithinCircle(temperatureMap, new Coords(r, c), heatedRange);
                            tilesInRadiusAux = GeneralUtil.GetCoordsWithinCircle(temperatureMap, new Coords(r, c), heatedRange * 2);
                        }
                        else if (temperatureMap[r, c] < temperatureThreshhold_COLD)
                        {
                            tilesInRadius = GeneralUtil.GetCoordsWithinCircle(temperatureMap, new Coords(r, c), coldRange);
                            tilesInRadiusAux = GeneralUtil.GetCoordsWithinCircle(temperatureMap, new Coords(r, c), coldRange * 2);
                        }
                        else
                        {
                            tilesInRadius = GeneralUtil.GetCoordsWithinCircle(temperatureMap, new Coords(r, c), temperateRange);
                            tilesInRadiusAux = GeneralUtil.GetCoordsWithinCircle(temperatureMap, new Coords(r, c), temperateRange*2);
                        }

                        //  First, check distances for aux "lesser" impact
                        foreach (Coords tileCoordinates in tilesInRadiusAux) {
                            //  First, check if it is near ocean
                            if (elevationMap[tileCoordinates.xCoord, tileCoordinates.yCoord] < sealevel) {
                                hydrationMap[r, c] = hydrationID_HYDRATED;
                            }
                            //  Next, calculate hydration for rivers and lakes
                            //  First for Lakes
                            if (allLakeTiles.Contains(tileCoordinates)) {
                                hydrationMap[r, c] = hydrationID_HYDRATED;
                            }

                        }

                        //  Second, check for primary "proximate" impact
                        foreach (Coords tileCoordinates in tilesInRadius) {
                            //  First, check if it is near ocean
                            if (elevationMap[tileCoordinates.xCoord, tileCoordinates.yCoord] < sealevel) {
                                hydrationMap[r, c] = hydrationID_HYDRATED;
                            }
                            //  Next, calculate hydration for rivers and lakes
                            //  First for Lakes
                            if (allLakeTiles.Contains(tileCoordinates))
                            {
                                hydrationMap[r, c] = hydrationID_HIGHLYHYDRATED;
                            }
 
                        }
                    }

                    


                }
            }

            //  Identify regions that are within the range of a river: bump hydration

            for (int r = 0; r < hydrationMap.GetLength(0); r++)
            {
                for (int c = 0; c < hydrationMap.GetLength(0); c++)
                {
                    //  If Land
                    if (elevationMap[r, c] >= sealevel)
                    {
                        //  If NOT WATER and NOT NORIVER
                        if (riversMap[r, c] != riverForm_noRiver)
                        {
                            //  Bump hydration
                            if (hydrationMap[r, c] == hydrationID_ARID || hydrationMap[r, c] == hydrationID_DRY)
                            {
                                //hydrationMap[r, c] = hydrationID_HYDRATED;
                                hydrationMap[r, c] = hydrationID_HIGHLYHYDRATED;
                            }
                            else if (hydrationMap[r, c] == hydrationID_HYDRATED)
                            {
                                hydrationMap[r, c] = hydrationID_HIGHLYHYDRATED;
                            }
                            else if (hydrationMap[r, c] == hydrationID_HIGHLYHYDRATED)
                            {
                                hydrationMap[r, c] = hydrationID_SUPERHYDRATED;
                            }


                        }
                    }
                }
            }

            return hydrationMap;
        }

        #endregion

        //  SECTION:    River Generation
        #region River Generation
        //  TODO
        public static int[,] generateRiversMap(int[] mapArgs, int[,] elevationMap) {
            int size = mapArgs[argIndex_arraysize];
            int sealevel = mapArgs[argIndex_sealevel];
            int[,] riversMap = new int[size, size];
            bool debug = true;
            int riverLengthModifier = 1;



            //  Create a list of lands
            List<List<Coords>> landmassMaps = GeneralUtil.FindIslandsInRange(elevationMap, sealevel, 64, false, false);



            //  Intialize the rivers array
            for (int r = 0; r < size; r++) {
                for (int c = 0; c < size; c++) {

                    if (elevationMap[r,c] < sealevel)
                    {
                        riversMap[r, c] = riverForm_water;
                    } else
                    {
                        riversMap[r, c] = riverForm_noRiver;
                    }

                    
                }
            }

            
            
            return riversMap;


        }

        #endregion






        #endregion Integer Map Generators





        //  SECTION:    ASCII Printing of Mapsets
        #region ASCII Represenation of Mapsets
        public static void ASCII_printRiversMap(int[] mapArgs, int[,] riversMap) {
            int size = mapArgs[argIndex_arraysize];
            for (int counter = 0; counter < size; counter++) {
                Console.Write("");
            }

            for (int i = 0; i < riversMap.GetLength(0); i++) { 
                
                for (int j = 0; j < riversMap.GetLength(1); j++) {





                   
                    

                    
                    //  No rivers
                    if (riversMap[i, j] == riverForm_noRiver)
                    {
                        Console.Write("*   *");
                    }
                    else if (riversMap[i, j] == riverForm_water)
                    {
                        Console.Write("     ");
                    }
                    
                    else if (riversMap[i, j] == riverForm_riverGeneric)
                    {
                        Console.Write("[ R ]");
                    }

                    //  Pools
                    else if (riversMap[i, j] == riverForm_poolNorth)
                    {
                        Console.Write("[ O ]");
                    }
                    else if (riversMap[i, j] == riverForm_poolEast)
                    {
                        Console.Write("[-O ]");
                    }
                    else if (riversMap[i, j] == riverForm_poolSouth)
                    {
                        Console.Write("[ O ]");
                    }
                    else if (riversMap[i, j] == riverForm_poolWest)
                    {
                        Console.Write("[ O-]");
                    }


                    //  Straights
                    else if (riversMap[i, j] == riverForm_straightNS)
                    {
                        Console.Write("[---]");
                    }
                    else if (riversMap[i, j] == riverForm_straightEW)
                    {
                        Console.Write("[ | ]");
                    }


                    //  L-Joints
                    else if (riversMap[i, j] == riverForm_LjointNW)
                    {
                        Console.Write("[ |-]");
                    }
                    else if (riversMap[i, j] == riverForm_LjointNE)
                    {
                        Console.Write("[-| ]");
                    }
                    else if (riversMap[i, j] == riverForm_LjointSW)
                    {
                        Console.Write("[ |-]");
                    }
                    else if (riversMap[i, j] == riverForm_LjointSE)
                    {
                        Console.Write("[-| ]");
                    }



                    else
                    {
                        Console.Write("[ ? ]");
                    }






                }

                Console.WriteLine();
                
                
                
            
            }

        }
        public static void ASCII_printElevationMap(int[,] elevationMap, int[] mapArgs)
        {
            int sealevel = mapArgs[argIndex_sealevel];
            int mountainLevel = mapArgs[argIndex_mountainThreshold];

            Console.WriteLine(" ");
            Console.WriteLine(" ");
            for (int i = 0; i < elevationMap.GetLength(0); i++)
            {
                for (int j = 0; j < elevationMap.GetLength(1); j++)
                {
                    if (elevationMap[i, j] < sealevel)
                    {
                        Console.Write(" " + elevationMap[i, j].ToString("00") + " ");
                    }
                    else if ((elevationMap[i, j] >= sealevel) && (elevationMap[i, j] < mountainLevel))
                    {
                        Console.Write("[" + elevationMap[i, j].ToString("00") + "]");
                    }

                    else if ((elevationMap[i, j] >= mountainLevel))
                    {
                        Console.Write("^" + elevationMap[i, j].ToString("00") + "^");
                    }
                    else
                    {
                        Console.Write("X" + elevationMap[i, j].ToString("00") + "X");
                    }

                }
                Console.WriteLine();
            }
            Console.WriteLine(" ");

        }
        public static void ASCII_printTemperatureMap(int[,] temperatureMap)
        {

            for (int i = 0; i < temperatureMap.GetLength(0); i++)
            {
                for (int j = 0; j < temperatureMap.GetLength(1); j++)
                {
                    if (temperatureMap[i, j] < -64)
                    {
                        Console.Write(" P");    //  PERMAFROST
                    }
                    else if (temperatureMap[i, j] < temperatureThreshhold_FREEZING)
                    {
                        Console.Write(" *");    //  Freezing
                    }
                    else if (temperatureMap[i, j] >= temperatureThreshhold_FREEZING && temperatureMap[i, j] < temperatureThreshhold_COLD)
                    {
                        Console.Write(" C");    //  Cold
                    }
                    else if (temperatureMap[i, j] >= temperatureThreshhold_COLD && temperatureMap[i, j] < temperatureThreshhold_WARM)
                    {
                        Console.Write(" -");    //  Temperate
                    }
                    else if (temperatureMap[i, j] >= temperatureThreshhold_WARM && temperatureMap[i, j] < temperatureThreshhold_HOT)
                    {
                        Console.Write(" +");    //  WARM
                    }
                    else if (temperatureMap[i, j] >= temperatureThreshhold_HOT)
                    {
                        Console.Write(" H");    //  HOT
                    }


                    else
                    {
                        Console.Write(" ?");
                    }

                }
                Console.WriteLine();
            }
        }
        public static void ASCII_PrintTerrainMap(WorldTile[,] worldMap)
        {


            for (int i = 0; i < worldMap.GetLength(0); i++)
            {
                for (int j = 0; j < worldMap.GetLength(1); j++)
                {

                    switch (worldMap[i, j].landType)
                    {
                        case "OPENWATER":
                            switch (worldMap[i, j].terrainType)
                            {
                                case "COLDFRESHWATER":
                                    Console.Write("    ");
                                    break;
                                case "COLDOCEANIC":
                                    Console.Write("    ");
                                    break;
                                default:
                                    Console.Write(" !! ");
                                    break;

                            }
                            break;
                        case "LAND":
                            switch (worldMap[i, j].terrainType)
                            {
                                case "PLAINS":
                                    Console.Write(" ~-  ");
                                    break;
                                case "GRASSLAND":
                                    Console.Write(" =- ");
                                    break;
                                case "DESERT":
                                    Console.Write(" !- ");
                                    break;
                                case "TUNDRA":
                                    Console.Write(" *- ");
                                    break;
                                default:
                                    Console.Write(" !- ");
                                    break;
                            }
                            break;
                        case "MOUNTAIN":
                            switch (worldMap[i, j].terrainType)
                            {
                                case "PLAINS":
                                    Console.Write(" ~^  ");
                                    break;
                                case "GRASSLAND":
                                    Console.Write(" =^ ");
                                    break;
                                case "DESERT":
                                    Console.Write(" !^ ");
                                    break;
                                case "TUNDRA":
                                    Console.Write(" *^ ");
                                    break;
                                default:
                                    Console.Write(" !^ ");
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine("ERROR");
                            break;

                    }



                }
            }

        }
        #endregion








        //  SECTION:    Base World Generation

        #region Base World Generation
        //  Given a tile, update its parameters to be consistent with its features
        public static WorldTile assignTileParameters(WorldTile tile, int[] args) {
            int sealevel = args[argIndex_sealevel];
            int mountainLevel = args[argIndex_mountainThreshold];
            int size = args[argIndex_arraysize];
            int temperatureModifier = args[4];

            //  STEP ONE: Assign the LandForm Type
            if (tile.elevation < sealevel) {
                tile.landType = "OPENWATER";
            }
            else if ((tile.elevation >= sealevel) && (tile.elevation < mountainLevel)) {
                tile.landType = "LAND";
            }
            else if ((tile.elevation >= mountainLevel)) {
                tile.landType = "MOUNTAIN";
            }
            else {
                tile.landType = "ERROR";
            }



            //  STEP TWO: Assign the First half of the Climate type
            String climateType = "";
            //  Freezing
            if (tile.temperature < temperatureThreshhold_FREEZING) {
                climateType = "FROZEN";
            }
            //  Cold
            else if (tile.temperature >= temperatureThreshhold_FREEZING && tile.temperature < temperatureThreshhold_COLD) {
                climateType = "COLD";
            }
            //  Temperate
            else if (tile.temperature >= temperatureThreshhold_COLD && tile.temperature < temperatureThreshhold_WARM) {
                climateType = "TEMPERATE";
            }
            //  Warm
            else if (tile.temperature >= temperatureThreshhold_WARM && tile.temperature < temperatureThreshhold_HOT) {
                climateType = "WARM";
            }
            //  Hot
            else if (tile.temperature >= temperatureThreshhold_HOT) {
                climateType = "HOT";
            }
            //  STEP THREE: Assign the Second half of the Climate type
            switch (tile.hydration) {
                //  ARID
                case 1:
                    climateType += "-ARID";
                    break;
                //  DRY
                case 2:
                    climateType += "-DRY";
                    break;
                //  HYDRATED
                case 3:
                    climateType += "-HYDRATED";
                    break;
                //  WET
                case 4:
                    climateType += "-HIGHLYHYDRATED";
                    break;
                //  VERYWET
                case 5:
                    climateType += "-SUPERHYDRATED";
                    break;
                //  LAKE
                case 6:
                    climateType += "-FRESHWATERED";
                    break;
                //  OCEANIC
                case 7:
                    climateType += "-SEAWATERED";
                    break;

            }
            tile.climateType = climateType;

            //  Assign Biome Type

            //  Water Climates
            String[] ColdOceanClimates = { "COLD-SEAWATERED" };
            String[] FrozenOceanClimates = { "FROZEN-SEAWATERED"};
            String[] ColdFreshwaterClimates = { "COLD-FRESHWATERED" };
            String[] FrozenFreshwaterClimates = { "FROZEN-FRESHWATERED" };


            /**
            
            
            COLD-FRESHWATERED
            COLD-SEAWATERED
            FROZEN-FRESHWATERED
            FROZEN-SEAWATERED
            HOT-FRESHWATERED
            HOT-SEAWATERED
            TEMPERATE-FRESHWATERED
            TEMPERATE-SEAWATERED
            WARM-FRESHWATERED
            WARM-SEAWATERED
            
             * **/

            //  Land Climates
            //  Plains are dryer 
            String[] PlainsClimates = { "WARM-HYDRATED", "HOT-HYDRATED", "TEMPERATE-HYDRATED" };
            //  Grasslands are Greener
            String[] GrasslandClimates = { "TEMPERATE-HIGHLYHYDRATED", "WARM-HIGHLYHYDRATED", "WARM-SUPERHYDRATED", "HOT-HIGHLYHYDRATED", "HOT-SUPERHYDRATED", "TEMPERATE-SUPERHYDRATED" };
            //  Deserts are dry and hot
            String[] DesertClimates = { "WARM-ARID", "WARM-DRY", "HOT-ARID", "HOT-DRY", "TEMPERATE-ARID", "TEMPERATE-DRY" };
            //  Tundras are cold and dry
            String[] TundraClimates = { "COLD-ARID", "COLD-DRY", "FROZEN-ARID", "COLD-HIGHLYHYDRATED", "COLD-HYDRATED", "COLD-SUPERHYDRATED", "FROZEN-DRY", "FROZEN-HIGHLYHYDRATED", "FROZEN-HYDRATED", "FROZEN-SUPERHYDRATED" };
            

            //  Mountain Climates



            if (tile.landType == "OPENWATER") {
                if (tile.getTemperatureType() == "COLD" || tile.getTemperatureType() == "FROZEN") {
                    if (tile.getHydrationType() == "FRESHWATERED") {
                        tile.terrainType = "COLDFRESHWATER";
                    }
                    else if (tile.getHydrationType() == "SEAWATERED") {
                        tile.terrainType = "COLDOCEANIC";
                    }
                    else
                    {
                        tile.terrainType = "ERROR";

                    }

                }
                else {
                    if (tile.getHydrationType() == "FRESHWATERED") {
                        tile.terrainType = "FRESHWATER";
                    }
                    else if (tile.getHydrationType() == "SEAWATERED") {
                        tile.terrainType = "OCEANIC";
                    }
                    else
                    {
                        tile.terrainType = "ERROR";

                    }
                }
            }
            else if (tile.landType == "LAND") {

                if (PlainsClimates.Contains(tile.climateType))
                {
                    tile.terrainType = "PLAINS";
                } 
                else if (GrasslandClimates.Contains(tile.climateType))
                {
                    tile.terrainType = "GRASSLAND";
                }
                else if (DesertClimates.Contains(tile.climateType))
                {
                    tile.terrainType = "DESERT";
                }
                else if (TundraClimates.Contains(tile.climateType)) {
                    tile.terrainType = "TUNDRA";
                }
                else
                {
                    tile.terrainType = "ERROR";
                    Console.WriteLine("Tile was assigned " + tile.climateType);
                }
            } 
            else if (tile.landType == "MOUNTAIN") {
                if (PlainsClimates.Contains(tile.climateType))
                {
                    tile.terrainType = "PLAINS";
                }
                else if (GrasslandClimates.Contains(tile.climateType))
                {
                    tile.terrainType = "GRASSLAND";
                }
                else if (DesertClimates.Contains(tile.climateType))
                {
                    tile.terrainType = "DESERT";
                }
                else if (TundraClimates.Contains(tile.climateType))
                {
                    tile.terrainType = "TUNDRA";
                }
                else
                {
                    tile.terrainType = "ERROR";
                    Console.WriteLine("Tile was assigned " + tile.climateType);
                }

            }
            else {
                tile.terrainType = "ERROR";

            }

            //Edge cases

            
            return tile;
        }

            //  Given a tile, update the base images
        public static WorldTile assignTileBitmap(WorldTile tile, int[] args)
        {
            return tile;

        }

            //  Base function for generating a world. This generates only the physical world: it does not generate settlements, etc.
        public static WorldTile[,] generateWorldBase(WorldTile[,] worldMap, int[] args) {

            //  Initial Conditions
            bool verbose = false;
            bool debug = false;
            if (verbose) { Console.WriteLine("[GENWORLD]    Generation Begun..."); }
            
            
            //  Argument assignment for concisness
            int size = args[argIndex_arraysize];
            int seaThreshold = args[argIndex_sealevel];
            int mountThreshold = args[argIndex_mountainThreshold];
            //int mountThreshold  = args[3];



            //  Generate Component Maps
            int[,] elevationMap = generateElevationMap(args);
            int[,] temperMap = generateTemperatureMap(args);


            if (debug) { Console.WriteLine("[DEBUG]    Mapping Rivers..."); }
            int[,] riversMap = generateRiversMap(args, elevationMap);
            //if (debug) { ASCII_printRiversMap(args, riversMap); }




            int[,] hydrationMap = generateHydrationMap(args, elevationMap, temperMap, riversMap);


            //ASCII_printTemperatureMap(temperMap);





            if (verbose) { Console.WriteLine("[GENWORLD]    Component Mapping Completed..."); }
            

            if (verbose) { Console.WriteLine("[GENWORLD]    Assigning tile variation..."); }
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    String locationID = i.ToString("000000") + "_" + j.ToString("000000");
                    worldMap[i, j] = new WorldTile(locationID, new Coords(i, j));

                    worldMap[i, j].elevation = elevationMap[i, j];
                    worldMap[i,j].temperature = temperMap[i, j];
                    worldMap[i,j].hydration = hydrationMap[i, j];


                    //  Assign categories
                    assignTileParameters(worldMap[i, j], args);
                    
                    //  Assign the bitmap
                    //assignTileBitmap(worldMap[i, j], new Coords(i, j));
                    
                    if (verbose) {
                        
                        Console.WriteLine("[GENWORLD]    Tile [" + i + ", " + j + "] out of [" + size + "," + size + "] generated...");
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        GeneralUtil.ClearCurrentConsoleLine();
                    }
                }
                

            }
            
            




            
            if (verbose) { Console.WriteLine("[GENWORLD]   All tiles generated!"); }







            
            if (verbose) { Console.WriteLine("[GENWORLD]    Returning World Map..."); }
            return worldMap;
        }
            //  This function verifies the initial arguement to assure that they are compliant and won't cause any bugs
        public static int[] verifyArgs(int[] args)
        {
            int[] newArgs = new int[args.Length];
            
            
            //  0 is worldSize
            //  1 is seaLevel
            //  2 is mountain threshold
            //  3 is a placeholder
            //  4 is a placeholder
            //  5 is a placeholder
            //  6 is smoothness for temperature
            //  7 is centerstrength for temperature map
            //  8 is a placeholder
            //  9 is a placeholder
            //  10



            return newArgs;

        }

        //  This function generates information regarding the terrainType (Absent all other qualifiers, inc. terrainFeatures)

        #endregion Base World Generation














        //  Debug: Used to test the class
        public static void testWorldGeneration(int[] args) {
            //  Generate world
            bool verbose = true;

            if (verbose) {
                Console.WriteLine("[WorldGeneration] Beginning Process...    ");
            }
            WorldTile[,] worldMap = new WorldTile[args[argIndex_arraysize],args[argIndex_arraysize]];
            if (verbose) {
                Console.WriteLine("[WorldGeneration] Generating World...    ");
            }
            worldMap = generateWorldBase(worldMap, args);

            if (verbose) {
                Console.WriteLine("[WorldGeneration] Creating World Name    ");
            }
            //  Create a Planet Name and unique numeral identifier
            String[] planetnames = FileUtil.ReadAllLines("\\src\\logic\\world\\PlanetNames.txt");
            int planetNumAddition = random.Next(0,25600);
            String planetname = planetnames[random.Next(0, planetnames.Length)] + planetNumAddition;

            if (verbose) {
                Console.WriteLine("[WorldGeneration] World Name:    " + planetname);
            }



            //  Generate a bitmap for terrainType
            Bitmap terrainTypeWorldMapImage = GraphicalUtil.DEBUGgetBitmap_TerrainType(worldMap);


            if (verbose) {
                Console.WriteLine("[WorldGeneration] Saving " + planetname + " to world folder");
            }
            GraphicalUtil.saveImage(terrainTypeWorldMapImage, "\\world\\PNG\\Map_" + planetname  + "_.png");
            FileUtil.SaveToJSON(worldMap,                     "\\world\\JSON\\Map_" + planetname  + "_.json");
            
            if (verbose) {
                Console.WriteLine("[WorldGeneration] World Saved...    ");
            }
        }


        public static void GenerateWorld(int[] args)
        {

        }

    }
}
