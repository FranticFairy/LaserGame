using System.Collections.Generic;
using UnityEngine;

public class LaserBeam
{
    private GameObject laserObj;
    private LineRenderer laser;
    private List<Vector3> laserIndices = new List<Vector3>();
    private const int MaxBounces = 10;

    public LaserBeam(Material material)
    {
        laserObj = new GameObject("Laser Beam");
        laser = laserObj.AddComponent<LineRenderer>();
        laser.startWidth = 0.1f;
        laser.endWidth = 0.1f;
        laser.material = material;
        laser.startColor = Color.green;
        laser.endColor = Color.green;
        laser.sortingOrder = 1; // Render on top of sprites
    }

    public void UpdateBeam(Vector3 pos, Vector3 dir)
    {
        laserIndices.Clear();
        CastRay(pos, dir, 0);
        ApplyLaser();
    }

    private void CastRay(Vector3 pos, Vector2 dir, int bounceCount)
    {
        if (bounceCount > MaxBounces) return;

        laserIndices.Add(pos);

        // Use Physics2D instead of Physics
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, 30f);

        if (hit.collider != null)
        {
            CheckHit(hit, dir, bounceCount);
        }
        else
        {
            laserIndices.Add((Vector2)pos + dir * 30f);
        }
    }

    private void CheckHit(RaycastHit2D hitInfo, Vector2 direction, int bounceCount)
    {
        if (hitInfo.collider.CompareTag("Mirror"))
        {
            Vector2 reflectDir = Vector2.Reflect(direction, hitInfo.normal);
            Vector2 newPos = hitInfo.point + reflectDir * 0.001f;
            CastRay(newPos, reflectDir, bounceCount + 1);
        }
        else
        {
            laserIndices.Add((Vector3)hitInfo.point);
        }
    }

    private void ApplyLaser()
    {
        laser.positionCount = laserIndices.Count;
        for (int i = 0; i < laserIndices.Count; i++)
        {
            laser.SetPosition(i, laserIndices[i]);
        }
    }
}