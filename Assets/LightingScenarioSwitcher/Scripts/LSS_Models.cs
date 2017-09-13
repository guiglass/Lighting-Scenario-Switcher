using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSS_Components;

namespace LSS
{
	public class LSS_Models : MonoBehaviour {

		//
		// LightProbes Model:
		//
		[System.Serializable]
		public class SphericalHarmonicsModel {
			public float[] coefficients = new float[27];
		}


		//
		// Scene GameObject Model:
		//
		[System.Serializable]
		public class GameObjectModel : BaseModel, ModelsInterface { // Scene Lights Info

			public object GetComponentObject() {
				return LSS_UID.allGuids[GetUniqueID()].gameObject; // Locate the scene object by UID and return the attached component of specified type.
			}

			public void CustomFunction() {
				LSS_UID.allGuids[uniqueId].gameObject.GetComponent<LSS_GameObject>().SetComponentInfo (this); // Pass instance of class to the component class.
			}

			public bool enabled;               // Parameter Verbose Name: Enabled
		}                                                                                     


		//
		// MeshRenderers Model:
		//
		[System.Serializable]
		public class RendererModel : BaseModel, ModelsInterface  { // Renderers Lightmap Info.

			public object GetComponentObject() {
				return LSS_UID.allGuids[GetUniqueID()].gameObject.GetComponent<MeshRenderer>(); // Locate the scene object by UID and return the attached component of specified type.
			}

			public void CustomFunction() {
				var obj = LSS_UID.allGuids[uniqueId].gameObject.GetComponent<MeshRenderer>();
				obj.lightmapIndex = lightmapIndex;

				if (!obj.isPartOfStaticBatch) {
					obj.lightmapScaleOffset = lightmapOffsetScale;
				}
				if (obj.isPartOfStaticBatch ) {
					Debug.LogError("Object " + obj.gameObject.name + " is part of static batch, skipping lightmap offset and scale.");
				}
			}

			public int lightmapIndex;           // Parameter Verbose Name: LightMapIndex Color
			public Vector4 lightmapOffsetScale; // Parameter Verbose Name: LightMapOffsetScale
		}


		//
		// Terrains Model:
		//
		[System.Serializable]
		public class TerrainModel : BaseModel, ModelsInterface { // Terrain Lightmap Info

			public object GetComponentObject() {
				return LSS_UID.allGuids[GetUniqueID()].gameObject.GetComponent<Terrain>(); // Locate the scene object by UID and return the attached component of specified type.
			}

			public void CustomFunction() {
				var obj = LSS_UID.allGuids[uniqueId].gameObject.GetComponent<Terrain>();
				obj.lightmapIndex = lightmapIndex;
				obj.lightmapScaleOffset = lightmapScaleOffset;
			}

			public int lightmapIndex;           // Parameter Verbose Name: LightMapIndex
			public Vector4 lightmapScaleOffset; // Parameter Verbose Name: LightmapScaleOffset
		}


		//
		// Scene Lights Model:
		//
		[System.Serializable]
		public class LightModel : BaseModel, ModelsInterface { // Scene Lights Info

			public object GetComponentObject() {
				return LSS_UID.allGuids[GetUniqueID()].gameObject.GetComponent<Light>(); // Locate the scene object by UID and return the attached component of specified type.
			}

			public void CustomFunction() {
				LSS_UID.allGuids[uniqueId].gameObject.GetComponent<LSS_Light>().SetComponentInfo (this); // Pass instance of class to the component class.
			}

			public bool enabled;               // Parameter Verbose Name: Enabled
			public float intensity;            // Parameter Verbose Name: Intensity
			public float range;                // Parameter Verbose Name: Range
			public float shadowStrength;       // Parameter Verbose Name: ShadowStrength
			public LightShadows shadows;       // Parameter Verbose Name: Shadows
			public Vector4 color;              // Parameter Verbose Name: Color
		}                                                                                     


		//
		// Lens Flare model.
		//
		[System.Serializable]
		public class FlareModel : BaseModel, ModelsInterface { // Scene Lights Info

			public object GetComponentObject() {
				return LSS_UID.allGuids[GetUniqueID()].gameObject.GetComponent<LensFlare>(); // Locate the scene object by UID and return the attached component of specified type.
			}

			public void CustomFunction() {
				LSS_UID.allGuids[uniqueId].gameObject.GetComponent<LSS_LensFlare>().SetComponentInfo (this); // Pass instance of class to the component class.
			}

			public bool enabled;               // Parameter Verbose Name: Enabled
			public Vector4 color;              // Parameter Verbose Name: Color
			public float brightness;		   // Parameter Verbose Name: Brightness
			public float fadeSpeed;            // Parameter Verbose Name: FadeSpeed
		}                                                                             
			

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
			public GameObjectModel[] gameObjectInfos;
			public RendererModel[] rendererInfos;
			public TerrainModel[] terrainInfos;
			public FlareModel[] flareInfos;
			public LightModel[] lightInfos;
		}


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


	}
}


