
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace UI
{
	public class CutoutMask : Image
	{
		public override Material materialForRendering
		{
			get
			{
				var material = new Material(base.materialForRendering);
				material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
				return material;
			}
		}
	}
}























