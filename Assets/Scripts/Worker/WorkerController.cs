using UnityEngine;

public class WorkerController
{
    private readonly WorkerView _view;

    public WorkerController(
        WorkerView view
    )
    {
        _view = view;
        WorkerManager.Instance.RegisterWorker(this);
    }

    public void Update(float deltaTime)
    {
    }
}