using UnityEngine;

// Attach to mirror prefab alongside MirrorInteraction
public class MirrorDeletable : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            Destroy(gameObject);
        }
    }
}