using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

    [AddComponentMenu("UI/HoldButton", 30)]
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [System.Serializable]
        public class HoldButtonClickedEvent : UnityEvent { }

        [SerializeField]
        private HoldButtonClickedEvent m_OnHoldClick = new HoldButtonClickedEvent();

        public float holdThreshold = 2f; // ???????2??????
        public float currentHoldTime = 0f;

        private bool isHolding = false;
        private bool hasTriggered = false; // ? ?????????

        public HoldButtonClickedEvent onHoldClick
        {
            get { return m_OnHoldClick; }
            set { m_OnHoldClick = value; }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            currentHoldTime = 0f;
            hasTriggered = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
            currentHoldTime = 0f;
            hasTriggered = false;
        }

        private void Update()
        {
            if (isHolding && !hasTriggered)
            {
                currentHoldTime += Time.deltaTime;

                if (currentHoldTime >= holdThreshold)
                {
                    Press();
                    hasTriggered = true;
                }
            }
        }

        private void Press()
        {
            UISystemProfilerApi.AddMarker("HoldButton.onHoldClick", this);
            m_OnHoldClick.Invoke();
        }
    }
