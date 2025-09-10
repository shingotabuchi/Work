using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class WorkerView : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool _useWasdInputs;
    [SerializeField] private bool _useDebugModel;
    [SerializeField] private DebugWorkerModel _debugModel;
#endif

    private Animator _animator;
    private CharacterController _characterController;

    private WorkerController _controller;
    private IWorkerModel _model;
    private IWorkerInputs _inputs;

    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;

    private int _animIDSpeed;
    private int _animIDMotionSpeed;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        _controller = new WorkerController(this);
        _model = _controller.Model;
        _inputs = _controller.Inputs;

        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

#if UNITY_EDITOR
        if (_useDebugModel)
        {
            _model = _debugModel;
        }
        if (_useWasdInputs)
        {
            _inputs = WorkerWasdInputs.Instance;
        }
#endif
    }

    public void UpdateView(float deltaTime)
    {
        float targetSpeed = _inputs.Sprint ? _model.SprintSpeed : _model.MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_inputs.Move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _inputs.MoveMagnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                deltaTime * _model.SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_inputs.Move != Vector2.zero)
        {
            _targetRotation = _inputs.DirectionAngle;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                _model.RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _characterController.Move(targetDirection.normalized * (_speed * deltaTime));

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, deltaTime * _model.SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
    }
}