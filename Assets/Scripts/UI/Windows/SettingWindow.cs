
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
	public class SettingWindow : Window
	{
		[SerializeField] private Button _exit;
		
		[Space]
		[SerializeField] private Slider _sfx;
		[SerializeField] private Slider _music;

		private void Awake()
		{
			_sfx.value = AudioManager.SFXVolume;
			_music.value = AudioManager.MusicVolume;
		}

		private void Start()
		{
			_exit.onClick.AddListener(ClickButtonExit);
			
			_sfx.onValueChanged.AddListener(AudioManager.SetSFXVolume);
			_music.onValueChanged.AddListener(AudioManager.SetMusicVolume);
		}

		private void ClickButtonExit()
		{
			if (UIManager.PreviousWindow == UIManager.MainWindow)
				UIManager.Open<MainWindow>();
			else
				UIManager.Open<GameWindow>();
		}
	}
}