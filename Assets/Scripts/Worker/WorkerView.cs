using Pathfinding;
using UnityEngine;

public class WorkerView : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool _useWasdInputs;
    [SerializeField] private bool _useDebugModel;
    [SerializeField] private DebugWorkerModel _debugModel;
#endif

    private Animator _animator;
    private WorkerFaceAnimationController _faceAnimation;
    private FollowerEntity _followerEntity;

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
        _animator = GetComponentInChildren<Animator>();
        _faceAnimation = GetComponentInChildren<WorkerFaceAnimationController>();
        _followerEntity = GetComponent<FollowerEntity>();

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
        _faceAnimation.UpdateView(deltaTime);

        var targetSpeed = _inputs.Sprint ? _model.SprintSpeed : _model.MoveSpeed;

        if (_inputs.Move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        // a reference to the players current horizontal velocity
        // var currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

        var speedOffset = 0.1f;
        var inputMagnitude = _inputs.MoveMagnitude;

        // // accelerate or decelerate to target speed
        // if (currentHorizontalSpeed < targetSpeed - speedOffset ||
        //     currentHorizontalSpeed > targetSpeed + speedOffset)
        // {
        //     _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
        //         deltaTime * _model.SpeedChangeRate);

        //     _speed = Mathf.Round(_speed * 1000f) / 1000f;
        // }
        // else
        // {
        //     _speed = targetSpeed;
        // }

        if (_inputs.Move != Vector2.zero)
        {
            _targetRotation = _inputs.DirectionAngle;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                _model.RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // _characterController.Move(targetDirection.normalized * _speed * deltaTime);
        Debug.Log(_followerEntity.velocity.magnitude);
        _animationBlend = Mathf.Lerp(_animationBlend, _followerEntity.velocity.magnitude, deltaTime * _model.SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        _animator.SetFloat(_animIDSpeed, _followerEntity.velocity.magnitude);
    }

    public void LateUpdateView(float deltaTime)
    {
        _faceAnimation.LateUpdateView(deltaTime);
    }
}