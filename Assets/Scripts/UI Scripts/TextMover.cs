using UnityEngine.UI;

namespace UI_Scripts
{
    public class TextMover : Button
    {
        private TMPro.TextMeshProUGUI _text;
        private float _move = 20;

        protected override void Awake()
        {
            base.Awake();
            _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }
        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _text.transform.position -= _text.transform.up * _move;
        }
        public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _text.transform.position += _text.transform.up * _move;
        }
    }
}
