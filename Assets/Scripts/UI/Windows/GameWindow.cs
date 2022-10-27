
using System;
using System.Collections.Generic;
using System.Linq;
using Services.Advertising;
using TMPro;
using UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
	public class GameWindow : Window
	{
		[SerializeField] private Button _setting;
		[SerializeField] private Button _reloadLevel;
		[SerializeField] private Button _exit;
		
		[SerializeField] private TextMeshProUGUI _countMoves;
		[SerializeField] private TextMeshProUGUI _countPoint;
		[SerializeField] private TextMeshProUGUI _levelGoal;

		[SerializeField] private Slider _progressBar;

		[SerializeField] private List<Prefabs> _prefabs = new List<Prefabs>();
		[SerializeField] private SpriteRenderer _destructionObject;

		public event Action ReloadCurrentLevel;

		private void Start()
		{
			_setting.onClick.AddListener(ClickButtonSetting);
			_reloadLevel.onClick.AddListener(ClickButtonReloadLevel);
			_exit.onClick.AddListener(ClickButtonExit);
		}

		public void OnSetGoalLevel(string destructionType, int count)
		{
			var spriteDestructionObject = _prefabs.First(p => p.Name == destructionType).Sprite;
			_destructionObject.sprite = spriteDestructionObject;
			_levelGoal.text = $"X {count}";
		}

		public void OnSetCountMoves(string countMoves)
		{
			_countMoves.text = countMoves;
		}

		public void OnSetCountPoint(string countPoint, string neededPoint)
		{
			_countPoint.text = $"{countPoint} / {neededPoint}";
		}

		public void OnSetValueOnProgressBar(float value)
		{
			_progressBar.value = value;
		}	
		
		private void ClickButtonSetting()
		{
			UIManager.Open<SettingPopup>();
		}

		private void ClickButtonReloadLevel()
		{
			ReloadCurrentLevel?.Invoke();
		}

		private void ClickButtonExit()
		{
			UIManager.Open<MainWindow>();
		}

		private void OnEnable()
		{
			AdvertisingManager.ShowBannerAd();
		}

		private void OnDisable()
		{
			AdvertisingManager.HideBannerAd();
		}

		[Serializable]
		private class Prefabs
		{
			public Sprite Sprite;
			public string Name;
		}
	}
}
