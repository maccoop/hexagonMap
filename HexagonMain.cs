using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexagonMain : MonoBehaviour
{
    const string SAVE_IDMAP = "ID_MAP";
    private const string SCORE = "score";
    private const string POSITION = "position";
    private const string TARGET = "target";
    private const int CHUNK_SIZE = 5;
    private Dictionary<Vector2Int, List<Vector2Int>> chunks = new Dictionary<Vector2Int, List<Vector2Int>>();

    public static HexagonMain Instance { get; set; }
    public HexagonItem Current
    {
        get
        {
            return this.current;
        }
        set
        {
            this.current = value;
            OnChangePosition();
        }
    }
    private int currentGenPoint = 10;
    public List<HexagonItem> Childs => childs;
    public List<GameObject> objChild;
    public float HexSize => hexSize;
    public static bool IsEnd { get; internal set; }
    public static UnityAction<int> OnScored;
    public static UnityAction OnEnd;

    public GameObject prefab;
    public HexagonCharacterController characterPrefab;
    public TMPro.TMP_Text detail;

    int idMap;
    private HexagonItem itemStart;
    private int crrscore;
    private int targetScore = 0;
    private List<HexagonItem> childs;
    private INIParser data;
    private int childCount;
    private int idStart;
    private HexagonCharacterController character;
    private HexagonItem current;
    private float hexSize;
    private int col, row;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OnScored += OnEventScored;
        OnEnd += OnStageEnd;
        GetData();
        GenerateChunks();
        GenMap(new Vector2Int(0, 0));
        Begin();
    }

    private void OnStageEnd()
    {
        IsEnd = true;
        bool win = crrscore >= targetScore;
        detail.text = win ? "You Win" : "You Lose";
        if (win)
        {
            PlayerPrefs.SetInt("id", idMap++);
            transform.DOScaleZ(1, 0.5f).OnComplete(() =>
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
        if (crrscore < 0)
        {
            OnStageEnd();
        }
    }


    private void Begin()
    {
        detail.text = $"{crrscore}/{targetScore}";
        IsEnd = false;
        foreach (var e in childs)
        {
            e.State = HexagonItem.EState.Hidden;
        }
        itemStart.State = HexagonItem.EState.Selected;
    }
    public void Reset()
    {
        detail.text = $"{crrscore}/{targetScore}";
        IsEnd = false;
        foreach (var e in childs)
        {
            e.State = HexagonItem.EState.Hidden;
        }
        itemStart.State = HexagonItem.EState.Selected;
        crrscore = 0;
        if (character != null)
            Destroy(character.gameObject);
        character = null;
    }

    public void OnChangePosition()
    {
        if (character == null)
        {
            character = Instantiate(characterPrefab, null);
            character.SetState(HexagonCharacterController.State.Born);
            character.transform.position = current.transform.position;
        }
        else
        {
            character.SetState(HexagonCharacterController.State.Move);
            character.SetTarget(current.transform.position);
        }
    }

    public Vector3 GetLocalPosition(Vector3 world)
    {
        return transform.InverseTransformPoint(world);
    }

    public Vector3 GetWorldPosition(Vector3 local)
    {
        return transform.TransformPoint(local);
    }

    private void GetData()
    {
        idMap = PlayerPrefs.GetInt(SAVE_IDMAP, 0);
        var asset = Resources.Load<TextAsset>($"MapData/map_{idMap}");
        data = new INIParser();
        data.Open(asset);
        childCount = data.ReadValue("MAP", "childCount", 0);
        targetScore = data.ReadValue("MAP", "targetScore", 0);
        idStart = data.ReadValue("MAP", "itemStart", 0);
        hexSize = data.ReadValue("MAP", "hexSize", 0f);
        col = data.ReadValue("MAP", "col", 5);
        row = data.ReadValue("MAP", "row", 5);
    }

    void GenerateChunks()
    {
        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x / CHUNK_SIZE, y / CHUNK_SIZE);
                Vector2Int cellPos = new Vector2Int(x, y);

                if (!chunks.ContainsKey(chunkPos))
                {
                    chunks[chunkPos] = new List<Vector2Int>();
                }

                chunks[chunkPos].Add(cellPos);
            }
        }
    }


    private void GenMap(Vector2Int pos)
    {
        if (childCount == 0)
        {
            throw new NullReferenceException($"id map {idMap}");
        }
        childs = new List<HexagonItem>();
        objChild = new List<GameObject>();
        for (int i = 0; i < childCount; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.transform.position = data.ReadValue(i.ToString(), POSITION, Vector3.zero);
            var script = obj.GetComponent<HexagonItem>();
            script.State = HexagonItem.EState.Hidden;
            script.score = data.ReadValue(i.ToString(), SCORE, 0);
            childs.Add(script);
            objChild.Add(obj);
        }

        List<HexagonItem> result = new();
        int cid = 0;
        for (int i = 0; i < childs.Count; i++)
        {
            result = new();
            string[] ids = data.ReadValue(i.ToString(), TARGET, "").Split(",");
            if (ids.Length == 0)
                continue;
            foreach (var id in ids)
            {
                if (id != null && id.Length != 0)
                {
                    cid = int.Parse(id);
                    if (cid == -1)
                        continue;
                    result.Add(childs[cid]);
                }
            }
            childs[i].targets = result.ToArray();
            itemStart = childs[idStart];
        }
    }
}
