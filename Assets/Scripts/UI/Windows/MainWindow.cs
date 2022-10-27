
using UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
	public class MainWindow : Window
	{
		[SerializeField] private Button _play;
		[SerializeField] private Button _setting;
		[SerializeField] private Button _localization;
		[SerializeField] private Button _exit;

		private void Start()
		{
			_play.onClick.AddListener(ClickButtonPlay);
			_setting.onClick.AddListener(ClickButtonSetting);
			_localization.onClick.AddListener(ClickButtonLocalization);
			_exit.onClick.AddListener(ClickButtonExit);
		}

		private void ClickButtonPlay()
		{
			UIManager.Open<LevelSelectionWindow>();
		}

		private void ClickButtonSetting()
		{
			UIManager.Open<SettingPopup>();
		}
		
		private void ClickButtonLocalization()
		{
			UIManager.Open<LocalizationPopup>();
		}

		private void ClickButtonExit()
		{
			UIManager.Open<ExitPopup>();
		}
	}
}
