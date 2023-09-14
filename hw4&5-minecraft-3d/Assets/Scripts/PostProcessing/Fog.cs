using System;
using UnityEngine;

public class Fog : MonoBehaviour
{
    public Shader fogShader;
    [NonSerialized]
	private Material _material;
	private Camera _camera;

	[Range(0.0f, 3.0f)]
	public float fogDensity = 1.0f;
	public Color fogColor = Color.white;
	public float fogStart = 0.0f;
	public float fogEnd = 2.0f;

	public void SetDensity(float value)
	{
		fogDensity = value;
	}

	void OnEnable() {
		if (_camera == null)
		{
			_camera = GetComponent<Camera>();
		}
		_camera.depthTextureMode |= DepthTextureMode.Depth;
	}
	
	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if (_material == null)
		{
			_material = new Material(fogShader);
			_material.hideFlags = HideFlags.HideAndDontSave;
		}
		
		Matrix4x4 frustumCorners = Matrix4x4.identity;
		float fov = _camera.fieldOfView;
		float near = _camera.nearClipPlane;
		float aspect = _camera.aspect;

		float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
		Vector3 toRight = _camera.transform.right * halfHeight * aspect;
		Vector3 toTop = _camera.transform.up * halfHeight;

		Vector3 topLeft = _camera.transform.forward * near + toTop - toRight;
		float scale = topLeft.magnitude / near;

		topLeft.Normalize();
		topLeft *= scale;

		Vector3 topRight = _camera.transform.forward * near + toRight + toTop;
		topRight.Normalize();
		topRight *= scale;

		Vector3 bottomLeft = _camera.transform.forward * near - toTop - toRight;
		bottomLeft.Normalize();
		bottomLeft *= scale;

		Vector3 bottomRight = _camera.transform.forward * near + toRight - toTop;
		bottomRight.Normalize();
		bottomRight *= scale;

		frustumCorners.SetRow(0, bottomLeft);
		frustumCorners.SetRow(1, bottomRight);
		frustumCorners.SetRow(2, topRight);
		frustumCorners.SetRow(3, topLeft);

		_material.SetMatrix("_FrustumCornersRay", frustumCorners);
		_material.SetFloat("_FogDensity", fogDensity);
		_material.SetColor("_FogColor", fogColor);
		_material.SetFloat("_FogStart", fogStart);
		_material.SetFloat("_FogEnd", fogEnd);

		Graphics.Blit (src, dest, _material);
	}
}
