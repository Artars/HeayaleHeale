using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

	public int damage;
	public float force;
	public int projectileLife;
	private int hpAtual;

	[HideInInspector]
	public string nomeDoAtirador;

    void Start(){
		hpAtual = projectileLife;

    }


	
	public virtual void morrer(){
		if(isServer)
			Destroy(gameObject);
	}

	public void levarDano(int dano){
		hpAtual -=dano;
		if(hpAtual <= 0){
			morrer();
		}
	}
	void OnCollisionEnter2D(Collision2D col){
		if(isServer){
			Bullet bul = col.gameObject.GetComponent<Bullet>();
			if(bul != null)
				levarDano(bul.damage);
		}
		else if(col.gameObject.tag == "Player")
			col.gameObject.GetComponent<Health>().CmdHeal(damage);
	}

}
