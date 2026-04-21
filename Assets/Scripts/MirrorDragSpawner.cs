using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attach this to each mirror icon/button in your UI panel
public class MirrorDragSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject mirrorPrefab;     // Assign your mirror prefab in Inspector
    public Canvas canvas;               // Assign your main Canvas

    private GameObject dragPreview;     // A UI image ghost while dragging
    private GameObject spawnedMirror;   // The actual world mirror, created on drop

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Create a ghost UI image that follows the cursor while dragging
        dragPreview = new GameObject("DragPreview");
        dragPreview.transform.SetParent(canvas.transform, false);

        Image img = dragPreview.AddComponent<Image>();
        img.sprite = GetComponent<Image>().sprite;
        img.SetNativeSize();
        img.raycastTarget = false;

        RectTransform rt = dragPreview.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragPreview == null) return;

        // Move the ghost image with the cursor
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint
        );
        dragPreview.GetComponent<RectTransform>().localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(dragPreview);

        // Only spawn if dropped outside the UI panel
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 worldPos = ScreenToWorldPoint(eventData.position);
        spawnedMirror = Instantiate(mirrorPrefab, worldPos, Quaternion.identity);
    }

    private Vector3 ScreenToWorldPoint(Vector2 screenPos)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        pos.z = 0;
        return pos;
    }
}