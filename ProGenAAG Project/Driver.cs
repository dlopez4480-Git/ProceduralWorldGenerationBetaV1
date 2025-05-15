
using Newtonsoft.Json.Linq;

namespace ProGenAAG_Project
{
    class Driver {
        static void Main(string[] args) {
            Console.WriteLine("This is the World Generator program MK. 1");
            Console.WriteLine("May 2025");
            

            driverSequence();




            Console.WriteLine("Code Complete");
        }



        public static void driverSequence() {




            Boolean running = true;
            while (running) {
                Console.WriteLine("Please select one of the commands");
                Console.WriteLine("     >HELP");
                Console.WriteLine("     >GENWORLD");
                Console.WriteLine("     >GENDEF");
                Console.WriteLine("     >EXIT");

                string command = Console.ReadLine();
                switch (command) {
                    case "HELP":
                        helpCommand();
                        break;
                    case "GENWORLD":
                        Console.WriteLine("Generating Custom World:");
                        generateWorldCommand();

                        break;
                    case "GENDEF":
                        Console.WriteLine("Generating Default World:");
                        test_generation();
                        break;
                    case "EXIT":
                        Console.WriteLine("Exiting Loop ");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("ERROR: Command not recognized");
                        break;
                        
                }
                
            }


        }

        public static void helpCommand() {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(".........................................................................................................................................................");
            Console.WriteLine("GENWORLD allows you to specify the particular parameters of the world");
            Console.WriteLine("GENDEF generates a world based on predetermined default parameters");
            Console.WriteLine("EXIT exits the program");
            Console.WriteLine("All maps can be found in the \\world\\ folder, in png and json form");
            Console.WriteLine("Map Key:");
            Console.WriteLine("");
            Console.WriteLine("LIME TILES represent fertile land. This spawns around sources of fresh water");
            Console.WriteLine("GREEN TILES represent grass lands. This spawns around sources of water in temperate regions");
            Console.WriteLine("TAN TILES represent deserts. These spawn in arid regions, hot regions and those far from water");
            Console.WriteLine("WHITE TILES represent tundra. These spawn close to the equators, and can be dry or wet");
            Console.WriteLine("");
            Console.WriteLine("LIGHT BLUE TILES (temperate) represent FRESH WATER. These generate more fertile lands");
            Console.WriteLine("LIGHT BLUE TILES (frozen) represent FRESH WATER in cold enviorments. Because they are colder, they have less of a benefit to fertility");
            Console.WriteLine("OCEAN TILES can be BLUE or DARK BLUE depending on their temperature.");
            Console.WriteLine(".........................................................................................................................................................");
            Console.WriteLine("");
            Console.WriteLine("");





            Console.WriteLine("This program utilized the Libnoise and NewtonSoft Library");
            Console.WriteLine("");
        }
        public static void generateWorldCommand() {
            //  0 is worldSize
            //  1 is seaLevel
            //  2 is mountain threshold
            //  3 is continent amounts
            //  4 is a placeholder
            //  5 is a placeholder
            //  6 is smoothness for temperature
            //  7 is centerstrength for temperature map
            //  8 is a placeholder
            //  9 is a placeholder
            Console.WriteLine("Choose your parameters for the world");
            Console.WriteLine("WARNING: Straying too far from the reccomended parameter range may cause the program to endlessly loop or crash");


            Console.WriteLine("Enter WorldSize (Choose a minimum of 64, preferably a power of 2)");
            string command = Console.ReadLine();
            int worldSize = Convert.ToInt32(command);
            
            Console.WriteLine("Enter Sealevel (Choose between 0 and 63)");
            command = Console.ReadLine();
            int sealevel = Convert.ToInt32(command);

            Console.WriteLine("Enter Mountain Threshold (Choose above your sealevel threshold, but below 63");
            command = Console.ReadLine();
            int mountthresh = Convert.ToInt32(command);


            Console.WriteLine("Enter Continent Amount (A value around 2 works best to avoid blocky edges)");
            command = Console.ReadLine();
            int contamount = Convert.ToInt32(command);

            //Console.WriteLine("Enter the Strength of the Polar temperatures (Enter an integer value between 0 and 10");
            //command = Console.ReadLine();
            //int tempertureCold = Convert.ToInt32(command)*100;
            int temperatureGradient = 200;

            Console.WriteLine("Enter the Strength of the Equatorial temperatures (Enter an integer value between 0 and 10");
            command = Console.ReadLine();
            int tempertureEquat = Convert.ToInt32(command)*100;


            Console.WriteLine("Generating world with chosen parameters");
            int[] args = { worldSize, sealevel, mountthresh, contamount, 0, 0, temperatureGradient, tempertureEquat, 0, 0, 0};
            WorldGenUtil.testWorldGeneration(args);

            Console.WriteLine("World Generated and placed in the \\world folder");


        }


