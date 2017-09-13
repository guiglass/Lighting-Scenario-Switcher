using UnityEngine;
using UnityEditor;
using LSS;

namespace LSS_Components
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LSS_ComponentBase), true)]
	public class LSS_ComponentEditor : Editor
	{
		LSS_ComponentBase targetObject;

		public void OnEnable () {
			targetObject = (LSS_ComponentBase)target;
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI ();

			var scenariosManager = targetObject.GetScenariosManager ();// (GameObject)EditorUtility.InstanceIDToObject (LSS_UID.managerInstanceID);
			if (scenariosManager != null) {
				
				var customButton = new GUIStyle ("Button");
				customButton.fontSize = 12;
				customButton.fontStyle = FontStyle.Bold;
				if (GUILayout.Button ("UPDATE COMPONENT", customButton)) {
					if (!EditorUtility.DisplayDialog (
							"Update Values For Component?", 
							"Update Lighting Scenarios component:\n\""+targetObject.GetType()+"\"\n in \""+scenariosManager.resourceFolder+"\" resources folder?", 
							"OK", 
							"Cancel")) {
						return;
					} 

					if (targetObject.UpdateComponentInfos (scenariosManager.LoadJsonFile ()) == -1) {
						if (!EditorUtility.DisplayDialog (
							    "Update Unsuccessful", 
							    "\"" + scenariosManager.GetResourcesStorageDirectory () + scenariosManager.jsonFileName + "\"\ndid not contain the unique ID: " + targetObject.GetComponent<LSS_UID> ().uniqueId, 
							    "Insert",
							    "Cancel")) {
							return;
						}
						AssetDatabase.Refresh (); // Refresh so any json files can be found and loaded.
						targetObject.UpdateComponentInfos (scenariosManager.LoadJsonFile (), true);
					}
				}
			} else {
				GUIStyle customLabel = new GUIStyle ("Label");
				customLabel.fontSize = 14;
				customLabel.normal.textColor = Color.blue;
				customLabel.fontStyle = FontStyle.BoldAndItalic;
				GUILayout.Label ("No Lighting Scenarios Manager In Scene", customLabel);


			}
		}
	}
}