// Script for generating a unique but persistent string identifier belonging to this 
// component
//
// We construct the identifier from two parts, the scene name and a guid.
// 
// The guid is guaranteed to be unique across all components loaded at 
// any given time. In practice this means the ID is unique within this scene. We 
// then append the name of the scene to it. This ensures that the identifier will be 
// unique accross all scenes. (as long as your scene names are unique)
// 
// The identifier is serialised ensuring it will remaing the same when the level is 
// reloaded
//
// This code copes with copying the game object we are part of, using prefabs and 
// additive level loading
//
// Written by Diarmid Campbell 2017 - Modified (for incorporation into the Lighting Scenario Switcher) by Grant Olsen 2017
//
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using LSS_Components;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace LSS
{
	[ExecuteInEditMode]
	public class LSS_UID : MonoBehaviour {
		
		// global lookup of IDs to Components - we can esnure at edit time that no two 
		// components which are loaded at the same time have the same ID. 
		public static Dictionary<string, LSS_UID> allGuids = new Dictionary<string, LSS_UID> ();


		public string uniqueId { get { return GenerateUniqueID (); } }		
		[HideInInspector]
		public string m_uniqueId;		

		public static int managerInstanceID;
		//public static int managerInstanceID { get { return m_managerInstanceID; }}

		[SerializeField]
		private List<string> m_searchTags;
		public List<string> searchTags { get { return m_searchTags; } }

		public void DestroyLightingScenarioComponent () { //Called using message whenever the scene wants to remove everything!
			StartCoroutine (WaitOnDestroy ());
		}
		private IEnumerator WaitOnDestroy(){
			yield return null;
			if (ObjectHasScenariosComponents ()) {; //do next frame so new component count is accurate and we can check if there are still dependencies.
				DestroyImmediate (this);
			}
		}
			
		private bool ObjectHasScenariosComponents () {
			if (GetComponents<LSS_ComponentBase> ().Length > 0) {
				return GetComponents<LSS_ComponentBase> ().Length > 0;
			}
			return false;
		}
			
		public void OnSearchTagChange () {
			var tags = new List<string> (); // Tags from all LSS_UID components.
			foreach (KeyValuePair<string,LSS_UID> uid in LSS_UID.allGuids) {
				try {
					foreach (string tag in m_searchTags) {
						if (!searchTags.Contains (tag)) {
							searchTags.Add (tag);
						}
					}
				} catch {}
			}
		}


		public void Awake () {
			GenerateUniqueID ();
			SetManagerInstanceID ();
		}
		public void SetManagerInstanceID () {
			if (GetComponent<LSS_FrontEnd> ()) {
				managerInstanceID = GetComponent<LSS_FrontEnd> ().gameObject.GetInstanceID ();
			}
		}
		public void OnLevelWasLoaded () {
			SetManagerInstanceID ();
			allowRecreate = true;
		}






		// Only compile the code in an editor build
		#if UNITY_EDITOR
		private void RecreateIfComponentsExist() {
			if (Application.isEditor || allowRecreate) {
				if (ObjectHasScenariosComponents ()) {
					LSS_UID copy = gameObject.AddComponent<LSS_UID>();
					if (copy != null) { //Game starting?
						copy.m_uniqueId = m_uniqueId;
						allGuids.Add(m_uniqueId, copy);
						EditorUtility.DisplayDialog (
							"Can't remove component", 
							"Can't remove " + this.GetType() + " until all other Lighting Scenario Switcher (LSS) components have been removed.\n\nNote: If this unique ID is removed then all previously saved lighting scenarios will no longer sync with this object!",
							"OK");
					}
				}
			}
		}

		public void CreateNewScenariosManager() {
			var go = new GameObject();
			go.AddComponent<LSS_FrontEnd> ();
			go.name = "LightingScenariosSwitcher";
			managerInstanceID = go.GetInstanceID ();
			Selection.instanceIDs = new int[] { managerInstanceID };
		}

		// When we get destroyed (which happens when unloading a level)
		// we must remove ourselves from the global list otherwise the
		// entry still hangs around when we reload the same level again
		// but now the THIS pointer has changed and end up changing 
		// our ID

		// Whenever something changes in the editor (note the [ExecuteInEditMode])
		private void Update() {
			// Don't do anything when running the game
			if (Application.isPlaying) {
				return;
			}
			GenerateUniqueID ();
			SetManagerInstanceID ();
		}
		#endif

		public string GenerateUniqueID () {
			// Construct the name of the scene with an underscore to prefix to the Guid
			string sceneName = gameObject.scene.name + "_";

			// if we are not part of a scene then we are a prefab so do not attempt to set the id
			if  (sceneName == null) return "";

			// Test if we need to make a new id
			bool hasSceneNameAtBeginning = (m_uniqueId != null && 
				m_uniqueId.Length > sceneName.Length && 
				m_uniqueId.Substring (0, sceneName.Length) == sceneName);

			bool anotherComponentAlreadyHasThisID = (m_uniqueId != null && 
				allGuids.ContainsKey (m_uniqueId) && 
				allGuids [m_uniqueId] != this);

			if (!hasSceneNameAtBeginning || anotherComponentAlreadyHasThisID) {
				m_uniqueId =  sceneName + Guid.NewGuid ();
				EditorUtility.SetDirty (this);
				EditorSceneManager.MarkSceneDirty (gameObject.scene);
			}

			// We can be sure that the key is unique - now make sure we have it in our list
			if (!allGuids.ContainsKey (m_uniqueId)) {
				allGuids.Add(m_uniqueId, this);
			} 
			return m_uniqueId;
		}




		private float m_LastEditorUpdateTime;
		protected virtual void OnEnable()
		{
			m_LastEditorUpdateTime = Time.realtimeSinceStartup;
			EditorApplication.update += OnEditorUpdate;
		}


		protected virtual void OnEditorUpdate()
		{
			//GenerateUniqueID ();
			//Debug.Log ("Editor");
			// In here you can check the current realtime, see if a certain
			// amount of time has elapsed, and perform some task.
		}




		bool allowRecreate = false;
		public virtual void OnDestroy () {
			allGuids.Remove(m_uniqueId);
			EditorApplication.update -= OnEditorUpdate;
			//RecreateIfComponentsExist (); // Warn the user that synced components are still connected to this object.
		}
	
	}
}