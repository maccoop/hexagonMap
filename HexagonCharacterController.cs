using DG.Tweening;
using DG.Tweening.Core;
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
    private TweenerCore<Vector3, Vector3, VectorOptions> tweenMove;

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
        tweenMove.Kill();
        tweenMove = transform.DOMove(worldPosition, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
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
