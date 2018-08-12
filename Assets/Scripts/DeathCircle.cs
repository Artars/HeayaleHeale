﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathCircle : NetworkBehaviour {
	public float minDamage;
	public float maxDamage;
	public float minRadius;
	public float maxRadius;
	public float hurtTimeInterval;
	public float sizeDelta;
	public float sizeChangeInterval;
	public Transform[] closePoints;

	private List<GameObject> playersInside;

	private enum State{
		IsIdle, IsShrinking, isStable, finished
	};

	private float porcentage = 1;
	private Transform myTransform;
	[SyncVar]
	private State currentState = State.IsIdle;
	private float timerSize;
	private float timerDamage;
	private Vector3 currentScale;
	private float scaleSpeed;
	private bool shouldDamage = true;


	private void Awake() {
		myTransform = transform;
		playersInside = new List<GameObject>();
	}

	public void startAtRandom() {
		if(!isServer)
			return;
		Transform position = closePoints[Random.Range(0,closePoints.Length)];
		myTransform.position = position.position;
		currentScale = new Vector3(maxRadius, maxRadius,1);
		myTransform.localScale = currentScale;
		currentState = State.isStable;
		timerSize = sizeChangeInterval;
		timerDamage = hurtTimeInterval;
		scaleSpeed = sizeDelta/sizeChangeInterval;
		porcentage = 0;
	}	

	private void Update() {
		if(!isServer)
			return;
		float delta = Time.deltaTime;
		timerDamage -= delta;
		timerDamage -= delta;
		if(timerDamage <= 0){
			if(shouldDamage){
				float Damage = (maxDamage-minDamage)* porcentage + minDamage;

				foreach(GameObject g in playersInside) {
					Health h = g.GetComponent<Health>();
					h.CmdHurt((int) Damage);
				}
			}
			timerDamage = hurtTimeInterval;
		}
		if(timerSize <= 0){
			if(currentState == State.isStable) {
				currentState = State.IsShrinking;	
			}
			if(currentState == State.IsShrinking) {
				currentState = State.isStable;
				if(currentScale.x <= minRadius){
					currentState = State.finished;
					GameManager.instance.CmdHasEndedCircle();
					shouldDamage = false;
				}
			}
			timerSize = sizeChangeInterval;

		}
		if(currentState == State.IsShrinking){
			currentScale -= delta * scaleSpeed * new Vector3(1,1,0);
			myTransform.localScale = currentScale;
		}
	}

	[Command]
	public void CmdSetCanDamage(bool canDamage) {
		this.shouldDamage = canDamage;
	}

	[Command]
	public float currentRadius() {
		float scale = myTransform.localScale.x;
		if(currentState == State.IsIdle)
			scale = 0;
		return myTransform.localScale.x;
	}
} 
