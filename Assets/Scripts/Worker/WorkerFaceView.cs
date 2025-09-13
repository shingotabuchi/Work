using UnityEngine;

public class WorkerFaceAnimationController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _eyeMaskRendererL;
    [SerializeField] private SkinnedMeshRenderer _eyeFrameRendererL;
    [SerializeField] private SkinnedMeshRenderer _eyeMaskRendererR;
    [SerializeField] private SkinnedMeshRenderer _eyeFrameRendererR;

    public float EyeOpenScale = 1.0f;
    [Range(0, 1)] public float EyeOpenAmountFromAnimator = 1.0f;
    [Range(0, 1)] public float EyeOpenAmount = 1.0f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void UpdateView(float deltaTime)
    {
        var eyeOpen = Mathf.Clamp01(EyeOpenAmountFromAnimator * EyeOpenAmount * EyeOpenScale);
        var blink = (1 - eyeOpen) * 100;
        _eyeMaskRendererL.SetBlendShapeWeight(0, blink);
        _eyeFrameRendererL.SetBlendShapeWeight(0, blink);
        _eyeMaskRendererR.SetBlendShapeWeight(0, blink);
        _eyeFrameRendererR.SetBlendShapeWeight(0, blink);
    }


#if UNITY_EDITOR
    [Fwk.Editor.Button]
#endif
    private void Blink()
    {
        _animator.Play("Blink", 1, 0);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        UpdateView(0.016f);
    }
#endif
}