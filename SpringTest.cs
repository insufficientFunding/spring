using UnityEngine;
using Malware.Springs;

public class SpringTest : MonoBehaviour
{
    private Vector3Spring spring;

    public float speed = 1f;
    [Range (0f, 1f)] public float damper = 1f;

    public bool go;

    private void Awake ()
    {
        spring = new Vector3Spring (new Vector3 (1f, 0f, 0f), 1f, 1f);
    }

    private void FixedUpdate ()
    {
        spring.Speed = speed;
        spring.Damper = damper;

        if (go)
        {
            go = false;
            spring.Target = Random.onUnitSphere;
        }

        transform.position = spring.Position;
    }
}
