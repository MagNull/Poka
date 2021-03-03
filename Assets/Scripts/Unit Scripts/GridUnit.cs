using System.Collections.Generic;
using UnityEngine;

namespace Unit_Scripts
{
    public class GridUnit : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Vector2Int size = Vector2Int.one;
        private List<Color> _normalColors = new List<Color>();
        private Grid _grid;
        public Vector2Int Size { get => size; set => size = value; }

        private void Awake()
        {
            foreach (var t1 in renderers)
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
            for (var j = 0; j < renderers.Length; j++)
            {
                for (var i = 0; i < renderers[j].materials.Length; i++)
                {
                    renderers[j].materials[i].color = available ? Color.green : Color.red;                
                }
            }
        }
        public void SetColorNormal()
        {
            int last = 0;
            for(var j = 0; j < renderers.Length; j++)
            {
                Material[] mats = renderers[j].materials;
                for (var i = 0; i < mats.Length; i++)
                {
                    renderers[j].materials[i].color = _normalColors[last++];
                }
            }
        }
    }
}
