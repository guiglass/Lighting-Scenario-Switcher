using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using LSS_Components;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LSS
{
	public class LSS_FrontEnd : LSS_FrontEndBase 
	{

		//
		// Declaring The Master Array!!
		//
		//[SerializeField] //Uncomment this if you want to inspect the loaded JSON data arrays in the inspector.
		private LSS_Models.LightingScenarioModel lightingScenariosData;


		//
		// Main Application Initialization
		//
		[SerializeField] 
		[Tooltip("Load scenario from the currently entered folder when Awake() is called (and Application.isPlaying is true).")]
		private bool loadOnAwake = false; // Load the selected lighmap when this script wakes up (aka when game starts).
		public void Awake () {
			if (Application.isPlaying && loadOnAwake) {
				SceneManager.sceneLoaded += OnSceneLoaded; //After scene is loaded so should all unique IDs.
			}
		}
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
			Load (); // If load on awak then do this right after the scene has fully loaded to give things time to get ready.
		}


		//
		// Deserialized JSON And Load Resources
		//
		public void Load (string folder) {
			resourceFolder = folder;
			Load ();
		}

		public void Load () { // Call this to load whatever json exists in the currently set m_resourceStorageFolder 
			lightingScenariosData = LoadJsonFile ();

			//The JSON file was " + lightingScenariosData == null ? " null or not present!"
			if (!CheckLightingScenarioValidForScene(lightingScenariosData)) {
				#if UNITY_EDITOR
				int option = EditorUtility.DisplayDialogComplex (
					"JSON File Not Valid For Scene!",
					"The file about to be loaded has wrong unique ID for this scene.", 
					"Do Nothing",
					"Proceed",
					"Change Permanterly"
				);

				switch (option)	{
				case 0: // Do Nothing
					return;
				case 1: // Proceed
					break;
				case 2: // Change UID Permanterly
					lightingScenariosData.uid = GetUniqueID (gameObject);
					WriteJsonFile (lightingScenariosData, GetResourcesStorageDirectory());
					break;
				}
				#else
				Debug.LogError ("JSON File Not Valid For Scene!");
				#endif
			}

			var newLightmaps = new LightmapData[lightingScenariosData.lightmaps.Length];
			for (int i = 0; i < newLightmaps.Length; i++) {
				newLightmaps[i] = new LightmapData();
				newLightmaps[i].lightmapColor = Resources.Load<Texture2D>(resourceFolder+"/" + lightingScenariosData.lightmaps[i]);
				if (lightingScenariosData.lightmapsMode != LightmapsMode.NonDirectional) {
					if (lightingScenariosData.lightmapsDir.Length > i && lightingScenariosData.lightmapsDir [i] != null) { // If the textuer existed and was set in the data file.
						newLightmaps [i].lightmapDir = Resources.Load<Texture2D> (resourceFolder + "/" + lightingScenariosData.lightmapsDir [i]);
					}
					if (lightingScenariosData.lightmapsShadow.Length > i && lightingScenariosData.lightmapsShadow [i] != null) { // If the textuer existed and was set in the data file.
						newLightmaps [i].shadowMask = Resources.Load<Texture2D> (resourceFolder + "/" + lightingScenariosData.lightmapsShadow [i]);
					}
				}
			}
			LightmapSettings.lightmaps = newLightmaps;

			RegisterAllUIDs (); // Ensure that all LSS_UID unique id's have been generated 
			StartCoroutine( ApplySceneInfo(lightingScenariosData.gameObjectInfos));
			StartCoroutine( ApplySceneInfo(lightingScenariosData.terrainInfos));
			StartCoroutine( ApplySceneInfo(lightingScenariosData.rendererInfos));
			StartCoroutine( ApplySceneInfo(lightingScenariosData.lightInfos));
			StartCoroutine( ApplyMaterialsInfo(lightingScenariosData.materials));
			StartCoroutine( LoadLightProbes(lightingScenariosData.lightProbes));

			#if UNITY_EDITOR
			Debug.Log("Loaded Lighting Scenario: \"" + resourceFolder + "\"");
			#endif
		}
			

		//
		// Apply Newly Loaded Resources To Scene
		//
		public IEnumerator ApplySceneInfo (LSS_Models.ModelsInterface[] infos) {
			if (infos != null) {
				for (int i = 0; i < infos.Length; i++) {
					try	{
						var info = infos[i];
						if (!LSS_UID.allGuids.ContainsKey (info.GetUniqueID())) { //TODO Find disabled game objects as they do not appear in the list at the moment
							if (isVerbose == true) {
								Debug.LogError("The globla list of unique IDs did not contain: " + info.GetUniqueID());
							}
							continue;
						}
						var obj = info.GetComponentObject();
						if (obj == null) {
							if (isVerbose == true) {
								Debug.LogError(obj.GetType() + " for unique ID: " + info.GetUniqueID() + " was null.");
							}
							continue;
						}
						try	{
							info.CustomFunction(); // Apply changes, logging, cleanup, custom actions in the custom function interface.
						} catch (Exception e) {
							if (isVerbose == true) {
								Debug.LogError ("Error while applying custom function for: " + infos.GetType ().ToString () + " - Was an object removed without updating JSON file?\n" + e);
							}
						}
					} catch (Exception e) {
						if (isVerbose == true) {
							Debug.LogError ("Error in ApplyRendererModel:" + infos.GetType ().ToString () + "\n" + e);
						}

					}
				}
			} else {
				if (isVerbose == true) {
					Debug.LogError ("Info array was null for type MeshRenderer");
				}
			}
			yield return null;
		}


		//
		// Apply Newly Loaded Materials To Scene
		//
		private IEnumerator ApplyMaterialsInfo (string[] infos) {
			if (infos != null) { // JSON file did not contain materials infos.
				var materialParams = GetComponent<LSS_Material> ();
				if (materialParams != null) { // ChangeLightmap does not have a ChangeLightmapMaterialParams component attached, maybe the user didn't add this optional feature. 
					string[] materialNames = new string[materialParams.materials.Length]; // Create an array of the names so they can be used for IndexOf lookups and get the coorisopnding materis in the lightmap materials list (aka the scene materials). 
					for(int i = 0; i < materialNames.Length; i++) { 
						materialNames[i] = materialParams.materials [i] == null ? null : materialParams.materials[i].name; // Add the name or insert a null to keep indexes in sync with the lightmap materials list (aka the scene materials).
					}
					for (int i = 0; i < infos.Length; i++) { // Fore each material present in the json file's list of material resources.
						int m = Array.IndexOf (materialNames, infos [i]); // Attempt to match a material in the lightmap materials list, if not -1 then it matched a valid index for material in the lightmap materials list (aka the scene materials). 
						if (m != -1) {
							Material newMaterial = Resources.Load<Material> (GetJsonResourcePath (infos [i])); // Get a temporary instance of the material from the resources folder.
							materialParams.materials [m].CopyPropertiesFromMaterial (newMaterial); // Update the current material to be identical to the one in the resources folder.
							materialParams.materials [m].globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive; 
							Resources.UnloadAsset (newMaterial); // After updating the scene material remove the instance to the new material so it doesn't leak.
						}
					}
				} else {
					if (isVerbose == true) {
						Debug.Log ("Info array was null for type ChangeLightmapLightParams.MaterialInfo");
					}
				}
			}
			yield return null;
		}


		//
		// Apply Newly Loaded Lightprobes Coefficients
		//
		private IEnumerator LoadLightProbes(LSS_Models.SphericalHarmonicsModel[] lightProbes) {
			var sphericalHarmonicsArray = new SphericalHarmonicsL2[lightProbes.Length];
			for (int i = 0; i < lightProbes.Length; i++) {
				var sphericalHarmonics = new SphericalHarmonicsL2();
				// j is coefficient
				for (int j = 0; j < 3; j++)	{
					//k is channel ( r g b )
					for (int k = 0; k < 9; k++)	{
						sphericalHarmonics[j, k] = lightProbes[i].coefficients[j * 9 + k];
					}
				}
				sphericalHarmonicsArray[i] = sphericalHarmonics; // Apply the changes
			}

			try	{
				LightmapSettings.lightProbes.bakedProbes = sphericalHarmonicsArray;
			} catch { 
				Debug.LogError("Error when trying to load lightprobes for scenario - Did you bake lighting before building the game?"); 
			}
			yield return null;
		}

		#if UNITY_EDITOR
		//
		// Generate JSON Data From Resources InfosArrays Then Copy Scene Files To Resources Folder
		//
		public void GenerateLightmapInfoStore () { 
			// Do some heavy lifting by generating all nescessary arrays for the json file and copying all textures to the Resources folder

			RegisterAllUIDs (); // Ensure that all LSS_UID unique id's have been generated 

			lightingScenariosData = new LSS_Models.LightingScenarioModel ();

			lightingScenariosData.uid = GetOrCreateUniqueID (gameObject); // The ChangeLightmapUniqueID.uniqueId from the component attached to this gameobject. Ensures this data is for this scene.

			var newLightsInfos = new List<LSS_Models.LightModel>();
			var newLensFlareInfos = new List<LSS_Models.FlareModel>();
			var newGameObjectInfos = new List<LSS_Models.GameObjectModel>();
			var newRendererModels = new List<LSS_Models.RendererModel>();
			var newTerrainsInfos = new List<LSS_Models.TerrainModel>();
			var newSphericalHarmonicsModelList = new List<LSS_Models.SphericalHarmonicsModel>();

			var newLightmapsTextures = new List<Texture2D>();
			var newLightmapsTexturesDir = new List<Texture2D>();
			var newLightmapsTexturesShadow = new List<Texture2D>();

			var newLightmapsMode = LightmapSettings.lightmapsMode;
			lightingScenariosData.lightmapsMode = newLightmapsMode;

			//
			// Begin Appending Scene Lights Info To InfosArrays.
			//
			var lightParams = Resources.FindObjectsOfTypeAll(typeof(LSS_Light));
			foreach (LSS_Light lightParam in lightParams) {
				GetOrCreateUniqueID (lightParam.gameObject);
				newLightsInfos.Add((LSS_Models.LightModel)lightParam.GetComponentInfo());
			}
			lightingScenariosData.lightInfos = newLightsInfos.ToArray();

			//
			// Begin Appending LensFlare Info To InfosArrays.
			//
			var flareParams = Resources.FindObjectsOfTypeAll(typeof(LSS_LensFlare));
			foreach (LSS_LensFlare flareParam in flareParams) {
				GetOrCreateUniqueID (flareParam.gameObject);
				newLensFlareInfos.Add((LSS_Models.FlareModel)flareParam.GetComponentInfo());
			}
			lightingScenariosData.flareInfos = newLensFlareInfos.ToArray();

		
			//
			// Begin Appending LensFlare Info To InfosArrays.
			//
			var gameObjcetParams = Resources.FindObjectsOfTypeAll(typeof(LSS_GameObject));
			foreach (LSS_GameObject gameObjcetParam in gameObjcetParams) {
				GetOrCreateUniqueID (gameObjcetParam.gameObject);
				newGameObjectInfos.Add((LSS_Models.GameObjectModel)gameObjcetParam.GetComponentInfo());
			}
			lightingScenariosData.gameObjectInfos = newGameObjectInfos.ToArray();


			//
			// Begin Appending Terrain Lightmaps Info To InfosArrays.
			//
			foreach (Terrain terrain in Resources.FindObjectsOfTypeAll(typeof(Terrain))) {
				if (GetLightmapIndexValid(terrain.lightmapIndex)) {
					string uid = GetOrCreateUniqueID (terrain.gameObject);

					var info = new LSS_Models.TerrainModel();
					info.uniqueId = uid;
					info.lightmapScaleOffset = terrain.lightmapScaleOffset;

					if (newLightmapsMode != LightmapsMode.NonDirectional) {
						//first directional lighting
						Texture2D lightmapdir = LightmapSettings.lightmaps[terrain.lightmapIndex].lightmapDir;
						if (lightmapdir != null) {
							if (newLightmapsTexturesDir.IndexOf(lightmapdir) == -1) {
								newLightmapsTexturesDir.Add (lightmapdir);
							}
						}
						//now the shadowmask
						Texture2D lightmapshadow = LightmapSettings.lightmaps[terrain.lightmapIndex].shadowMask;
						if (lightmapshadow != null) {
							if (newLightmapsTexturesShadow.IndexOf(lightmapshadow) == -1) {
								newLightmapsTexturesShadow.Add (lightmapshadow);
							}
						}
					}
					Texture2D lightmaplight = LightmapSettings.lightmaps[terrain.lightmapIndex].lightmapColor;
					info.lightmapIndex = newLightmapsTextures.IndexOf(lightmaplight);
					if (lightmaplight != null) {
						if (newLightmapsTextures.IndexOf(lightmaplight) == -1) { // A value of -1 means no lightmap has been assigned, 
							info.lightmapIndex = newLightmapsTextures.Count;
							newLightmapsTextures.Add (lightmaplight);
						}
					}		
					newTerrainsInfos.Add(info);
				}
			}
			//lightingScenariosData.lightmaps = TextureListToArrayOfNames(newLightmapsTextures);
			lightingScenariosData.terrainInfos = newTerrainsInfos.ToArray();


			//
			// Begin Appending MeshRenderes Lightmaps Info To InfosArrays
			//
			foreach (MeshRenderer renderer in Resources.FindObjectsOfTypeAll(typeof(MeshRenderer))) {
				if (GetLightmapIndexValid(renderer.lightmapIndex)) {
					string uid = GetOrCreateUniqueID (renderer.gameObject);

					if (String.IsNullOrEmpty (uid)) { // if object is not part of a scene then it is a prefab so do not attempt to set the id
						if (isVerbose == true) {
							Debug.Log("Object " + renderer.name + " is part of a prefab, skipping.");
						}
						continue;
					}

					var info = new LSS_Models.RendererModel();
					info.uniqueId = uid;
					//info.renderer = renderer; // REMOVED - added uniqueId as the new lookup method
					info.lightmapOffsetScale = renderer.lightmapScaleOffset;

					if (newLightmapsMode != LightmapsMode.NonDirectional) {
						//first directional lighting
						Texture2D lightmapdir = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapDir;
						if (lightmapdir != null) {
							if (newLightmapsTexturesDir.IndexOf(lightmapdir) == -1) {
								newLightmapsTexturesDir.Add (lightmapdir);
							}
						}
						//now the shadowmask
						Texture2D lightmapshadow = LightmapSettings.lightmaps[renderer.lightmapIndex].shadowMask;
						if (lightmapshadow != null) {
							if (newLightmapsTexturesShadow.IndexOf(lightmapshadow) == -1) {
								newLightmapsTexturesShadow.Add (lightmapshadow);
							}
						}
					}
					Texture2D lightmaplight = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
					info.lightmapIndex = newLightmapsTextures.IndexOf(lightmaplight);
					if (lightmaplight != null) {
						if (newLightmapsTextures.IndexOf(lightmaplight) == -1) { // A value of -1 means no lightmap has been assigned, 
							info.lightmapIndex = newLightmapsTextures.Count;
							newLightmapsTextures.Add (lightmaplight);
						}
					}
					newRendererModels.Add(info);
				}
			}
			lightingScenariosData.lightmapsMode = newLightmapsMode;
			lightingScenariosData.lightmaps = TextureListToArrayOfNames(newLightmapsTextures);

			if (newLightmapsMode != LightmapsMode.NonDirectional) {
				lightingScenariosData.lightmapsDir = TextureListToArrayOfNames(newLightmapsTexturesDir);
				lightingScenariosData.lightmapsShadow = TextureListToArrayOfNames(newLightmapsTexturesShadow);
			} else {
				if (isVerbose == true) {
					Debug.Log ("Lightmap settings are non-directional. Not saving directional and shadow textures.");
				}
			}

			lightingScenariosData.rendererInfos = newRendererModels.ToArray();

			var scene_LightProbes = new SphericalHarmonicsL2[LightmapSettings.lightProbes.bakedProbes.Length];
			scene_LightProbes = LightmapSettings.lightProbes.bakedProbes;

			for (int i = 0; i < scene_LightProbes.Length; i++) {
				var SHCoeff = new LSS_Models.SphericalHarmonicsModel();

				// j is coefficient
				for (int j = 0; j < 3; j++)	{
					//k is channel ( r g b )
					for (int k = 0; k < 9; k++)	{
						SHCoeff.coefficients[j*9+k] = scene_LightProbes[i][j, k];
					}
				}
				newSphericalHarmonicsModelList.Add(SHCoeff);
			}
			lightingScenariosData.lightProbes = newSphericalHarmonicsModelList.ToArray ();


			//
			// Start Creating/Copying Resources Files To The Temporary Resources Folder 
			//
			CreateResourcesTemporaryDirectory ();
	

			//
			// Copy Textures To Temporary Folder 
			//
			CopyTextureToResources (lightingScenariosData.lightmaps);
			CopyTextureToResources (lightingScenariosData.lightmapsDir);
			CopyTextureToResources (lightingScenariosData.lightmapsShadow);


			//
			// Copy Materials To Temporary Folder 
			//
			var materialsParams = GetComponent<LSS_Material> ();
			if (materialsParams != null) {
				Material[] _materials = new Material[materialsParams.materials.Length];
				for (int i = 0; i < materialsParams.materials.Length; i++) {
					var mat = materialsParams.materials [i];
					if (mat == null) {
						continue;
					}
					_materials [i] = materialsParams.materials [i];
				}
				lightingScenariosData.materials = CopyMaterialToResources (_materials);
			}


			//
			// Write JSON File To Temporary Folder 
			//
			WriteJsonFile (lightingScenariosData, GetResourcesTemporaryDirectory()); // Write the fully configured JSON file to the temporary folder.
			AssetDatabase.Refresh (); // Refresh so the new files can be found and loaded.


			//
			// Transfer Temporary Files Then Load The New Resourcesses 
			//
			TransferTemporaryFilesToStorage (); // Final step before successful completion, this deletes old resources folder, creats a new one and transfers all temp files over.
			Load(); // After all processing and file transfering the current lightmaps may have become invald (if they were the ones currently loaded from the old deleted resources folder) so we should just reload everything!

			PrintFinalResults (lightingScenariosData);  // Display a successfully completed message to the user showing how many resources have been created.


			//
			// Clean Up 
			//
			DeleteResourcesTemporaryDirectory ();
			AssetDatabase.SaveAssets (); // Refresh so the new files can be found and loaded.
		}
		#endif
	}
}
