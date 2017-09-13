using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetAllChildLightsEnabled))]
public class SetAllChildLightsEnabledInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();

		SetAllChildLightsEnabled main = (SetAllChildLightsEnabled)target;

		if (GUILayout.Button("On"))
		{
			main.toggle (true);
		}
		if (GUILayout.Button("Off"))
		{
			main.toggle (false);
		}
	}
}
