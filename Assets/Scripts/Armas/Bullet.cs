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
			Bullet bul = col.gameObject.GetComponent<Bullet>();
			if(bul != null){
				if(col.gameObject.GetComponent<Bullet>().nomeDoAtirador != nomeDoAtirador)
				levarDano(bul.damage);
			}
			else if(col.gameObject.tag == "Player" && col.gameObject.GetComponent<Player>().username != nomeDoAtirador){
				col.gameObject.GetComponent<Health>().CmdHeal(damage);
				morrer();
			}
		}
	}

}
