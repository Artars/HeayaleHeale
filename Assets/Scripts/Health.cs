using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Health : NetworkBehaviour {

	public float hpMax;
	public float hpInicial;
	[SyncVar(hook = "UpdateCurrentLife")]
	private float hpAtual;
	public Slider slider;

	void Start(){
		hpAtual = hpInicial;
	}

	[Command]
	public void CmdHurt(float dano){
		hpAtual -=dano;
		if(hpAtual <= 0){
			GameManager.instance.Cmdwon(gameObject);
		}else if(hpAtual >= hpMax)
			hpAtual = hpMax;
	}

	[Command]
	public void CmdHeal(float dano){
		CmdHurt(-dano);
	}


	/// <summary>
	/// Atualizar barra de vida para a posição correta
	/// </summary>
	/// <param name="newCurrentLife"></param>
	public void UpdateCurrentLife(float newCurrentLife) {
		float porcentage = newCurrentLife / hpMax;
		
		if(slider != null)
			slider.value = porcentage;
	}

	public void setBar(Slider bar){
		slider = bar;
		UpdateCurrentLife(hpAtual);
	}

	public float getCurrentLife(){
		return hpAtual;
	}
}
