using UnityEngine;
using UnityEditor;

public class CoolToonEditor : ShaderGUI
{
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    MaterialProperty blendMode = null;
    MaterialProperty srcBlend = null;
    MaterialProperty dstBlend = null;
    MaterialProperty zWrite = null;
    MaterialProperty surface = null;
    MaterialProperty cutoff = null;
    MaterialProperty cull = null;
    MaterialProperty outlineCull = null;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // Find properties
        blendMode = FindProperty("_BlendMode", properties);
        srcBlend = FindProperty("_SrcBlend", properties);
        dstBlend = FindProperty("_DstBlend", properties);
        zWrite = FindProperty("_ZWrite", properties);
        surface = FindProperty("_Surface", properties);
        cutoff = FindProperty("_Cutoff", properties);
        cull = FindProperty("_Cull", properties);
        outlineCull = FindProperty("_OutlineCull", properties);

        Material material = materialEditor.target as Material;

        // Blend Mode
        EditorGUI.BeginChangeCheck();
        BlendMode mode = (BlendMode)blendMode.floatValue;
        mode = (BlendMode)EditorGUILayout.EnumPopup("Blend Mode", mode);
        if (EditorGUI.EndChangeCheck())
        {
            materialEditor.RegisterPropertyChangeUndo("Blend Mode");
            blendMode.floatValue = (float)mode;
            SetBlendMode(material, mode);
        }

        // Show cutoff only for cutout mode
        if (mode == BlendMode.Cutout)
        {
            materialEditor.ShaderProperty(cutoff, "Alpha Cutoff");
        }

        // Cull Mode
        materialEditor.ShaderProperty(cull, "Cull Mode");

        // Outline Cull Mode
        materialEditor.ShaderProperty(outlineCull, "Outline Cull Mode");

        // Draw the rest of the properties
        DrawDefaultInspector(materialEditor, properties);
    }

    void SetBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "Opaque");
                material.SetOverrideTag("Queue", "Geometry");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                material.SetFloat("_ZWrite", 1.0f);
                material.SetFloat("_Surface", 0.0f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetOverrideTag("Queue", "AlphaTest");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                material.SetFloat("_ZWrite", 1.0f);
                material.SetFloat("_Surface", 0.0f);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetOverrideTag("Queue", "Transparent");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0.0f);
                material.SetFloat("_Surface", 1.0f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetOverrideTag("Queue", "Transparent");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0.0f);
                material.SetFloat("_Surface", 1.0f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                break;
        }
    }

    void DrawDefaultInspector(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // Skip the blend mode related properties as we handle them above
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].name == "_BlendMode" ||
                properties[i].name == "_SrcBlend" ||
                properties[i].name == "_DstBlend" ||
                properties[i].name == "_ZWrite" ||
                properties[i].name == "_Surface" ||
                properties[i].name == "_Cull" ||
                properties[i].name == "_OutlineCull" ||
                (properties[i].name == "_Cutoff" && blendMode.floatValue != 1.0f))
                continue;

            materialEditor.ShaderProperty(properties[i], properties[i].displayName);
        }
    }
}
