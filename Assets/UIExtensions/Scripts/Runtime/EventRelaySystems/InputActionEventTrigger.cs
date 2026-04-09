using System;
using eviltwo.UIExtensions.EventRelaySystems;
using UnityEngine;
using UnityEngine.Events;

namespace eviltwo.UIExtensions
{
    public class InputActionEventTrigger : MonoBehaviour, IInputActionRelayHandler
    {
        [Serializable]
        public class TargetEvent
        {
            public string ActionName;
            public UnityEvent OnTrigger;
        }

        public TargetEvent[] TargetEvents = Array.Empty<TargetEvent>();

        public void OnInputAction(InputActionEventData eventData)
        {
            foreach (var targetEvent in TargetEvents)
            {
                if (eventData.ActionName == targetEvent.ActionName)
                {
                    targetEvent.OnTrigger?.Invoke();
                }
            }
        }
    }
}
