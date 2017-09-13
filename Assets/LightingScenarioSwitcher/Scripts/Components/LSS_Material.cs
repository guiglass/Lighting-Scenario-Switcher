using UnityEngine;
using LSS;

namespace LSS_Components
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LSS_FrontEnd))]
	public class LSS_Material : MonoBehaviour {
		
		[SerializeField] 
		public Material[] materials = new Material[]{null};

	}
}