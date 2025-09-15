using Fwk;

public class WorkerManager : SingletonGeneric<WorkerManager>
{
    private readonly WorkerController[] _workers = new WorkerController[WorkerConsts.MAX_WORKERS];

    public void Update(float deltaTime)
    {
        for (int i = 0; i < _workers.Length; i++)
        {
            _workers[i]?.Update(deltaTime);
        }
    }

    public void LateUpdate(float deltaTime)
    {
        for (int i = 0; i < _workers.Length; i++)
        {
            _workers[i]?.LateUpdate(deltaTime);
        }
    }

    public void RegisterWorker(WorkerController worker)
    {
        for (int i = 0; i < _workers.Length; i++)
        {
            if (_workers[i] == null)
            {
                _workers[i] = worker;
                return;
            }
        }

        throw new System.Exception("Max workers reached");
    }
}