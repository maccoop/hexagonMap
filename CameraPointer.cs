using UnityEngine;

public interface IPointer
{
    bool IsEnableClick { get; }
    void Click();
}

[System.Serializable]
public enum View
{
    E2D, E3D
}

public class CameraPointer : MonoBehaviour
{
    public View cameraView;
    Vector2 mousePosition;
    bool hasInput;
    Ray ray;
    RaycastHit hitcollider3D;
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
            if (cameraView == View.E2D)
            {
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
            else
            {
                ray = Camera.main.ScreenPointToRay(mousePosition);
                if(Physics.Raycast(ray, out hitcollider3D))
                {
                    pointer = hitcollider3D.collider.gameObject.GetComponent<IPointer>();
                    if (pointer != null && pointer.IsEnableClick)
                    {
                        pointer.Click();
                    }
                }
            }
        }

        hasInput = false;
    }
}
