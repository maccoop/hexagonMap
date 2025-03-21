using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class HexagonMapDrawing : MonoBehaviour
{
    public static UnityAction<int> OnScored;
    public static UnityAction OnEnd;
    public int targetScore = 0;
    public TMPro.TMP_Text detail;
    public int id;
    public HexagonType _type;
    public GameObject hexPrefab;  // Prefab của ô hexagon
    public HexagonItem itemStart; // item bắt đầu game
    public int gridWidth = 5;     // Số ô theo chiều ngang
    public int gridHeight = 5;    // Số ô theo chiều dọc
    public float hexSize = 1f;    // Kích thước ô hex
    private int crrscore;
    public List<HexagonItem> childs;

    private void Start()
    {
        itemStart.State = HexagonItem.EState.Selection;
        OnScored += OnEventScored;
        OnEnd += OnStageEnd;
        detail.text = $"{crrscore}/{targetScore}";
    }

    public void Reset()
    {
        foreach(var e in childs)
        {
            e.State = HexagonItem.EState.Hidden;
        }
        itemStart.State = HexagonItem.EState.Selection;
        crrscore = 0;
        detail.text = $"{crrscore}/{targetScore}";
    }

    private void OnStageEnd()
    {
        detail.text = crrscore >= targetScore ? "You Win": "You Lose";
    }

    private void OnEventScored(int value)
    {
        crrscore += value;
        detail.text = $"{crrscore}/{targetScore}";
    }

    internal void GenerateGridPointTop()
    {
        float hexWidth = Mathf.Sqrt(3) * hexSize; // ≈ 1.732 * hexSize
        float hexHeight = 2f * hexSize; // Hex cao hơn rộng

        for (int q = 0; q < gridWidth; q++)
        {
            for (int r = 0; r < gridHeight; r++)
            {
                float xPos = q * hexWidth; // Mỗi cột cách nhau hexWidth
                float yPos = r * (hexHeight * 0.75f); // Dịch hàng 0.75 khoảng cách

                // Nếu hàng r là số lẻ, dịch cột sang phải 1/2 hexWidth
                if (r % 2 == 1)
                {
                    xPos += hexWidth / 2f;
                }

                GameObject hex = Instantiate(hexPrefab, transform);
                hex.transform.localPosition = new Vector3(xPos, yPos, 0);
                hex.name = $"Hex {q},{r}";
            }
        }
    }

    public void GenerateGridFlatTop()
    {
        float hexWidth = hexSize * 2f;
        float hexHeight = Mathf.Sqrt(3) * hexSize; // Cao hơn chiều rộng khi flat-top

        for (int q = 0; q < gridWidth; q++)
        {
            for (int r = 0; r < gridHeight; r++)
            {
                // Dịch hàng r sang trái để có dạng tổ ong
                float xPos = q * hexWidth * 0.75f;
                float yPos = r * hexHeight + (q % 2 == 1 ? hexHeight / 2f : 0);

                GameObject hex = Instantiate(hexPrefab, transform);
                hex.transform.localPosition = new Vector3(xPos, yPos, 0);
                hex.name = $"Hex {q},{r}";
            }
        }
    }

    public void InitChilds()
    {
        List<HexagonItem> result = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            var component = transform.GetChild(i).GetComponent<HexagonItem>();
            if (component != null)
            {
                result.Add(component);
            }
        }
        this.childs = result;
    }
}

[System.Serializable]
public enum HexagonType
{
    FlatTop, PointTop
}

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
        //line += $"gridHeight={script.gridHeight}" + enter;
        for (int i = 0; i < script.childs.Count; i++)
        {
            line += $"[{i}]" + enter;
            line += $"position={script.childs[i].transform.position}" + enter;
            line += "target=";
            foreach (var e in script.childs[i].targets)
            {
                line += $"{script.childs.FindIndex(x => x.gameObject.GetInstanceID() == e.gameObject.GetInstanceID())},";
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
