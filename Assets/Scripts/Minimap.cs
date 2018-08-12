﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {

	public RectTransform imageMap;
	public GameObject playerPointerPrefab;
	public GameObject cratePointerPrefab;
	public GameObject circlePrefab;

	public int numberOfPlayersToSpawn = 20;
	public int numberOfBoxesToSpawn = 30;

	public float relativeSizePlayer;
	public float relativeSizeCrate;

	public Vector3 lowerLeftCorner;
	public Vector3 topRightCorner;

	private List<RectTransform> playerPointer;
	private List<RectTransform> boxPointer;
	private RectTransform circlePointer;
	
	private void Start() {
		playerPointer = new List<RectTransform>();
		boxPointer = new List<RectTransform>();
		for(int i = 0; i < numberOfPlayersToSpawn; i++){
			GameObject pointer = GameObject.Instantiate(playerPointerPrefab);
			playerPointer.Add(pointer.GetComponent<RectTransform>());
			pointer.transform.SetParent(imageMap.transform);

			pointer.SetActive(false);
		}

		for(int i = 0; i < numberOfBoxesToSpawn; i++){
			GameObject pointer = GameObject.Instantiate(cratePointerPrefab);
			boxPointer.Add(pointer.GetComponent<RectTransform>());
			pointer.transform.SetParent(imageMap.transform);

			pointer.SetActive(false);
		}

		GameObject circle = GameObject.Instantiate(circlePrefab);
		circle.transform.SetParent(imageMap.transform);
		circlePointer = circle.GetComponent<RectTransform>();
		circle.SetActive(false);
	}

	private void Update() {
		float sizeX = imageMap.rect.xMax - imageMap.rect.xMin;
		float sizeY = imageMap.rect.yMax - imageMap.rect.yMin;
		float mySizeX,mySizeY;

		mySizeX = sizeX *relativeSizePlayer;
		mySizeY = sizeY *relativeSizePlayer;

		for(int i = 0; i < playerPointer.Count; i++){
			if(i < GameManager.instance.players.Count){
				Vector2 porc = porcentagePosition(GameManager.instance.players[i].transform.position);
				Vector2 pos = new Vector2(sizeX*porc.x,sizeY*porc.y);
				playerPointer[i].gameObject.SetActive(true);
				playerPointer[i].SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,pos.x-mySizeX/2,mySizeX);
				playerPointer[i].SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom,pos.y-mySizeY/2,mySizeY);
			}
			else{
				if(playerPointer[i].gameObject.activeInHierarchy){
					playerPointer[i].gameObject.SetActive(false);
				}
				else{
					break;
				}
			}
		}

		mySizeX = sizeX * relativeSizeCrate;
		mySizeY = sizeY * relativeSizeCrate;

		// for(int i = 0; i < playerPointer.Count; i++){
		// 	if(i < GameManager.instance. .Count){
		// 		Vector2 porc = porcentagePosition(GameManager.instance.players[i].transform.position);
		// 		Vector2 pos = new Vector2(sizeX*porc.x,sizeY*porc.y);
		// 		playerPointer[i].SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,pos.x-mySizeX/2,mySizeX);
		// 		playerPointer[i].SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom,pos.y-mySizeY/2,mySizeY);
		// 	}
		// 	else{
		// 		if(playerPointer[i].gameObject.activeInHierarchy){
		// 			playerPointer[i].gameObject.SetActive(false);
		// 		}
		// 		else{
		// 			break;
		// 		}
		// 	}
		// }

		if( GameManager.instance.DeathCircle != null) {
			if(circlePointer.gameObject.activeInHierarchy == false)
				circlePointer.gameObject.SetActive(true);
			float radius = GameManager.instance.DeathCircle.currentRadius();
			mySizeX = mySizeY = porcentagePosition(lowerLeftCorner + new Vector3(radius,0,0)).x * sizeX;
			Vector2 porcent = porcentagePosition(GameManager.instance.DeathCircle.transform.position);
			Vector2 posi = new Vector2(sizeX*porcent.x,sizeY*porcent.y);
			circlePointer.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,posi.x-mySizeX/2,mySizeX);
			circlePointer.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom,posi.y-mySizeY/2,mySizeY);
		}
		else{
			if(circlePointer.gameObject.activeInHierarchy == true)
				circlePointer.gameObject.SetActive(false);
			
		}
	}

	private Vector2 porcentagePosition(Vector3 position) {
		float xAxis = (position.x - lowerLeftCorner.x)/(topRightCorner.x - lowerLeftCorner.x);
		float yAxis = (position.y - lowerLeftCorner.y)/(topRightCorner.y - lowerLeftCorner.y);

		xAxis = Mathf.Clamp(xAxis,0,1);
		yAxis = Mathf.Clamp(yAxis,0,1);

		return new Vector2(xAxis,yAxis);
	}

	
}