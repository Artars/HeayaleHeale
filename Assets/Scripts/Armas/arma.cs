using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class arma  {
	public int damage;
	public float fireCooldown;

	public int maxAmmo;

	public float knockbackTime;
	public float Force;

	public Transform lugarDaArma;

	public Sprite img;
	public PlayerAnimationHandler.HandStances handstances;
}
