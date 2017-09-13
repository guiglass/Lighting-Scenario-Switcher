using UnityEngine;
using UnityEditor;
using LSS_Components;
using System.Collections.Generic;
using System.Collections;
using System;

namespace LSS
{
	[CustomEditor(typeof(LSS_FrontEnd))]
	public class LSS_FrontEndEditor : Editor
	{
		int selectedTag = 0;
		List<string> tags; // Tags from all LSS_UID components.

		private void SearchForTags () {
			tags = new List<string> (); // Tags from all LSS_UID components.
			foreach (KeyValuePair<string,LSS_UID> uid in LSS_UID.allGuids) {
				try {
					foreach (string tag in uid.Value.searchTags) {
						if (!tags.Contains (tag)) {
							tags.Add (tag);
						}
					}
				} catch (Exception e) {
					Debug.LogError ("A search tag had errors: " + e);
				}
			}
		}

		float m_searchInterval = 5; // How ofter to search for unique tags.
		float m_searchIntervalOffset = 0;
		float m_LastSearchUpdateTime = 0;
		public void ScanTagsTimer() {
			if (Time.realtimeSinceStartup - m_LastSearchUpdateTime >= m_searchInterval - m_searchIntervalOffset) {
				m_LastSearchUpdateTime = Time.realtimeSinceStartup;
				m_searchIntervalOffset = 0;
				SearchForTags ();
			}
		}

		private float m_LastEditorUpdateTime;
		protected virtual void OnEnable()
		{
			m_LastEditorUpdateTime = Time.realtimeSinceStartup;
			EditorApplication.update += OnEditorUpdate;

			m_LastSearchUpdateTime = Time.realtimeSinceStartup;
			m_searchIntervalOffset = m_searchInterval - 1;

			SearchForTags ();
		}
			
		protected virtual void OnDisable()
		{
			EditorApplication.update -= OnEditorUpdate;
		}

		protected virtual void OnEditorUpdate()
		{
			ScanTagsTimer ();
			// In here you can check the current realtime, see if a certain
			// amount of time has elapsed, and perform some task.
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI ();

			LSS_FrontEnd frontend = (LSS_FrontEnd)target;

			bool resourcesFolderExists = frontend.CheckResourcesStorageDirectoryExists ();

			EditorGUILayout.Space ();

			GUILayout.Label (resourcesFolderExists ? 
				"Lighting scenario folder exists and contains valid data." : 
				"No lighting scenario resources located for currently entered folder name."
			);
			if (resourcesFolderExists && GUILayout.Button("Load Lighting Scenario")) {
				frontend.Load ();
			}  

			if (GUILayout.Button(resourcesFolderExists ? "Replace Existing Scenario" : "Create New Scenario")) {
				if (frontend.CheckResourcesStorageDirectoryExists ()) {
					if (!EditorUtility.DisplayDialog (
						"Replace Lightmap Resources?", 
						"A lighting scenario currently exists in:\n\n\"" + frontend.resourceFolder + "\".\n\nPress Replace to overwrite existing scenario.", 
						"Replace", 
						"Cancel")) {
						return;
					} 
				} else {
					if (!EditorUtility.DisplayDialog (
						"Create Lightmap Resources?", 
						"Create new lighmap Resources folder: \"" + frontend.resourceFolder + "?", 
						"OK", 
						"Cancel")) {
						return;
					}
				}
				frontend.GenerateLightmapInfoStore ();
			}  

			EditorGUILayout.Space ();
			if (frontend.GetComponent<LSS_Material>() == null) {
				GUILayout.Label ("No materials will be stored for lighting scenario.");

				if (GUILayout.Button ("ADD MATERIALS NODE")) {
					frontend.gameObject.AddComponent<LSS_Material> ();
				}
			}

			EditorGUILayout.Space ();

			GUILayout.Label ("Goto " + (resourcesFolderExists ? "\"" + frontend.resourceFolder + "\" folder.": "/Assets/Resources directory"));
			if (resourcesFolderExists) {
				GUIStyle customButton = new GUIStyle ("button");
				customButton.fontSize = 14;
				customButton.fontStyle = FontStyle.Bold;
				if (GUILayout.Button ("SHOW IN EXPLORER", customButton)) {

					EditorUtility.RevealInFinder(Application.dataPath + "/Resources/" + frontend.resourceFolder );
				}
			} else {
				if (GUILayout.Button ("SHOW IN EXPLORER")) {
					frontend.CreateResourcesDirectory ();
					EditorUtility.RevealInFinder(Application.dataPath + "/Resources/");
				}
			}

			EditorGUILayout.Space ();

			if (tags != null && tags.Count > 0) {
				GUILayout.Label ("Select all gameobjects with UIDs and this tag");
				selectedTag = EditorGUILayout.Popup ("Unique ID Tags", selectedTag, tags.ToArray ()); 
				if (GUILayout.Button ("SELECT")) {
					
					List<int> gosFound = new List<int> ();
					foreach (KeyValuePair<string,LSS_UID> uid in LSS_UID.allGuids) {
						if (uid.Value != null && uid.Value.searchTags.Contains (tags[selectedTag])) {
							gosFound.Add (uid.Value.gameObject.GetInstanceID ());
						}
					}

					if (gosFound.Count > 0) {
						Selection.instanceIDs = gosFound.ToArray ();
					}
				}
			}

		}
	}
}