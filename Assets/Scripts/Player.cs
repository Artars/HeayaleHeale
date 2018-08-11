using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	Joystick joystick;
	Button button;

	[SyncVar]
	public string username;
	public float speed;
	public float skinIndex;
	public bool canWalk = true;
	public bool canShoot = true;
	public float meleeRange;

	private Rigidbody2D rb2d;

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();
		//Pega nome das preferencia

		GameManager.instance.CmdAddPlayer(gameObject);
		rb2d = GetComponent<Rigidbody2D>();
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		if(spr != null) spr.color = Color.red;

		joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
		button = GameObject.Find("Button").GetComponent<Button>();
	}

	[ClientRpc]
	public void RpcSetSkin(int newSkin) {
		skinIndex = newSkin;
		//Logica de mudança de skin
	} 
	[ClientRpc]
	public void RpcSetCanMove(bool canMove) {
		if(isLocalPlayer){
			canWalk = canMove;
		}
	}

	[ClientRpc]
	public void RpcSetCanShoot(bool can) {
		if(isLocalPlayer){
			canShoot = can;
		}
	}

	[ClientRpc]
	public void RpcPush(Vector2 force) {
		rb2d.AddForce(force, ForceMode2D.Impulse);
	}

	[Command]
	public void CmdEquip(GameObject weapon){

	}

	[Command]
	public void CmdUnequip(GameObject weapon){

	}

	// private void Update() {
	// 	if(!isLocalPlayer)
	// 		return;
		

	// 	//Logica de movimento
	// 	float verticalAxis = Input.GetAxis("Vertical");
	// 	float horizontalAxis = Input.GetAxis("Horizontal");
	// 	Debug.Log("Vertical: " + verticalAxis + " Horizontal: " + horizontalAxis);
	// 	if(Mathf.Abs(verticalAxis) > 0 || Mathf.Abs(horizontalAxis) > 0) {
	// 		float angle = Mathf.Atan2(verticalAxis,horizontalAxis);
	// 		Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
	// 		transform.position += speed * Time.deltaTime * direction;
	// 		transform.rotation = Quaternion.Euler(0,0,angle * Mathf.Rad2Deg);
	// 	}
	// }


	private void Update(){

		if(!isLocalPlayer){
			return;
		}

		Vector3 moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);

		if (moveVector != Vector3.zero){ 
			transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
			transform.Translate(moveVector * speed * Time.deltaTime, Space.World);
		}

		if(button.Pressed){
			Debug.Log("tiro");
		}

	}
}

