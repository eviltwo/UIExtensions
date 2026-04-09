#if UNITY_INPUT_SYSTEM_ENABLE_UI
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace eviltwo.UIExtensions.EventRelaySystems
{
    public class EventRelaySystem : MonoBehaviour
    {
        public bool InvokeEvenIfHandledByEventSystem = false;

        public InputActionReference[] InputActions = System.Array.Empty<InputActionReference>();

        private void Update()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null) return;

            var selectedObject = eventSystem.currentSelectedGameObject;
            if (selectedObject == null) return;

            var inputModule = eventSystem.currentInputModule as InputSystemUIInputModule;
            if (inputModule == null) return;

            var cancelAction = inputModule.cancel?.action;
            if (cancelAction != null
                && cancelAction.WasPerformedThisDynamicUpdate()
                && (InvokeEvenIfHandledByEventSystem || !IsHandledByEventSystem<ICancelHandler>(selectedObject)))
            {
                var handler = GetHandler<ICancelRelayHandler>(selectedObject);
                if (handler != null) handler.OnCancel(new BaseEventData(eventSystem));
            }

            var submitAction = inputModule.submit?.action;
            if (submitAction != null
                && submitAction.WasPerformedThisDynamicUpdate()
                && (InvokeEvenIfHandledByEventSystem || !IsHandledByEventSystem<ISubmitHandler>(selectedObject)))
            {
                var handler = GetHandler<ISubmitRelayHandler>(selectedObject);
                if (handler != null) handler.OnSubmit(new BaseEventData(eventSystem));
            }

            for (var i = 0; i < InputActions.Length; i++)
            {
                var inputAction = InputActions[i]?.action;
                if (inputAction == null) continue;
                if (inputAction.WasPerformedThisDynamicUpdate())
                {
                    var handler = GetHandler<IInputActionRelayHandler>(selectedObject);
                    if (handler == null) continue;
                    handler.OnInputAction(new InputActionEventData(eventSystem)
                    {
                        ActionName = inputAction.name,
                        ActionIndex = i
                    });
                }
            }
        }

        private static bool IsHandledByEventSystem<T>(GameObject target) where T : IEventSystemHandler
        {
            return target.TryGetComponent<T>(out _);
        }

        private T GetHandler<T>(GameObject target) where T : IEventRelaySystemHandler
        {
            return target.GetComponentInParent<T>(includeInactive: false);
        }
    }

    public class InputActionEventData : BaseEventData
    {
        public string ActionName { get; set; }

        public int ActionIndex { get; set; }

        public InputActionEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }
    }
}
#endif
