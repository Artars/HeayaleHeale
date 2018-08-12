using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Box :  NetworkBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if(	isServer && col.gameObject.tag == "Player"){
			Player aux = col.gameObject.GetComponent<Player>();
			if(col.gameObject == null)
				Debug.Log("nulo");
			if(aux == null)
				Debug.Log("nulo2");
			aux.CmdEquip(aux.getRand());
			Destroy(gameObject);
		}
	}

}
