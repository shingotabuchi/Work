using UnityEngine;

[System.Serializable]
public class DebugWorkerModel : IWorkerModel
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _speedChangeRate;
    [SerializeField] private float _rotationSmoothTime;

    public float MoveSpeed => _moveSpeed;
    public float SprintSpeed => _sprintSpeed;
    public float SpeedChangeRate => _speedChangeRate;
    public float RotationSmoothTime => _rotationSmoothTime;

}
