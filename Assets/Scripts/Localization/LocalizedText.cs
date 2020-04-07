using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Localization
{
    /// <summary>
    ///  キー設定用
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] string key;
        public string GetKey => key;

        private Text _ui;
        private Text UI => _ui ?? GetComponent<Text>();
        private string value;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (UI != null) UI.text = LocalizationManager.GetLocalizedValue(key);
        }
#endif

        private Font default_font;
        private Font thsarabunnew;

        private IEnumerator Start()
        {
            default_font = UI.font;
            // mplus = Resources.Load<Font>("Fonts/mplus-1p-regular");
            thsarabunnew = Resources.Load<Font>("Fonts/THSarabunNew");

            while (!LocalizationManager.IsReady) 
            {
                yield return null;
            }
            
            if (string.IsNullOrEmpty(value))
            {
                var txt = LocalizationManager.GetLocalizedValue(key);
                UI.text = txt;
                if (LocalizationManager.lang == SystemLanguage.Thai){
                    UI.font = thsarabunnew;
                } else {
                    UI.font = default_font;
                }
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
                if (LocalizationManager.lang == SystemLanguage.Thai){
                    UI.font = thsarabunnew;
                } else {
                    UI.font = default_font;
                }
            } else {
                UI.text = value;
            }
        }

        /// <summary>
        /// キー変更
        /// </summary>
        public void TextWithKey(string key, params object[] args)
        {
            this.key = key;
            Text(args);
        }

        /// <summary>
        /// テキストを設定する
        /// </summary>
        public void Text(params object[] args)
        {
            string format = LocalizationManager.GetLocalizedFormat(key);
            value = string.Format(format, args);
            UI.text = value;
        }
    }
}