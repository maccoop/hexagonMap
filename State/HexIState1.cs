using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class HexIState1 : AHexIState, IPointer
{
    static UnityAction OnSelected;
    private TweenerCore<Vector3, Vector3, VectorOptions> anim;

    public bool IsEnableClick => Data.State == HexagonItem.EState.Selection;

    public override void OnStateOff()
    {
        gameObject.transform.localScale = Vector3.one;
        anim.Kill();
    }

    public override void OnStateOn()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        anim?.Kill();
        transform.DOScale(1, 0.5f).SetEase(Ease.InOutSine).SetDelay(0.2f).OnComplete(() =>
        {
            anim = transform.DOScale(0.9f, 0.4f).SetLoops(-1, LoopType.Yoyo);
        });
        OnSelected += OnOtherSelect;
    }

    private void OnOtherSelect()
    {
        OnSelected -= OnOtherSelect;
        Data.State = HexagonItem.EState.Hidden;
        anim = transform.DOScale(0, 0.4f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
        });
    }

    public void Click()
    {
        OnSelected -= OnOtherSelect;
        OnSelected?.Invoke();
        Data.State = HexagonItem.EState.Selected;
    }
}
