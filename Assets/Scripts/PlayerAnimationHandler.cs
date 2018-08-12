using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour {

	[Header("Sprites")]
	public Sprite[] baseSprite;
	public Sprite[] rightLegForwardSprite;
	public Sprite[] leftLegForwardSprite;
	public Sprite[] rightArmForwardSprite;
	public Sprite[] leftArmForwardSprite;
	public Sprite[] bothArmsCloseSprite;
	public Sprite[] bothArmsOpenSprite;

	public Sprite[] leftLegForwardWithArmSprite;
	public Sprite[] rightLegForwardWithArmSprite;


	[Header("References")]
	public SpriteRenderer baseRenderer;
	public SpriteRenderer handsRenderer;

	[Header("Properties")]
	public int skinVariation;
	public HandStances handStance = HandStances.noArms;
	private int currentAniamtionStage = 0;
	
	
	public enum HandStances{
		noArms, rightArm, leftArm, bothArmsClose, bothArmsOpen 
	};

	public void setSkinVariation(int skinVariation) {
		this.skinVariation = skinVariation;
		if(skinVariation >= baseSprite.Length) skinVariation = baseSprite.Length -1;
		setAnimationFrame(currentAniamtionStage);
		setHandStance(handStance);
		
	}

	public void setAnimationFrame(int stage){
		currentAniamtionStage = stage;
		switch(stage){
			case -1:
				baseRenderer.sprite = leftLegForwardSprite[skinVariation];
				if(handStance == HandStances.noArms) {
					baseRenderer.sprite = leftLegForwardWithArmSprite[skinVariation];
				}
				break;
			case 0:
				baseRenderer.sprite = baseSprite[skinVariation];
				break;
			case 1:
				baseRenderer.sprite = rightLegForwardSprite[skinVariation];
				if(handStance == HandStances.noArms){
					baseRenderer.sprite = rightLegForwardWithArmSprite[skinVariation];
				}
				break;
			default:
				baseRenderer.sprite = baseSprite[skinVariation];
				break;

		}
	}

	public void setHandStance(HandStances hand) {
		handStance = hand;
		switch (handStance) {
			case HandStances.noArms:
				handsRenderer.sprite = null;
				break;
			case HandStances.bothArmsClose:
				handsRenderer.sprite = bothArmsCloseSprite[skinVariation];
				break;
			case HandStances.bothArmsOpen:
				handsRenderer.sprite = bothArmsOpenSprite[skinVariation];
				break;
			case HandStances.leftArm:
				handsRenderer.sprite = leftArmForwardSprite[skinVariation];
				break;
			case HandStances.rightArm:
				handsRenderer.sprite = rightArmForwardSprite[skinVariation];
				break;
		}
	}

}