        public static void generateWorldDefault() {
            //  0 is worldSize
            //  1 is seaLevel
            //  2 is mountain threshold
            //  3 is continent amounts
            //  4 is a placeholder
            //  5 is a placeholder
            //  6 is smoothness for temperature
            //  7 is centerstrength for temperature map
            //  8 is a placeholder
            //  9 is a placeholder
            Console.WriteLine("Generating World at default parameters");
            int worldSize = 128;
            int sealevel = 27;
            int mountthresh = 38;
            int contnumber = 3;
            int smoothnesstemper = 1000;
            int strengthtemperture = 1;

            int[] args = { worldSize, sealevel, mountthresh, contnumber, 0, 0, smoothnesstemper, strengthtemperture, 0, 0, 0};
            WorldGenUtil.testWorldGeneration(args);
            Console.WriteLine("World Generated and placed in the \\world folder");
        }












        public static void generateClimNames()
        {
            
            String[] temperatureStrings = { "FROZEN", "COLD", "TEMPERATE", "WARM", "HOT" };
            String[] hydrationStrings = { "ARID", "DRY", "HYDRATED", "HIGHLYHYDRATED", "SUPERHYDRATED", "FRESHWATERED", "SEAWATERED" };
            int size = temperatureStrings.GetLength(0) * hydrationStrings.GetLength(0);

            List<String> names = new List<String>();

            for (int hycount = 0; hycount < hydrationStrings.GetLength(0); hycount++)
            {
                for (int temcount = 0; temcount < temperatureStrings.GetLength(0); temcount++)
                {
                    String name = temperatureStrings[temcount] + "-" + hydrationStrings[hycount];
                    names.Add(name);

                }

            }

            String[] arrayNames = names.ToArray();



            FileUtil.AlphabetizeWriteListFile("\\debug\\worldGeneration\\namesClimate.txt", arrayNames);

            foreach (String name in arrayNames) {
                Console.WriteLine("case #" + name + "#:");
                Console.WriteLine("     ");
                Console.WriteLine("     break;     ");
            }
            Console.WriteLine("     ");
            Console.WriteLine("     ");
            Console.WriteLine(names[0].Split("-")[0]);
        }
        public static void generateListNames()
        {
            String[] terraintypeString = { "OPENWATER", "LAND", "HIGHLAND", "MOUNTAIN" };
            String[] temperatureString = { "FROZEN", "COLD", "TEMPERATE", "WARM", "HOT" };
            String[] hydrationStrings = { "ARID", "DRY", "HYDRATED", "WET", "VERYWET", "LAKE", "OCEANIC" };


            String[] pngNames = new string[200];

            for (int counter = 0; counter < pngNames.Length; counter++)
            {
                String insert = "";
                for (int hynum = 0; hynum < hydrationStrings.GetLength(0); hynum++)
                {
                    for (int temnum = 0; temnum < terraintypeString.GetLength(0); temnum++)
                    {
                        for (int tnum = 0; tnum < terraintypeString.GetLength(0); tnum++)
                        {
                            if (terraintypeString[tnum] != "OPENWATER" && hydrationStrings[hynum] == "LAKE" || hydrationStrings[hynum] == "OCEANIC")
                            {
                                break;
                            }
                            else
                            {
                                insert += terraintypeString[tnum];
                                insert += "_" + temperatureString[temnum];
                                insert += "_" + hydrationStrings[hynum] + ".png";
                                Console.WriteLine(insert);

                                pngNames[counter] = insert;
                                insert = "";
                            }

                        }

                    }
                }
                Console.WriteLine();
            }

            Array.Sort(pngNames);
            foreach (String name in pngNames)
            {
                //Console.WriteLine(name);
            }
            FileUtil.AlphabetizeWriteListFile("\\debug\\worldGeneration\\namesToDo.txt", pngNames);
        }

        public static void generateListRivers()
        {
            for (int i = 0; i < 25; i++)
            {
                Console.WriteLine("else if (riversMap[i,j] == riverID_" + Convert.ToString(i,2) + ") {");
                Console.WriteLine("     Console.Write('');");
                Console.WriteLine("}");
            }
        }
        public static void test_generation() {

            //  0 is worldSize
            //  1 is seaLevel
            //  2 is mountain threshold
            //  3 is continent amounts
            //  4 is a placeholder
            //  5 is a placeholder
            //  6 is smoothness for temperature
            //  7 is centerstrength for temperature map
            //  8 is a placeholder
            //  9 is a placeholder
            //  10

            
            //  The map generated can be displayed in the folder \ProGenAAG Project\debug\worldGeneration\worlds\testMapTerrain_.png
            int[] testargs = { 128, 27, 40, 2, 0, 0, 200, 1, 0, 0, 0};
            WorldGenUtil.testWorldGeneration(testargs);


        }
         


    }
}