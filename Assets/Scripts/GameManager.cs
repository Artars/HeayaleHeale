﻿using System.Collections;
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

	public int numberOfSkins = 1;
	private List<int> remainingSkins;
	private bool isPaused = false;

	private GameManager () {
		if(instance == null) {
			instance = this;
			players = new List<GameObject>();
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

	public void won(GameObject playerObject) {
		Player player = playerObject.GetComponent<Player>();
		Debug.Log("Jogador " +  player.username + " ganhou!");
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
			won(players[minIndex]);
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