using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    public Color requiredColor = Color.magenta;
    public bool isActivated = false;

    private SpriteRenderer sr;
    private Color baseColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    private void Update()
    {
        isActivated = false;
        sr.color = Color.Lerp(sr.color, baseColor, Time.deltaTime * 10f);
    }

    public void OnHitByLaser(Color incomingColor)
    {
        if (ColorMatches(incomingColor, requiredColor))
        {
            isActivated = true;
            sr.color = requiredColor * 1.5f;
            LevelManager.Instance.CheckWinCondition();
        }
    }

    private bool ColorMatches(Color a, Color b, float tolerance = 0.2f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}