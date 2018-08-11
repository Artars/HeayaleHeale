using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class armaLongRange : armaBase {
	// public float velbala;//velocidade da bala

	// public float spread;
	// public float knockbackTime;
	// public float Force;
	// public GameObject bala;
	// public Transform lugarDeTiro;
	// public float projectileTime;

	// [Command]
	// public override void CmdAtirar (){
	// 	if(playerScript.canShoot){ 
	// 		base.CmdAtirar();

	// 		GameObject aux =Instantiate(bala, lugarDeTiro.position, trans.rotation);

	// 		float rand = Random.Range(-spread, spread);

	// 		trans.Rotate(0,0,rand);
	// 		aux.GetComponent<Rigidbody2D>().velocity = (trans.up ) * velbala;
	// 		player.GetComponent<Rigidbody2D>().AddForce(-trans.up* Force );
	// 		trans.Rotate(0,0,-rand);
	// 		NetworkServer.Spawn(aux);
			
	// 		Destroy(aux,projectileTime);

	// 		playerScript.RpcSetCanWalk(false);
	// 		StartCoroutine(playerScript.esperaKnock(knockbackTime));

	// 		//aux.GetComponent<Rigidbody2D>().velocity = new Vector2(velbala,0f);
	// 		Bullet balaGerada = aux.GetComponent<Bullet>();
	// 		balaGerada.nomeDoAtirador = nomeDoAtirador;
	// 		balaGerada.damage = damage;

			
	// 	}
	// }



}

