using UnityEngine;
using UnityEngine.EventSystems;

namespace eviltwo.UIExtensions.EventSystems
{
    public class SelectionRecovery : MonoBehaviour
    {
        private GameObject _lastSelected;

        private void Update()
        {
            var eventSystem = EventSystem.current;
            var currentSelected = eventSystem.currentSelectedGameObject;
            if (currentSelected == null)
            {
                if (_lastSelected != null && _lastSelected.activeInHierarchy)
                {
                    eventSystem.SetSelectedGameObject(_lastSelected);
                }
            }
            else
            {
                _lastSelected = currentSelected;
            }
        }
    }
}
