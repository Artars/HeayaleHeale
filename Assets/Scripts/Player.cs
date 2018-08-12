﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	Joystick joystick;
	Button button;
	public Health vida;
	Transform camTransform;
	public Vector3 camOffset;

	[SyncVar]
	public string username;
	public float speed;
	public float skinIndex;
	[HideInInspector]
	public bool canWalk = true;
	[HideInInspector]
	public bool canShoot = true;
	[HideInInspector]
	public armaBase gun;

	public GameObject testeGun;//PARA DEBUG

	private Transform trans;
	private Rigidbody2D rigi;
	private void Start(){
		trans = GetComponent<Transform>();
		rigi = GetComponent<Rigidbody2D>();
		CmdEquip(testeGun);//DEBUG;
	}

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();
		//Pega nome das preferencia

		GameManager.instance.CmdAddPlayer(gameObject);
		rigi = GetComponent<Rigidbody2D>();
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		if(spr != null) spr.color = Color.red;

		joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
		button = GameObject.Find("Button").GetComponent<Button>();	
		camTransform = Camera.main.transform;
		camOffset = new Vector3(0,0,-18);
		camTransform.position = transform.position + camOffset;
		foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList){//[0].ToString());
			Debug.Log(ip.ToString());
		}
	}

	[ClientRpc]
	public void RpcSetSkin(int id){
		if(isLocalPlayer){
			skinIndex = id;
		}
	}
	
	[ClientRpc]
	public void RpcSetCanWalk(bool valor){
		if(isLocalPlayer){
			canWalk = valor;
		}
	}

	[ClientRpc]
	public void RpcSetCanShoot(bool valor){
		if(isLocalPlayer){
			canShoot = valor;
		}
	}

	[ClientRpc]
	public void RpcPush(Vector2 force) {
		rigi.AddForce(force, ForceMode2D.Impulse);
	}

	[Command]
	public void CmdUnequip(){
		Destroy(gun.gameObject);
		gun = null;
	}

	[Command]
	public void CmdEquip( GameObject newGun){
		if(gun != null)Destroy(gun.gameObject);
		gun = newGun.GetComponent<armaBase>();
		gun.equipado(gameObject);
	}

	public IEnumerator esperaKnock(float knockbackTime){
        yield return new WaitForSeconds(knockbackTime);
		RpcSetCanWalk(true);
    }

	public IEnumerator esperaReLoad(float fireCooldown){
        yield return new WaitForSeconds(fireCooldown);
		RpcSetCanShoot(true);
    }


	private void Update(){

		if(!isLocalPlayer){
			return;
		}

		if(canWalk)
			walk();
		if(canShoot && gun != null && button.Pressed)
			atirar();

	}

	private void atirar(){
		Debug.Log("tiro");
		gun.CmdAtirar();

	}

	private void walk(){
		Vector3 moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);

		if (moveVector != Vector3.zero){ 
			transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
			transform.Translate(moveVector * speed * Time.deltaTime, Space.World);
			//camTransform.Translate(moveVector * speed * Time.deltaTime, Space.World);
			camTransform.position = transform.position + camOffset;
		}
	}

}