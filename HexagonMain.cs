using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexagonMain : MonoBehaviour
{
    public static bool IsEnd { get; internal set; }
    public static UnityAction<int> OnScored;
    public static UnityAction OnEnd;

    public GameObject prefab;
    public TMPro.TMP_Text detail;

    int idMap;
    private HexagonItem itemStart;
    private int crrscore;
    private int targetScore = 0;
    private List<HexagonItem> childs;
    private INIParser data;
    private int childCount;
    private int idStart;

    private void Start()
    {
        GenMap();
        Reset();
    }

    private void OnStageEnd()
    {
        IsEnd = true;
        bool win = crrscore >= targetScore;
        detail.text = win ? "You Win" : "You Lose";
        if (win)
        {
            PlayerPrefs.SetInt("id", idMap++);
            transform.DOScaleZ(2, 0.5f).OnComplete(() =>
            {
                if (win)
                {
                    foreach (var e in childs)
                    {
                        if (e.State != HexagonItem.EState.Selected)
                            e.State = HexagonItem.EState.Selected;
                    }
                }
            });
        }
    }

    private void OnEventScored(int value)
    {
        crrscore += value;
        detail.text = $"{crrscore}/{targetScore}";
    }

    public void Reset()
    {
        itemStart.State = HexagonItem.EState.Selection;
        crrscore = 0;
        detail.text = $"{crrscore}/{targetScore}";
        IsEnd = false;
        foreach (var e in childs)
        {
            e.State = HexagonItem.EState.Hidden;
        }
        itemStart.State = HexagonItem.EState.Selection;
        OnScored += OnEventScored;
        OnEnd += OnStageEnd;
    }

    private void GenMap()
    {
        idMap = PlayerPrefs.GetInt("id", 0);
        var asset = Resources.Load<TextAsset>($"MapData/map_{idMap}");
        data = new INIParser();
        data.Open(asset);
        childCount = data.ReadValue("MAP", "childCount", 0);
        targetScore = data.ReadValue("MAP", "targetScore", 0);
        idStart = data.ReadValue("MAP", "itemStart", 0);
        if (childCount == 0)
        {
            throw new NullReferenceException("MAP/childCount");
        }
        for (int i = 0; i < childCount; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.transform.position = data.ReadValue(i.ToString(), "position", Vector3.zero);
        }
    }
}
