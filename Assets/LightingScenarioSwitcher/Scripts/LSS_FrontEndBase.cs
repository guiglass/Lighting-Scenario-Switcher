using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR

using UnityEditor;
#endif


namespace LSS
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LSS_UID))]
	public class LSS_FrontEndBase : MonoBehaviour 
	{

		//
		// JSON and Resources data files path initilization
		//

		private string m_jsonFileName = "mapconfig.txt"; // Name of the json data file.
		public string jsonFileName {get { return m_jsonFileName;} }
		[SerializeField]
		[Tooltip("The folder name relative to Assets/Resources/ where to store or retrieve the resources data files.")]
		private string m_resourceStorageFolder = "LightMapData_1";
		private string m_resourceTemporaryFolder;
		public string resourceFolder {get { return m_resourceStorageFolder; } set { m_resourceStorageFolder = value; }}

		//TODO : enable logs only when verbose enabled
		[SerializeField] 
		[Tooltip("Print debugging information to console upon encountering missing or corrupt resources.")]
		private bool m_isVerbose = false;

		public bool isVerbose { 
			get { return m_isVerbose; }
		}
			
		//
		// Presistent Unique ID Management
		//
		public string GetUniqueID (GameObject go) {
			return go.GetComponent<LSS_UID> ().uniqueId;
		}
		#if UNITY_EDITOR
		public string GetOrCreateUniqueID (GameObject go) {
			LSS_UID uid = go.GetComponent<LSS_UID> ();
			if (!uid) { // If this object does not have a presistant unique id, then add one.
				go.AddComponent<LSS_UID> ();
				return go.GetComponent<LSS_UID> ().uniqueId;
			} 
			return uid.uniqueId;
		}
			

		//
		// Success Message And Final Report
		//
		private string BuildMessageString(object[] inArray, string inString ) {
			if (inArray != null && inArray.Length > 0) {
				return inArray.Length.ToString () + inString;
			}
			return "";
		}

		public void PrintFinalResults (LSS_Models.LightingScenarioModel lightingScenariosData) {
			string message = BuildMessageString(lightingScenariosData.rendererInfos, " meshrenderers, ");
			message += BuildMessageString(lightingScenariosData.lightmaps, " ligthmaps, ");
			message += BuildMessageString(lightingScenariosData.materials, " materials, ");
			message += BuildMessageString(lightingScenariosData.lightInfos, " lights, ");
			message += BuildMessageString(lightingScenariosData.flareInfos, " lens flares, ");
			message += BuildMessageString(lightingScenariosData.gameObjectInfos, " gameobjects, ");
			Debug.Log("Stored info for " + message);
		}

		public string[] TextureListToArrayOfNames (List<Texture2D> inArray) {
			string[] outArray = new string[inArray.Count];
			for(int i = 0; i < inArray.Count; i++) {
				if (inArray [i] != null) {
					outArray [i] = Path.GetFileName(AssetDatabase.GetAssetPath(inArray [i]));
				}
			}
			return outArray;
		}

		private TextureImporter GetTextureImporter (Texture2D texture) {
			string newTexturePath =  AssetDatabase.GetAssetPath (texture);
			TextureImporter importer = AssetImporter.GetAtPath (newTexturePath) as TextureImporter;
			return importer;
		}

		private Texture2D GetCurrentLightmapTexture (string name) {
			string path = AssetDatabase.GetAssetPath (LightmapSettings.lightmaps [0].lightmapColor); // Path to the currently loaded lightmap texture (the filename portion will be removed)
			path = path.Substring(0, path.Length - Path.GetFileName(path).Length); // Rremove the file name revealing only the path.
			path = Path.Combine (path, name); // Construct a path that assumes the destinatnion is the target texture of the current lightmap.
			return AssetDatabase.LoadAssetAtPath<Texture2D> (path); // Return the current light/direction/shadow map texture.
		}
			
		public bool GetLightmapIndexValid (int lightmapIndex) {
			if (lightmapIndex != -1) {
				if (lightmapIndex != 0xFFFE) { // A value of 0xFFFE is internally used for objects that have their scale in lightmap set to 0.
					return true;
				}
			}
			return false;
		}
	
		private bool GetInputResourcesValid (object[] arr, int i) { 
			if (arr [i] == null) {
				if (isVerbose) {
					Debug.Log("Null element in " + arr.GetType() + " at index " + i + " texture was null (Maybe it was the optional shadowmap?).");
				}
				return false;
			}
			return true;
		}


		//
		// File IO And Directory Lookups
		//
		private bool GetInputResoursesValid (object[] arr, int i) {
			if (arr [i] == null) {
				if (isVerbose == true) {
					Debug.Log("Null element in " + arr.GetType() + " at index " + i + " texture was null (Maybe it was the optional shadowmap?).");
				}
				return false;
			}
			return true;
		}

		public string[] CopyMaterialToResources (Material[] _materials) {
			string[] _filenames = new string[_materials.Length];
			for (int i = 0; i < _materials.Length; i++) {
				if (!GetInputResoursesValid(_materials, i)) {
					continue;
				}
				Material material = _materials [i]; // Get material

				if (material != null) { // Maybe the optional shadowmask didn't exist?
					string newName = material.name;
					string fromPath = AssetDatabase.GetAssetPath (material);
					string toPath = GetResourcesTemporaryDirectory() + Path.GetFileName(fromPath) ;
					AssetDatabase.CopyAsset(fromPath, toPath);
					_filenames [i] = newName;//newMaterial.name; // Set the new material unique filename as the reference in the Json file.
				} 
				else if (isVerbose == true)	{
					Debug.Log("Material " + _materials[i] + " was not found in the lightmap textures.");
				}
			}
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh (); // Refresh so the newTexture file can be found and loaded.
			return _filenames;
		}
			
		public void CopyTextureToResources (string[] textures) {
			if ( textures == null )
			{
				return;
			}
			
			for (int i = 0; i < textures.Length; i++) {
				if (!GetInputResoursesValid(textures, i)) {
					continue;
				}
				try {
					Texture2D texture = GetCurrentLightmapTexture (textures [i]); // Get texture by texture name.
					if (texture != null) { // Maybe the optional shadowmask didn't exist?
						string fromPath = AssetDatabase.GetAssetPath (texture); // Path to the currently loaded texture;

						string toPath = GetResourcesStorageDirectory() + Path.GetFileName(fromPath); // First get a path without the index appended to the filename so we can check if from and to paths are the same. 

						if (fromPath.Equals(toPath)) { //they are the exact same texture
							fromPath = toPath; // Make it so it will copy the texture from the old directory to the new temporary directory
						}

						string newName = texture.name + "_" + i; // Path was not the same as fromPath so now the index can be appended to the filename.
						toPath = GetResourcesTemporaryDirectory () + newName + Path.GetExtension (AssetDatabase.GetAssetPath (texture)); // Now get the actual toPath in the resourses temp resources folder.

						AssetDatabase.CopyAsset(fromPath, toPath); // Create a copy of the currently loaded texture and place in the temp resources folder.

						AssetDatabase.Refresh (); // Refresh so the newTexture file can be found and loaded.
						Texture2D newTexture = Resources.Load<Texture2D> (m_resourceTemporaryFolder + "/" + newName); // Load the new texture as an object.

						CopyTextureImporterProperties (texture, newTexture); // Ensure new texture takes on same properties as origional texture.

						AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( newTexture ) ); // Re-import texture file so it will be successfully compressed to desired format.
						EditorUtility.CompressTexture (newTexture, texture.format, UnityEditor.TextureCompressionQuality.Best); // Now compress the texture.

						textures [i] = newTexture.name; // Set the new texture as the reference in the Json file.
						Resources.UnloadAsset(newTexture); // Since we will no longer be processing on this texture it can be unloaded.
					} 
					else if (isVerbose)	{
						Debug.Log("Texture " + textures[i] + " was not found in the lightmap textures.");
					}

				} catch (Exception e) {
					Debug.LogError ("Error while copying resources:" + textures.GetType ().ToString () + "\nMaybe you forgot to bake lightmaps?");
				} 
			}
		}

		private void CopyTextureImporterProperties (Texture2D fromTexture, Texture2D toTexture)	{
			TextureImporter fromTextureImporter = GetTextureImporter (fromTexture);
			TextureImporter toTextureImporter = GetTextureImporter (toTexture);

			toTextureImporter.wrapMode = fromTextureImporter.wrapMode;
			toTextureImporter.anisoLevel = fromTextureImporter.anisoLevel;
			toTextureImporter.sRGBTexture = fromTextureImporter.sRGBTexture;
			toTextureImporter.textureType = fromTextureImporter.textureType;
			toTextureImporter.textureCompression = fromTextureImporter.textureCompression;
		}

		public void TransferTemporaryFilesToStorage() {
			DeleteResourcesStorageDirectory ();
			CreateResourcesStorageDirectory ();

			AssetDatabase.Refresh (); // Refresh so the new files can be found and loaded.

			DirectoryInfo dir = new DirectoryInfo(GetResourcesTemporaryDirectory());
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files) {
				string status = AssetDatabase.MoveAsset(
					GetResourcesTemporaryDirectory() + file.Name,
					GetResourcesStorageDirectory() + file.Name);

				if (status != "") { // Everything is OK if status is empty
					if (isVerbose == true) {
						Debug.Log(status + " For file: " + file);
					}
				}
			}
		}


		public void CreateResourcesDirectory() {
			Directory.CreateDirectory ("Assets/Resources/");
		}
		public void CreateResourcesStorageDirectory() {
			if (!CheckResourcesStorageDirectoryExists ()) {
				Directory.CreateDirectory (GetResourcesStorageDirectory());
			}
		}

		public void CreateResourcesTemporaryDirectory() {
			if (!CheckResourcesTemporaryDirectoryExists ()) {
				m_resourceTemporaryFolder = m_resourceStorageFolder + "_" + System.Guid.NewGuid();
				Directory.CreateDirectory (GetResourcesTemporaryDirectory());
			}
		}

		public bool CheckResourcesStorageDirectoryExists ()	{
			return CheckResourcesDirectoryExists (GetResourcesStorageDirectory ());
		}
		public bool CheckResourcesTemporaryDirectoryExists () {
			return CheckResourcesDirectoryExists (GetResourcesTemporaryDirectory ());
		}
		private bool CheckResourcesDirectoryExists (string path) {
			return path.Length > 0 ?  Directory.Exists (path) : false;
		}	


		public string GetResourcesStorageDirectory () {
			return  GetResourcesDirectory(m_resourceStorageFolder); // The directory where data resides.
		}
		public string GetResourcesTemporaryDirectory ()	{
			return GetResourcesDirectory(m_resourceTemporaryFolder); // The directory where data will be kept until all processing is finished.
		}
		private string GetResourcesDirectory (string _folder) {
			return  _folder == null || _folder.Length == 0 ? "" : "Assets/Resources/"+_folder+"/"; // The directory where data will be kept until all processing is finished.
		}


		private void DeleteResourcesStorageDirectory () {
			DeleteResourcesDirectory (GetResourcesStorageDirectory ());
		}
		public void DeleteResourcesTemporaryDirectory () {
			DeleteResourcesDirectory (GetResourcesTemporaryDirectory ());
			m_resourceTemporaryFolder = "";
		}
		private void DeleteResourcesDirectory (String path) {
			if (CheckResourcesDirectoryExists (path)) {
				Directory.Delete (path, true);
			}
		}
			

		public void WriteJsonFile (LSS_Models.LightingScenarioModel lightingScenariosData, string path) {
			File.WriteAllText (path + m_jsonFileName, JsonUtility.ToJson(lightingScenariosData)); // Write all the data to the file.
		}

		public bool CheckLightingScenarioValidForScene (LSS_Models.LightingScenarioModel scenario) {
			return scenario.uid == GetOrCreateUniqueID(gameObject); // Given the deserialized and loaded JSON data (as LightingScenarioData), check if the uid field matches the UniqueID attached to this gameobject (if they match then that JSON file was built by this script in this scene).
		}
		#endif


		//
		// Runtime File IO And Directory Lookups
		//
		public void RegisterAllUIDs () {
			// Ensure that all LSS_UID unique id's have been generated and the list populated incase a LSS_GameObject was disabled and it's awake() had not been called.
			var res = Resources.FindObjectsOfTypeAll<LSS_UID> ();
			foreach (LSS_UID uid in res) {
				uid.GenerateUniqueID();
			}
		}



		public string GetJsonResourcePath (string f) { //Expects just the file name without the extention, and will return the path to the resource from the currently set m_resourceFolder
			string baseName = f.Substring(0, f.Length - Path.GetExtension(f).Length);
			return Path.Combine (m_resourceStorageFolder, baseName);
		}

		public LSS_Models.LightingScenarioModel LoadJsonFile () {
			
			string json = Resources.Load (GetJsonResourcePath (m_jsonFileName)).ToString();
			return JsonUtility.FromJson<LSS_Models.LightingScenarioModel> (json);
		}

	}
}
