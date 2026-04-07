using eviltwo.UIExtensions.EventRelaySystems;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelSample : MonoBehaviour, ICancelRelayHandler, ISubmitRelayHandler
{
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnCancel(BaseEventData eventData)
    {
        Debug.Log("OnCancel");
        Close();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Debug.Log("OnSubmit");
    }
}
