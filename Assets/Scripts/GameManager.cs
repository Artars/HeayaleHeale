using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {
	public static GameManager instance = null;
	public List<GameObject> players;
	
	//Lista de drops

	public DeathCircle DeathCircle;
	
	
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
			players = new List<GameObject>();
			remainingSkins = new List<int>();
		// }
		// else if (instance != this) {
			// Destroy(this);
		// }
	}

	[Command]
	public void CmdAddPlayer(GameObject playerObject){
		Player player = playerObject.GetComponent<Player>();
		if(player != null) {
			players.Add(playerObject);
			if(remainingSkins.Count < 1) {
				fillSkinList();
				int i = Random.Range(0,remainingSkins.Count);
				player.RpcSetSkin(remainingSkins[i]);
				remainingSkins.RemoveAt(i);
			}
		}
	}

	public void RpcUpdatePlayerReferences(List<GameObject> players){
		this.players = players;
	}

	[Command]
	public void Cmdwon(GameObject playerObject) {
		Player player = playerObject.GetComponent<Player>();
		Debug.Log("Jogador " +  player.username + " ganhou!");
		RpcShowWhoWon("Jogador " +  player.username + " ganhou!");
	}

	[ClientRpc]
	public void RpcShowWhoWon(string text){
		ipAdress.text = text;
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
		if(isServer){
			RpcSetTextIp("HostIP: " + System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[3].ToString());
			foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList){//[0].ToString());
   Debug.Log(ip.ToString());
  			}

			onGameStart();
		}

	}

	private void Update() {
		if(isServer){
			bool canSpawn = false;
			float x =0 ,y = 0;
			timerSpawn -= Time.deltaTime;
			if(timerSpawn <= 0) {
				while(!canSpawn) {
					x = Random.Range(lowerPos.x, upperPos.x);
					y = Random.Range(lowerPos.y, upperPos.y);
					canSpawn = true;

				}
				GameObject toInstance = GameObject.Instantiate(itemPrefab ,new Vector3 (x,y,0),Quaternion.identity);
				NetworkServer.Spawn(toInstance);
				timerSpawn = timeToSpawn;
			}
		}
	}

	[ClientRpc]
	public void RpcSetTextIp(string ip) {
		Debug.Log(ip);
		ipAdress.text = ip;
	}


	private void onGameStart(){
		DeathCircle.CmdStartAtRandom();

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
}