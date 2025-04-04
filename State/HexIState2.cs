using System;
using UnityEngine;
using UnityEngine.Events;

public class HexIState2 : AHexIState
{
    private GameObject effect;

    public void Click()
    {
        //foreach (var e in HexagonMain.Instance.Childs)
        //{
        //    if (e.State == HexagonItem.EState.Selection)
        //    {
        //        e.State = HexagonItem.EState.Hidden;
        //    }
        //}
        foreach (var e in Data.targets)
        {
            if (e.State == HexagonItem.EState.Hidden)
                e.State = HexagonItem.EState.Selection;
        }
        HexagonMain.Instance.Current = Data;
    }

    void OnClick()
    {

    }

    public override void OnStateOff()
    {
    }

    public override void OnStateOn()
    {
        HexagonMain.OnScored?.Invoke(Data.score);
        GenEffect(Data.score, transform.position);
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.one;
        HexagonMain.Instance.Current = Data;
    }

    public override void Reset()
    {
        effect?.gameObject.SetActive(false);
    }

    private void GenEffect(int score, Vector3 position)
    {
        if (effect == null)
        {
            var prefab = Resources.Load<GameObject>($"Effect/effect{score}");
            if (prefab == null)
                return;
            effect = Instantiate(prefab, transform);
            var direct = (position - HexagonMain.Instance.Current.transform.position).normalized;
            effect.transform.position = position + direct * 0.2f;
        }
        Vector3 direction = HexagonMain.Instance.Current.transform.position - effect.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        effect.transform.rotation = Quaternion.Euler(0, 0, angle);
        effect?.gameObject.SetActive(true);
    }
}
