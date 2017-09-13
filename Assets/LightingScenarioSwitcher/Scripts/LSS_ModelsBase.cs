using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSS
{
	public class LSS_ModelsBase : MonoBehaviour {
		/*
		//
		// Interface for common functions
		//
		public interface ModelsInterface { // When ApplySceneInfo runs through all the arrays it will have an interface for each model "type".
			string GetUniqueID(); // used for looking up the attached gameobject in the scene regardless of Unity's InstanceID.
			object GetComponentObject(); // Get just the object refrence (for checking if an entry was null regardless of type, or just grabbing a generic reference).
			void CustomFunction(); // The function that specifies what to do with the deserialized data from the from the lighting scenarios JSON file.
		}


		//
		// Base Model:
		//
		[System.Serializable]
		public class BaseModel { // All models with ModelsInterface will inherit the base model.
			public string GetUniqueID() { // used for looking up the attached gameobject in the scene regardless of Unity's InstanceID.
				return uniqueId;
			}
			public string uniqueId;             // Parameter Verbose Name: UniqueId
		}

		//
		// LightProbes Model:
		//
		[System.Serializable]
		public class SphericalHarmonicsModel {
			public float[] coefficients = new float[27];
		}
		//public SphericalHarmonicsModel sphericalHarmonicsInfo;
		*/
		/*
		//
		// JSON File Model:
		//
		[System.Serializable]
		public class LightingScenarioModel {
			public string uid; // The ChangeLightmapUniqueID.uniqueId from the component attached to this gameobject. Ensures this data is for this scene.

			public LightmapsMode lightmapsMode;

			public string[] lightmaps;
			public string[] lightmapsDir;
			public string[] lightmapsShadow;

			public string[] materials;

			public SphericalHarmonicsModel[] lightProbes;
			public RendererModel[] rendererInfos;

			public FlareModel[] flareInfos;
			public LightModel[] lightInfos;
		}
		*/
	}
}