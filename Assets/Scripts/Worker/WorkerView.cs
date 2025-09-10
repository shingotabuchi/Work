using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WorkerView : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool _enableWasdMovement = true;
    [SerializeField] private bool _useDebugModel;
    [SerializeField] private DebugWorkerModel _debugModel;
#endif

    public Animator Animator => _animator;
    private Animator _animator;

    private WorkerController _controller;
    private IReadOnlyWorkerModel _model;
    private WorkerAnimationView _animationView;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = new WorkerController(this);
        _model = _controller.Model;
        _animationView = new WorkerAnimationView(_animator);

#if UNITY_EDITOR
        _controller.EnableWasdMovement = _enableWasdMovement;
        if (_useDebugModel)
        {
            _model = _debugModel;
        }
#endif
    }

    public void UpdateView(float deltaTime)
    {
        _animationView.Update(_controller);
    }
}