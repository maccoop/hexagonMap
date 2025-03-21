using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class HexagonItem : MonoBehaviour
{
    private IHexIState[] states;
    private EState _state;
    public HexagonItem[] targets;

    private void Awake()
    {
        List<IHexIState> result = new();
        for (int i = 0; i < (int)EState.Count; i++)
        {
            var component = gameObject.AddComponent(GetTypeByName("HexIState" + i));
            result.Add((IHexIState)component);
        }
        states = result.ToArray();
        State = EState.Hidden;
    }
    public void OnChangeState()
    {
        states[(int)_state].OnStateOn();
    }
    private static Type GetTypeByName(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(t => t.Name == typeName);
    }
    public EState State
    {
        get => _state;
        set
        {
            states[(int)_state].OnStateOff();
            _state = value;
            OnChangeState();
        }
    }
    [System.Serializable]
    public enum EState
    {
        Hidden, Selection, Selected, 
        Count
    }
}

[CustomEditor(typeof(HexagonItem))]
public class HexagonItemEditor : Editor
{
    HexagonItem script => (HexagonItem)target;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }

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

public abstract class AHexIState : MonoBehaviour, IHexIState
{
    private HexagonItem _data;
    public HexagonItem Data
    {
        get
        {
            if (_data == null) _data = GetComponent<HexagonItem>();
            return _data;
        }
    }
    public abstract void OnStateOff();
    public abstract void OnStateOn();
}

public interface IHexIState
{
    public void OnStateOn();
    public void OnStateOff();
}
