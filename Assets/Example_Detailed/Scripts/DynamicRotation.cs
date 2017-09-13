using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DynamicRotation : MonoBehaviour {

	[SerializeField] Vector3 spinAmount;

	void Update () {
		transform.Rotate ( spinAmount * Time.deltaTime);
	}


}
