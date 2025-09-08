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

    // Stencil properties
    MaterialProperty stencilRef = null;
    MaterialProperty stencilComp = null;
    MaterialProperty stencilPass = null;
    MaterialProperty stencilFail = null;
    MaterialProperty stencilZFail = null;
    MaterialProperty stencilWriteMask = null;
    MaterialProperty stencilReadMask = null;

    // Render Queue property
    MaterialProperty renderQueue = null;

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

        // Stencil properties
        stencilRef = FindProperty("_StencilRef", properties);
        stencilComp = FindProperty("_StencilComp", properties);
        stencilPass = FindProperty("_StencilPass", properties);
        stencilFail = FindProperty("_StencilFail", properties);
        stencilZFail = FindProperty("_StencilZFail", properties);
        stencilWriteMask = FindProperty("_StencilWriteMask", properties);
        stencilReadMask = FindProperty("_StencilReadMask", properties);

        // Render Queue property
        renderQueue = FindProperty("_RenderQueue", properties);

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

        // Stencil
        EditorGUILayout.Space();
        GUILayout.Label("Stencil", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        materialEditor.ShaderProperty(stencilRef, "Reference Value (0-255)");

        // Stencil comparison function
        EditorGUI.BeginChangeCheck();
        var stencilCompValue = (int)stencilComp.floatValue;
        string[] comparisonOptions = { "Disabled", "Never", "Less", "Equal", "LessEqual", "Greater", "NotEqual", "GreaterEqual", "Always" };
        int[] comparisonValues = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        stencilCompValue = EditorGUILayout.IntPopup("Comparison", stencilCompValue, comparisonOptions, comparisonValues);
        if (EditorGUI.EndChangeCheck())
        {
            stencilComp.floatValue = stencilCompValue;
        }

        // Stencil operations
        EditorGUI.BeginChangeCheck();
        var stencilPassValue = (int)stencilPass.floatValue;
        var stencilFailValue = (int)stencilFail.floatValue;
        var stencilZFailValue = (int)stencilZFail.floatValue;
        string[] operationOptions = { "Keep", "Zero", "Replace", "IncrSat", "DecrSat", "Invert", "IncrWrap", "DecrWrap" };
        int[] operationValues = { 0, 1, 2, 3, 4, 5, 6, 7 };

        stencilPassValue = EditorGUILayout.IntPopup("Pass Operation", stencilPassValue, operationOptions, operationValues);
        stencilFailValue = EditorGUILayout.IntPopup("Fail Operation", stencilFailValue, operationOptions, operationValues);
        stencilZFailValue = EditorGUILayout.IntPopup("ZFail Operation", stencilZFailValue, operationOptions, operationValues);

        if (EditorGUI.EndChangeCheck())
        {
            stencilPass.floatValue = stencilPassValue;
            stencilFail.floatValue = stencilFailValue;
            stencilZFail.floatValue = stencilZFailValue;
        }

        materialEditor.ShaderProperty(stencilWriteMask, "Write Mask (0-255)");
        materialEditor.ShaderProperty(stencilReadMask, "Read Mask (0-255)");

        EditorGUI.indentLevel--;

        // Render Queue
        EditorGUILayout.Space();
        GUILayout.Label("Render Queue", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        // Current queue display
        int currentQueue = material.renderQueue;
        EditorGUILayout.LabelField("Current Queue", currentQueue.ToString());

        // Queue dropdown
        EditorGUI.BeginChangeCheck();
        var queueOverride = (int)renderQueue.floatValue;
        queueOverride = EditorGUILayout.IntField("Queue Value (-1 = Auto)", queueOverride);
        if (EditorGUI.EndChangeCheck())
        {
            renderQueue.floatValue = queueOverride;
            if (queueOverride >= 0)
            {
                SetRenderQueue(material, queueOverride);
            }
            else
            {
                // Reset to automatic based on blend mode
                BlendMode currentMode = (BlendMode)blendMode.floatValue;
                SetBlendMode(material, currentMode);
            }
        }
        EditorGUI.indentLevel--;

        // Draw the rest of the properties
        DrawDefaultInspector(materialEditor, properties);
    }

    void SetBlendMode(Material material, BlendMode blendMode)
    {
        // Check if we have a manual render queue override
        int queueOverride = (int)renderQueue.floatValue;

        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "Opaque");
                if (queueOverride < 0) material.SetOverrideTag("Queue", "Geometry");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                material.SetFloat("_ZWrite", 1.0f);
                material.SetFloat("_Surface", 0.0f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                if (queueOverride < 0) material.renderQueue = 2000; // Geometry
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                if (queueOverride < 0) material.SetOverrideTag("Queue", "AlphaTest");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                material.SetFloat("_ZWrite", 1.0f);
                material.SetFloat("_Surface", 0.0f);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                if (queueOverride < 0) material.renderQueue = 2450; // AlphaTest
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                if (queueOverride < 0) material.SetOverrideTag("Queue", "Transparent");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0.0f);
                material.SetFloat("_Surface", 1.0f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                if (queueOverride < 0) material.renderQueue = 3000; // Transparent
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                if (queueOverride < 0) material.SetOverrideTag("Queue", "Transparent");
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0.0f);
                material.SetFloat("_Surface", 1.0f);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                if (queueOverride < 0) material.renderQueue = 3000; // Transparent
                break;
        }

        // Apply manual queue override if set
        if (queueOverride >= 0)
        {
            material.renderQueue = queueOverride;
        }
    }

    void SetRenderQueue(Material material, int queue)
    {
        if (queue >= 0)
        {
            material.renderQueue = queue;
            renderQueue.floatValue = queue;
        }
        else
        {
            // Auto mode - let blend mode determine queue
            renderQueue.floatValue = -1;
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
                properties[i].name == "_StencilRef" ||
                properties[i].name == "_StencilComp" ||
                properties[i].name == "_StencilPass" ||
                properties[i].name == "_StencilFail" ||
                properties[i].name == "_StencilZFail" ||
                properties[i].name == "_StencilWriteMask" ||
                properties[i].name == "_StencilReadMask" ||
                properties[i].name == "_RenderQueue" ||
                (properties[i].name == "_Cutoff" && blendMode.floatValue != 1.0f))
                continue;

            materialEditor.ShaderProperty(properties[i], properties[i].displayName);
        }
    }
}
