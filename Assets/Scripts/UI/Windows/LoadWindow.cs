
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
	public class LoadWindow : Window
	{
		[SerializeField] private Text _headerTexts;

		private void Start()
		{
			AnimationLoad();
		}

		private async void AnimationLoad()
		{
			await _headerTexts.DOText("FRUIT\nMATCH-3\nGAME", 3f).SetEase(Ease.Linear);

			UIManager.Open<MainWindow>();
		}
	}
}