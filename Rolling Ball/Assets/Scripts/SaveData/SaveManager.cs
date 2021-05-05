using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveManager: MonoBehaviour
{
    public string[] saveFiles;      // File names
    public string directory;        // File directory

    private void Start()
    {
        // Set directory
        directory = "/" + SceneManager.GetActiveScene().name + "/";

        // Sort files if need be
        SortFiles();
    }

    // Obtains data files and sets their path to saveFiles
    public void GetLoadFiles(string directory)
    {
        // Check if passed directory exists, create if it doesn't
        if (!Directory.Exists(Application.persistentDataPath + directory))
        {
            Directory.CreateDirectory(Application.persistentDataPath + directory);
        }

        // Get all file names from the directory
        saveFiles = Directory.GetFiles(Application.persistentDataPath + directory);
    }

    // Sorts files using selection sort
    public void SortFiles()
    {
        // Get all load files
        GetLoadFiles(directory);
        
        // Amount of files
        int size = saveFiles.Length;

        // Create SaveData list to sort objects
        List<SaveData> s = new List<SaveData>();

        // Load each file into Save Data objects
        for (int i = 0; i < size; i++)
        {
            // Adding data
            s.Add((SaveData)SerializationManager.Load(saveFiles[i]));
        }

        // Selection sort algorithm
        for (int i = 0; i < size; i++)
        {
            int min = i;
            for (int j = i + 1; j < size; j++)
            {
                if (s[j].time < s[min].time)
                    min = j;
            }

            SaveData tmp = s[min];
            s[min] = s[i];
            s[i] = tmp;
        }

        // Delete all files
        DeleteAllFiles();

        // Save all files with new rank
        for (int i = 0; i < size; i++)
        {
            s[i].rank = i + 1;
            SerializationManager.Save(s[i].rank.ToString(), directory, s[i]);
        }
    }

    // Deletes all files
    public void DeleteAllFiles()
    {
        // For each file in saveFiles delete said file
        foreach (string file in saveFiles)
        {
            File.Delete(file);
        }
    }
}
