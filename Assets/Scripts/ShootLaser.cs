using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    public Material material;
    private LaserBeam beam;

    private void Start()
    {
        beam = new LaserBeam(material);
    }

    private void Update()
    {
        beam.UpdateBeam(transform.position, transform.up);
    }
}