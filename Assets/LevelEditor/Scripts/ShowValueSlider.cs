
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor.Scripts
{
	public class ShowValueSlider : MonoBehaviour
	{
		[SerializeField] private Text _textValue;
		[SerializeField] private Slider _slider;

		private void Start()
		{
			_slider.onValueChanged.AddListener(SetValue);
		}

		private void SetValue(float value)
		{
			_textValue.text = _slider.value.ToString();
		}
	}
}