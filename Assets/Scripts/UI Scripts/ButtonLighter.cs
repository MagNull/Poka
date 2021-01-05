using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLighter : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image icon;
    private bool _state;
    private UnityEngine.UI.Image _image;

    private void Awake()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
    }
    
    public void ChangeButtonState(bool state)
    {
        if(_image)
        {
            if (state)
            {
                _image.color = Color.white;
                GetComponent<UnityEngine.UI.Button>().interactable = true;
                if (icon)
                {
                    icon.color = Color.white;
                }
            }
            else
            {
                _image.color = new Color(.3f, .3f, .3f);
                GetComponent<UnityEngine.UI.Button>().interactable = false;
                if (icon)
                {
                    icon.color = Color.red;
                }
            }
        }
        _state = state;
       
    }

    public bool GetButtonState()
    {
        return _state;
    }
}
