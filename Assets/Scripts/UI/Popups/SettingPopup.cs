
using System;
using Game;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
	public class SettingPopup : Popup
	{
		[SerializeField] private Button _exit;
		
		[Space]
		[SerializeField] private Slider _sfx;
		[SerializeField] private Slider _music;
		
		public event Action OpenSettingsWindow;

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
			else if(UIManager.PreviousWindow == UIManager.GameWindow)
				UIManager.Open<GameWindow>();
		}

		private void OnEnable()
		{
			OpenSettingsWindow?.Invoke();
		}
	}
}