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
    [SerializeField][Range(0, 1)] private float _targetLookAtWeight = 1.0f;
    [SerializeField] private float _lookAtWeightLerpRate = 5.0f;
    [SerializeField][Range(0, 180)] private float _lookAtFov = 90.0f;
    [SerializeField] private float _lookAtRange = 10.0f;
    [SerializeField][Range(0, 1)] private float _lookAtChancePerSecond = 0.2f;
    
    [Header("Blink Settings")]
    [SerializeField][Range(0, 1)] private float _blinkChancePerSecond = 0.15f;
    
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
    private bool _isLookingAt = false;
    
    public float TargetLookAtWeight
    {
        get => _targetLookAtWeight;
        set => _targetLookAtWeight = Mathf.Clamp01(value);
    }
    
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
        
        // Check if target is within FOV and range
        if (_lookAtTarget != null && _eyePosition != null)
        {
            Vector3 toTarget = _lookAtTarget.position - _eyePosition.position;
            float distance = toTarget.magnitude;
            
            bool canLookAt = false;
            
            // Check range
            if (distance <= _lookAtRange)
            {
                // Check FOV using body/transform forward direction
                Vector3 directionToTarget = toTarget.normalized;
                Vector3 bodyForward = transform.forward;
                float angle = Vector3.Angle(bodyForward, directionToTarget);
                
                if (angle <= _lookAtFov)
                {
                    canLookAt = true;
                }
            }
            
            // Handle looking state
            if (!canLookAt)
            {
                // Target is out of range or FOV, stop looking
                _isLookingAt = false;
                _targetLookAtWeight = 0.0f;
            }
            else if (!_isLookingAt)
            {
                // Can look but not currently looking, random chance to start
                float chanceThisFrame = _lookAtChancePerSecond * deltaTime;
                if (Random.value < chanceThisFrame)
                {
                    _isLookingAt = true;
                    _targetLookAtWeight = 1.0f;
                }
                else
                {
                    _targetLookAtWeight = 0.0f;
                }
            }
            else
            {
                // Already looking and target is valid
                _targetLookAtWeight = 1.0f;
            }
        }
        else
        {
            _isLookingAt = false;
            _targetLookAtWeight = 0.0f;
        }
        
        // Lerp lookAtWeight towards targetLookAtWeight
        _lookAtWeight = Mathf.Lerp(_lookAtWeight, _targetLookAtWeight, _lookAtWeightLerpRate * deltaTime);
        
        // Random blink chance
        float blinkChanceThisFrame = _blinkChancePerSecond * deltaTime;
        if (Random.value < blinkChanceThisFrame)
        {
            Blink();
        }
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
        if (_animator != null)
        {
            _animator.SetTrigger("Blink");
        }
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