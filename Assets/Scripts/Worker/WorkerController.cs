using UnityEngine;

public class WorkerController : IWorkerAnimationController
{

#if UNITY_EDITOR
    public bool EnableWasdMovement;
#endif

    public IReadOnlyWorkerModel Model => _model;

    private readonly WorkerView _view;
    // private readonly WorkerModel _model;
    private readonly DebugWorkerModel _model;

    public WorkerController(
        WorkerView view
    )
    {
        _view = view;
        // _model = new WorkerModel();
        _model = new DebugWorkerModel();
        WorkerManager.Instance.RegisterWorker(this);
    }

    public void Update(float deltaTime)
    {
        _view.UpdateView(deltaTime);
    }
}