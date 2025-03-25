using DG.Tweening;
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
    public int id;
    public int targetScore;
    public HexagonType _type;
    public GameObject hexPrefab;  // Prefab của ô hexagon
    public HexagonItem itemStart; // item bắt đầu game
    public int gridWidth = 5;     // Số ô theo chiều ngang
    public int gridHeight = 5;    // Số ô theo chiều dọc
    public float R = 1f;    // bán kính tính từ tâm đến đỉnh
    public List<HexagonItem> childs;

    public static bool IsEnd { get; internal set; }


    public void GenerateGridPointTop()
    {
        float hexWidth = Mathf.Sqrt(3) * R; // ≈ 1.732 * hexSize
        float hexHeight = 2f * R; // Hex cao hơn rộng

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
        float hexWidth = R * 2f;
        float hexHeight = Mathf.Sqrt(3) * R; // Cao hơn chiều rộng khi flat-top

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
