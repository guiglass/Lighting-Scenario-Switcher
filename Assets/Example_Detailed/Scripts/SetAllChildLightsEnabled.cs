using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAllChildLightsEnabled : MonoBehaviour {

	public void toggle (bool state) {
		//foreach (Transform child in GetComponentsInChildren<Transform>()) {
			foreach (Light light in GetComponentsInChildren<Light>()) {
				light.enabled = state;
			}
		//}
	}

}
