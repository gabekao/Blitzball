using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;

public class SerializationManager 
{
    // Creates a save file of the passed name with provided data
    public static bool Save(string name, string directory, object saveData)
    {
        // Obtain formatter
        BinaryFormatter formatter = GetBinaryFormatter();

        // Check to see if saves directory exists, create if it doesn't exist
        if (!Directory.Exists(Application.persistentDataPath + directory))
        {
            Directory.CreateDirectory(Application.persistentDataPath + directory);
        }

        // Concatenate the name of the file to the path
        string path = Application.persistentDataPath + directory + name + ".data";

        // Create file from path
        FileStream fs = File.Create(path);

        // Serialize save data
        formatter.Serialize(fs, saveData);

        // Close file stream
        fs.Close();

        return true;
    }

    // Loads save file from provided path
    public static object Load(string path)
    {
        // Make sure file exists, if not return null
        if (!File.Exists(path))
        {
            Debug.Log("File does not exist: " + path);
            return null;
        }

        // Get binary formatter
        BinaryFormatter formatter = GetBinaryFormatter();

        // Open file
        FileStream fs = File.Open(path, FileMode.Open);

        // Try to open, if unable to open, print error before closing file stream
        try
        {
            // Deserialize file into an object and close file stream
            object save = formatter.Deserialize(fs);
            fs.Close();

            // Return save data
            return save;
        }
        catch
        {
            // Print debug error and close file stream
            Debug.LogErrorFormat("Fail to load file at {0}", path);
            fs.Close();

            // Return null
            return null;
        }
    }

    // Setup our binary formatter
    public static BinaryFormatter GetBinaryFormatter()
    {
        // Use base binary formatter
        BinaryFormatter formatter = new BinaryFormatter();

        // Create a surrogate selector
        SurrogateSelector selector = new SurrogateSelector();

        // Create surrogates for vector3 and quaternions
        Vector3Surrogate vector3Surrogate = new Vector3Surrogate();
        QuaternionSurrogate quaternionSurrogate = new QuaternionSurrogate();

        // Attach surrogates to selector
        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);

        // Attach selector to our formatter
        formatter.SurrogateSelector = selector;

        // Return formatter
        return formatter;
    }

    public static void DeleteFile(string name)
    {
        string file = Application.persistentDataPath + "/saves/" + name + ".data";
        File.Delete(file);
    }
}
