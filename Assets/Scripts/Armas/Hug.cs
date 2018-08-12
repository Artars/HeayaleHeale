using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Hug : NetworkBehaviour {
	[HideInInspector]
	public int damage;
	public GameObject player;


	void OnTriggerEnter2D(Collider2D col){
		Player aux = player.gameObject.GetComponent<Player>();
		if(col.gameObject.tag == "Player" && aux.acariciado == null){
				col.gameObject.GetComponent<Health>().CmdHeal(damage);

				col.gameObject.GetComponent<Player>().Huged = true;
				aux.Hugging = true;
				aux.acariciado = col.gameObject;

				col.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
				col.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
				player.GetComponent<Rigidbody2D>().angularVelocity = 0f;
				player.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
				aux.StartCoroutine(aux.esperaHug(((armaMelee)aux.armaAtual).tempoHug));
		}
	}
}
