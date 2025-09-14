using Fwk;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorkerWasdInputs : SingletonGeneric<WorkerWasdInputs>, IWorkerInputs
{
    public bool Sprint { get; private set; }
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public float MoveMagnitude { get; private set; }
    public float DirectionAngle { get; private set; }

    private InputAction _sprintAction;
    private InputAction _moveAction;
    private InputAction _lookAction;

    private bool _initialized;

    public void Initialize()
    {
        _sprintAction = InputSystem.actions.FindAction("Sprint");
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _initialized = true;
    }

    public void Update(float deltaTime)
    {
        if (!_initialized)
        {
            return;
        }

        Sprint = _sprintAction.IsPressed();
        Move = _moveAction.ReadValue<Vector2>();
        Look = _lookAction.ReadValue<Vector2>();
        MoveMagnitude = Move.magnitude;
        DirectionAngle = Mathf.Atan2(Move.x, Move.y) * Mathf.Rad2Deg
            + CameraManager.Instance.MainCamera.transform.eulerAngles.y;
    }
}