using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace eviltwo.UIExtensions
{
    [AddComponentMenu("UI (Canvas)/UnselectableButton", 30)]
    public class UnselectableButton : Unselectable, IPointerClickHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent
        {
        }

        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick");
        }
    }
}
