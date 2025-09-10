using UnityEngine;

public class WorkerAnimationView
{
    private readonly int _animIDSpeed;
    private readonly int _animIDMotionSpeed;
    private readonly Animator _animator;

    public WorkerAnimationView(Animator animator)
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animator = animator;
    }
}