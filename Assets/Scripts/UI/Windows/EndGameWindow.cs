
using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace UI.Windows
{
	public class EndGameWindow : Window
	{
		[SerializeField] private ParticleSystem _victory;
		[SerializeField] private ParticleSystem _lose;

		[SerializeField] private Button _reloadLevel;
		[SerializeField] private Button _nextLevel;
		[SerializeField] private Button _rewardedAd;
		[SerializeField] private Button _exit;
		
		[SerializeField] private LocalizeStringEvent _localizeStringEvent;
		
		private ParticleSystem _currentEffect;
		
		public event Action ReloadCurrentLevel;
		public event Action LoadNextLevel;

		private void OnEnable()
		{
			_currentEffect.Play();
		}

		private void Start()
		{
			_reloadLevel.onClick.AddListener(ClickButtonReloadLevel);
			_nextLevel.onClick.AddListener(ClickButtonNextLevel);
			_exit.onClick.AddListener(ClickButtonExit);
		}

		public void OnVictory()
		{
			_nextLevel.gameObject.SetActive(true);
			_rewardedAd.gameObject.SetActive(false);
			
			_currentEffect = _victory;

			var localizedString = new LocalizedString("LocalizationAssets",
				"Victory");
			_localizeStringEvent.StringReference = localizedString;
		}

		public void OnLose()
		{
			_nextLevel.gameObject.SetActive(false);
			_rewardedAd.gameObject.SetActive(true);

			_currentEffect = _lose;
			
			var localizedString = new LocalizedString("LocalizationAssets", 
				"Lose");
			_localizeStringEvent.StringReference = localizedString;
		}

		private void ClickButtonReloadLevel()
		{
			ReloadCurrentLevel?.Invoke();
			
			UIManager.Open<GameWindow>();
		}

		private void ClickButtonNextLevel()
		{
			LoadNextLevel?.Invoke();
			
			UIManager.Open<GameWindow>();
		}

		private void ClickButtonExit()
		{
			UIManager.Open<MainWindow>();
		}

		private void OnDisable()
		{
			_victory.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			_lose.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
	}
}