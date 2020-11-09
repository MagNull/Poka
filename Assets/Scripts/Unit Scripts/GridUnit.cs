using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit : MonoBehaviour
{
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private Vector2Int _size = Vector2Int.one;
    private List<Color> _normalColors = new List<Color>();
    private Grid _grid;
    public Vector2Int Size { get => _size; set => _size = value; }

    private void Awake()
    {
        foreach (var t1 in _renderers)
        {
            Material[] mats = t1.materials;
            foreach (var t in mats)
            {
                _normalColors.Add(t.color);
            }
        }

        _grid = FindObjectOfType<Grid>();//Performance
    }
    public void SetTransparent(bool available)
    {
        for (var j = 0; j < _renderers.Length; j++)
        {
            for (var i = 0; i < _renderers[j].materials.Length; i++)
            {
                _renderers[j].materials[i].color = available ? Color.green : Color.red;                
            }
        }
    }
    public void SetColorNormal()
    {
        int last = 0;
        for(var j = 0; j < _renderers.Length; j++)
        {
            Material[] mats = _renderers[j].materials;
            for (var i = 0; i < mats.Length; i++)
            {
                _renderers[j].materials[i].color = _normalColors[last++];
            }
        }
    }
}
