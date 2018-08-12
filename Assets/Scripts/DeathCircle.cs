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

	private State currentState = State.IsIdle;
	public float timerSize;
	private float timerDamage;
	private Vector3 currentScale;
	private float scaleSpeed;
	private bool shouldDamage = true;


	private void Awake() {
		myTransform = transform;
		playersInside = new List<GameObject>();
	}

	[Command]
	public void CmdStartAtRandom() {
		Transform position = closePoints[Random.Range(0,closePoints.Length)];
		RpcStartAtTransform(position.position);
		myTransform.position = position.position;
		currentScale = new Vector3(maxRadius, maxRadius,1);
		myTransform.localScale = currentScale;
		currentState = State.isStable;
		timerSize = sizeChangeInterval;
		timerDamage = hurtTimeInterval;
		scaleSpeed = sizeDelta/sizeChangeInterval;
		porcentage = 0;
	}

	[ClientRpc]
	public void RpcStartAtTransform(Vector3 position){
		myTransform.position = position;
		currentScale = new Vector3(maxRadius, maxRadius,1);
		myTransform.localScale = currentScale;
		currentState = State.isStable;
		timerSize = sizeChangeInterval;
		timerDamage = hurtTimeInterval;
		scaleSpeed = sizeDelta/sizeChangeInterval;
		porcentage = 0;
	}

	private void Update() {
		float delta = Time.deltaTime;
		timerDamage -= delta;
		timerSize -= delta;
		if(timerDamage <= 0){
			if(shouldDamage && isServer){
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
			else if(currentState == State.IsShrinking) {
				currentState = State.isStable;
				if(currentScale.x <= minRadius){
					currentState = State.finished;
					GameManager.instance.CmdHasEndedCircle();
					shouldDamage = false;
				}
				else{
					if(isServer){
						RpcForceRadius(currentScale.x,currentState);
					}
				}
			}
			timerSize = sizeChangeInterval;
		}
		if(currentState == State.IsShrinking){
			currentScale -= delta * scaleSpeed * new Vector3(1,1,0);
			myTransform.localScale = currentScale;
		}
	}

	[ClientRpc]
	private void RpcForceRadius(float radius, State forceState){
		currentScale = new Vector3(radius,radius,1);
		myTransform.localScale = currentScale;
		currentState = forceState;
		this.timerSize = sizeChangeInterval;
	}

	[Command]
	public void CmdSetCanDamage(bool canDamage) {
		this.shouldDamage = canDamage;
	}

	public float currentRadius() {
		float scale = myTransform.localScale.x;
		if(currentState == State.IsIdle)
			scale = 0;
		return myTransform.localScale.x;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(!isServer)
			return;
		if(other.tag == "Player") {
			playersInside.Add(other.gameObject);
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if(!isServer)
			return;
		if(other.tag == "Player") {
			playersInside.Remove(other.gameObject);
		}
	}
} 
