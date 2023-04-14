using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;

public class AnchorIMEXManager : MonoBehaviour
{
    private string folderName = "AnchorData";
    private string fileName = "SavedAnchorData";
    private string fileType = ".txt";

    private string directoryPath;
    private string textFilePath;

    public TextMeshPro OutputTextPanel;

    //void Start()
    //{
    //    //directoryPath = Application.streamingAssetsPath + "/" + folderName + "/";
    //    //textFilePath = Application.streamingAssetsPath + "/" + folderName + "/" + fileName + fileType;
    //    
    //}

    public void ExportFile()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        directoryPath = Application.persistentDataPath + "/" + folderName + "/";
        textFilePath = Application.persistentDataPath + "/" + folderName + "/" + fileName + fileType;


        CreateFolderInDirectory();
        CreateFileInDirectory();

        

        //ReadFromFile(textFilePath);
    }

    private void CreateFolderInDirectory()
    {
        
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            OutputTextPanel.text = "Directory created.";
        }

    }
    private void CreateFileInDirectory()
    {
        //Create the text file at the already created directory
        if (!File.Exists(textFilePath))
        {
            File.WriteAllText(textFilePath, "Saved Achor data \n \n");
            OutputTextPanel.text += " File exported.";
        }

    }

    private void ReadFromFile(string pathToFile)
    {
        List<string> fileLines = File.ReadAllLines(pathToFile).ToList();
        
        foreach (string line in fileLines)
            Debug.Log(line);
    }
}
