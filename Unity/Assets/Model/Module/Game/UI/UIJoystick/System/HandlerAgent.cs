using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace ETModel
{
    public class HandlerAgent : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private bool isInit = false;

        private Action<PointerEventData> onPointerDown;
        private Action<PointerEventData> onDrag;
        private Action<PointerEventData> onPointerUp;

        public void Init(Action<PointerEventData> onPointerDown, Action<PointerEventData> onDrag, Action<PointerEventData> onPointerUp)
        {
            this.onPointerDown = onPointerDown;
            this.onDrag = onDrag;
            this.onPointerUp = onPointerUp;

            isInit = true;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (isInit)
            {
                onPointerDown(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isInit)
            {
                onDrag(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isInit)
            {
                onPointerUp(eventData);
            }
        }
    }

    public enum JoystickType { Fixed, Floating, Dynamic }
    public enum AxisOptions { Both, Horizontal, Vertical }
}