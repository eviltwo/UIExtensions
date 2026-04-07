#if UNITY_INPUT_SYSTEM_ENABLE_UI
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace eviltwo.UIExtensions.EventRelaySystems
{
    public class EventRelaySystem : MonoBehaviour
    {
        public bool InvokeEvenIfHandledByEventSystem = false;

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
}
#endif
