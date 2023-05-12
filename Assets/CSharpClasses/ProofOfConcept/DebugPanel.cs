using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public TextMeshPro text;

    public enum Colors
    {
        RED,
        ORANGE,
        GREEN,
        BLUE,
        WHITE,
        BLACK
    }

    public void PrintDebug(string inputText, Colors textColor = Colors.WHITE)
    {
        string color = "";
        
        switch (textColor)
        {
            case Colors.RED:
                color = "red";
                break;
            case Colors.ORANGE:
                color = "#ce490e";
                break;
            case Colors.GREEN:
                color = "#00FF00";
                break;
            case Colors.BLUE:
                color = "#0000FF";
                break;
            case Colors.WHITE:
                color = "#111111";
                break;
            case Colors.BLACK:
                color = "#000000";
                break;
        }

        text.text = $"<color={color}>{inputText}</color>";
    }
}
