using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json.Converters;

namespace ProGenAAG_Project
{
    public class FileUtil {
        public static readonly string sourceDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        

        //  Converts a short to a valid filepath
        
        public static string ConvertToDir(string path) {

            return sourceDirectory + path;
        }
        
        //  Converts a valid filepath string to a shorter version
        public static string ConvertFromDir(string path)
        {
            // Check if filepath is already dynamic; if true, return path, if false, proceed
            try
            {
                if (!path.Substring(0, sourceDirectory.Length).Equals(sourceDirectory))
                {
                    Console.WriteLine("ERROR: Path does NOT begin with @C:..., returning path");
                    return path;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}: returning valid filepath ");
            }

            string result = path.Remove(0, sourceDirectory.Length);
            return result;
        }
        public static string ConvertToShortPath(string path)
        {
            if (!path.StartsWith(sourceDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return path; // Already a valid full path, return unchanged
            }

            string shortPath = path.Substring(sourceDirectory.Length).TrimStart(Path.DirectorySeparatorChar);
            return string.IsNullOrEmpty(shortPath) ? path : shortPath; // Ensure valid return
        }




        //  Creates a directory at the specified filepath
        public static void CreateDirectory(string path)
        {
            string filepath = ConvertToDir(path);
            try
            {
                //  Test if Directory Exists
                if (Directory.Exists(filepath))
                {
                    Console.WriteLine("Directory " + path + " Already Exists");
                    return;
                }
                //  Create Directory
                DirectoryInfo di = Directory.CreateDirectory(filepath);
                Console.WriteLine("The directory " + path + " was created successfully at {0}.", Directory.GetCreationTime(filepath));
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }


        //  Given a path to a .JSON file, this returns an object of specified objectType at the JSON
        public static ObjType LoadFromJSON<ObjType>(string path) where ObjType : class {
            string filepath = ConvertToDir(path);
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException("The specified JSON file was not found.");
            }
            string jsonContent = File.ReadAllText(filepath);

            return JsonConvert.DeserializeObject<ObjType>(jsonContent);
        }


        //  Given an object and filepath, this saves an object as a .JSON file
        public static void SaveToJSON<ObjType>(ObjType obj, string path) where ObjType : class
        {
            string filepath = ConvertToDir(path);
            string jsonContent = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(filepath, jsonContent);
        }


        //  Given any object, print the object to string in JSON Format
        public static void JObjectToString(object complexObject)
        {
            var jsonString = JsonConvert.SerializeObject(complexObject, Formatting.Indented, new JsonConverter[] { new StringEnumConverter() });
            Console.WriteLine("");
            Console.WriteLine(jsonString);
            Console.WriteLine("");
        }


        //  Read all lines of a .txt file, returns an array of Strings
        public static string[] ReadAllLines(string path)
        {
            string filepath = ConvertToDir(path);
            string[] strings = File.ReadAllLines(filepath);
            return strings;
        }


        //  Creates a list .txt file from specified array at specified location
        public static void WriteAllLines(string path, string[] contents) {
            string filepath = ConvertToDir(path);
            File.WriteAllLines(filepath, contents);
        }


        //  Creates an alphabetized list .txt file from specified array at specified location
        public static void AlphabetizeWriteListFile(string path, string[] contents)
        {
            Array.Sort(contents);
            string[] strings = contents.Distinct().ToArray();
            WriteAllLines(path, strings);
        }


        //  Given a .text containing a list of strings, print the strings with a label of index
        public static void StringFileToString(string path)
        {
            string[] strings = FileUtil.ReadAllLines(path);

            for (int i = 0; i < strings.Length; i++)
            {
                Console.WriteLine("[" + i + "] " + strings[i]);

            }
        }
    }
}
