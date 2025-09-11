using UnityEngine;

public class WorkerFaceView : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _eyeMaskRendererL;
    [SerializeField] private SkinnedMeshRenderer _eyeFrameRendererL;
    [SerializeField] private SkinnedMeshRenderer _eyeMaskRendererR;
    [SerializeField] private SkinnedMeshRenderer _eyeFrameRendererR;
    [SerializeField] private AnimationCurve _normalCurve;

    public float EyeOpenScale = 1.0f;
    [Range(0, 1)] public float EyeOpenAmount = 1.0f;

    public void UpdateView(float deltaTime)
    {
        var blink = (1 - EyeOpenAmount * EyeOpenScale) * 100;
        _eyeMaskRendererL.SetBlendShapeWeight(0, blink);
        _eyeFrameRendererL.SetBlendShapeWeight(0, blink);
        _eyeMaskRendererR.SetBlendShapeWeight(0, blink);
        _eyeFrameRendererR.SetBlendShapeWeight(0, blink);
    }

    // [Alchemy.Inspector.Button]
    private void TestBlink()
    {
    }
}