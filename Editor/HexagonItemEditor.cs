
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexagonItem))]
public class HexagonItemEditor : Editor
{
    HexagonItem script => (HexagonItem)target;

    private void OnSceneGUI()
    {
        Handles.color = Color.green;
        foreach (var e in script.targets)
        {
            if (e != null)
            {
                Handles.DrawLine(script.transform.position, e.transform.position);
            }
        }
    }
}
