using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class Player : NetworkBehaviour {

	public enum PlayerState
	{
		Lobby,Playing
	}

	public Animator animator;

	private AudioSource audioSource;

	[SyncVar(hook="changedID")]
	public int playerID = -1;

	[SyncVar]
	public PlayerState playerState = PlayerState.Lobby;

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
	[SyncVar(hook="UpdateSkin")]
	public int skinIndex = 0;
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

	public GameObject prefabHealthBar;
	public UnityEngine.UI.Text ammoText;
	private int maxAmmo;
	private bool shouldShowAmmo;

	private Transform trans;
	private Rigidbody2D rigi;
	private void Start(){
		trans = GetComponent<Transform>();
		rigi = GetComponent<Rigidbody2D>();
		audioSource = GetComponent<AudioSource>();
		if(gun == null)
			gun = new List<arma>();
		for(int i=0;i<gunM.Count;i++)
			gun.Add((arma)gunM[i]);
		for(int i=0;i<gunR.Count;i++)
			gun.Add((arma)gunR[i]);
		armaAtual = gun[armaInicial];
		animator = GetComponent<Animator>();
		GetComponent<PlayerAnimationHandler>().setSkinVariation(skinIndex);

		if(!isLocalPlayer) {	
			GameObject toSpawn = GameObject.Instantiate(prefabHealthBar);
			toSpawn.GetComponent<Follower>().target = transform;
			UnityEngine.UI.Slider slider = toSpawn.GetComponentInChildren<UnityEngine.UI.Slider>();
			GetComponent<Health>().setBar(slider);
		} else {

			if(playerID != -1) {
				changedID(playerID);
			}

			GetComponent<Health>().setBar(PlayerManager.instance.playerSlider);
			ammoText = PlayerManager.instance.ammoText;
		}

		Debug.Log("Started: " + playerID);
	}

	public override void OnNetworkDestroy()
	{
		if(isServer)
		{
			GameManager.instance.RemovePlayer(this);
		}
		else
		{
			if(PlayerManager.instance.players.ContainsKey(playerID))
				PlayerManager.instance.players.Remove(playerID);
		}
	}

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();

		Debug.Log("Local player");
		//Pega nome das preferencia
		//GameManager.instance.CmdAddPlayer(gameObject);
		// id = GameManager.instance.getNewID();
		// skinIndex = GameManager.instance.getSkin();
		// CmdUpdateSkinAndID(id,skinIndex);
		// skinIndex = GameManager.instance.CmdGetSkin();
		// changedID(id);
		// changedSkin(skinIndex);

		//FOR ANDROID
		#if UNITY_ANDROID
		joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
		button = GameObject.Find("Button").GetComponent<Button>();	
		
		#else
		GameObject.Find("Joystick").SetActive(false);
		GameObject.Find("Button").SetActive(false);	
		#endif

		camTransform = Camera.main.transform;
		camOffset = new Vector3(0,0,-18);
		camTransform.position = transform.position + camOffset;
		// foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList){//[0].ToString());
		// 	Debug.Log(ip.ToString());
		// }

	}

	public override void OnStartServer() {
		base.OnStartServer();
		Debug.Log("Server Start");
		GameManager.instance.AddPlayer(this);
	}

	public void UpdateSkin(int id){
		skinIndex = id;
		GetComponent<PlayerAnimationHandler>().setSkinVariation(skinIndex);
	}

	public void changedID(int newID){
		int previousId = playerID;
		playerID = newID;
		if(isLocalPlayer){
			PlayerManager.instance.thisPlayerId = playerID;
			PlayerManager.instance.thisLocalPlayer = this;
		}
		if(playerID != previousId && previousId != -1)
		{
			if(PlayerManager.instance.players.ContainsKey(previousId))
			PlayerManager.instance.players.Remove(previousId);
		}
		if(!PlayerManager.instance.players.ContainsKey(playerID))
			PlayerManager.instance.players.Add(playerID, this);
		PlayerManager.instance.players[playerID] = this;

	}

	[Command]
	public void CmdSetReady()
	{
		GameManager.instance.SetReady(playerID);
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
		armaAtual.lugarDaArma.GetComponent<SpriteRenderer>().sprite = null;
		armaAtual = gun[id];
		GetComponent<PlayerAnimationHandler>().setHandStance(armaAtual.handstances);
		if(isLocalPlayer){
			ammoAtual = armaAtual.maxAmmo;
			maxAmmo = armaAtual.maxAmmo;
			if(maxAmmo > 500)
				shouldShowAmmo = false;
			else
				shouldShowAmmo = true;
			updateAmmoText();
		}
		audioSource.clip = armaAtual.soundClip;
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

		if(playerState == PlayerState.Lobby)
			return;

		if(Time.timeScale == 0)
			return;


		//Atualizar posição da camera
		camTransform.position = transform.position + camOffset;

		if(canWalk && !Huged && !Hugging)
			walk();
		
		#if UNITY_ANDROID
		if( button.Pressed){
		#else
		if(Input.GetButton("Jump")){
		#endif

			if(canShoot &&  !Huged && !Hugging){
				ammoAtual -= 1;

				updateAmmoText();

				canShoot = false;
				if(ammoAtual <= 0){
					StartCoroutine(esperaReLoad(0.2f));
					CmdEquip(0);
				}else{
					StartCoroutine(esperaReLoad(armaAtual.fireCooldown));
				}

				if(armaAtual.GetType() == typeof(armaRanged)){
					armaRanged armaux = (armaRanged)armaAtual;
					float rand = Random.Range(-armaux.spread, armaux.spread);
					trans.Rotate(0,0,rand);
					rigi.AddForce(-trans.up*armaux.Force);

					CmdAtirar(trans.up);
					
					Debug.Log("armaRanged");

					// RpcSetCanWalk(false);
					if(armaAtual.knockbackTime != 0)
					{
						canWalk = false;
						StartCoroutine(esperaKnock(armaAtual.knockbackTime));
					}
				}
				else if(armaAtual.GetType() == typeof(armaMelee)){
					// Remover abraço porque é bugado
					// armaMelee armaux  = (armaMelee)armaAtual;
					// Debug.Log("armaMelle");
					// if(armaux.tipo == 0){
					// 	CmdHug();
					// }
				}else{
					Debug.Log("outraArma??");
				}
				
				// Colocar só na arma normal por enqunato
				// RpcSetCanWalk(false);
				// StartCoroutine(esperaKnock(armaAtual.knockbackTime));
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

	private void updateAmmoText(){
		if(ammoText != null) {
			ammoText.text = "Ammo: " + ammoAtual + " / " + maxAmmo;
			if(!shouldShowAmmo){
				ammoText.text = "";
			}
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
	private void CmdAtirar (Vector3 direction){
		armaRanged armaux = (armaRanged)armaAtual;
		// trans.rotation = rotation;
		// float rand = Random.Range(-armaux.spread, armaux.spread);
		// trans.Rotate(0,0,rand);

		// // RpcPush(-trans.up* armaux.Force) ;

		trans.up = direction;
		RpcPlaySound();
		gerarBala();

		if(armaux.tipo == 1){
			trans.Rotate(0,0,25);
			gerarBala();
			trans.Rotate(0,0,-50);
			gerarBala();
			trans.Rotate(0,0, 25);
		}
			



	}

	private void walk(){
		#if UNITY_ANDROID
		Vector3 moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);
		#else
		Vector3 moveVector = (Vector3.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("Vertical"));
		#endif

		bool wallAhead = false;
		//Verificar se não há parede à frente
		if(moveVector != Vector3.zero){
			Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + moveVector*0.3f, 0.1f);
			foreach(Collider2D c2d in hits) {
				if(c2d.gameObject.layer > 9){
				  wallAhead = true;
				  Debug.Log("Wall ahead");
				  break;
				}
				
			}
		}

		if (moveVector != Vector3.zero && !Huged && !Hugging){ 
			transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);

			if(!wallAhead){
				animator.SetBool("Walking",true);

				transform.Translate(moveVector * speed * Time.deltaTime, Space.World);
				//camTransform.Translate(moveVector * speed * Time.deltaTime, Space.World);
			}
			else{
				animator.SetBool("Walking",false);
			}
		}else {
			animator.SetBool("Walking",false);
		}

	}


	public int getRand(){
		return Random.Range(1, gun.Count);
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


	private void gerarBala(){
		armaRanged armaux = (armaRanged)armaAtual;
		GameObject aux =Instantiate(armaux.bala, armaux.lugarDeTiro.position, trans.rotation);


		aux.GetComponent<Rigidbody2D>().velocity = (trans.up ) * armaux.velbala;
		

		Bullet balaGerada = aux.GetComponent<Bullet>();
		balaGerada.nomeDoAtirador = username;
		balaGerada.damage = armaux.damage;
		balaGerada.force = armaux.Force;
		balaGerada.playerID = playerID;

		NetworkServer.Spawn(aux);
		Destroy(aux,armaux.projectileTime);
		
		balaGerada.CmdSyncTransformVelocity(aux.transform.position, aux.transform.rotation,(trans.up ) * armaux.velbala);
	}

	[Command]
	private void CmdAllPlaySound(){
		RpcPlaySound();
	}

	[ClientRpc]
	private void RpcPlaySound(){
		if(audioSource.clip != null)
		audioSource.Play();


	}
}