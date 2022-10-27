
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
	public class ExitPopup : Popup
	{
		[SerializeField] private Button _yes;
		[SerializeField] private Button _no;

		private void Start()
		{
			_yes.onClick.AddListener(ClickButtonYes);
			_no.onClick.AddListener(ClickButtonNo);
		}

		private void ClickButtonYes()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			Application.Quit();
		}

		private void ClickButtonNo()
		{
			UIManager.Open<MainWindow>();
		}
	}
}