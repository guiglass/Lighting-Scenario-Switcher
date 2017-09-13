using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LSS;

namespace LSS_Components
{
	[RequireComponent(typeof(Light))]
	public class LSS_Light : LSS_ComponentBase {

		private Light component { get { return GetComponent<Light> (); } }

		public void SetComponentInfo(LSS_Models.LightModel info) { 
			component.enabled = info.enabled;                 	// Parameter Verbose Name: Enabled
			component.intensity = info.intensity;             	// Parameter Verbose Name: Intensity
			component.range = info.range;                     	// Parameter Verbose Name: Range
			component.shadows = info.shadows;                 	// Parameter Verbose Name: Shadows
			component.shadowStrength = info.shadowStrength;   	// Parameter Verbose Name: BakeType
			component.color = info.color;                     	// Parameter Verbose Name: Color
		}

		// Only need to do this in editor due to game will not be creating/saving scenarios or lightmaps.
		#if UNITY_EDITOR 
		public override object GetComponentInfo () {
			var componentInfo = new LSS_Models.LightModel();
			componentInfo.uniqueId = GetOrCreateUniqueID (gameObject).uniqueId;

			componentInfo.enabled = component.enabled;                  // Parameter Verbose Name: Enabled
			componentInfo.intensity = component.intensity;              // Parameter Verbose Name: Intensity
			componentInfo.range = component.range;                      // Parameter Verbose Name: Range
			componentInfo.shadows = component.shadows;                  // Parameter Verbose Name: Shadows
			componentInfo.shadowStrength = component.shadowStrength;    // Parameter Verbose Name: BakeType
			componentInfo.color = component.color;                      // Parameter Verbose Name: Color

			return componentInfo;
		}
			
		public override int UpdateComponentInfos(LSS_Models.LightingScenarioModel lightingScenariosModels, bool insert = false) {
			var infos = lightingScenariosModels.lightInfos;
			var newInfos = GetModifiedList(infos, insert);

			if (newInfos != null) {
				lightingScenariosModels.lightInfos = newInfos.Select(s => (LSS_Models.LightModel)s).ToArray();
				GetScenariosManager ().WriteJsonFile (lightingScenariosModels, GetScenariosManager ().GetResourcesStorageDirectory ());
			}
			return newInfos == null ? -1 : newInfos.Count();
		}
		#endif
	}
}