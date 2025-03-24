using System;
using UnityEngine;
using UnityEngine.Events;

public class HexIState2 : AHexIState
{
    public override void OnStateOff()
    {
    }

    public override void OnStateOn()
    {
        HexagonMain.OnScored?.Invoke(Data.score);
        GenEffect(Data.score, transform.position);
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.one;
        if (HexagonMain.IsEnd)
        {

            return;
        }
        if(Data.targets.Length == 0)
        {
            HexagonMain.OnEnd?.Invoke();
        }
        foreach (var e in Data.targets)
        {
            if (e.State == HexagonItem.EState.Hidden)
                e.State = HexagonItem.EState.Selection;
        }
    }

    private void GenEffect(int score, Vector3 position)
    {
        var prefab = Resources.Load<GameObject>($"Effect/eff{score}");
        var obj = Instantiate(prefab, transform);
        obj.transform.position = position;
    }
}
