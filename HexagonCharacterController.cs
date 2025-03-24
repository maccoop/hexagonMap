using DG.Tweening;
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
                    Invoke("SetStateIdle", 1.5f);
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
        transform.DOMove(worldPosition, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
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
