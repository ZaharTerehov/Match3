
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
	public class LocalizationPopup : Popup
	{
		[SerializeField] private TextMeshProUGUI _headerText;
		
		[SerializeField] private Button _exit;

		[Space]
		[SerializeField] private List<Button> _buttonLocalizations = new List<Button>();

		private Image _lastSelectedButtonImage;

		private Tween _selectionAnimation;

		public event Action<int> IndexSelectedLanguage;

		private void Start()
		{
			_exit.onClick.AddListener(ClickButtonExit);

			foreach (var buttonLocalization in _buttonLocalizations)
			{
				buttonLocalization.onClick.AddListener(() => {  ClickButton(buttonLocalization);});	
			}
		}
		
		private async void ClickButtonExit()
		{
			await _selectionAnimation;

			UIManager.Open<MainWindow>();
		}

		private void ClickButton(Button button)
		{
			var indexButtonLocalization = _buttonLocalizations.IndexOf(button);

			PlayButtonClickAnimation(button);
			IndexSelectedLanguage?.Invoke(indexButtonLocalization);
		}

		public void FindButtonByIndex(int indexLanguage)
		{
			var buttonLocalization = _buttonLocalizations[indexLanguage];
			PlayButtonClickAnimation(buttonLocalization);
		}

		private async void PlayButtonClickAnimation(Button button)
		{
			await _selectionAnimation;
			
			var image = button.gameObject.GetComponent<Image>();
			image.raycastTarget = false;
			
			var buttonGameObject = button.gameObject;
			var startPositionY = buttonGameObject.transform.position.y;
			var startScale = buttonGameObject.transform.localScale;

			var languageSelectionAnimation = DOTween.Sequence().Pause();
			
			languageSelectionAnimation.Join(button.gameObject.transform.DOScale(1.1f, 
				0.3f).SetEase(Ease.Linear).Pause());
			languageSelectionAnimation.Join(button.gameObject.transform.DOMoveY(startPositionY + 0.3f,
				0.3f).SetEase(Ease.OutBack).Pause());

			languageSelectionAnimation.Play().OnComplete(() =>
			{
				button.gameObject.transform.DOMoveY(startPositionY, 0.3f).SetEase(Ease.OutCubic);
				button.gameObject.transform.DOScale(startScale, 0.3f).SetEase(Ease.OutCubic);
			});

			if(_lastSelectedButtonImage != null)
				_lastSelectedButtonImage.DOFade(0.6f, 1f);
			
			_selectionAnimation = image.DOFade(1f, 1f).OnComplete(() =>
			{
				image.raycastTarget = true;
				_lastSelectedButtonImage = image;
			});
		}

		private void OnEnable()
		{
			_headerText.alpha = 0;
			_headerText.DOFade(1, 2f);
		}
	}
}