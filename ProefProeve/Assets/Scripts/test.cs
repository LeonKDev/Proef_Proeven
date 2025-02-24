using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private List<GameObject> cubes = new List<GameObject>();
    private List<LineRenderer> lines = new List<LineRenderer>();
    private float timeOffset = 0f;

    void Start()
    {
        // Generate initial cubes
        for (int i = 0; i < 20; i++)
        {
            CreateCube(i);
        }

        // Create lines between cubes
        for (int i = 0; i < cubes.Count; i++)
        {
            GameObject lineObj = new GameObject($"Line_{i}");
            LineRenderer line = lineObj.AddComponent<LineRenderer>();

            // Configure line appearance
            line.startWidth = 0.1f;
            line.endWidth = 0.1f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = cubes[i].GetComponent<Renderer>().material.color;
            line.endColor = cubes[(i + 1) % cubes.Count].GetComponent<Renderer>().material.color;

            lines.Add(line);
        }
    }

    void CreateCube(int index)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = Vector3.one * 0.5f; // Small cube size

        // Random vibrant color
        Renderer renderer = cube.GetComponent<Renderer>();
        renderer.material.color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f);

        cubes.Add(cube);
    }

    void Update()
    {
        timeOffset += Time.deltaTime;

        // Animate each cube
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i] != null)
            {
                // Calculate angle for perfect distribution (360 degrees / number of cubes)
                float angleStep = 360f / cubes.Count;
                float angle = ((i * angleStep) + timeOffset * 30f) * Mathf.Deg2Rad;
                float height = Mathf.Sin(timeOffset + i * (2f * Mathf.PI / cubes.Count)) * 2f;
                float radius = 5f + Mathf.Sin(timeOffset * 0.5f + i * (2f * Mathf.PI / cubes.Count)) * 2f;

                Vector3 position = new Vector3(
                    Mathf.Cos(angle) * radius,
                    height,
                    Mathf.Sin(angle) * radius
                );

                cubes[i].transform.position = position;
                cubes[i].transform.Rotate(Vector3.up * 90f * Time.deltaTime, Space.World);
                cubes[i].transform.Rotate(Vector3.right * 45f * Time.deltaTime, Space.World);

                // Update line positions
                if (lines[i] != null)
                {
                    lines[i].SetPosition(0, cubes[i].transform.position);
                    lines[i].SetPosition(1, cubes[(i + 1) % cubes.Count].transform.position);
                }
            }
        }
    }
}
