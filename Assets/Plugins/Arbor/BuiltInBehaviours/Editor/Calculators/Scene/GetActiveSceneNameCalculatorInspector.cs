﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2021 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor.Calculators
{
	using Arbor.Calculators;
	using ArborEditor.Inspectors;

	[CustomEditor(typeof(GetActiveSceneNameCalculator))]
	internal sealed class GetActiveSceneNameCalculatorInspector : InspectorBase, ICalculatorBehaviourEditor
	{
		public bool IsResizableNode()
		{
			return false;
		}

		public float GetNodeWidth()
		{
			return 200f;
		}

		protected override void OnRegisterElements()
		{
			RegisterProperty("_Output");
		}
	}
}