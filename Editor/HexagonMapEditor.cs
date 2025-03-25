
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexagonMapDrawing))]
public class HexagonMapEditor : Editor
{
    HexagonMapDrawing script => (HexagonMapDrawing)target;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Grid"))
        {
            switch (script._type)
            {
                case HexagonType.FlatTop:
                    {
                        script.GenerateGridFlatTop();
                        break;
                    }
                case HexagonType.PointTop:
                    {
                        script.GenerateGridPointTop();
                        break;
                    }
            }
        }
        if (GUILayout.Button("Get Child"))
        {
            script.InitChilds();
        }
        if (GUILayout.Button("Gen Child Target"))
        {
            GenChildTarget();
        }
        if (GUILayout.Button("Save To File"))
        {
            SaveToFile();
        }
        if (GUILayout.Button("Reset Child"))
        {
            for (int i = 0; i < script.transform.childCount; i++)
            {
                DestroyImmediate(script.transform.GetChild(i).gameObject);
            }
        }
    }

    private void GenChildTarget()
    {
        float R = script.R;
        foreach (var item in script.childs)
        {
            var neighbor = HexagonAstar.GetNeigbor(item.transform.position);
            List<HexagonItem> cache = new();
            foreach (var e in neighbor)
            {
                var obj = script.childs.Find(x => Vector3.Distance(x.transform.localPosition, e) <= R);
                if (obj != null)
                {
                    cache.Add(obj);
                }
            }
            item.targets = cache.ToArray();
            item.gameObject.SetActive(false);
        }
    }

    private void SaveToFile()
    {
        string enter = "\n";
        string line = $"[MAP]" + enter;
        line += $"childCount={script.childs.Count}" + enter;
        line += $"targetScore={script.targetScore}" + enter;
        line += $"itemStart={GetIndex(script.itemStart)}" + enter;
        line += $"hexSize={script.R.ToString()}" + enter;

        //line += $"gridHeight={script.gridHeight}" + enter;
        for (int i = 0; i < script.childs.Count; i++)
        {
            line += $"[{i}]" + enter;
            line += $"position={script.childs[i].transform.position}" + enter;
            line += $"score={script.childs[i].score}" + enter;
            line += "target=";
            foreach (var e in script.childs[i].targets)
            {
                line += $"{GetIndex(e)},";
            }
            line += enter;
        }
        string folder = Path.Combine(Application.dataPath, "Resources/MapData");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        string filename = "map_" + script.id + ".txt";
        File.WriteAllText(Path.Combine(folder, filename), line);
        Debug.Log("Save Done");
    }

    private int GetIndex(HexagonItem e)
    {
        if (e == null)
            return -1;
        return script.childs.FindIndex(x => x.gameObject.GetInstanceID() == e.gameObject.GetInstanceID());
    }
}
