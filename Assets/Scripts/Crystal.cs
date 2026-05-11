using UnityEngine;

public class Crystal : MonoBehaviour
{
    public bool IsHitThisFrame { get; private set; }
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Reset every frame — laser must hit continuously to count
        IsHitThisFrame = false;
        sr.color = Color.Lerp(sr.color, new Color(0.5f, 0f, 0.5f), Time.deltaTime * 5f);
    }

    public void OnHitByLaser(Color incomingColor)
    {
        IsHitThisFrame = true;
        sr.color = Color.magenta;
    }
}