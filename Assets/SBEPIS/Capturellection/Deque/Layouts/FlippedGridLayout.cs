using System;
using System.Collections.Generic;
using System.Linq;
using SBEPIS.Capturellection.Storage;
using SBEPIS.Utils;
using UnityEngine;

namespace SBEPIS.Capturellection.Deques
{
	[Serializable]
	public class FlippedGridLayout : DequeLayoutBase
	{
		public Vector2 offset = new(0.1f, 0.1f);
		
		public void Layout<TState>(List<Storable> inventory, TState state) where TState : DirectionState, FlippedState
		{
			foreach (Storable storable in inventory)
				storable.Layout(Quaternion.Euler(0, 0, -60) * state.Direction);
			
			int numCardsX = Mathf.CeilToInt(Mathf.Sqrt(inventory.Count));
			Vector2 gridCount = new(numCardsX, Mathf.Ceil((float)inventory.Count / numCardsX));
			
			Vector3 maxSize = inventory.Select(storable => storable.MaxPossibleSize).Aggregate(ExtensionMethods.Max);
			Vector3x2 direction = new(state.Direction, Quaternion.Euler(0, 0, -90) * state.Direction);
			Vector3x2 absDirection = direction.Select(Mathf.Abs);
			Vector2 lengthSum = absDirection.AggregateIndex((index, absDir) => offset[index] * (gridCount[index] - 1) + Vector3.Project(maxSize, absDir).magnitude * gridCount[index]);
			
			Vector3 pos = (-lengthSum / 2 * direction).Sum();
			foreach (IEnumerable<Storable> row in inventory.Divide(numCardsX))
			{
				Vector2 length = Vector3x2.Project(maxSize, absDirection).magnitude;
				
				pos += direction.y * length.y / 2;
				
				foreach (Storable storable in row)
				{
					pos += direction.x * length.x / 2;
					
					storable.Position = pos;
					storable.Rotation = (!state.FlippedStorables.Contains(storable) && !storable.HasAllCardsEmpty ? Quaternion.Euler(0, 170, 0) : Quaternion.identity) * DequeLayout.GetOffsetRotation(state.Direction);
					
					pos += direction.x * (offset.x + length.x / 2);
				}
				pos -= direction.x * (lengthSum.x + offset.x);
				
				pos += direction.y * (offset.y + length.y / 2);
			}
		}
	}
	
	[Serializable]
	public class FlippedGridState : InventoryState, DirectionState, FlippedState
	{
		public List<Storable> Inventory { get; set; } = new();
		public Vector3 Direction { get; set; }
		public List<Storable> FlippedStorables { get; } = new();
	}
}
