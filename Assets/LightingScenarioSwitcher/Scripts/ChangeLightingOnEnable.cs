using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSS;

public class ChangeLightingOnEnable : MonoBehaviour {

	[SerializeField] string loadFloader;

	void Awake () {
		GameObject.FindObjectOfType<LSS_FrontEnd> ().Load(loadFloader);
	}

}
