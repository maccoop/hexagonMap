using UnityEngine;

public class HexIState0 : MonoBehaviour, IHexIState
{
    public void OnStateOff()
    {
    }

    public void OnStateOn()
    {
        gameObject.SetActive(false);
    }

    public void Reset()
    {
    }
}
