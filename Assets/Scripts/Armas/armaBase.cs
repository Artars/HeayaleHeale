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

	
	[Command]
	public void Cmdequipado(GameObject pla){
		player = pla;
		playerScript = player.GetComponent<Player>();
		nomeDoAtirador = playerScript.username;

//		trans = GetComponent<Transform>();
		RpcSetParent(gameObject, player);
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


	[ClientRpc]
	void RpcSetParent(GameObject obj, GameObject parent){ 
		obj.transform.parent = parent.transform; 
	}


}
