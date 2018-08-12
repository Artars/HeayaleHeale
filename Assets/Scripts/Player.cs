using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class Player : NetworkBehaviour {


	public List<arma> gun ;
//	enum armas{um,dois,tres};

	private arma armaAtual;
	private int armaInicial = 0;
	Joystick joystick;
	Button button;
	public Health vida;
	Transform camTransform;
	public Vector3 camOffset;
	private int ammoAtual = 10;
	[SyncVar]
	public string username;
	public float speed;
	public float skinIndex;
	[HideInInspector]
	public bool canWalk = true;
	[HideInInspector]
	public bool canShoot = true;



	private Transform trans;
	private Rigidbody2D rigi;
	private void Start(){
		trans = GetComponent<Transform>();
		rigi = GetComponent<Rigidbody2D>();
		armaAtual = gun[armaInicial];
	}

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();
		//Pega nome das preferencia

		GameManager.instance.CmdAddPlayer(gameObject);
		rigi = GetComponent<Rigidbody2D>();
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		if(spr != null) spr.color = Color.red;

		joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
		button = GameObject.Find("Button").GetComponent<Button>();	
		camTransform = Camera.main.transform;
		camOffset = new Vector3(0,0,-18);
		camTransform.position = transform.position + camOffset;
		foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList){//[0].ToString());
			Debug.Log(ip.ToString());
		}
	}

	[ClientRpc]
	public void RpcSetSkin(int id){
		if(isLocalPlayer){
			skinIndex = id;
		}
	}
	
	[ClientRpc]
	public void RpcSetCanWalk(bool valor){
		if(isLocalPlayer){
			canWalk = valor;
		}
	}

	[ClientRpc]
	public void RpcSetCanShoot(bool valor){
		if(isLocalPlayer){
			canShoot = valor;
		}
	}

	[ClientRpc]
	public void RpcPush(Vector2 force) {
		rigi.AddForce(force, ForceMode2D.Impulse);
	}


	//[Command]
	public void CmdEquip(int id){
		armaAtual.lugarDaArma.GetComponent<SpriteRenderer>().sprite = null;
		armaAtual = gun[id];	
		ammoAtual = armaAtual.maxAmmo;
		armaAtual.lugarDaArma.GetComponent<SpriteRenderer>().sprite= armaAtual.img;
	}

	public IEnumerator esperaKnock(float knockbackTime){
        yield return new WaitForSeconds(knockbackTime);
		RpcSetCanWalk(true);
    }

	public IEnumerator esperaReLoad(float fireCooldown){
        yield return new WaitForSeconds(fireCooldown);
		RpcSetCanShoot(true);
    }


	private void Update(){

		if(!isLocalPlayer){
			return;
		}

		if(canWalk)
			walk();
		if(canShoot  && button.Pressed)
			CmdAtirar();

	}

	[Command]
	private void CmdAtirar (){
		ammoAtual -= 1;
		if(ammoAtual <= 0){
			RpcSetCanShoot(false);
			StartCoroutine(esperaReLoad(0.2f));
			CmdEquip(0);
		}else{
			RpcSetCanShoot(false);
			StartCoroutine(esperaReLoad(armaAtual.fireCooldown));
		}

		GameObject aux =Instantiate(armaAtual.bala, armaAtual.lugarDeTiro.position, trans.rotation);

			float rand = Random.Range(-armaAtual.spread, armaAtual.spread);

			trans.Rotate(0,0,rand);
			aux.GetComponent<Rigidbody2D>().velocity = (trans.up ) * armaAtual.velbala;
			RpcPush(-trans.up* armaAtual.Force) ;
			trans.Rotate(0,0,-rand);
			NetworkServer.Spawn(aux);
			
			Destroy(aux,armaAtual.projectileTime);
			RpcSetCanWalk(false);
			StartCoroutine(esperaKnock(armaAtual.knockbackTime));

			Bullet balaGerada = aux.GetComponent<Bullet>();
			balaGerada.nomeDoAtirador = username;
			balaGerada.damage = armaAtual.damage;
	}

	private void walk(){
		Vector3 moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);

		if (moveVector != Vector3.zero){ 
			transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
			transform.Translate(moveVector * speed * Time.deltaTime, Space.World);
			//camTransform.Translate(moveVector * speed * Time.deltaTime, Space.World);
			camTransform.position = transform.position + camOffset;
		}
	}


	public int getRand(){
		return Random.Range(0, gun.Count);
	}
}