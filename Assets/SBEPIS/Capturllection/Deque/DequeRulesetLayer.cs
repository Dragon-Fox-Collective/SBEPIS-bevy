using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBEPIS.Capturllection
{
	public class DequeRulesetLayer : DequeRuleset<DequeRulesetLayerState>
	{
		public List<DequeRuleset> rulesets;
		
		public override string dequeName => rulesets.Aggregate("", (name, ruleset) => name + ruleset.dequeName);
		
		public override void Tick(List<Storable> inventory, DequeRulesetLayerState state, float deltaTime, Vector3 direction) => rulesets.Zip(state.states).Reverse().Do(zip => zip.Item1.Tick(inventory, zip.Item2, deltaTime, direction));
		public override Vector3 GetMaxPossibleSizeOf(List<Storable> inventory) => rulesets[0].GetMaxPossibleSizeOf(inventory);
		
		public override bool CanFetchFrom(List<Storable> inventory, DequeRulesetLayerState state, DequeStorable card) => rulesets.Zip(state.states).Reverse().Any(zip => zip.Item1.CanFetchFrom(inventory, zip.Item2, card));
		
		public override int GetIndexToStoreInto(List<Storable> inventory, DequeRulesetLayerState state) => rulesets[^1].GetIndexToStoreInto(inventory, state.states[^1]);
		public override int GetIndexToFlushBetween(List<Storable> inventory, DequeRulesetLayerState state, Storable storable) => rulesets[^1].GetIndexToFlushBetween(inventory, state.states[^1], storable);
		public override int GetIndexToInsertBetweenAfterStore(List<Storable> inventory, DequeRulesetLayerState state, Storable storable, int originalIndex) => rulesets[^1].GetIndexToInsertBetweenAfterStore(inventory, state.states[^1], storable, originalIndex);
		public override int GetIndexToInsertBetweenAfterFetch(List<Storable> inventory, DequeRulesetLayerState state, Storable storable, int originalIndex) => rulesets[^1].GetIndexToInsertBetweenAfterFetch(inventory, state.states[^1], storable, originalIndex);
		
		public override DequeRulesetState GetNewState()
		{
			DequeRulesetLayerState state = new();
			state.states = rulesets.Select(ruleset => ruleset.GetNewState()).ToList();
			return state;
		}
		
		public override IEnumerable<Texture2D> GetCardTextures() => rulesets.SelectMany(ruleset => ruleset.GetCardTextures());
		public override IEnumerable<Texture2D> GetBoxTextures() => rulesets.SelectMany(ruleset => ruleset.GetBoxTextures());
	}
}
