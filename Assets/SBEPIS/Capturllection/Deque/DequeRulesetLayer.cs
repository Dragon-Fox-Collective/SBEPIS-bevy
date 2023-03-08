using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBEPIS.Capturllection
{
	public class DequeRulesetLayer : DequeRuleset
	{
		public List<DequeRuleset> rulesets;

		public override string dequeName => rulesets.Aggregate("", (name, ruleset) => name += ruleset.dequeName);

		public override Vector3 Tick(List<Storable> inventory, float deltaTime, Vector3 direction) => rulesets.AsEnumerable().Reverse().Aggregate(Vector3.zero, (_, deque) => deque.Tick(inventory, deltaTime, direction));
		
		public override bool CanFetchFrom(List<Storable> inventory, DequeStorable card) => rulesets.AsEnumerable().Reverse().Any(deque => deque.CanFetchFrom(inventory, card));
		
		public override int GetIndexToStoreInto(List<Storable> inventory) => rulesets[^1].GetIndexToStoreInto(inventory);
		public override int GetIndexToFlushBetween(List<Storable> inventory, Storable storable) => rulesets[^1].GetIndexToFlushBetween(inventory, storable);
		public override int GetIndexToInsertBetweenAfterStore(List<Storable> inventory, Storable storable, int originalIndex) => rulesets[^1].GetIndexToInsertBetweenAfterStore(inventory, storable, originalIndex);
		public override int GetIndexToInsertBetweenAfterFetch(List<Storable> inventory, Storable storable, int originalIndex) => rulesets[^1].GetIndexToInsertBetweenAfterFetch(inventory, storable, originalIndex);
		
		public override IEnumerable<Texture2D> GetCardTextures() => rulesets.SelectMany(ruleset => ruleset.GetCardTextures());
		public override IEnumerable<Texture2D> GetBoxTextures() => rulesets.SelectMany(ruleset => ruleset.GetBoxTextures());
	}
}
