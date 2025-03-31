using System;
using System.Collections.Generic;
using UnityEngine;

public class LazyMapLoad : MonoBehaviour
{
    public GameObject Character;
    public Sprite sprite;
    public int Width { get; set; }
    public int Height { get; set; }
    public float CellSize { get; set; }
    public int Range { get; set; }
    public int OutRange { get; set; }
    public Vector2 Pivot { get; set; }
    public string IDFormat { set; get; }



    private Vector2Int _crrLocation;
    private Vector2Int _lastLocation;
    private Dictionary<Vector2Int, GameObject> localtionDics;
    private List<Vector2Int> NeiborLocation;


    private void Awake()
    {
        Width = 100;
        Height = 100;
        CellSize = 5;
        Pivot = Vector2.zero;
        IDFormat = "{0}_{1}";
        Range = 2;
        OutRange = Range + 1;
        localtionDics = new Dictionary<Vector2Int, GameObject>();
        _lastLocation = new Vector2Int(-1, -1);
        NeiborLocation = new List<Vector2Int>();
        for (int i = -Range; i < Range + 1; i++)
        {
            for (int j = -Range; j < Range + 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                NeiborLocation.Add(new Vector2Int(i, j));
            }
        }
    }

    private void Update()
    {
        if (!ValidateValue()) return;
        _crrLocation = GetLocation(Character.transform.position);
        if (_crrLocation != _lastLocation)
        {
            if (_crrLocation.x < 0)
            {
                Character.transform.position = new Vector3(0, Character.transform.position.y);
            }
            else if (_crrLocation.y < 0)
            {
                Character.transform.position = new Vector3(Character.transform.position.x, 0);
            }
            else
            {
                _lastLocation = _crrLocation;
                OnSwitchIdMap(_crrLocation);
            }
        }
    }

    private void OnSwitchIdMap(Vector2Int location)
    {
        List<Vector2Int> neibors = new List<Vector2Int>();
        neibors.Add(location);
        for (int i = 0; i < NeiborLocation.Count; i++)
        {
            neibors.Add(location + NeiborLocation[i]);
        }
        foreach (var e in localtionDics)
        {
            if (Vector2Int.Distance(e.Key, location) >= OutRange)
            {
                Destroy(e.Value);
            }
            //if (!neibors.Contains(e.Key))
            //{
            //    e.Value.SetActive(false);
            //}
        }
        foreach (var neibor in neibors)
        {
            if (localtionDics.ContainsKey(neibor))
            {
                if (localtionDics[neibor] == null)
                {
                    localtionDics[neibor] = GenMap(neibor);
                }
                localtionDics[neibor].gameObject.SetActive(true);
            }
            else
            {
                localtionDics.Add(neibor, GenMap(neibor));
            }
        }
    }

    private GameObject GenMap(Vector2Int location)
    {
        string idmap = GetMapID(location);
        var prefab = Resources.Load<GameObject>("map/" + idmap);
        if (prefab == null)
        {
            var obj = new GameObject(idmap);
            var render = obj.AddComponent<SpriteRenderer>();
            render.sprite = sprite;
            obj.transform.parent = transform;
            obj.transform.localPosition = GetPosition(location);
            return obj;
        }
        else
        {
            var obj = Instantiate(prefab, transform);
            obj.gameObject.name = idmap;
            obj.transform.localPosition = GetPosition(location);
            return obj;

        }
    }

    private bool ValidateValue()
    {
        if (Width <= 0 || Height <= 0 || CellSize <= 0 || Range <= 0 || IDFormat.Length == 0)
        {
            Debug.LogWarning("Please check properties!");
            return false;
        }
        return true;
    }

    private Vector2Int GetLocation(Vector3 position)
    {
        Vector2Int result = new Vector2Int();
        result.x = Mathf.FloorToInt((position.x - Pivot.x) / CellSize);
        result.y = Mathf.FloorToInt((position.y - Pivot.y) / CellSize);
        return result;
    }

    private Vector2 GetPosition(Vector2Int location)
    {
        Vector2 result = new Vector2();
        result.x = location.x * CellSize + Pivot.x;
        result.y = location.y * CellSize + Pivot.y;
        return result;
    }

    private string GetMapID(Vector2Int location)
    {
        return string.Format(IDFormat, location.x, location.y);
    }
}
