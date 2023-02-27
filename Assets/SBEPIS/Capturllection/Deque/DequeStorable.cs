using SBEPIS.Controller;
using SBEPIS.Thaumaturgy;
using System;
using System.Collections.Generic;
using System.Linq;
using SBEPIS.Capturllection.CardState;
using SBEPIS.Utils;
using UnityEngine;

namespace SBEPIS.Capturllection
{
	[RequireComponent(typeof(Grabbable), typeof(SplitTextureSetup), typeof(Animator))]
	[RequireComponent(typeof(LerpTargetAnimator))]
	public class DequeStorable : MonoBehaviour
	{
		public bool isStoringAllowed = true;
		public readonly List<Func<bool>> storePredicates = new();
		
		public Grabbable grabbable { get; private set; }
		public SplitTextureSetup split { get; private set; }
		public CardStateMachine state { get; private set; }
		public LerpTargetAnimator animator { get; private set; }
		
		public DequeOwner owner { get; set; }
		
		public bool isStored => owner;
		public bool canStore => storePredicates.All(predicate => predicate.Invoke());
		
		private List<DequeCaptureLayout> layouts = new();
		
		private void Awake()
		{
			grabbable = GetComponent<Grabbable>();
			split = GetComponent<SplitTextureSetup>();
			state = new CardStateMachine(GetComponent<Animator>());
			animator = GetComponent<LerpTargetAnimator>();
			
			storePredicates.Add(() => isStoringAllowed);
			storePredicates.Add(() => !isStored);

			// This sucks but it's the best place to put it for now :/
			Capturellectainer container = GetComponent<Capturellectainer>();
			if (container)
				storePredicates.Add(() => container.capturedItem);

			Punchable punchable = GetComponent<Punchable>();
			if (punchable)
				storePredicates.Add(() => punchable.punchedBits.isPerfectlyGeneric);
		}
		
		public void SetStateGrabbed(bool grabbed) => state.isGrabbed = grabbed;
		
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out DequeCaptureLayout layout) && canStore)
				AddLayout(layout);
		}
		
		private void OnTriggerExit(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out DequeCaptureLayout layout) && layout.HasTemporaryTarget(this))
				RemoveLayout(layout);
		}

		private void AddLayout(DequeCaptureLayout layout)
		{
			layouts.Add(layout);
			if (layouts.Count == 1)
				state.isInLayoutArea = true;
			layout.AddTemporaryTarget(this);
		}
		
		private void RemoveLayout(DequeCaptureLayout layout)
		{
			layouts.Add(layout);
			if (layouts.Count == 1)
				state.isInLayoutArea = true;
			layout.AddTemporaryTarget(this);
		}
		
		public DequeCaptureLayout PopAllLayouts()
		{
			DequeCaptureLayout lastLayout = layouts[^1];
			while (layouts.Count > 0)
				RemoveLayout(layouts[0]);
			return lastLayout;
		}
	}
}