using Fwk;

public class PlayerManager : SingletonGeneric<PlayerManager>
{
    private bool _initialized;

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        if (PlayerMover.Instance == null)
        {
            return;
        }

        PlayerInputs.Instance.Initialize();
        _initialized = true;
    }

    public void Update(float deltaTime)
    {
        if (!_initialized)
        {
            return;
        }

        PlayerInputs.Instance.Update(deltaTime);
        PlayerMover.Instance.UpdateMover(deltaTime);
    }
}