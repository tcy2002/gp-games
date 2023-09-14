using System;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    public float length;
    public float width;
    public int quality;
    public float rate;

    private Mesh _mesh;
    private Vector3[] _vertices;

    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _vertices = new Vector3[quality * 2];
        
        //顶点
        var intervalLength = length / quality;
        //上面顶点
        for (var i = 0; i < quality; i++) {
            _vertices[i] = new Vector3(i * intervalLength, width, 0);
        }
        //下面顶点
        for (var i = quality; i < 2 * quality; i++) {
            _vertices[i] = new Vector3((i - quality) * intervalLength, 0, 0);
        }
        //设置三角形
        var angleCount = (quality * 2 - 2) * 3;
        var triangles = new int[angleCount];

        var current = 0;
        for (var i = quality; i < 2 * quality - 1; i++) {
            triangles[current++] = i;
            triangles[current++] = i - quality;
            triangles[current++] = i - quality + 1;

            triangles[current++] = i;
            triangles[current++] = i - quality + 1;
            triangles[current++] = i + 1;
        }

        var uvs = new Vector2[_vertices.Length];
        for (int i = 0; i < _vertices.Length; i++) {
            uvs[i] = new Vector2(_vertices[i].x / length, _vertices[i].y / width);
        }
        
        _mesh.Clear();
        _mesh.vertices = _vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
    }

    void Update()
    {
        // Update the vertex positions based on a sine wave function
        for (var i = 0; i < quality; i++)
        {
            _vertices[i].y = width + (float)Math.Sin(Time.time + _vertices[i].x / rate) * 0.2f * rate * (float)Math.Sin(Time.time);
        }

        _mesh.vertices = _vertices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
    }
}