using UnityEngine;
using System.Collections;

public class LensFlareSpeed : MonoBehaviour {
	public LensFlare lf;




	void Update() {
		lf.fadeSpeed = 0.5F;
	}
}