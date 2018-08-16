using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {
	[HideInInspector]
	public int damage;
	[HideInInspector]
	public float force;
	public int projectileLife;
	private int hpAtual;
	public Sprite img;

	[HideInInspector]
	public string nomeDoAtirador;

	[HideInInspector]
	public int playerID;


    void Start(){
		hpAtual = projectileLife;
		if(isServer){
			GetComponent<SpriteRenderer>().sprite = img;
			RpcAtualizarSprite();
		}
    }
	[ClientRpc]
	void RpcAtualizarSprite(){
	 	GetComponent<SpriteRenderer>().sprite = img;
	}
	
	public virtual void morrer(){//CMD
		if(isServer)
			Destroy(gameObject);
	}

	public void levarDano(int dano){//CMD
		hpAtual -=dano;
		if(hpAtual <= 0){
			morrer();
		}
	}
	void OnCollisionEnter2D(Collision2D col){//CMD
		if(isServer){
			if(col.gameObject.tag == "Player" && col.gameObject.GetComponent<Player>().id != playerID){
				col.gameObject.GetComponent<Health>().CmdHeal(damage);
				morrer();
			}
			else {
				morrer();
			}
		}
	}

	[Command]
	public void CmdSyncTransformVelocity(Vector3 pos, Quaternion rotation, Vector2 velocity) {
		RpcSyncTransformVelocity(pos, rotation, velocity);
	}

	[ClientRpc]
	private void RpcSyncTransformVelocity(Vector3 pos, Quaternion rotation, Vector2 velocity) {
		transform.position = pos;
		transform.rotation = rotation;
		GetComponent<Rigidbody2D>().velocity = velocity;
	}

}
