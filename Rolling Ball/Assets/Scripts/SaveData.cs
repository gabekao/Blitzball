using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveData
{
    public static void savePosotion (PlayerController player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/ghost.data";
        FileStream stream = new FileStream(path, FileMode.Create);
        GhostData data = new GhostData(player); 

        formatter.Serialize(stream, data);
        stream.Close();   
    }
    public static string LoadPlayer ()
    {
        string path = Application.persistentDataPath + "/ghost.data";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string data = formatter.Deserialize(stream) as string;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("File not found!");
            return null;
        }
    }
}
