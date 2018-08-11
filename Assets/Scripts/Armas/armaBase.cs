using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class armaBase : NetworkBehaviour {

	public int damage;
	public float fireCooldown;

	public int maxAmmo;
	public int id;

	protected int ammoAtual;
	[HideInInspector]
	public string nomeDoAtirador;
	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	protected Player playerScript;
	[HideInInspector]
	public Transform trans;

	void Start (){
		ammoAtual = maxAmmo;
		trans = GetComponent<Transform>();
	}

	public void equipado(GameObject pla){
		player = pla;
		playerScript = player.GetComponent<Player>();
		nomeDoAtirador = playerScript.username;

		trans = GetComponent<Transform>();
		player.GetComponent<NetworkTransformChild>().target = trans;
	}

	[Command]
	public virtual void CmdAtirar (){
		ammoAtual -= 1;
		if(ammoAtual <= 0){
			playerScript.CmdUnequip();
		}
		playerScript.RpcSetCanShoot(false);
		StartCoroutine(playerScript.esperaReLoad(fireCooldown));
    }



}
