using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayChildEnable : MonoBehaviour {
	
	[SerializeField] GameObject child;
	[SerializeField] float delay = 0;
	[SerializeField] bool toState = true;

	void Start () {
		Invoke ("EnableChild", delay);
	}

	void EnableChild () {
		child.SetActive (toState);
		Destroy (this);
	}
}
