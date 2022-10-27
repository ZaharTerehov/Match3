
using Game;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Buttons
{
	public class DefaultButton : Button
	{
		public override void OnPointerClick(PointerEventData eventData)
		{
			AudioManager.PlaySound("ClickButton");
			base.OnPointerClick(eventData);
		}
	}
}