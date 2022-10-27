
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace UI
{
	public class LanguageManager : MonoBehaviour
	{
		private static int _indexLanguage
		{
			get => PlayerPrefs.GetInt("IndexLanguage");
			set => PlayerPrefs.SetInt("IndexLanguage", value);
		}

		public static int IndexLanguage => _indexLanguage;

		private void Start()
		{
			InitLanguage();
		}

		private async void InitLanguage()
		{
			await LocalizationSettings.InitializationOperation;
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_indexLanguage];
		}
		
		public static void SetLanguage(int indexLanguage)
		{
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[indexLanguage];
			_indexLanguage = indexLanguage;
		}
	}
}