using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {
	public static GameManager instance = null;
	public List<Player> players;
	
	//Lista de drops
	
	
	//DEBUG
	public Text ipAdress;

	public int numberOfSkins = 1;
	private List<int> remainingSkins;

	private GameManager () {
		if(instance == null) {
			instance = this;
			players = new List<Player>();
			remainingSkins = new List<int>();
		}
		else if (instance != this) {
			Destroy(this);
		}
	}

	[Command]
	public void CmdAddPlayer(GameObject playerObject){
		Player player = playerObject.GetComponent<Player>();
		if(player != null) {
			players.Add(player);
			if(remainingSkins.Count < 1) {
				fillSkinList();
				int i = Random.Range(0,remainingSkins.Count);
				player.RpcSetSkin(remainingSkins[i]);
				remainingSkins.RemoveAt(i);
			}
		}
	}

	public void won(GameObject playerObject) {
		Player player = playerObject.GetComponent<Player>();
		Debug.Log("Jogador " +  player.username + " ganhou!");
	}

	[Command]
	public void CmdHasEndedCircle(){

	}

	private void Start() {
		if(isServer){
			RpcSetTextIp("HostIP: " + System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[3].ToString());
			foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList){//[0].ToString());
   Debug.Log(ip.ToString());
  }
		}

	}

	[ClientRpc]
	public void RpcSetTextIp(string ip) {
		Debug.Log(ip);
		ipAdress.text = ip;
	}


	private void onGameStart(){
	}

	private void fillSkinList(){
		for(int i = 0; i < numberOfSkins; i++) {
			remainingSkins.Add(i);
		}
	}
}