using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace eviltwo.UIExtensions.UI
{
    public class ValueDriver : Selectable
    {
        [SerializeField]
        private bool _wholeNumbers = false;

        public bool WholeNumbers
        {
            get => _wholeNumbers;
            set => _wholeNumbers = value;
        }

        [SerializeField]
        private bool _hasMinValue = true;

        public bool HasMinValue
        {
            get => _hasMinValue;
            set => _hasMinValue = value;
        }

        [SerializeField]
        private float _minValue = 0.0f;

        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        [SerializeField]
        private bool _hasMaxValue = true;

        public bool HasMaxValue
        {
            get => _hasMaxValue;
            set => _hasMaxValue = value;
        }

        [SerializeField]
        private float _maxValue = 1.0f;

        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        [Min(0)]
        [SerializeField]
        private float _deltaValue = 0.1f;

        public float DeltaValue
        {
            get => _deltaValue;
            set => _deltaValue = value;
        }

        [SerializeField]
        private float _value = 0.0f;

        public float Value
        {
            get => _value;
            set => SetValue(value);
        }

        public enum InputDirection
        {
            LeftToRight = 0,
            RightToLeft = 1,
            DownToUp = 2,
            UpToDown = 3
        }

        public InputDirection Direction = InputDirection.LeftToRight;

        [Serializable]
        public class ValueDriverEvent : UnityEvent<float>
        {
        }

        [SerializeField]
        private ValueDriverEvent _onValueChanged = new();

        public ValueDriverEvent OnValueChanged => _onValueChanged;

        [SerializeField]
        private string _eventStringFormat = "F1";

        public string EventStringFormat
        {
            get => _eventStringFormat;
            set => _eventStringFormat = value;
        }

        [Serializable]
        public class ValueDriverEventString : UnityEvent<string>
        {
        }

        [SerializeField]
        private ValueDriverEventString _onValueChangedString = new();

        public ValueDriverEventString OnValueChangedString => _onValueChangedString;

#if UNITY_EDITOR
        private bool _delayedInvokeEventInEditor;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_wholeNumbers)
            {
                _minValue = Mathf.Round(_minValue);
                _maxValue = Mathf.Round(_maxValue);
                _deltaValue = Mathf.Max(1, Mathf.Round(_deltaValue));
            }

            _value = ClampValue(_value);
            if (Application.isPlaying)
            {
                _delayedInvokeEventInEditor = true;
            }
        }
#endif

        protected override void Start()
        {
            base.Start();
            InvokeValueChanged();
        }

#if UNITY_EDITOR
        protected void Update()
        {
            if (_delayedInvokeEventInEditor)
            {
                _delayedInvokeEventInEditor = false;
                InvokeValueChanged();
            }
        }
#endif

        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        public Axis InputAxis => Direction == InputDirection.LeftToRight || Direction == InputDirection.RightToLeft ? Axis.Horizontal : Axis.Vertical;

        public bool InputDirectionReversed => Direction == InputDirection.RightToLeft || Direction == InputDirection.UpToDown;

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            var moveDir = eventData.moveDir;
            var moveAxis = moveDir == MoveDirection.Left || moveDir == MoveDirection.Right ? Axis.Horizontal : Axis.Vertical;
            if (moveAxis != InputAxis)
            {
                base.OnMove(eventData);
                return;
            }

            var increase = moveDir is MoveDirection.Right or MoveDirection.Up;
            if (InputDirectionReversed) increase = !increase;

            if (increase)
            {
                IncreaseValue();
            }
            else
            {
                DecreaseValue();
            }
        }

        public void IncreaseValue() => IncreaseValue(_deltaValue);

        public void IncreaseValue(float deltaValue)
        {
            if (deltaValue <= 0) return;
            SetValue(_value + deltaValue, true);
        }

        public void DecreaseValue() => DecreaseValue(_deltaValue);

        public void DecreaseValue(float deltaValue)
        {
            if (deltaValue <= 0) return;
            SetValue(_value - deltaValue, true);
        }

        public void SetValueWithoutNotify(float newValue) => SetValue(newValue, false);

        public void SetValue(float newValue) => SetValue(newValue, true);

        private void SetValue(float newValue, bool notify)
        {
            newValue = ClampValue(newValue);
            if (Mathf.Approximately(_value, newValue)) return;
            _value = newValue;
            if (notify)
            {
                InvokeValueChanged();
            }
        }

        public void SetValueString(string stringValue) => SetValueString(stringValue, true);

        public void SetValueStringWithoutNotify(string stringValue) => SetValueString(stringValue, false);

        private void SetValueString(string stringValue, bool notify)
        {
            if (float.TryParse(stringValue, out var result))
            {
                SetValue(result, notify);
            }
        }

        private void InvokeValueChanged()
        {
            OnValueChanged.Invoke(_value);
            string text;
            try
            {
                text = _value.ToString(_eventStringFormat);
            }
            catch (FormatException)
            {
                text = ((int)_value).ToString(_eventStringFormat);
            }

            OnValueChangedString.Invoke(text);
        }

        private float ClampValue(float value)
        {
            var newValue = value;
            if (HasMinValue) newValue = Mathf.Max(newValue, MinValue);
            if (HasMaxValue) newValue = Mathf.Min(newValue, MaxValue);
            if (WholeNumbers) newValue = Mathf.Round(newValue);
            return newValue;
        }
    }
}
