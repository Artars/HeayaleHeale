using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour {
	public static PlayerManager instance = null;
	public Dictionary<int, Player> players;
	public List<GameObject> boxes;

	public class ReadyDictionary : SyncDictionary<int,bool> {}

	[SyncVar]
	public bool hasGameStarted = false;

	public readonly ReadyDictionary readyPlayers = new ReadyDictionary();

	public Player thisLocalPlayer;
	public int thisPlayerId = -1;

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

	private bool isPaused = false;
	
	private void Awake()
	{
		if(instance == null) {
			instance = this;
			boxes = new List<GameObject>();
			players = new Dictionary<int, Player>();
		}
		else if (instance != this) {
			Destroy(this);
		}
	}

	private void Start() {

		if(readyPlayers != null)
		{
			readyPlayers.Callback += UpdateReadyStatus;
			UpdateInitialHUD();
		}
		else
		{
			Debug.Log("Failed to set callback");
		}
		if(readyPlayers == null)
		
		if(isClient &&  hasGameStarted){
			quitGame();
		}

		pause(true);

	}


	[ClientRpc]
	public void RpcStartGame(){
		StartGame();
	}

	public void StartGame(){
		pause(false);
		StartScreen.SetActive(false);
	}

	public void UpdateReadyStatus(ReadyDictionary.Operation op, int id, bool value){
		UpdateInitialHUD();
	}

	[ClientRpc]
	public void RpcForceUpdateInitialHUD()
	{
		UpdateInitialHUD();
	}

	public void UpdateInitialHUD() {
		int numPlayers = readyPlayers.Count;
		float count = 0;
		foreach(var player in readyPlayers){
			if(player.Value)
				count += 1;
		}
		float porcentage = count / numPlayers;
		if(progressReady != null)
			progressReady.value = porcentage;
		if(readyText != null)
			readyText.text = count + " / " + numPlayers;
	}


	public void addLocalBoxReference(GameObject box) {
		boxes.Add(box);
	}

	public void removeLocalBoxReference(GameObject box) {
		boxes.Remove(box);
	}

	[ClientRpc]
	public void RpcShowWhoWon(string text){
		winPanel.SetActive(true);
		winText.text = text;
	} 


	public void SetReady(){
		if(thisLocalPlayer != null) {
			thisLocalPlayer.CmdSetReady();
		}
	}

	[ClientRpc]
	public void RpcSetTextIp(string ip) {
		Debug.Log(ip);
		ipAdress.text = ip;
	}

	[ClientRpc]
	public void RpcSetGamePaused(bool toPause){
		isPaused = toPause;
		Time.timeScale = (isPaused) ? 0 : 1;
	}

	private void pause(bool toPause){
		if(isPaused ^ toPause){
			isPaused = toPause;
			Time.timeScale = (isPaused) ? 0 : 1;
		}
		Debug.Log("Paused: " + isPaused);
	}


	public void quitGame() {
		NetworkManager.Shutdown();
	}

	public void OnOpenMenu() {
		if(isServer)
		{
			GameManager.instance.PauseGameAll(true);
		}
	}

	public void OnCloseMenu() {
		if(isServer)
		{
			GameManager.instance.PauseGameAll(false);
		}
	}
}