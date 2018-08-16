using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkHUD : MonoBehaviour {
	public InputField input;

	private NetworkManager networkManager;
	private NetworkDiscovery networkDiscovery;

	public string address;

	private void Start() {
		GameObject networkObj = GameObject.Find("NetworkManager");
		networkManager = networkObj.GetComponent<NetworkManager>();
		networkDiscovery = networkObj.GetComponent<NetworkDiscovery>();
		if(PlayerPrefs.HasKey("LastIP")) {
			address = PlayerPrefs.GetString("LastIP");
			input.text = address;
		}
	}

	public void updateAdress(){
		if(input != null){
			address = input.text;
			PlayerPrefs.SetString("LastIP", address);
		}
	}

	public void onJoinClick() {
		networkManager.networkAddress = address;
		networkManager.StartClient();
	}

	public void onHostClick() {
		if(networkDiscovery != null){
			networkDiscovery.Initialize();
			//networkDiscovery.StartAsServer();
		}
		networkManager.StartHost();
	}

	public void onAutoJoinClick(){
		if(networkDiscovery != null){
			networkDiscovery.Initialize();
		}
	}

}
