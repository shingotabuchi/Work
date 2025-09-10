using UnityEngine;

public class WorkerController
{
    public IWorkerModel Model => _model;
    public IWorkerInputs Inputs => _inputs;

    private readonly WorkerView _view;
    // private readonly WorkerModel _model;
    private readonly DebugWorkerModel _model;
    private readonly WorkerWasdInputs _inputs;

    public WorkerController(
        WorkerView view
    )
    {
        _view = view;
        // _model = new WorkerModel();
        _model = new DebugWorkerModel();
        _inputs = new WorkerWasdInputs();
        WorkerManager.Instance.RegisterWorker(this);
    }

    public void Update(float deltaTime)
    {
        _view.UpdateView(deltaTime);
    }
}