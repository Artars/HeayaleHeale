using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class armaRanged : arma {

	public float velbala;//velocidade da bala
	public float spread;

	public GameObject bala;
	public Transform lugarDeTiro;

	public float projectileTime;
	public int tipo;//0=nao shotgun;1=shotgun

	
}
