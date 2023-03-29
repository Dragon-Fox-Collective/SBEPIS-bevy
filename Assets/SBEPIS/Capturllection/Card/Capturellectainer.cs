using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SBEPIS.Capturllection
{
	public class Capturellectainer : MonoBehaviour
	{
		public Capturellectable defaultCapturedItemPrefab;
		
		public CaptureEvent onCapture = new();
		[FormerlySerializedAs("onRetrieve")]
		public CaptureEvent onFetch = new();
		
		public Capturellectable CapturedItem { get; private set; }
		public bool HasCapturedItem => CapturedItem;
		public bool IsEmpty => !HasCapturedItem;
		
		private string originalName;
		
		private void Awake()
		{
			if (defaultCapturedItemPrefab)
			{
				Capturellectable item = Instantiate(defaultCapturedItemPrefab);
				Capture(item);
			}
		}
		
		public void Capture(Capturellectable item)
		{
			if (!item)
				return;
			if (HasCapturedItem)
				Fetch();
			
			CapturedItem = item;
			originalName = name;
			name += $" ({item})";
			item.gameObject.SetActive(false);
			item.transform.SetParent(transform);
			onCapture.Invoke(this, item);
			item.onCapture.Invoke(this, item);
		}
		
		public Capturellectable Fetch()
		{
			if (!HasCapturedItem)
				return null;
			
			Capturellectable item = CapturedItem;
			CapturedItem = null;
			name = originalName;
			item.gameObject.SetActive(true);
			item.transform.SetParent(null);
			onFetch.Invoke(this, item);
			item.onFetch.Invoke(this, item);
			return item;
		}
	}
	
	[Serializable]
	public class CaptureEvent : UnityEvent<Capturellectainer, Capturellectable> { }
}
