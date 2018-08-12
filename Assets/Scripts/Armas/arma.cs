using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class arma  {
	
	public int damage;
	public float fireCooldown;

	public int maxAmmo;

	public float velbala;//velocidade da bala
	public float spread;
	public float knockbackTime;
	public float Force;
	public GameObject bala;
	public Transform lugarDeTiro;

	public Transform lugarDaArma;
	public float projectileTime;

	public Sprite img;
}
