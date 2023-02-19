using SBEPIS.Utils;
using UnityEngine;

namespace SBEPIS.Capturllection
{
	public class LerpTargetDequeTargetter : MonoBehaviour
	{
		public void Couple(LerpTargetAnimator animator)
		{
			DequeBox dequeBox = animator.GetComponent<DequeBox>();
			dequeBox.state.SetBool(DequeBox.IsCoupled, true);
		}
	}
}