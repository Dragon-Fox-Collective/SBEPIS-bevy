﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2021 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
	using Arbor.Internal;
	using Arbor.ValueFlow;

#if ARBOR_DOC_JA
	/// <summary>
	/// 型を指定する出力スロットクラス
	/// </summary>
	/// <remarks>
	/// 使用可能な属性 : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="HideSlotFields" /></description></item>
	/// </list>
	/// </remarks>
#else
	/// <summary>
	/// Output slot class specifying type
	/// </summary>
	/// <remarks>
	/// Available Attributes : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="HideSlotFields" /></description></item>
	/// </list>
	/// </remarks>
#endif
	[System.Serializable]
	public sealed class OutputSlotTypable : OutputSlotBase, IValueSetter, IOutputSlotAny
	{
		private IValueContainer _ValueContainer;
		private System.Type _ValueType;

		[SerializeField]
		[HideSlotFields]
		private ClassTypeReference _Type = new ClassTypeReference();

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットに格納されるデータの型
		/// </summary>
#else
		/// <summary>
		/// The type of data stored in the slot
		/// </summary>
#endif
		public override System.Type dataType
		{
			get
			{
				return _Type.type;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// OutputSlotTypableのコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// OutputSlotTypable constructor
		/// </summary>
#endif
		public OutputSlotTypable()
		{
			_Type = new ClassTypeReference();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// OutputSlotTypableのコンストラクタ
		/// </summary>
		/// <param name="type">スロットに格納するデータ型</param>
#else
		/// <summary>
		/// OutputSlotTypable constructor
		/// </summary>
		/// <param name="type">Data type to be stored in the slot</param>
#endif
		public OutputSlotTypable(System.Type type)
		{
			SetType(type);
		}

		internal void SetType(System.Type type)
		{
			_Type = new ClassTypeReference(type);
		}

		private IValueContainer GetContainer(System.Type valueType)
		{
			if (_ValueContainer != null && _ValueType == valueType)
			{
				return _ValueContainer;
			}

			_ValueType = valueType;

			if (ValueMediator.HasMediator(_ValueType))
			{
				_ValueContainer = ValueMediator.Get(_ValueType).CreateContainer();
			}
			else
			{
				if (_ValueContainer == null || !(_ValueContainer is IValueContainer<object>))
				{
					_ValueContainer = new ValueContainer<object>();
				}
			}

			return _ValueContainer;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を設定する
		/// </summary>
		/// <param name="value">設定する値</param>
#else
		/// <summary>
		/// Set the value
		/// </summary>
		/// <param name="value">The value to be set</param>
#endif
		public void SetValue(object value)
		{
			System.Type valueType = value != null ? value.GetType() : typeof(object);

			var container = GetContainer(valueType);
			if (container == null)
			{
				return;
			}

			var currentValue = container.GetValueObject();
			bool updated = currentValue != value;
			container.SetValueObject(value);
			Used(updated);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を設定する。
		/// </summary>
		/// <typeparam name="T">値の型</typeparam>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// Set the value.
		/// </summary>
		/// <typeparam name="T">Value type</typeparam>
		/// <param name="value">Value</param>
#endif
		public void SetValue<T>(T value)
		{
			var container = GetContainer(typeof(T));

			var c = container as IValueContainer<T>;
			if (c != null)
			{
				var currentValue = c.GetValue();
				bool updated = !EqualityComparerEx<T>.Default.Equals(currentValue, value);
				c.SetValue(value);
				Used(updated);
			}
			else if (container != null)
			{
				var valueObject = (object)value;
				var currentValue = container.GetValueObject();
				bool updated = currentValue != valueObject;
				container.SetValueObject(valueObject);
				Used(updated);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を返す。
		/// </summary>
		/// <returns>値を返す。</returns>
#else
		/// <summary>
		/// Returns the value.
		/// </summary>
		/// <returns>Returns the value.</returns>
#endif
		public override object GetValue()
		{
			if (_ValueContainer == null)
			{
				return null;
			}
			return _ValueContainer.GetValueObject();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を取得する。
		/// </summary>
		/// <typeparam name="T">値の型</typeparam>
		/// <returns>値を返す。</returns>
#else
		/// <summary>
		/// Get the value.
		/// </summary>
		/// <typeparam name="T">Value type</typeparam>
		/// <returns>Returns a value.</returns>
#endif
		public T GetValue<T>()
		{
			if (_ValueContainer.TryGetValue<T>(out T value))
			{
				return value;
			}

			return default(T);
		}

		bool IOutputSlotAny.TryGetValue<T>(out T value)
		{
			return _ValueContainer.TryGetValue<T>(out value);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を文字列に変換して返す。
		/// </summary>
		/// <returns>変換した文字列。</returns>
#else
		/// <summary>
		/// Converts a value to a string and returns it.
		/// </summary>
		/// <returns>Converted string.</returns>
#endif
		public override string GetValueString()
		{
			if (_ValueContainer == null)
			{
				return "null";
			}
			return _ValueContainer.ToString();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値の型を返す。
		/// </summary>
		/// <returns>値の型を返す。</returns>
#else
		/// <summary>
		/// Returns the type of the value.
		/// </summary>
		/// <returns>Returns the type of the value.</returns>
#endif
		public override System.Type GetValueType()
		{
			return _ValueType;
		}

		void IValueSetter.SetValueObject(object value)
		{
			SetValue(value);
		}

		void OnTypeChanged()
		{
			ConnectableTypeChanged();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ISerializationCallbackReceiver.OnAfterDeserializeから呼び出される。
		/// </summary>
#else
		/// <summary>
		/// Called from ISerializationCallbackReceiver.OnAfterDeserialize.
		/// </summary>
#endif
		protected override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			_Type.onTypeChanged -= OnTypeChanged;
			_Type.onTypeChanged += OnTypeChanged;
		}
	}
}