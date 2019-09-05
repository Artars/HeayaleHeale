using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance = null;
	public Dictionary<int,Player> players;
	public List<GameObject> boxes;

	[SyncVar]
	public bool hasGameStarted = false;

	public Dictionary<int, bool> readyPlayers;
	
	public int numPlayers = 0;

	//Lista de drops

	[Header("References")]
	public DeathCircle DeathCircle;

	public int numberOfSkins = 0;
	private List<int> remainingSkins;
	private bool isPaused = false;

	[Header("Item spawn")]
	public GameObject itemPrefab;
	public float timeToSpawn;
	private float timerSpawn = 0;
	public Vector3 lowerPos;
	public Vector3 upperPos;

    public void Awake()
    {
        if(instance == null) {
			instance = this;
			players = new Dictionary<int, Player>();
            readyPlayers = new Dictionary<int, bool>();
			remainingSkins = new List<int>();
			boxes = new List<GameObject>();
		}
		else if (instance != this) {
			Destroy(this);
		}
    }

    public void AddPlayer(Player player){
		if(player != null) {
			if(remainingSkins.Count < 1)
				fillSkinList();
			int i = Random.Range(0,remainingSkins.Count);
			player.skinIndex = (remainingSkins[i]);
			remainingSkins.RemoveAt(i);
			player.playerID = player.connectionToClient.connectionId;
            int id = player.playerID;

			numPlayers ++;

            //Update local dictionaries
            if(!readyPlayers.ContainsKey(id))
                readyPlayers.Add(id, false);
            else
                readyPlayers[id] = false;
            if(!players.ContainsKey(id))
                players.Add(id, player);
            else
                players[id] = player;

			Debug.Log("Added player " + id);
            PlayerManager.instance.readyPlayers.Add(player.playerID, false);

            UpdateReadyStatus();
		}
	}

    public void RemovePlayer(Player player)
    {
        if(player != null) {
            int id = player.playerID;

            players.Remove(id);
            readyPlayers.Remove(id);

            PlayerManager.instance.readyPlayers.Remove(player.playerID);

            UpdateReadyStatus();
        }
    }

    public void SetReady(int playerId){
		if(readyPlayers.ContainsKey(playerId))
        {
            Debug.Log("Player " + playerId + " is ready!");
            readyPlayers[playerId] = true;

            PlayerManager.instance.readyPlayers[playerId] = true;
            PlayerManager.instance.UpdateInitialHUD();

            UpdateReadyStatus();
        }
        else
        {
            Debug.LogWarning("PLAYER WITH ID " + playerId + " DOESN'T EXIST");
        }
	}

    /// <summary>
    /// Will check if game can start
    /// </summary>
    protected void UpdateReadyStatus()
    {
        bool isReady = true;
        foreach(KeyValuePair<int,bool> decision in readyPlayers)
        {
            if(decision.Value == false)
            {
                isReady = false;
                break;
            }
        }

		if(isReady){
			StartGame();
			hasGameStarted = true;
			DeathCircle.StartAtRandom();
		}
    }

	public void StartGame(){
		PlayerManager.instance.RpcStartGame();

        foreach(var player in players)
        {
            player.Value.playerState = Player.PlayerState.Playing;
        }
	}


	public int getSkin(){
		return Random.Range(0,numberOfSkins);
	}

	public void PlayerWon(Player player) {
		Debug.Log("Jogador " +  player.playerID + " ganhou!");
		PlayerManager.instance.RpcShowWhoWon("Player   " +  (player.playerID+1) + "   won!");
	}

	public void HasEndedCircle(){
		int minIndex = -1;
		float minValue = float.MaxValue;

        foreach (var player in players)
        {
            Health health = player.Value.GetComponent<Health>();
            if(health.getCurrentLife() < minValue){
				minValue = health.getCurrentLife();
				minIndex = player.Key;
			}	
        }

		if(minIndex != -1) {
			PlayerWon(players[minIndex]);
		}
	}

	private void Start() {

        string text = "";
        foreach (var ip in System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList){//[0].ToString());
            if(ip.ToString().Length < 17)
                text = ip.ToString();
        }
        PlayerManager.instance.RpcSetTextIp("HostIP: " + text);
	

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


	private void fillSkinList(){
		for(int i = 0; i < numberOfSkins; i++) {
			remainingSkins.Add(i);
		}
	}

}
