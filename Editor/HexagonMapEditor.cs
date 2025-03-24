
using System.IO;
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
        if (GUILayout.Button("Show Event Child"))
        {
            SceneView.duringSceneGui += OnSceneGUI;
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

    private void SaveToFile()
    {
        string enter = "\n";
        string line = $"[MAP]" + enter;
        line += $"childCount={script.childs.Count}" + enter;
        line += $"targetScore={script.targetScore}" + enter;
        line += $"itemStart={GetIndex(script.itemStart)}" + enter;

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
        return script.childs.FindIndex(x => x.gameObject.GetInstanceID() == e.gameObject.GetInstanceID());
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Handles.color = Color.red;

        // Duyệt qua tất cả object trong Scene
        foreach (var obj in script.childs)
        {
            Vector3 center = obj.transform.position;
            Vector3 normal = Vector3.up;
            Vector3 from = obj.transform.right;
            Handles.DrawSolidArc(center, normal, from, 360, 0.2f);
        }
    }
}
