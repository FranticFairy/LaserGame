using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class LaserBeam
{
    private GameObject laserObj;
    private LineRenderer laser;
    private List<Vector3> laserIndices = new List<Vector3>();
    private const int MaxBounces = 10;

    // Track segments separately so each can have its own color
    private List<(Vector3 from, Vector3 to, Color color)> segments = new();

    public LaserBeam(Material material)
    {
        laserObj = new GameObject("Laser Beam");
        laser = laserObj.AddComponent<LineRenderer>();
        laser.startWidth = 0.1f;
        laser.endWidth = 0.1f;
        laser.sortingOrder = 1;

        // Use a vertex-color-friendly material instead of your custom one
        laser.material = new Material(Shader.Find("Sprites/Default"));

        laser.startColor = Color.green;
        laser.endColor = Color.green;
    }

    public void UpdateBeam(Vector3 pos, Vector3 dir)
    {
        segments.Clear();
        CastRay(pos, dir, 0, Color.green);
        ApplyLaser();
    }

    private void CastRay(Vector3 pos, Vector2 dir, int bounceCount, Color currentColor)
    {
        if (bounceCount > MaxBounces) return;

        // Cast through everything, collecting all hits in order
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, dir, 30f);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Vector3 segmentStart = pos;
        Color segmentColor = currentColor;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            if (hit.collider.CompareTag("Crystal"))
            {
                // Add segment up to crystal
                segments.Add((segmentStart, hit.point, segmentColor));
                // Turn purple and pass through
                segmentColor = Color.magenta;
                segmentStart = hit.point;

                // Notify the crystal it was hit
                hit.collider.GetComponent<Crystal>()?.OnHitByLaser(currentColor);
            }
            else if (hit.collider.CompareTag("Mirror"))
            {
                segments.Add((segmentStart, hit.point, segmentColor));
                Vector2 reflectDir = Vector2.Reflect(dir, hit.normal);
                Vector2 newPos = (Vector2)hit.point + reflectDir * 0.001f;
                CastRay(newPos, reflectDir, bounceCount + 1, segmentColor);
                return;
            }
            else
            {
                // Wall or other blocker — stop here
                segments.Add((segmentStart, hit.point, segmentColor));

                // Notify target if hit by correct color
                hit.collider.GetComponent<LaserTarget>()?.OnHitByLaser(segmentColor);
                return;
            }
        }

        // No blocker hit — draw to end of range
        segments.Add((segmentStart, (Vector2)segmentStart + dir * 30f, segmentColor));
    }

    private void ApplyLaser()
    {
        if (segments.Count == 0) return;

        List<Vector3> positions = new();
        float totalLength = 0f;

        foreach (var seg in segments)
            totalLength += Vector3.Distance(seg.from, seg.to);

        positions.Add(segments[0].from);
        foreach (var seg in segments)
            positions.Add(seg.to);

        laser.positionCount = positions.Count;
        for (int i = 0; i < positions.Count; i++)
            laser.SetPosition(i, positions[i]);

        // Only add a key when the color actually changes
        List<GradientColorKey> colorKeys = new();
        List<GradientAlphaKey> alphaKeys = new();

        float accumulated = 0f;
        Color lastColor = segments[0].color;
        colorKeys.Add(new GradientColorKey(lastColor, 0f));
        alphaKeys.Add(new GradientAlphaKey(1f, 0f));

        foreach (var seg in segments)
        {
            float segLen = Vector3.Distance(seg.from, seg.to);
            accumulated += segLen;
            float t = Mathf.Clamp01(accumulated / totalLength);

            if (seg.color != lastColor)
            {
                // Add a key just before and at the transition point
                float tBefore = Mathf.Clamp01((accumulated - segLen) / totalLength);
                colorKeys.Add(new GradientColorKey(lastColor, tBefore));
                colorKeys.Add(new GradientColorKey(seg.color, t));
                alphaKeys.Add(new GradientAlphaKey(1f, tBefore));
                alphaKeys.Add(new GradientAlphaKey(1f, t));
                lastColor = seg.color;
            }
        }

        // Always cap with a final key at t=1
        colorKeys.Add(new GradientColorKey(lastColor, 1f));
        alphaKeys.Add(new GradientAlphaKey(1f, 1f));

        // Clamp to Unity's 8 key limit as a safety net
        if (colorKeys.Count > 8) colorKeys = colorKeys.GetRange(0, 8);
        if (alphaKeys.Count > 8) alphaKeys = alphaKeys.GetRange(0, 8);

        Gradient gradient = new Gradient();
        gradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
        laser.colorGradient = gradient;
    }
}