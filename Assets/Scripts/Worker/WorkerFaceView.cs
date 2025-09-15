using UnityEngine;

public class WorkerFaceAnimationController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _eyeMaskRendererL;
    [SerializeField] private SkinnedMeshRenderer _eyeFrameRendererL;
    [SerializeField] private SkinnedMeshRenderer _eyeMaskRendererR;
    [SerializeField] private SkinnedMeshRenderer _eyeFrameRendererR;
    [SerializeField] private Transform _neckBone;
    [SerializeField] private Transform _lookAtTarget;
    [SerializeField] private Transform _eyePosition;
    [SerializeField][Range(0, 1)] private float _lookAtWeight = 1.0f;
    [SerializeField] private bool _invertVertical = false;
    [SerializeField] private bool _invertHorizontal = false;

    public float EyeOpenScale = 1.0f;
    [Range(0, 1)] public float EyeOpenAmountFromAnimator = 1.0f;
    [Range(0, 1)] public float EyeOpenAmount = 1.0f;

    private Animator _animator;
    private Quaternion _initialNeckRotation;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        // Store the initial rotation as our "forward facing" reference
        _initialNeckRotation = _neckBone.rotation;
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

    public void LateUpdateView(float deltaTime)
    {
        // Calculate the direction from eye to target in world space
        Vector3 targetDirection = (_lookAtTarget.position - _eyePosition.position).normalized;

        // Apply inversion if needed (common for character rigs)
        if (_invertHorizontal) targetDirection.x = -targetDirection.x;
        if (_invertVertical) targetDirection.y = -targetDirection.y;

        // Get the initial forward direction (what the character considers "forward")
        Debug.Log($"Target Direction: {targetDirection}");
        Debug.Log($"_eyePosition Forward: {_eyePosition.forward}");
        // Calculate the rotation needed to go from initial forward to target direction
        Quaternion lookOffset = Quaternion.FromToRotation(_eyePosition.forward, targetDirection);
        Debug.Log($"Look Offset: {lookOffset.eulerAngles}");

        Quaternion currentWorldRotation = _neckBone.rotation;

        // Apply the look offset to the initial rotation to get our target rotation
        Quaternion targetWorldRotation = lookOffset * currentWorldRotation;

        // // Get current animated rotation (this includes any animation playing)
        // Quaternion currentWorldRotation = _neckBone.rotation;

        // Blend between current animated rotation and target look rotation
        Quaternion blendedWorldRotation = Quaternion.Slerp(currentWorldRotation, targetWorldRotation, _lookAtWeight);

        // Apply the blended world rotation directly
        _neckBone.rotation = blendedWorldRotation;
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