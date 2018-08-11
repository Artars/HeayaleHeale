using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
	public static GameManager instance = null;
	public List<Player> players;
	
	//Lista de drops
	
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

	private void fillSkinList(){
		for(int i = 0; i < numberOfSkins; i++) {
			remainingSkins.Add(i);
		}
	}
}