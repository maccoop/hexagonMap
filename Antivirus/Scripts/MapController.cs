using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Morse
{

    private static readonly Dictionary<char, string> MorseCodeMap = new Dictionary<char, string>
    {
        {'A', ".-"}, {'B', "-..."}, {'C', "-.-."}, {'D', "-.."}, {'E', "."},
        {'F', "..-."}, {'G', "--."}, {'H', "...."}, {'I', ".."}, {'J', ".---"},
        {'K', "-.-"}, {'L', ".-.."}, {'M', "--"}, {'N', "-."}, {'O', "---"},
        {'P', ".--."}, {'Q', "--.-"}, {'R', ".-."}, {'S', "..."}, {'T', "-"},
        {'U', "..-"}, {'V', "...-"}, {'W', ".--"}, {'X', "-..-"}, {'Y', "-.--"},
        {'Z', "--.."}, {'1', ".----"}, {'2', "..---"}, {'3', "...--"}, {'4', "....-"},
        {'5', "....."}, {'6', "-...."}, {'7', "--..."}, {'8', "---.."}, {'9', "----."},
        {'0', "-----"}, {' ', "/"}
    };

    public static char[] ToMorse(string text)
    {
        StringBuilder morseBinaryText = new StringBuilder();
        foreach (char c in text.ToUpper())
        {
            if (MorseCodeMap.ContainsKey(c))
            {
                morseBinaryText.Append(MorseCodeMap[c]);
            }
        }
        return morseBinaryText.ToString().ToCharArray();

    }
    public static char[] MorseToByte(string morse)
    {
        StringBuilder morseBinaryText = new StringBuilder();
        foreach (char c in morse)
        {
            morseBinaryText.Append(c == '-' ? '1' : c == '.' ? '0' : ' ');
        }
        return morseBinaryText.ToString().ToCharArray();

    }
}

public class MapController : MonoBehaviour
{
    public string Input;

    internal void EncodeMorse()
    {
        Input = new string(Morse.ToMorse(Input));
    }
    internal void EncodeByte()
    {
        Input = new string(Morse.MorseToByte(Input));
    }
}

[CustomEditor(typeof(MapController))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (MapController)target;
        if (GUILayout.Button("Encode Morse"))
        {
            script.EncodeMorse();
        }
        if (GUILayout.Button("Encode Byte"))
        {
            script.EncodeByte();
        }
    }
}
