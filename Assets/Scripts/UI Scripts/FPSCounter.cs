using UnityEngine;

namespace UI_Scripts
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Text text;
        public float updateInterval = 0.5F;
        private double _lastInterval;
        private int _frames = 0;
        private float _fps;
        void Start()
        {
            _lastInterval = Time.realtimeSinceStartup;
            _frames = 0;
        }

        void OnGUI()
        {
            GUILayout.Label("" + _fps.ToString("f2"));
        }

        void Update()
        {
            ++_frames;
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > _lastInterval + updateInterval)
            {
                _fps = (float)(_frames / (timeNow - _lastInterval));
                _frames = 0;
                _lastInterval = timeNow;
            }
            text.text = ((int)_fps).ToString();
        }
    }
}
