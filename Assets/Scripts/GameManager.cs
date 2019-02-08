using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {
	public static GameManager instance = null;
	public Dictionary<int,GameObject> players;
	public List<GameObject> boxes;

	[SyncVar]
	public bool hasGameStarted = false;

	public Dictionary<int, bool> readyPlayers;
	
	[SyncVar(hook = "UpdatePlayerNumber")]
	public int numPlayers = 0;

	public int thisPlayer = -1;

	//Lista de drops

	[Header("References")]
	public GameObject StartScreen;
	public UnityEngine.UI.Slider progressReady;
	public Text readyText;
	public DeathCircle DeathCircle;
	public Slider playerSlider;
	public Text ammoText;
	public GameObject winPanel;
	public Text winText;
	//DEBUG
	public Text ipAdress;

	public int numberOfSkins = 0;
	private List<int> remainingSkins;
	private bool isPaused = false;

	[Header("Item spawn")]
	public GameObject itemPrefab;
	public float timeToSpawn;
	private float timerSpawn = 0;
	public Vector3 lowerPos;
	public Vector3 upperPos;
	

	private GameManager () {
		// if(instance == null) {
			instance = this;
			players = new Dictionary<int, GameObject>();
			remainingSkins = new List<int>();
			boxes = new List<GameObject>();
		// }
		// else if (instance != this) {
			// Destroy(this);
		// }
	}

	[Command]
	public void CmdUpdateAll() {
		foreach(KeyValuePair<int,bool> kpair in readyPlayers){
			RpcUpdateReadyStatus(kpair.Key,kpair.Value);	
		}
	}

	[Command]
	public void CmdUpdateStatus(int id, bool value) {
		Debug.Log("User " + id + "is ready? " + value);
		readyPlayers[id] = value;
		RpcUpdateReadyStatus(id, value);
		bool isReady = true;
		for(int i = 0; i < numPlayers; i++){
			if(!readyPlayers.ContainsKey(i)){
				isReady = false;
				break;
			}
			if(!readyPlayers[i]){
				isReady = false;
				break;
			}
		}
		if(isReady){
			RpcStartGame();
			StartGame();
			hasGameStarted = true;
			DeathCircle.CmdStartAtRandom();

		}
	}

	[ClientRpc]
	public void RpcStartGame(){
		StartGame();
	}

	public void StartGame(){
		pause(false);
		StartScreen.SetActive(false);
	}

	[ClientRpc]
	public void RpcUpdateReadyStatus(int id, bool value){
		readyPlayers[id] = value;
		UpdatePlayerNumber(numPlayers);
	}

	public void UpdatePlayerNumber(int numNumber) {
		if(readyPlayers == null)
			readyPlayers = new Dictionary<int, bool>();

		if(!readyPlayers.ContainsKey(numNumber-1))
			readyPlayers.Add(numNumber-1,false);
		if(numNumber == 0)
			return;
		float count = 0;
		for(int i = 0; i < numPlayers; i++){
			if(readyPlayers.ContainsKey(i) && readyPlayers[i])
				count += 1;
		}
		float porcentage = count / numPlayers;
		if(progressReady != null)
			progressReady.value = porcentage;
		if(readyText != null)
			readyText.text = count + " / " + numPlayers;
	}

	[Command]
	public void CmdAddPlayer(GameObject playerObject){
		Player player = playerObject.GetComponent<Player>();
		if(player != null) {
			Debug.Log("Passou");
			if(remainingSkins.Count < 1)
				fillSkinList();
			int i = Random.Range(0,remainingSkins.Count);
			player.RpcSetSkin(remainingSkins[i]);
			remainingSkins.RemoveAt(i);
			player.RpcSetID(numPlayers);
			numPlayers ++;
		}
	}

	public int getNewID(){
		int id = numPlayers;
		numPlayers++;
		// UpdatePlayerNumber(numPlayers);
		return id;
	}

	public int getSkin(){
		return Random.Range(0,numberOfSkins);
	}

	public void addLocalReference(int id,GameObject player){
		if(!players.ContainsKey(id))
			this.players.Add(id,player);
		else
			this.players[id] = player;
	}

	public void addLocalBoxReference(GameObject box) {
		boxes.Add(box);
	}

	public void removeLocalBoxReference(GameObject box) {
		boxes.Remove(box);
	}

	[Command]
	public void Cmdwon(GameObject playerObject) {
		Player player = playerObject.GetComponent<Player>();
		Debug.Log("Jogador " +  player.id + " ganhou!");
		RpcShowWhoWon("Player   " +  (player.id+1) + "   won!");
	}

	[ClientRpc]
	public void RpcShowWhoWon(string text){
		winPanel.SetActive(true);
		winText.text = text;
	} 

	[Command]
	public void CmdHasEndedCircle(){
		int minIndex = -1;
		float minValue = float.MaxValue;
		for(int i = 0; i < players.Count; i++){
			Health health = players[i].GetComponent<Health>();
			if(health.getCurrentLife() < minValue){
				minValue = health.getCurrentLife();
				minIndex = i;
			}			
		}
		if(minIndex != -1) {
			Cmdwon(players[minIndex]);
		}
	}

	private void Start() {
		if(readyPlayers == null)
			readyPlayers = new Dictionary<int, bool>();
		if(isServer){
			string text = "";
			foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList){//[0].ToString());
				if(ip.ToString().Length < 17)
					text = ip.ToString();
  			}
			RpcSetTextIp("HostIP: " + text);

			// onGameStart();
		}

		if(isClient &&  hasGameStarted){
			quitGame();
		}

		pause(true);

	}

	private void Update() {
		if(isServer){
			if(hasGameStarted) {
				bool canSpawn = false;
				float x =0 ,y = 0;
				timerSpawn -= Time.deltaTime;
				if(timerSpawn <= 0) {
					while(!canSpawn) {
						x = Random.Range(lowerPos.x, upperPos.x);
						y = Random.Range(lowerPos.y, upperPos.y);

						canSpawn = true;
						Collider2D[] hits = Physics2D.OverlapCircleAll(new Vector2(x,y),1);
						foreach(Collider2D c2 in hits) {
							if(c2.gameObject.layer > 9){
								canSpawn = false;
								break;
							}
						}
					}
					GameObject toInstance = GameObject.Instantiate(itemPrefab ,new Vector3 (x,y,0),Quaternion.identity);
					NetworkServer.Spawn(toInstance);
					timerSpawn = timeToSpawn;
				}
			}
		}
	}

	public void toggleReady(){
		if(thisPlayer != -1) {
			if(!readyPlayers.ContainsKey(thisPlayer) || !readyPlayers[thisPlayer]){
				CmdUpdateStatus(thisPlayer, true);
			}
		}
	}

	[ClientRpc]
	public void RpcSetTextIp(string ip) {
		Debug.Log(ip);
		ipAdress.text = ip;
	}



	private void fillSkinList(){
		for(int i = 0; i < numberOfSkins; i++) {
			remainingSkins.Add(i);
		}
	}

	public void CmdSetGamePaused(bool toPause) {
		RpcSetGamePaused(toPause);
	}
	private void RpcSetGamePaused(bool toPause){
		if(isPaused ^ toPause){
			isPaused = toPause;
			Time.timeScale = (isPaused) ? 1 : 0;
		}
	}

	private void pause(bool toPause){
		if(isPaused ^ toPause){
			isPaused = toPause;
			Time.timeScale = (isPaused) ? 0 : 1;
		}
		Debug.Log("Paused: " + isPaused);
	}


	public void quitGame() {
		NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		// NetworkDiscovery NetworkDiscovery = GameObject.Find("NetworkManager").GetComponent<NetworkDiscovery>();
		if(isServer){
			// if(NetworkDiscovery != null)
			// 	NetworkDiscovery.StopBroadcast();
			networkManager.StopHost();
		}
		if(isClient){
			// if(NetworkDiscovery != null)
			// 	NetworkDiscovery.StopBroadcast();
			networkManager.StopClient();
		}
	}
}