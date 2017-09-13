using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LSS;

namespace LSS_Components
{
	[RequireComponent(typeof(LensFlare))]
	public class LSS_LensFlare : LSS_ComponentBase {

		private LensFlare component { get { return GetComponent<LensFlare> (); } }

		public void SetComponentInfo(LSS_Models.FlareModel info) {        
			component.enabled = info.enabled;                 	// Parameter Verbose Name: Enabled
			component.color = info.color;                     	// Parameter Verbose Name: Color
			component.brightness = info.brightness;             // Parameter Verbose Name: Brightness
			component.fadeSpeed = info.fadeSpeed;             	// Parameter Verbose Name: FadeSpeed
		}  

		// Only need to do this in editor due to game will not be creating/saving scenarios or lightmaps.
		#if UNITY_EDITOR 
		public override object GetComponentInfo () {
			var componentInfo = new LSS_Models.FlareModel();
			componentInfo.uniqueId = GetOrCreateUniqueID (gameObject).uniqueId;

			componentInfo.enabled = component.enabled;  			// Parameter Verbose Name: Enabled
			componentInfo.color = component.color;      			// Parameter Verbose Name: Color
			componentInfo.brightness = component.brightness;      // Parameter Verbose Name: Brightness
			componentInfo.fadeSpeed = component.fadeSpeed;        // Parameter Verbose Name: FadeSpeed

			return componentInfo;
		}

		public override int UpdateComponentInfos(LSS_Models.LightingScenarioModel lightingScenariosModels, bool insert = false) {
			var infos = lightingScenariosModels.flareInfos;
			var newInfos = GetModifiedList(infos, insert);

			if (newInfos != null) {
				lightingScenariosModels.flareInfos = newInfos.Select(s => (LSS_Models.FlareModel)s).ToArray();
				GetScenariosManager ().WriteJsonFile (lightingScenariosModels, GetScenariosManager ().GetResourcesStorageDirectory ());
			}
			return newInfos == null ? -1 : newInfos.Count();
		}
		#endif

	}
}