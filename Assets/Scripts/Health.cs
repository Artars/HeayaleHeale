using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public const float maxLife = 100;
	[SyncVar(hook = "UpdateCurrentLife")]
	public float currentLife = maxLife;

	[Command]
	public void CmdHurt(float damage){
		currentLife -= damage;
		if(currentLife <= 0){
			GameManager.instance.won(gameObject);
		}
	}

	[Command]
	public void CmdHeal(float cure) {
		currentLife += cure;
		if(currentLife > maxLife)
			currentLife = maxLife;
	}

	/// <summary>
	/// Atualizar barra de vida para a posição correta
	/// </summary>
	/// <param name="newCurrentLife"></param>
	public void UpdateCurrentLife(float newCurrentLife) {

	}

}
