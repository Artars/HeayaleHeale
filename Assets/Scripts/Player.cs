using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public Health vida;

	[SyncVar]
	public string username;
	public float speed;
	public int skinIndex;
	[HideInInspector]
	public bool canWalk = true;
	[HideInInspector]
	public bool canShoot = true;
	[HideInInspector]
	public armaBase gun;

	public GameObject testeGun;//PARA DEBUG

	private Transform trans;
	private Rigidbody2D rigi;
	private void Start(){
		trans = GetComponent<Transform>();
		rigi = GetComponent<Rigidbody2D>();
		CmdEquip(testeGun);//DEBUG;
	}

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();
		//Pega nome das preferencia

		GameManager.instance.CmdAddPlayer(gameObject);
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		if(spr != null) spr.color = Color.red;
	}


	public void Update(){
		if(!isLocalPlayer)
			return;

		if(canWalk)
			walk();
		if(canShoot && gun != null&&Input.GetButton("Fire1"))
			atirar();
	}

	private void walk(){
		float ang  = 0;
		if(Input.GetAxis("Vertical") != 0|| Input.GetAxis("Horizontal") != 0){
			//float angle = Mathf.Atan2(verticalAxis,horizontalAxis);
			//Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
			//transform.position += speed * Time.deltaTime * direction;
			//transform.rotation = Quaternion.Euler(0,0,angle * Mathf.Rad2Deg);
			ang = Mathf.Atan2 (-Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"))* Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0f,0f,ang);
			rigi.velocity = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")) * speed;
		}
	}

	private void atirar(){
		gun.CmdAtirar();

	}


	[Command]
	public void CmdUnequip(){
		Destroy(gun.gameObject);
		gun = null;
	}

	[Command]
	public void CmdEquip( GameObject newGun){
		if(gun != null)Destroy(gun.gameObject);
		gun = newGun.GetComponent<armaBase>();
		gun.equipado(gameObject);
	}

	[ClientRpc]
	public void RpcSetSkin(int id){
		if(isLocalPlayer){
			skinIndex = id;
		}
	}
	[ClientRpc]
	public void RpcSetCanShoot(bool valor){
		if(isLocalPlayer){
			canShoot = valor;
		}
	}
	[ClientRpc]
	public void RpcSetCanWalk(bool valor){
		if(isLocalPlayer){
			canWalk = valor;
		}
	}

	[ClientRpc]
	public void RpcPush(Vector2 force) {
		rigi.AddForce(force, ForceMode2D.Impulse);
	}



	public IEnumerator esperaKnock(float knockbackTime){
        yield return new WaitForSeconds(knockbackTime);
		RpcSetCanWalk(true);
    }

	
	public IEnumerator esperaReLoad(float fireCooldown){
        yield return new WaitForSeconds(fireCooldown);
		RpcSetCanShoot(true);
    }
}
