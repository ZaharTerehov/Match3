
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Popups;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UIManager : MonoBehaviour
	{
		private static UIManager _instance;

		[Header("Windows")]
		[SerializeField] private MainWindow _mainWindow;
		[SerializeField] private GameWindow _gameWindow;
		[SerializeField] private LevelSelectionWindow _levelSelectionWindow;
		[SerializeField] private EndGameWindow _endGameWindow;

		[Space]
		[SerializeField] private List<Window> _windows = new List<Window>();

		[Space]
		[SerializeField] private Image _fadePanelPopup;

		[Space]
		[Header("Popups")]
		[SerializeField] private SettingPopup _settingPopup;
		[SerializeField] private LocalizationPopup _localizationPopup;
		[SerializeField] private List<Popup> _popups = new List<Popup>();

		[SerializeField] private RectTransform _mask;
		[SerializeField] private Image _imageRaycastTarget;

		private Window _currentWindow;
		private Window _previousWindow;
		
		private Type _currentTypeWindow;
		
		private Popup _lastPopup;

		public static GameWindow GameWindow => _instance._gameWindow;
		public static MainWindow MainWindow => _instance._mainWindow;
		public static LevelSelectionWindow LevelSelectionWindow => _instance._levelSelectionWindow;
		public static EndGameWindow EndGameWindow => _instance._endGameWindow;
		public static Window CurrentWindow => _instance._currentWindow;
		public static Window PreviousWindow => _instance._previousWindow;
		
		private readonly Dictionary<Type, Window> _windowsDictionary = new Dictionary<Type, Window>();
		private readonly Dictionary<Type, Popup> _popupsDictionary = new Dictionary<Type, Popup>();

		private void Awake()
		{
			if (_instance == null)
				_instance = this;
			else
				Destroy(gameObject);
			
			FillDictionary(_popupsDictionary, _popups);
			FillDictionary(_windowsDictionary, _windows);
		}

		private void Start()
		{
			_settingPopup.OpenSettingsWindow += Analytics.SettingsOpened;
			_localizationPopup.IndexSelectedLanguage += LanguageManager.SetLanguage;

			_localizationPopup.FindButtonByIndex(LanguageManager.IndexLanguage);
			
			Open<LoadWindow>();
		}
		
		private void FillDictionary<T>(Dictionary<Type, T> dictionary, List<T> list)
		{
			foreach (var element in list)
			{
				dictionary.Add(element.GetType(), element);
			}
		}

		private async UniTask CloseCurrent(Type typeWindow)
		{
			if (typeWindow.IsSubclassOf(typeof(Window)))
			{
				if (_currentWindow != null)
				{
					_previousWindow = _currentWindow;

					_imageRaycastTarget.raycastTarget = true;
					await DOTween.To(() => _instance._mask.sizeDelta, v => _instance._mask.sizeDelta = v,
						new Vector2(-50,-50), 1.8f).SetEase(Ease.OutSine);

					_currentWindow.Hide();
				}
			}
			else if (typeWindow.IsSubclassOf(typeof(Popup)))
			{
				if (_lastPopup != null)
				{
					await _lastPopup.CloseStart();
					_lastPopup = null;
				}
			}
		}

		public static async void Open<T>()
		{
			if (_instance._currentTypeWindow != null && !typeof(T).IsSubclassOf(typeof(Popup)))
			{
				await _instance.CloseCurrent(_instance._currentTypeWindow);
				_instance._fadePanelPopup.gameObject.SetActive(false);
			}

			_instance._currentTypeWindow= typeof(T);

			if (typeof(T).IsSubclassOf(typeof(Window)))
			{
				var window = _instance._windowsDictionary[typeof(T)];

				_instance._currentWindow = window;

				_instance._currentWindow.Show();

				await DOTween.To(() => _instance._mask.sizeDelta, v => _instance._mask.sizeDelta = v,
					new Vector2(1200,1300), 1.8f).SetEase(Ease.OutSine);
				_instance._imageRaycastTarget.raycastTarget = false;
			}
			else if (typeof(T).IsSubclassOf(typeof(Popup)))
			{
				_instance._previousWindow = _instance._currentWindow;

				var popup = _instance._popupsDictionary[typeof(T)];
				
				if(popup.UseFadePanel)
					_instance._fadePanelPopup.gameObject.SetActive(true);
				
				await popup.OpenStart();
				_instance._lastPopup = popup;
			}
		}
	}
}