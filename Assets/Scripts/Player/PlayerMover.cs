using Fwk;
using Pathfinding.RVO;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(RVOController))]
public class PlayerMover : Singleton<PlayerMover>
{
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _sprintSpeed = 6f;
    [SerializeField] private float _speedChangeRate = 10f;
    [SerializeField] private float _lookSensitivity = 50f;
    [SerializeField] private Transform _camTarget;

    private CharacterController _characterController;
    private RVOController _rvoController;
    private Transform _playerTransform;

    private float _speed;
    private float _pitch;
    private float _yaw;

    protected override void Awake()
    {
        base.Awake();
        _characterController = GetComponent<CharacterController>();
        _rvoController = GetComponent<RVOController>();
        _playerTransform = transform;
        var camTargetEuler = _camTarget.localRotation.eulerAngles;
        var euler = _playerTransform.rotation.eulerAngles;
        _yaw = euler.y;
        _pitch = camTargetEuler.x;
    }

    public void UpdateMover(float deltaTime)
    {
        var inputs = PlayerInputs.Instance;
        var move = inputs.Move;
        var sprint = inputs.Sprint;
        var look = inputs.Look;

        _pitch -= look.y * _lookSensitivity * deltaTime;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);
        _yaw += look.x * _lookSensitivity * deltaTime;

        _camTarget.localRotation = Quaternion.Euler(_pitch, 0, 0);
        _playerTransform.rotation = Quaternion.Euler(0, _yaw, 0);

        var targetSpeed = sprint ? _sprintSpeed : _walkSpeed;

        if (move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        var currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

        var speedOffset = 0.1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                deltaTime * _speedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        var direction = move.y * _playerTransform.forward + move.x * _playerTransform.right;
        direction.y = 0;
        direction.Normalize();
        _characterController.Move(direction * _speed * deltaTime);
        _rvoController.velocity = _characterController.velocity;
    }
}