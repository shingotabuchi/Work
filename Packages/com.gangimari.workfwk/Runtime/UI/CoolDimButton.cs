using UnityEngine;

namespace Fwk.UI
{
    public class CoolDimButton : CoolButton
    {
        [SerializeField] private GameObject _interactableDim;
        public override void SetInteractable(bool interactable)
        {
            base.SetInteractable(interactable);
            _interactableDim.SetActiveFast(!interactable);
        }
    }
}