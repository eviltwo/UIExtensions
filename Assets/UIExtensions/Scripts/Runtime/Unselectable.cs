using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace eviltwo.UIExtensions
{
    public class Unselectable :
        UIBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        public enum Transition
        {
            None,
            ColorTint,
            SpriteSwap,
            Animation
        }

        [SerializeField]
        private Transition m_Transition = Transition.ColorTint;

        public Transition transition
        {
            get => m_Transition;
            set => m_Transition = value;
        }

        [SerializeField]
        private ColorBlock m_Colors = ColorBlock.defaultColorBlock;

        public ColorBlock colors
        {
            get => m_Colors;
            set => m_Colors = value;
        }

        [SerializeField]
        private SpriteState m_SpriteState;

        public SpriteState spriteState
        {
            get => m_SpriteState;
            set => m_SpriteState = value;
        }

        [SerializeField]
        private AnimationTriggers m_AnimationTriggers = new();

        public AnimationTriggers animationTriggers
        {
            get => m_AnimationTriggers;
            set => m_AnimationTriggers = value;
        }

        [SerializeField]
        private Graphic m_TargetGraphic;

        public Graphic targetGraphic
        {
            get => m_TargetGraphic;
            set => m_TargetGraphic = value;
        }

        [SerializeField]
        private bool m_Interactable = true;

        public bool interactable
        {
            get => m_Interactable;
            set => m_Interactable = value;
        }

        private bool m_GroupsAllowInteraction = true;

        private bool isPointerInside { get; set; }
        private bool isPointerDown { get; set; }

        protected Unselectable()
        {
        }

        public Image image
        {
            get => m_TargetGraphic as Image;
            set => m_TargetGraphic = value;
        }

        public Animator animator => GetComponent<Animator>();

        protected override void OnCanvasGroupChanged()
        {
            m_GroupsAllowInteraction = ParentGroupAllowsInteraction();
        }

        private readonly List<CanvasGroup> m_CanvasGroupCache = new();

        private bool ParentGroupAllowsInteraction()
        {
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(m_CanvasGroupCache);
                foreach (var canvasGroup in m_CanvasGroupCache)
                {
                    if (canvasGroup.enabled && !canvasGroup.interactable)
                        return false;

                    if (canvasGroup.ignoreParentGroups)
                        return true;
                }

                t = t.parent;
            }

            return true;
        }

        public bool IsInteractable()
        {
            return m_GroupsAllowInteraction && m_Interactable;
        }

        protected override void OnDidApplyAnimationProperties()
        {
            OnSetProperty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            isPointerDown = false;
            m_GroupsAllowInteraction = ParentGroupAllowsInteraction();
        }

        protected override void OnDisable()
        {
            InstantClearState();
            base.OnDisable();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            OnCanvasGroupChanged();
        }

        private void OnSetProperty()
        {
            var instant = false;
#if UNITY_EDITOR
            instant = !Application.isPlaying;
#endif
            DoStateTransition(currentSelectionState, instant);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && IsPressed())
            {
                InstantClearState();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Colors.fadeDuration = Mathf.Max(m_Colors.fadeDuration, 0.0f);

            if (isActiveAndEnabled)
            {
                if (!interactable && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                    EventSystem.current.SetSelectedGameObject(null);

                // Need to clear out the override image on the target...
                DoSpriteSwap(null);

                // If the transition mode got changed, we need to clear all the transitions, since we don't know what the old transition mode was.
                StartColorTween(Color.white, true);
                TriggerAnimation(m_AnimationTriggers.normalTrigger);

                // And now go to the right state.
                DoStateTransition(currentSelectionState, true);
            }
        }

        private enum SelectionState
        {
            Normal,
            Highlighted,
            Pressed,
            Disabled,
        }

        private SelectionState currentSelectionState
        {
            get
            {
                if (!IsInteractable())
                    return SelectionState.Disabled;
                if (isPointerDown)
                    return SelectionState.Pressed;
                if (isPointerInside)
                    return SelectionState.Highlighted;
                return SelectionState.Normal;
            }
        }

        private void InstantClearState()
        {
            string triggerName = m_AnimationTriggers.normalTrigger;

            isPointerInside = false;
            isPointerDown = false;

            switch (m_Transition)
            {
                case Transition.ColorTint:
                    StartColorTween(Color.white, true);
                    break;
                case Transition.SpriteSwap:
                    DoSpriteSwap(null);
                    break;
                case Transition.Animation:
                    TriggerAnimation(triggerName);
                    break;
            }
        }

        private void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            Color tintColor;
            Sprite transitionSprite;
            string triggerName;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = m_Colors.normalColor;
                    transitionSprite = null;
                    triggerName = m_AnimationTriggers.normalTrigger;
                    break;
                case SelectionState.Highlighted:
                    tintColor = m_Colors.highlightedColor;
                    transitionSprite = m_SpriteState.highlightedSprite;
                    triggerName = m_AnimationTriggers.highlightedTrigger;
                    break;
                case SelectionState.Pressed:
                    tintColor = m_Colors.pressedColor;
                    transitionSprite = m_SpriteState.pressedSprite;
                    triggerName = m_AnimationTriggers.pressedTrigger;
                    break;
                case SelectionState.Disabled:
                    tintColor = m_Colors.disabledColor;
                    transitionSprite = m_SpriteState.disabledSprite;
                    triggerName = m_AnimationTriggers.disabledTrigger;
                    break;
                default:
                    tintColor = Color.black;
                    transitionSprite = null;
                    triggerName = string.Empty;
                    break;
            }

            switch (m_Transition)
            {
                case Transition.ColorTint:
                    StartColorTween(tintColor * m_Colors.colorMultiplier, instant);
                    break;
                case Transition.SpriteSwap:
                    DoSpriteSwap(transitionSprite);
                    break;
                case Transition.Animation:
                    TriggerAnimation(triggerName);
                    break;
            }
        }

        protected override void Reset()
        {
            m_TargetGraphic = GetComponent<Graphic>();
        }

#endif

        void StartColorTween(Color targetColor, bool instant)
        {
            if (m_TargetGraphic == null)
                return;

            m_TargetGraphic.CrossFadeColor(targetColor, instant ? 0f : m_Colors.fadeDuration, true, true);
        }

        void DoSpriteSwap(Sprite newSprite)
        {
            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }

        void TriggerAnimation(string triggername)
        {
            if (transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(m_AnimationTriggers.normalTrigger);
            animator.ResetTrigger(m_AnimationTriggers.highlightedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.pressedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.selectedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.disabledTrigger);
            animator.SetTrigger(triggername);
        }

        private bool IsPressed()
        {
            if (!IsActive() || !IsInteractable()) return false;
            return isPointerDown;
        }

        private void EvaluateAndTransitionToSelectionState()
        {
            if (!IsActive() || !IsInteractable()) return;
            DoStateTransition(currentSelectionState, false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            isPointerDown = true;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            isPointerDown = false;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            EvaluateAndTransitionToSelectionState();
        }
    }
}
