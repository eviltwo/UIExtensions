using UnityEngine.EventSystems;

namespace eviltwo.UIExtensions.EventRelaySystems
{
    public interface IEventRelaySystemHandler
    {
    }

    public interface ICancelRelayHandler : IEventRelaySystemHandler
    {
        void OnCancel(BaseEventData eventData);
    }

    public interface ISubmitRelayHandler : IEventRelaySystemHandler
    {
        void OnSubmit(BaseEventData eventData);
    }

    public interface IInputActionRelayHandler : IEventRelaySystemHandler
    {
        void OnInputAction(InputActionEventData eventData);
    }
}
