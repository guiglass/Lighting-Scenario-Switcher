using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LSS;

namespace LSS_Components
{
	[RequireComponent(typeof(GameObject))]
	public class LSS_GameObject : LSS_ComponentBase {

		private GameObject component { get { return gameObject; } }

		public void SetComponentInfo(LSS_Models.GameObjectModel info) { 
			component.SetActive(info.enabled);                 	// Parameter Verbose Name: Enabled
		}   
		// Only need to do this in editor due to game will not be creating/saving scenarios or lightmaps.
		#if UNITY_EDITOR 
		public override object GetComponentInfo () {
			var componentInfo = new LSS_Models.GameObjectModel();
			componentInfo.uniqueId = GetOrCreateUniqueID (gameObject).uniqueId;             
			componentInfo.enabled = component.activeInHierarchy;     // Parameter Verbose Name: Enabled
			return componentInfo;
		}

		public override int UpdateComponentInfos(LSS_Models.LightingScenarioModel lightingScenariosModels, bool insert = false) {
			var infos = lightingScenariosModels.gameObjectInfos;
			var newInfos = GetModifiedList(infos, insert);

			if (newInfos != null) {
				lightingScenariosModels.gameObjectInfos = newInfos.Select(s => (LSS_Models.GameObjectModel)s).ToArray();
				GetScenariosManager ().WriteJsonFile (lightingScenariosModels, GetScenariosManager ().GetResourcesStorageDirectory ());
			}
			return newInfos == null ? -1 : newInfos.Count();
		}
		#endif
	}
}