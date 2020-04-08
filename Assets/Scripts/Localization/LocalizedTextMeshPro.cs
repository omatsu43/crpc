using TMPro;
using UnityEngine;
using System.Collections;

namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextMeshPro : MonoBehaviour
    {
        [SerializeField] string key;
        public string GetKey => key;

        private TextMeshProUGUI _ui;
        private TextMeshProUGUI UI => _ui ?? GetComponent<TextMeshProUGUI>();
        private string value;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (UI != null) UI.text = LocalizationManager.GetLocalizedValue(key);
        }
#endif

        private IEnumerator Start()
        {
            while (!LocalizationManager.IsReady) 
            {
                yield return null;
            }
            
            if (string.IsNullOrEmpty(value))
            {
                var txt = LocalizationManager.GetLocalizedValue(key);
                UI.text = txt;
            } else {
                UI.text = value;
            }
        }

        void Update()
        {
            if (string.IsNullOrEmpty(value))
            {
                var txt = LocalizationManager.GetLocalizedValue(key);
                UI.text = txt;
            } else {
                UI.text = value;
            }
        }

        public void TextWithKey(string key, params object[] args)
        {
            this.key = key;
            Text(args);
        }

        public void Text(params object[] args)
        {
            string format = LocalizationManager.GetLocalizedFormat(key);
            value = string.Format(format, args);
            UI.text = value;
        }
    }
}