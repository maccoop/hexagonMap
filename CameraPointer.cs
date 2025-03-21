using UnityEngine;

public interface IPointer
{
    bool IsEnableClick { get; }
    void Click();
}

public class CameraPointer : MonoBehaviour
{
    Vector2 mousePosition;
    bool hasInput;
    RaycastHit2D hitcollider;
    IPointer pointer;

    void Update()
    {
        hasInput = Input.GetMouseButtonDown(0) || Input.touchCount > 0;
        if (hasInput)
        {
            if (Input.touchCount > 0)
            {
                mousePosition = Input.touches[0].deltaPosition;
            }
            else
            {
                mousePosition = Input.mousePosition;
            }
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            hitcollider = Physics2D.Raycast(mousePosition, transform.forward);
            if (hitcollider.collider != null)
            {
                pointer = hitcollider.collider.gameObject.GetComponent<IPointer>();
                if (pointer != null && pointer.IsEnableClick)
                {
                    pointer.Click();
                }
            }
        }

        hasInput = false;
    }
}
