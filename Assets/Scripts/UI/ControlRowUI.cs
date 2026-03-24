using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlRowUI : MonoBehaviour
{
    public TextMeshProUGUI actionNameText;
    public TextMeshProUGUI bindingText;
    private InputAction _action;
    private int _bindingIndex;
    private SettingsManager _manager;

    public void Setup(InputAction action, int bindingIndex, string displayName, SettingsManager manager)
    {
        _action = action;
        _bindingIndex = bindingIndex;
        _manager = manager;
        actionNameText.text = displayName;
        bindingText.text = action.GetBindingDisplayString(bindingIndex);
    }

    public void Rebind()
    {
        if (_manager != null) _manager.StopAnyActiveRebinding();

        bindingText.text = "PRESS KEY";
        _action.Disable();

        var rebindOperation = _action.PerformInteractiveRebinding(_bindingIndex)
            .WithControlsExcluding("<Mouse>/delta")
            .WithControlsExcluding("<Mouse>/position")
            .OnMatchWaitForAnother(0.1f)
            .OnApplyBinding((operation, path) => {
                _action.ApplyBindingOverride(_bindingIndex, path);
            })
            .OnComplete(operation => {
                FinishRebind();
                if (_manager != null) _manager.ControlsChanged();
                operation.Dispose();
            })
            .OnCancel(operation => {
                FinishRebind();
                operation.Dispose();
            });

        if (_manager != null) _manager.RegisterRebindOperation(rebindOperation);
        rebindOperation.Start();
    }

    private void FinishRebind()
    {
        bindingText.text = _action.GetBindingDisplayString(_bindingIndex);
        _action.Enable();
    }
}
