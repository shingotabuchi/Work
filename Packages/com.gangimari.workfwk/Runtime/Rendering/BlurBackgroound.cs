using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ぼかし背景適用UIに付けるコンポーネント
/// Imageコンポーネントと同時に設定する前提
/// </summary>
[RequireComponent(typeof(Image))]
[ExecuteAlways] // 起動してない状態でも表示更新されてほしいため
public class BlurBackground : MonoBehaviour
{
    private static readonly string BlurBackgroundUIShaderPath = "BlurBackgroundUI";
    private static readonly int ShaderPropertyIdBlurBlendRate = Shader.PropertyToID("_BlurBlendRate");

    [SerializeField][Range(0.0f, 1.0f)] private float _blurBlendRate = 1.0f;

    private Material _material;

#if UNITY_EDITOR
    // blurBlendRateを更新した際にEditor上で即反映されて欲しいため
    // Editro時のみ定義
    private void Update()
    {
        BlendRate = _blurBlendRate;
    }
#endif

    private void OnEnable()
    {
        SetImageMaterial();
    }

    private void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
            _material = null;
        }
    }

    public float BlendRate
    {
        get => _blurBlendRate;
        set
        {
            _blurBlendRate = Mathf.Clamp01(value);

            if (_material != null)
            {
                _material.SetFloat(ShaderPropertyIdBlurBlendRate, _blurBlendRate);
            }
        }
    }

    /// <summary>
    /// マテリアルの構築/設定
    /// 個別のパラメータ変更を可能とするため、インスタンス毎にマテリアルを構築する
    /// </summary>
    private void SetImageMaterial()
    {
        // 専用マテリアルの設定
        if (_material == null)
        {
            _material = new Material(Shader.Find(BlurBackgroundUIShaderPath));
        }

        _material.SetFloat(ShaderPropertyIdBlurBlendRate, _blurBlendRate);

        Image targetImage = GetComponent<Image>();
        targetImage.material = _material;
    }
}