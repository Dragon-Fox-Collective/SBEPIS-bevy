using System;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using SBEPIS.Capturellection.Storage;
using SBEPIS.Controller;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace SBEPIS.Capturellection
{
	[RequireComponent(typeof(Grabber))]
	public class Capturellector : ValidatedMonoBehaviour
	{
		[SerializeField, Self] private Grabber grabber;
		
		[SerializeField, Anywhere(Flag.Optional)] private Inventory inventory;
		public Inventory Inventory
		{
			get => inventory;
			set => inventory = value;
		}
		
		public void OnCapture(CallbackContext context)
		{
			if (!isActiveAndEnabled || !context.performed || !grabber.HeldGrabbable)
				return;
			
			if (grabber.HeldGrabbable.TryGetComponent(out InventoryStorable card))
				RetrieveFromInventory(card).Forget();
			else if (grabber.HeldGrabbable.TryGetComponent(out CaptureContainer container))
				RetrieveFromContainer(container);
			else if (grabber.HeldGrabbable.TryGetComponent(out Capturellectable item))
				CaptureAndGrabCard(item).Forget();
		}
		
		public async UniTask<CaptureContainer> CaptureAndGrabCard(Capturellectable item)
		{
			if (item.IsBeingCaptured)
				return null;
			
			StoreResult result = await inventory.StoreItem(item);
			if (!result.wasSuccessful) return null;
			
			MoveEjectedItem(result.card, result.ejectedItem);
			TryGrab(result.card.transform);
			return result.container;
		}
		
		private static void MoveEjectedItem(InventoryStorable card, Capturellectable ejectedItem)
		{
			if (!ejectedItem)
				return;
			
			Action<Vector3, Quaternion> move = ejectedItem.TryGetComponent(out Rigidbody ejectedItemRigidbody)
				? ejectedItemRigidbody.Move
				: ejectedItem.transform.SetPositionAndRotation;
			if (card.DequeElement.ShouldBeDisplayed)
				move(card.transform.position, card.transform.rotation);
			else
				move(card.DequeElement.Deque.transform.position, card.DequeElement.Deque.transform.rotation);
		}
		
		private Capturellectable RetrieveFromContainer(CaptureContainer container)
		{
			Capturellectable item = container.Fetch();
			if (item) TryGrab(item.transform);
			return item;
		}
		
		public async UniTask<Capturellectable> RetrieveFromInventory(InventoryStorable card)
		{
			if (!inventory.CanFetch(card) || card.IsBeingFetched)
				return null;
			
			FetchResult result = await inventory.FetchItem(card);
			if (!result.wasSuccessful) return null;
			
			if (result.fetchedItem) TryGrab(result.fetchedItem.transform);
			return result.fetchedItem;
		}
		
		private void TryGrab(Transform item)
		{
			item.SetPositionAndRotation(grabber.transform.position, grabber.transform.rotation);
			
			if (item.TryGetComponent(out Grabbable itemGrabbable))
				grabber.GrabManually(itemGrabbable, true);
			else if (item.TryGetComponentInChildren(out Collider itemCollider))
				grabber.GrabManually(itemCollider, true);
		}
	}
}
