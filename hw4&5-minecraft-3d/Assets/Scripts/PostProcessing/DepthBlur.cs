using System;
using UnityEngine;

public class DepthBlur : MonoBehaviour
{
    //用于控制高斯模糊的参数
    [Range(0.2f, 3)]
    public float blurSize = 1;
    [Range(0, 8)]
    public int iterations = 3;
    [Range(1, 8)]
    public int downSample = 2;

    public Shader depthShader;
    [NonSerialized] 
    private Material _material;
    private Camera _camera;

    //对焦参数
    [Range(0, 20)]
    public float ramp = 10;

    public void SetRamp(float value)
    {
        ramp = value;
    }
    
    private void OnEnable()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }
        _camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null)
        {
            _material = new Material(depthShader);
            _material.hideFlags = HideFlags.HideAndDontSave;
        }
        
        _material.SetFloat("_Ramp", ramp);

        var w = source.width / downSample;
        var h = source.height / downSample;

        var buffer0 = RenderTexture.GetTemporary(w, h, 0);
        buffer0.filterMode = FilterMode.Bilinear;

        Graphics.Blit(source, buffer0);

        //前两个Pass做高斯模糊处理，结果保存在buffer0中
        for(int i = 0; i < iterations; i++)
        {
            _material.SetFloat("_BlurSize", blurSize * i + 1);

            var buffer1 = RenderTexture.GetTemporary(w, h, 0);
            buffer1.filterMode = FilterMode.Bilinear;
            Graphics.Blit(buffer0, buffer1, _material, 0);
            RenderTexture.ReleaseTemporary(buffer0);

            buffer0 = RenderTexture.GetTemporary(w, h, 0);
            Graphics.Blit(buffer1, buffer0, _material, 1);
            RenderTexture.ReleaseTemporary(buffer1);
        }

        // Graphics.Blit(buffer0, destination);
        //最后一个Pass用原图像和模糊图（buffer0）进行关于深度的插值计算，具体见Shader脚本
        _material.SetTexture("_BlurTex", buffer0);
        Graphics.Blit(source, destination, _material, 2);
        RenderTexture.ReleaseTemporary(buffer0);
    }
}
