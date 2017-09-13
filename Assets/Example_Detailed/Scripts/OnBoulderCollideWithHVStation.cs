using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBoulderCollideWithHVStation : MonoBehaviour {

	[SerializeField] Transform boulder;
	[SerializeField] GameObject explosionParent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnCollisionEnter (Collision col) {
		if (col.transform.IsChildOf(boulder)) {
			explosionParent.SetActive(true);
		}
	}
}
