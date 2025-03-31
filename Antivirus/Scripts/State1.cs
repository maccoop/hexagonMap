using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class State1 : MonoBehaviour
{
    public string input;
    public LineRenderer renderer;
    public Vector3[] points;
    float l = 1f;
    int max = 5;
    void Start()
    {
        GenMap();
    }

    public void GenMap()
    {
        var length = input.Length;
        points = new Vector3[length];
        points[0] = Vector3.right * -max;

        for (int i = 0; i < input.Length; i++)
        {
            points[i + 1] = dics[input[i]] + Vector3.right * i * l;
        }
        renderer.positionCount = points.Length;
        renderer.SetPositions(points);
    }

    private Dictionary<char, Vector3> dics = new()
    {
        { '1', Vector3.up },
        { '0', Vector3.down },
        { ' ', Vector3.zero }
    };
}

[CustomEditor(typeof(State1))]
public class State1Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var scr = (State1)target;
        if (GUILayout.Button("GenMap"))
        {
            scr.GenMap();
        }
    }
}
