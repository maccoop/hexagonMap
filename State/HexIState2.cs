using UnityEngine;
using UnityEngine.Events;

public class HexIState2 : AHexIState
{
    public override void OnStateOff()
    {
    }

    public override void OnStateOn()
    {
        HexagonMapDrawing.OnScored?.Invoke(1);
        if(Data.targets.Length == 0)
        {
            HexagonMapDrawing.OnEnd?.Invoke();
        }
        foreach (var e in Data.targets)
        {
            if (e.State == HexagonItem.EState.Hidden)
                e.State = HexagonItem.EState.Selection;
        }
    }
}
