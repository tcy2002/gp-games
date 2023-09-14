using System;
using UnityEngine;

public class MotionBlur : MonoBehaviour
{
    public Shader motionBlurShader;
    [NonSerialized]
    private Material _material;

    [Range(0.0f, 0.9f)]
    public float blurAmount = 0.5f;
	
    private RenderTexture accumulationTexture;

    public void SetAmount(float value)
    {
        blurAmount = value;
    }

    void OnDisable() {
        DestroyImmediate(accumulationTexture);
    }

    void OnRenderImage (RenderTexture src, RenderTexture dest) {
        if (_material == null)
        {
            _material = new Material(motionBlurShader);
            _material.hideFlags = HideFlags.HideAndDontSave;
        }
        
        if (accumulationTexture == null || accumulationTexture.width != src.width || accumulationTexture.height != src.height) {
            DestroyImmediate(accumulationTexture);
            accumulationTexture = new RenderTexture(src.width, src.height, 0);
            accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
            Graphics.Blit(src, accumulationTexture);
        }
        
        _material.SetFloat("_BlurAmount", 1.0f - blurAmount);
        Graphics.Blit (src, accumulationTexture, _material);
        Graphics.Blit (accumulationTexture, dest);
    }
}
