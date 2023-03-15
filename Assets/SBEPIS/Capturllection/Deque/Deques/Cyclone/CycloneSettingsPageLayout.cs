using SBEPIS.Utils;
using UnityEngine;

namespace SBEPIS.Capturllection.Deques
{
	public class CycloneSettingsPageLayout : DequeSettingsPageLayout<CycloneDeque>
	{
		public SliderCardAttacher timePerCardSlider;
		public float timePerCardMin = 0.1f;
		public float timePerCardMax = 5;
		
		public void ResetTimePerCardSlider() => timePerCardSlider.SliderValue = ruleset.timePerCard.Map(timePerCardMin, timePerCardMax, 0, 1);
		public void ChangeTimePerCard(float percent) => ruleset.timePerCard = percent.Map(0, 1, timePerCardMin, timePerCardMax);
	}
}