
using UI.Windows;

namespace UI.Popups
{
	public class NextQuestPopup : Popup
	{
		protected override void OpenCompleted()
		{
			base.OpenCompleted();
			gameObject.SetActive(false);
			UIManager.Open<GameWindow>();
		}
	}
}