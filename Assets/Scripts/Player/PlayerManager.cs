using Fwk;

public class PlayerManager : SingletonGeneric<PlayerManager>
{
    public void Initialize()
    {
        PlayerInputs.Instance.Initialize();
    }

    public void Update(float deltaTime)
    {
        PlayerInputs.Instance.Update(deltaTime);
        PlayerMover.Instance.UpdateMover(deltaTime);
    }
}