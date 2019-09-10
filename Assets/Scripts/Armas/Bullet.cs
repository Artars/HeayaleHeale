using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
			// RpcAtualizarSprite();
		}
		else
		{
			GetComponent<SpriteRenderer>().sprite = img;
			GetComponent<Rigidbody2D>().isKinematic = true;
		}
    }
	[ClientRpc]
	void RpcAtualizarSprite(){
	 	GetComponent<SpriteRenderer>().sprite = img;
	}
	
	public virtual void morrer(){//CMD
		if(isServer)
		{
			// Destroy(gameObject);
			NetworkServer.Destroy(gameObject);
		}
	}

	public void levarDano(int dano){//CMD
		hpAtual -=dano;
		if(hpAtual <= 0){
			morrer();
		}
	}
	void OnTriggerEnter2D(Collider2D col){//CMD
		if(isServer){
			if(col.gameObject.tag == "Player")
			{
				Player p = col.gameObject.GetComponent<Player>();
				if(p != null && p.playerID != playerID)
				{
					col.gameObject.GetComponent<Health>().Heal(damage);
					morrer();
				}
			}
			else  if (col.gameObject.tag != "DeathZone"){
				morrer();
			}
		}
		else
		{
			if (col.gameObject.tag != "DeathZone"){
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
