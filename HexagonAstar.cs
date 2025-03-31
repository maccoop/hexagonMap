using System.Collections.Generic;
using UnityEngine;

public class HexagonAstar
{
    public struct Phase
    {
        public List<Vector3> vectors;
        public float distance;

        public Phase(List<Vector3> l, float d)
        {
            vectors = l;
            distance = d;
        }
    }
#if UNITY_EDITOR
    private static float R => 0.55f;
#else
    private static float R => HexagonMain.Instance.HexSize;
#endif
    private static List<Phase> cache = new List<Phase>();
    public static Vector3[] GetMovingPosition(List<GameObject> pointers, Vector3 begin, Vector3 target)
    {
        cache = new();
        List<Vector3> ignore = new();
        Vector3 current = begin;
        Phase currentPhase = new Phase();
        currentPhase.vectors = new List<Vector3>();
        currentPhase.vectors.Add(begin);
        currentPhase.distance = Vector3.Distance(begin, target);
        int count = 0;
        GameObject obj = null;
        Debug.Log("Begin a*: " + System.DateTime.Now.ToString("ss.fff"));
        while (currentPhase.distance >= R)
        {
            if (count > 1000)
                throw new System.Exception();
            var neighbor = GetNeigbor(current);
            foreach (var e in neighbor)
            {
                if (ignore.FindIndex(x => Vector3.Distance(x, e) < R) != -1)
                    continue;
                obj = pointers.Find(x => Vector3.Distance(x.transform.localPosition, e) <= R);
                if (obj == null || !obj.activeSelf || obj.GetComponent<HexagonItem>().State != HexagonItem.EState.Selected)
                    continue;
                List<Vector3> l = new List<Vector3>();
                l.AddRange(currentPhase.vectors);
                l.Add(e);
                cache.Add(new Phase()
                {
                    vectors = l,
                    distance = Vector3.Distance(e, target)
                });
            }
            Sort();
            currentPhase = cache[0];
            current = currentPhase.vectors[currentPhase.vectors.Count - 1];
            ignore.Add(current);
            cache.RemoveAt(0);
            count++;
        }
        Debug.Log("End a*: " + System.DateTime.Now.ToString("ss.fff"));
        return currentPhase.vectors.ToArray();
    }
    private static void Sort()
    {
        for (int i = 0; i < cache.Count - 1; i++)
        {
            for (int j = i + 1; j < cache.Count; j++)
            {
                if (cache[i].distance > cache[j].distance)
                {
                    var c = cache[i];
                    cache[i] = cache[j];
                    cache[j] = c;
                }
            }
        }
    }
    public static Vector3[] GetNeigbor(Vector3 point)
    {
        Vector3[] result = new Vector3[6];
        float S = R * Mathf.Sqrt(3) / 2f;
        var H = Mathf.Sqrt(3f) * S;
        result[0] = new Vector3(point.x + 2f * S, point.y, point.z); // phải
        result[1] = new Vector3(point.x - 2f * S, point.y, point.z); // trái
        result[2] = new Vector3(point.x + S, point.y + H, point.z); // trên phải
        result[3] = new Vector3(point.x - S, point.y + H, point.z); // trên trái
        result[4] = new Vector3(point.x + S, point.y - H, point.z); // dưới phải
        result[5] = new Vector3(point.x - S, point.y - H, point.z); // dưới trái
        return result;
    }
    public static List<Vector2Int> GetHexNeighbors(Vector2Int hex)
    {
        // Các hướng của lưới hex pointy-top
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(1, 0),   // Đông
        new Vector2Int(1, -1),  // Đông Bắc
        new Vector2Int(0, -1),  // Tây Bắc
        new Vector2Int(-1, 0),  // Tây
        new Vector2Int(-1, 1),  // Tây Nam
        new Vector2Int(0, 1)    // Đông Nam
        };

        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (var dir in directions)
        {
            neighbors.Add(hex + dir);
        }

        return neighbors;
    }
    public static Vector2 HexToWorld(Vector2Int hex, Vector2 origin, float S)
    {
        float x = origin.x + S * (Mathf.Sqrt(3) * hex.x + Mathf.Sqrt(3) / 2 * hex.y);
        float y = origin.y + S * (1.5f * hex.y);
        return new Vector2(x, y);
    }
}
