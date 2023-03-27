using SBEPIS.Utils;

namespace SBEPIS.Capturllection.CardState
{
	public class CardMoveToUpperDequeState : CardTargettingState
	{
		protected override LerpTarget GetTargetToTargetTo(Card card) => card.Owner.Deque.upperTarget;
	}
}
