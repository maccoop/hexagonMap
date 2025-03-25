using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;

public class HexagonCharacterController : MonoBehaviour
{
    [System.Serializable]
    public enum State
    {
        Born, Idle, Move, React, Die
    }

    public Animator animator;
    public string[] clips;
    private State _state;

    private void Start()
    {
        HexagonMain.OnScored += OnScored;
    }

    bool isHurt = false;
    private TweenerCore<Vector3, Path, PathOptions> tweenMove;

    private void OnScored(int score)
    {
        isHurt = score < 0;
    }

    public void SetState(State state)
    {
        _state = state;
        animator.Play(clips[(int)_state]);
        switch (_state)
        {
            case State.Born:
            case State.React:
                {
                    CancelInvoke();
                    Invoke("SetStateIdle", 1.5f);
                    break;
                }
            default:
                {
                    CancelInvoke();
                    break;
                }
        }
    }

    private void SetStateIdle()
    {
        if (_state != State.Die)
            SetState(State.Idle);
    }

    public void SetTarget(Vector3 worldPosition)
    {
        var target = HexagonMain.Instance.GetLocalPosition(worldPosition);
        var begin = HexagonMain.Instance.GetLocalPosition(transform.position);
        var localpath = HexagonAstar.GetMovingPosition(HexagonMain.Instance.objChild, begin, target);
        var worldPoint = new Vector3[localpath.Length];
        for(int i = 0; i< worldPoint.Length; i++)
        {
            worldPoint[i] = HexagonMain.Instance.GetWorldPosition(localpath[i]);
        }
        tweenMove.Kill();
        tweenMove = transform.DOPath(worldPoint, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.position = worldPosition;
            if (isHurt)
            {
                SetState(State.React);
                Invoke("SetStateIdle", animator.GetCurrentAnimatorStateInfo(0).length);
            }
            else
                SetState(State.Idle);
        });
        Vector3 direction = worldPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
