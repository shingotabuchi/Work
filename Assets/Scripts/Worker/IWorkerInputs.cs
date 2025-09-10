using UnityEngine;

public interface IWorkerInputs
{
    bool Sprint { get; }
    Vector2 Move { get; }
    float MoveMagnitude { get; }
    float DirectionAngle { get; }

    void Update(float deltaTime);
}