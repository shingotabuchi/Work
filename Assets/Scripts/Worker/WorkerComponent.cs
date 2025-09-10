using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WorkerComponent : MonoBehaviour
{
    public Animator Animator => _animator;
    private Animator _animator;

    public void Initialize()
    {
        _animator = GetComponent<Animator>();
    }
}