using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Box :  NetworkBehaviour {
	public GameObject[] guns ;


	void OnCollisionEnter2D(Collision2D col){
		if(isServer && col.gameObject.tag == "Player"){
			int rand = Random.Range(0, guns.Length);
			GameObject aux =Instantiate(guns[rand]);
			NetworkServer.Spawn(aux);
			col.gameObject.GetComponent<Player>().CmdEquip(aux);
		}
	}
}
