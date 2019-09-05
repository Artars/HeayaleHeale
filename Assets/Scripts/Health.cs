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

	public void Hurt(float dano){
		hpAtual -=dano;
		if(hpAtual <= 0){
			GameManager.instance.PlayerWon(GetComponent<Player>());
		}else if(hpAtual >= hpMax)
			hpAtual = hpMax;
	}

	public void Heal(float dano){
		Hurt(-dano);
	}


	/// <summary>
	/// Atualizar barra de vida para a posição correta
	/// </summary>
	/// <param name="newCurrentLife"></param>
	public void UpdateCurrentLife(float newCurrentLife) {
		hpAtual = newCurrentLife;
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
