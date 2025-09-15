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

    [Header("IK Solver Settings")]
    [SerializeField][Range(1, 10)] private int _maxIterations = 5;
    [SerializeField][Range(0.001f, 0.1f)] private float _convergenceThreshold = 0.01f;
    [SerializeField][Range(0.1f, 1.0f)] private float _dampingFactor = 0.7f;

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
        Quaternion targetNeckRotation = SolveNeckRotationForEyeLookAt(_lookAtTarget.position, _maxIterations, _convergenceThreshold);

        // Get current animated rotation (this includes any animation playing)
        Quaternion currentWorldRotation = _neckBone.rotation;

        // Blend between current animated rotation and target look rotation
        Quaternion blendedWorldRotation = Quaternion.Slerp(currentWorldRotation, targetNeckRotation, _lookAtWeight);

        // Apply the blended world rotation directly
        _neckBone.rotation = blendedWorldRotation;
    }

    /// <summary>
    /// Iteratively solves for the neck rotation that makes the eye look at the target position.
    /// This accounts for the fact that neck rotation affects eye position and orientation.
    /// </summary>
    /// <param name="targetWorldPosition">World position to look at</param>
    /// <param name="maxIterations">Maximum number of iterations for convergence</param>
    /// <param name="convergenceThreshold">Angle threshold for convergence in radians</param>
    /// <returns>The neck rotation that makes the eye look at the target</returns>
    private Quaternion SolveNeckRotationForEyeLookAt(Vector3 targetWorldPosition, int maxIterations = 5, float convergenceThreshold = 0.01f)
    {
        // Store original state
        Quaternion originalNeckRotation = _neckBone.rotation;

        // Start with current neck rotation as initial guess
        Quaternion currentNeckRotation = originalNeckRotation;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            // Apply current guess to neck to get updated transforms
            _neckBone.rotation = currentNeckRotation;

            // Calculate current eye position and forward direction with this neck rotation
            Vector3 currentEyePosition = _eyePosition.position;
            Vector3 currentEyeForward = _eyePosition.forward;

            // Calculate desired direction from eye to target
            Vector3 desiredDirection = (targetWorldPosition - currentEyePosition).normalized;

            // Apply inversion if needed (common for character rigs)
            if (_invertHorizontal) desiredDirection.x = -desiredDirection.x;
            if (_invertVertical) desiredDirection.y = -desiredDirection.y;

            // Calculate the rotation needed to align eye forward with desired direction
            Quaternion lookRotation = Quaternion.FromToRotation(currentEyeForward, desiredDirection);

            // Apply this rotation to the current neck rotation
            Quaternion newNeckRotation = lookRotation * currentNeckRotation;

            // Check for convergence
            float angleDifference = Quaternion.Angle(currentNeckRotation, newNeckRotation) * Mathf.Deg2Rad;
            if (angleDifference < convergenceThreshold)
            {
                // Converged, use this result
                currentNeckRotation = newNeckRotation;
                break;
            }

            // Update for next iteration with some damping to prevent overshooting
            currentNeckRotation = Quaternion.Slerp(currentNeckRotation, newNeckRotation, _dampingFactor);
        }

        // Restore original neck rotation before returning result
        _neckBone.rotation = originalNeckRotation;

        return currentNeckRotation;
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