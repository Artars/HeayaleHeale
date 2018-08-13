using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public int hpMax;
	public int hpInicial;
	[SyncVar(hook = "UpdateCurrentLife")]
	private int hpAtual;
	public SpriteRenderer[] sprites;

	void Start(){
		hpAtual = hpInicial;
	}

	[Command]
	public void CmdHurt(int dano){
		hpAtual -=dano;
		if(hpAtual <= 0){
			GameManager.instance.Cmdwon(gameObject);
		}else if(hpAtual >= hpMax)
			hpAtual = hpMax;
	}

	[Command]
	public void CmdHeal(int dano){
		CmdHurt(-dano);
	}


	/// <summary>
	/// Atualizar barra de vida para a posição correta
	/// </summary>
	/// <param name="newCurrentLife"></param>
	public void UpdateCurrentLife(int newCurrentLife) {
		float porcentage = newCurrentLife / hpMax;
		Color color = new Color(1,1-porcentage,1-porcentage,1);
		foreach(SpriteRenderer sr in sprites){
			sr.color = color;
		}
	}

	public int getCurrentLife(){
		return hpAtual;
	}
}
