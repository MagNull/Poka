using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMover : Button
{
    private TMPro.TextMeshProUGUI text;
    private float _move = 20;

    protected override void Awake()
    {
        base.Awake();
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        text.transform.position -= text.transform.up * _move;
    }
    public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        text.transform.position += text.transform.up * _move;
    }
}
