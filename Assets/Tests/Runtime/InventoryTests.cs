using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using SBEPIS.Capturellection;
using SBEPIS.Capturellection.Storage;
using SBEPIS.Utils;
using SBEPIS.Tests.Scenes;
using UnityEngine.TestTools;

namespace SBEPIS.Tests
{
	public class InventoryTests : TestSceneSuite<InventoryScene>
	{
		[UnityTest]
		public IEnumerator StoringItem_GetsCard() => UniTask.ToCoroutine(async () =>
		{
			StorableStoreResult result = await Scene.inventory.StoreItem(Scene.item);
			Assert.That(result.card, Is.Not.Null);
		});
		
		[UnityTest]
		public IEnumerator FetchingItem_GetsOriginalItem() => UniTask.ToCoroutine(async () =>
		{
			StorableStoreResult result = await Scene.inventory.StoreItem(Scene.item);
			Capturellectable item = await Scene.inventory.FetchItem(result.card);
			Assert.That(item, Is.EqualTo(Scene.item));
		});

		[Test]
		public void Inventory_HasStartingCards()
		{
			Assert.That(Scene.inventory.Count(), Is.EqualTo(1));
		}
		
		[UnityTest]
		public IEnumerator FlushingCard_SetsCardParent() => UniTask.ToCoroutine(async () =>
		{
			await Scene.inventory.FlushCard(Scene.card);
			Assert.That(Scene.card.transform.parent, Is.EqualTo(Scene.inventory.CardParent));
		});
		
		[UnityTest]
		public IEnumerator FetchingCard_UnsetsCardParent() => UniTask.ToCoroutine(async () =>
		{
			await Scene.inventory.FlushCard(Scene.card);
			await Scene.inventory.FetchCard(Scene.card);
			Assert.That(Scene.card.transform.parent, Is.Null);
		});
	}
}