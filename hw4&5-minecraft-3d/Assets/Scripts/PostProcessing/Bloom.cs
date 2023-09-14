using System;
using UnityEngine;

public class Bloom : MonoBehaviour
{
    [Range(1, 5)]
    public int iterations = 1;
    [Range(0, 10)] 
    public float threshold = 1;
    [Range(0, 1)]
    public float softThreshold = 0.5f;
    [Range(0, 10)]
    public float intensity = 1;
    public Shader bloomShader;

    private RenderTexture[] _textures = new RenderTexture[16];
    [NonSerialized]
    private Material _bloom;

    const int BoxDownPrefilterPass = 0;
    private const int BoxDownPass = 1;
    private const int BoxUpPass = 2;
    private const int ApplyBloomPass = 3;

    public void SetIntensity(float value)
    {
        intensity = value;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_bloom == null)
        {
            _bloom = new Material(bloomShader);
            _bloom.hideFlags = HideFlags.HideAndDontSave;
        }
        var knee = threshold * softThreshold;
        Vector4 filter;
        filter.x = threshold;
        filter.y = filter.x - knee;
        filter.z = 2f * knee;
        filter.w = 0.25f / (knee + 0.00001f);
        _bloom.SetVector("_Filter", filter);
        _bloom.SetFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));
        
        var width = source.width / 2;
        var height = source.height / 2;
        var format = source.format;
        
        var currentDestination = _textures[0] =
            RenderTexture.GetTemporary(width, height, 0, format);
        Graphics.Blit(source, currentDestination, _bloom, BoxDownPrefilterPass);
        var currentSource = currentDestination;

        var i = 1;
        for (; i < iterations; i++)
        {
            width /= 2;
            height /= 2;
            if (height < 2)
            {
                break;
            }
            currentDestination = _textures[i] = 
                RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(currentSource, currentDestination, _bloom, BoxDownPass);
            currentSource = currentDestination;
        }
        
        for (i -= 2; i >= 0; i--) {
            currentDestination = _textures[i];
            _textures[i] = null;
            Graphics.Blit(currentSource, currentDestination, _bloom, BoxUpPass);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }

        _bloom.SetTexture("_SourceTex", source);
        Graphics.Blit(currentSource, destination, _bloom, ApplyBloomPass);
        RenderTexture.ReleaseTemporary(currentSource);
    }
}