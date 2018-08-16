using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkDiscoveryAuto : NetworkDiscovery {

	public override void OnReceivedBroadcast(string fromAddress, string data) {
        NetworkManager network = GetComponent<NetworkManager>(); 
		network.networkAddress = fromAddress;
        network.StartClient();
    }
}
