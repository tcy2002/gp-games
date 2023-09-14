using UnityEditor;
using UnityEngine;

public class CustomShaderGUI : ShaderGUI
{
    private MaterialEditor _editor;
    private MaterialProperty[] _properties;
    private Material _target;

    enum ShaderChoice
    {
        Phong, Normal
    }
        
    enum SpecularChoice {
        True, False
    }
        
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        _editor = editor;
        _properties = properties;
        _target = editor.target as Material;
        var mainTex = FindProperty("_MainTex", properties);
        var mainTexLabel = new GUIContent(mainTex.displayName);
        editor.TextureProperty(mainTex, mainTexLabel.text);

        var shaderChoice = ShaderChoice.Normal;
        if (_target && _target.IsKeywordEnabled("ENABLE_PHONG"))
        {
            shaderChoice = ShaderChoice.Phong;
        }

        var specularChoice = SpecularChoice.False;
        if (_target && _target.IsKeywordEnabled("USE_SPECULAR"))
        {
            specularChoice = SpecularChoice.True;
        }
            
        EditorGUI.BeginChangeCheck();
        shaderChoice = (ShaderChoice)EditorGUILayout.EnumPopup(
            new GUIContent("Shader Mode"), shaderChoice
        );

        if (_target) 
        {
            if (shaderChoice == ShaderChoice.Normal)
            {
                _target.DisableKeyword("ENABLE_PHONG");
            }
            else
            {
                _target.EnableKeyword("ENABLE_PHONG");
                
                specularChoice = (SpecularChoice)EditorGUILayout.EnumPopup(
                    new GUIContent("Use Specular?"), specularChoice
                );
                if (specularChoice == SpecularChoice.True)
                {
                    _target.EnableKeyword("USE_SPECULAR");
                }
                else
                {
                    _target.DisableKeyword("USE_SPECULAR");
                }
            
                if (specularChoice == SpecularChoice.True){
                    var shininess = FindProperty("_Shininess", properties);
                    editor.FloatProperty(shininess, "Specular Factor");
                }
            }
        }

        EditorGUI.EndChangeCheck();
    }
}