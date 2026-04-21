using UnityEngine;

public class MirrorInteraction : MonoBehaviour
{
    private bool isDragging = false;
    private bool isRotating = false;
    private Vector3 offset;
    private const float RotateSpeed = 0.5f;
    private float lastMouseX;

    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.R))
        {
            isRotating = true;
            lastMouseX = Input.mousePosition.x;
        }
        else
        {
            isDragging = true;
            offset = transform.position - ScreenToWorld(Input.mousePosition);
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPos = ScreenToWorld(Input.mousePosition) + offset;
            newPos.z = 0;
            transform.position = newPos;
        }

        if (isRotating)
        {
            float delta = (Input.mousePosition.x - lastMouseX) * RotateSpeed;
            transform.Rotate(0, 0, -delta);
            lastMouseX = Input.mousePosition.x;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        isRotating = false;
    }

    private Vector3 ScreenToWorld(Vector3 screenPos)
    {
        screenPos.z = 0;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}