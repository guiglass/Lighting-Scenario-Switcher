using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LSS;

namespace LSS_Components
{
	[ExecuteInEditMode]
	public class LSS_ComponentBase : MonoBehaviour {

		public virtual void Awake () {
			GetComponentInfo (); // Ensure that a UniqueID gets created when this object is added.
		}

		public LSS_FrontEnd GetScenariosManager () {
			try {
				return ((GameObject)EditorUtility.InstanceIDToObject (LSS_UID.managerInstanceID)).GetComponent<LSS_FrontEnd> ();
			} catch {
				return null;
			}
		}
			
		public void DestroyLightingScenarioComponent () { //Called using message whenever the user wants to remove everything!
			DestroyImmediate (this);
		}

		// Only need to do this in editor due to game will not be creating/saving scenarios or lightmaps.
		#if UNITY_EDITOR 
		public int GetEntryIndex (object[] infos) {
			for (int i = 0; i < infos.Length; i++) {
				LSS_Models.ModelsInterface model = (LSS_Models.ModelsInterface)infos [i];
				if (model.GetUniqueID ().Equals (GetComponent<LSS_UID> ().uniqueId)) {
					return i;
				}
			}
			return -1;
		}

		public virtual List<object> UpdateOrInsertComponentObject (List<object> infos, bool insert) {
			int i = GetEntryIndex (infos.ToArray());
			if (i != -1) {
				infos [i] = GetComponentInfo ();
			} else if (insert) {
				infos.Add (GetComponentInfo ());		
			} else {
				infos = null;
			}
			return infos;
		}

		public List<object> GetModifiedList(object[] infos, bool insert = false) {
			List<object> _infos = new List<object>(infos);
			return UpdateOrInsertComponentObject (_infos, insert);
		}

		public virtual void OnDestroy () {
			/*try {
				if (GetComponents<LSS_ComponentBase> ().Length <= 1) {
					GetComponent<LSS_UID>().SendMessage("DestroyLightingScenarioComponent");
				}
			} catch {
			}*/
		}

		private void IsMultipleInstance () {
			if (gameObject.GetComponents (this.GetType ()).Length > 1) {
				DestroyLightingScenarioComponent ();
			}
		}

		public LSS_UID GetOrCreateUniqueID (GameObject go) {
			LSS_UID uid = go.GetComponent<LSS_UID> ();
			IsMultipleInstance ();
			if (uid == null) { // If this object does not have a presistant unique id, then add one.
				go.AddComponent<LSS_UID> ();
				return go.GetComponent<LSS_UID> ();
			} 
			return uid;
		}
			
		// The Overrides
		public virtual object GetComponentInfo () {
			Debug.LogWarning ("Attempted to run a non-overridden (empty) superclass: " + this.GetType());
			return null;
		}

		public virtual int UpdateComponentInfos (LSS_Models.LightingScenarioModel lightingScenariosModels, bool insert = false) {
			Debug.LogWarning ("Attempted to run a non-overridden (empty) superclass: " + this.GetType());
			return -1;
		}
		#endif
	}
}