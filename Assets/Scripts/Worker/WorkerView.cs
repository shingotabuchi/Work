using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WorkerView : MonoBehaviour
{
    public Animator Animator => _animator;
    private Animator _animator;

    private WorkerController _controller;
    private WorkerAnimationView _animationView;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = new WorkerController(this);
        _animationView = new WorkerAnimationView(_animator);
    }
}