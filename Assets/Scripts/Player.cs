using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public Animator animator;

	public List<armaMelee> gunM ;
	public List<armaRanged> gunR ;
	//public List<armaRanged> gunL ;//lazer

	private List<arma> gun ;//todas as armas

	[HideInInspector]
	public arma armaAtual;

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
	public int skinIndex;
	[HideInInspector]
	public bool canWalk = true;
	[HideInInspector]
	public bool canShoot = true;
	[HideInInspector]
	[SyncVar]
	public bool Huged = false;
	[HideInInspector]
	[SyncVar]
	public bool Hugging = false;
	[SyncVar]
	public GameObject acariciado;


	private Transform trans;
	private Rigidbody2D rigi;
	private void Start(){
		trans = GetComponent<Transform>();
		rigi = GetComponent<Rigidbody2D>();
		if(gun == null)
			gun = new List<arma>();
		for(int i=0;i<gunM.Count;i++)
			gun.Add((arma)gunM[i]);
		for(int i=0;i<gunR.Count;i++)
			gun.Add((arma)gunR[i]);
		armaAtual = gun[armaInicial];
		animator = GetComponent<Animator>();
		GetComponent<PlayerAnimationHandler>().setSkinVariation(skinIndex);

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
			GetComponent<PlayerAnimationHandler>().setSkinVariation(id);
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


	[Command]
	public void CmdEquip(int id){
		RpcEquip(id);
	}
	[ClientRpc]
	private void RpcEquip(int id){
		GetComponent<PlayerAnimationHandler>().setHandStance(armaAtual.handstances);
		armaAtual.lugarDaArma.GetComponent<SpriteRenderer>().sprite = null;
		armaAtual = gun[id];
		ammoAtual = armaAtual.maxAmmo;
		armaAtual.lugarDaArma.GetComponent<SpriteRenderer>().sprite= armaAtual.img;
	}

	public IEnumerator esperaKnock(float knockbackTime){
        yield return new WaitForSeconds(knockbackTime);
		canWalk = true;
		//RpcSetCanWalk(true);
    }

	public IEnumerator esperaReLoad(float fireCooldown){
        yield return new WaitForSeconds(fireCooldown);
		canShoot = true;
		//RpcSetCanShoot(true);
    }

	public IEnumerator esperaHug(float tempoHug){
        yield return new WaitForSeconds(tempoHug);
		if(Hugging)CmdsoltarHug();
    }

	//espera hitbox desaparecer
	public IEnumerator esperaHitBox(GameObject hitbox, float tempoHitBox){
        yield return new WaitForSeconds(tempoHitBox);
		hitbox.SetActive(false);//pronto para revisao
    }
	private void Update(){

		if(!isLocalPlayer){
			return;
		}

		if(canWalk && !Huged && !Hugging)
			walk();
		if( button.Pressed){
			if(canShoot &&  !Huged && !Hugging){
				ammoAtual -= 1;

				canShoot = false;
				if(ammoAtual <= 0){
					StartCoroutine(esperaReLoad(0.2f));
					CmdEquip(0);
				}else{
					StartCoroutine(esperaReLoad(armaAtual.fireCooldown));
				}

				if(armaAtual.GetType() == typeof(armaRanged)){
					CmdAtirar();
					Debug.Log("armaRanged");
				}
				else if(armaAtual.GetType() == typeof(armaMelee)){
					armaMelee armaux  = (armaMelee)armaAtual;
					Debug.Log("armaMelle");
					if(armaux.tipo == 0){
						CmdHug();
					}
				}else{
					Debug.Log("outraArma??");
				}
				

				RpcSetCanWalk(false);
				StartCoroutine(esperaKnock(armaAtual.knockbackTime));
			}
		}else if(Hugging && !Input.GetKeyDown("space")){
			CmdsoltarHug();
			Player aux = acariciado.GetComponent<Player>();
			aux.canWalk = false;
			aux.StartCoroutine(aux.esperaKnock(armaAtual.knockbackTime));
			aux.canShoot = false;
			aux.StartCoroutine(aux.esperaReLoad(armaAtual.knockbackTime));	
		}
	

	}
	[Command]
	private void CmdHug(){
		armaMelee armaux = (armaMelee)armaAtual;
		RpcPush(trans.up* armaux.Force) ;
		armaux.hitbox.SetActive(true);
		StartCoroutine(esperaHitBox(armaux.hitbox, armaux.knockbackTime));
	}

	[Command]
	private void CmdAtirar (){
		armaRanged armaux = (armaRanged)armaAtual;
		GameObject aux =Instantiate(armaux.bala, armaux.lugarDeTiro.position, trans.rotation);

			float rand = Random.Range(-armaux.spread, armaux.spread);

			trans.Rotate(0,0,rand);
			aux.GetComponent<Rigidbody2D>().velocity = (trans.up ) * armaux.velbala;
			RpcPush(-trans.up* armaux.Force) ;
			trans.Rotate(0,0,-rand);
			NetworkServer.Spawn(aux);
			
			Destroy(aux,armaux.projectileTime);

			Bullet balaGerada = aux.GetComponent<Bullet>();
			balaGerada.nomeDoAtirador = username;
			balaGerada.damage = armaux.damage;

	}

	private void walk(){
		Vector3 moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);

		if (moveVector != Vector3.zero && !Huged && !Hugging){ 
			animator.SetBool("Walking",true);
			transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);

				transform.Translate(moveVector * speed * Time.deltaTime, Space.World);
				//camTransform.Translate(moveVector * speed * Time.deltaTime, Space.World);
				camTransform.position = transform.position + camOffset;
		}else {
			animator.SetBool("Walking",false);
			if(Huged || Hugging)
			camTransform.position = transform.position + camOffset;}
	}


	public int getRand(){
		return Random.Range(0, gun.Count);
	}


	[Command]
	private void CmdsoltarHug(){
		acariciado.GetComponent<Player>().RpcPush(trans.up* armaAtual.Force*3) ;
		RpcsoltarHug();
	}
	

	
	[ClientRpc]
	private void RpcsoltarHug(){
		Hugging = false;
		Player aux = acariciado.GetComponent<Player>();
		aux.Huged = false;
		acariciado = null;	
	}
}