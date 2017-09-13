using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAlarmSiren : MonoBehaviour {

	[SerializeField] Transform alarmSiren;

	void OnEnable () {
		alarmSiren.GetComponent<AudioSource> ().enabled = true;
	}
}
