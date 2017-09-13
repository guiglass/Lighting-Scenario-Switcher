using UnityEngine;
using UnityEditor;

namespace LSS
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LSS_UID))]
	public class LSS_UIDEditor : Editor
	{
		LSS_UID targetObject;
		bool isScenariosManager = false;

		public void OnEnable () {
			targetObject = (LSS_UID)target;
			isScenariosManager = targetObject.GetComponent<LSS_FrontEnd> () != null;
		}

		public override void OnInspectorGUI() {
			GUIStyle customLabel;
			GUIStyle customButton;

			if (isScenariosManager || LSS_UID.managerInstance != null) {

				if (isScenariosManager) {
					EditorGUILayout.Space ();
					customLabel = new GUIStyle ("Label");
					customLabel.alignment = TextAnchor.MiddleCenter;
					customLabel.fontSize = 16;
					customLabel.normal.textColor = Color.blue;
					customLabel.fontStyle = FontStyle.Bold;

					GUILayout.Label ("SCENE IDENTIFIER", customLabel);
				} 



				GUILayout.Label ("Presistent Unique ID:");
				customLabel = new GUIStyle ("TextField");
				customLabel.fontStyle = FontStyle.Bold;
				GUILayout.TextField (targetObject.uniqueId, customLabel);




				EditorGUILayout.Space ();
				if (!isScenariosManager) {
					customButton = new GUIStyle ("Button");
					customButton.fontSize = 12;
					customButton.fontStyle = FontStyle.Bold;
					if (GUILayout.Button ("LIGHTING SCENARIO MANAGER", customButton)) {
						Selection.instanceIDs = new int[] { LSS_UID.managerInstance.GetInstanceID () };
					}
				}


				EditorGUILayout.Space ();

				customLabel = new GUIStyle ("Label");
				customLabel.fontSize = 18;
				customLabel.normal.textColor = Color.yellow;
				customLabel.fontStyle = FontStyle.Bold;
				GUILayout.Label ("CAUTION:", customLabel);

				customLabel = new GUIStyle ("Label");
				customLabel.fontSize = 11;
				customLabel.fontStyle = FontStyle.BoldAndItalic;
				GUILayout.Label ("Unique One-Time ID", customLabel);
				EditorGUILayout.HelpBox ("Do not remove this reference,\nLighting Scenarios may loose sync with object.", MessageType.None);

				EditorGUILayout.Space ();



			} else {
				EditorGUILayout.HelpBox("There is no Lighting Scenarios manager in this scene.", MessageType.Warning);

				EditorGUILayout.Space ();

				customButton = new GUIStyle ("Button");
				customButton.normal.textColor = Color.blue;
				customButton.fontStyle = FontStyle.Bold;
				if (GUILayout.Button ("CREATE NEW LIGHTING MANAGER", customButton)) {

					if (!EditorUtility.DisplayDialog (
						"New Lighting Scenario Manager?", 
						"This will create an empty gameobject in the scene and add a LSS_FrontEnd lighting manager script\nIt will then be highlighted.", 
						"OK", 
						"Cancel")) {
						return;
					} 

					targetObject.CreateNewScenariosManager ();
				}

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				customButton = new GUIStyle ("Button");
				customButton.fontSize = 14;
				customButton.normal.textColor = Color.red;
				customButton.fontStyle = FontStyle.Bold;
				if (GUILayout.Button ("REMOVE ALL UNIQUE IDS!!", customButton)) {
					if (!EditorUtility.DisplayDialog (
						"Are You Sure?", 
						"This will remove all UIDs for lighting scenarios in this scene.\n\nOnce these references have been deleted then all previously baked and stored lighting scenarios for this scene will no longer be valid!", 
						"OK", 
						"Cancel")) {
						return;
					} 
					if (!EditorUtility.DisplayDialog (
						"Are You Really Sure?", 
						"This can't be undone...", 
						"YES!!", 
						"Nevermind")) {
						return;
					} 

					var uids = GameObject.FindObjectsOfType<LSS_UID> ();
					foreach (LSS_UID uid in uids) {
						uid.SendMessage("DestroyLightingScenarioComponent");
					}
				}

				EditorGUILayout.Space ();

			}
			if (!isScenariosManager && LSS_UID.managerInstance != null ) {
				base.OnInspectorGUI ();
			}
		}
	}
}