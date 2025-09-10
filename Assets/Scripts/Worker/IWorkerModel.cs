public interface IWorkerModel
{
    float MoveSpeed { get; }
    float SprintSpeed { get; }
    float SpeedChangeRate { get; }
    float RotationSmoothTime { get; }
}