using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class AnchorIMEXManager : MonoBehaviour
{
    private string folderName = "AnchorData";
    private string fileName = "SavedAnchorData";
    private string fileType = ".txt";

    private string directoryPath;
    private string textFilePath;



    void Start()
    {
        directoryPath = Application.streamingAssetsPath + "/" + folderName + "/";
        textFilePath = Application.streamingAssetsPath + "/" + folderName + "/" + fileName + fileType;

        CreateFolderInDirectory();
        CreateFileInDirectory();

        ReadFromFile(textFilePath);
    }

    private void CreateFolderInDirectory()
    {
        
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
    }
    private void CreateFileInDirectory()
    {
        //Create the text file at the already created directory
        string textFilePath = Application.streamingAssetsPath + "/" + folderName + "/" + fileName + fileType;
        if (!File.Exists(textFilePath))
            File.WriteAllText(textFilePath, "Saved Achor data \n \n");

    }

    private void ReadFromFile(string pathToFile)
    {
        List<string> fileLines = File.ReadAllLines(pathToFile).ToList();
        
        foreach (string line in fileLines)
            Debug.Log(line);
    }
}
