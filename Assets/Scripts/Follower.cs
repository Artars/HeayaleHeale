using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {

	public Transform target;
	public Vector3 offset = Vector3.zero;

	private Transform myTransform;

	private void Awake() {
		myTransform = transform;
	}

	private void Update() {
		if(target != null) {
			myTransform.position = target.position + offset;
		}
	}
}
