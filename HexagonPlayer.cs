using UnityEngine;

public class HexagonPlayer : MonoBehaviour
{
    private HexagonItem itemScript;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "cell")
        {
            itemScript = other.GetComponent<HexagonItem>();
            foreach (var e in itemScript.targets)
            {
                if (itemScript.State == HexagonItem.EState.Hidden)
                {
                    itemScript.State = HexagonItem.EState.Selection;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "cell")
        {
            itemScript = other.GetComponent<HexagonItem>();
            foreach (var e in itemScript.targets)
            {
                if (itemScript.State == HexagonItem.EState.Selection)
                {
                    itemScript.State = HexagonItem.EState.Hidden;
                }
            }
        }
    }
}
