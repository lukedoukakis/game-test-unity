using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class npc_ai : MonoBehaviour
{
	
	public Rigidbody rigidBody;
	public BoxCollider hitBox;
	public BoxCollider groundTrigger;
	
	public GameObject player;
	movement playerScript;
	
	public bool GROUNDTOUCH_THISFRAME;
	public bool GROUNDTOUCH_LASTFRAME;
	public bool isRunning;
	
	
	// *********************
	// distance settings
	
	public float senseDistance;
	public float maxFleeDistance;
	public float maxFollowDistance;
	public float minFollowDistance;
	public float maxJumpableObstacleHeight;
	public float maxJumpFromDistance;
	
	
	// *********************
	// movement vars
	
	Vector3 distanceVec;
	Quaternion targetRot;
	public float rotMagnitude;
	float rotMultiplier;
	
	float moveSpeed;
	Vector3 moveForce;
	
	public float jumpForce;
	Vector3 jumpVec;
	Vector3 surfaceAngle;
	
	Vector3 POS_THISFRAME;
	Vector3 POS_LASTFRAME;
	Vector3 travelVec;
	
	// *********************
	
	
	// sensing vars
	RaycastHit leftHitInfo;
	RaycastHit centerHitInfo;
	RaycastHit rightHitInfo;
	
	bool centerCast;
	bool leftCast;
	bool rightCast;
	
	float leftDistance;
	float centerDistance;
	float rightDistance;
	
	bool deadEnd;
	
	bool playerSensed;
	
	// *********************
	
	

    // Start is called before the first frame update
    void Start()
    {
		
		POS_LASTFRAME = transform.position + new Vector3(0,1,0);
		
		playerScript = player.GetComponent<movement>();
		moveSpeed = playerScript.getMoveSpeed() * 1;
        
    }

    // Update is called once per frame
    void Update()
    {
		
		POS_THISFRAME = transform.position;
		
		flee(player);
		//follow(player);
		
		GROUNDTOUCH_LASTFRAME = GROUNDTOUCH_THISFRAME;
		POS_LASTFRAME = POS_THISFRAME;

    }
	
	void rotateAway(Vector3 targetPos, float magnitude){
		
		Vector3 differenceVec = transform.position - targetPos;
		differenceVec.y = 0;
		
		targetRot = Quaternion.LookRotation(differenceVec, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, magnitude);
	}
	
	void rotateToward(Vector3 targetPos, float magnitude){
		
		Vector3 differenceVec = transform.position - targetPos;
		differenceVec.y = 0;
		
		targetRot = Quaternion.LookRotation(differenceVec*-1, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, magnitude);
	}
	
	void move(Vector3 direction, float magnitude){
		moveForce = transform.TransformDirection(direction) * moveSpeed;
		rigidBody.AddForce(moveForce * magnitude * Time.deltaTime, ForceMode.Impulse);
	}
	
	void jump(){
		rigidBody.MovePosition(transform.position + surfaceAngle);
		jumpVec = surfaceAngle * jumpForce * Time.deltaTime;
		rigidBody.AddForce(jumpVec, ForceMode.Impulse);
	}
	
	float getObstacleDistance(float d1, float d2, float d3){
		return Mathf.Min(Mathf.Min(d1, d2), d3);
	}
	
	
	bool canClearObstacle(){
		if(
				!Physics.Raycast(transform.position + new Vector3(0, .1f, 0) + Vector3.up*maxJumpableObstacleHeight, (transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.left)).normalized * 1f, out leftHitInfo, senseDistance/2)
			&& 	!Physics.Raycast(transform.position + new Vector3(0, .1f, 0) + Vector3.up*maxJumpableObstacleHeight, transform.TransformDirection(Vector3.forward).normalized * 1f, out leftHitInfo, senseDistance*2)
			&& 	!Physics.Raycast(transform.position + new Vector3(0, .1f, 0) + Vector3.up*maxJumpableObstacleHeight, (transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.right)).normalized * 1f, out rightHitInfo, senseDistance/2)
		){
			return true;
		}
		return false;
	}
	
	/*
	sets: 		Raycasts:				leftCast, centerCast, rightCast
				Raycast distances:		leftDistance, centerDistance, rightDistance}
				boolean:				deadEnd
			
	return:		true:					leftCast, centerCast, rightCast, or deadEnd
				false:					otherwise
				
	mode:		0:						sensing for dead end
				1:						sensing for general obstacle
	*/
	bool senseObstacle(){
		
		playerSensed = false;
		
		float castDistance = senseDistance;
		
		// set raycasts to reach castDistance units away
		leftCast = Physics.Raycast(transform.position + new Vector3(0, .1f, 0), (transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.left)).normalized, out leftHitInfo, castDistance);
		centerCast = Physics.Raycast(transform.position + new Vector3(0, .1f, 0), transform.TransformDirection(Vector3.forward).normalized * 1f, out centerHitInfo, castDistance);
		rightCast = Physics.Raycast(transform.position + new Vector3(0, .1f, 0), (transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.right)).normalized, out rightHitInfo, castDistance);
		
		// set leftDistance, centerDistance, rightDistance, and playerSensed;
		if(leftCast){
			leftDistance = (leftHitInfo.point - transform.position).magnitude;
			if(leftHitInfo.collider.gameObject.tag == "Player"){
				playerSensed = true;
			}
		}else{
			leftDistance = int.MaxValue;
		}
		if(centerCast){
			centerDistance = (centerHitInfo.point - transform.position).magnitude;
			if(centerHitInfo.collider.gameObject.tag == "Player"){
				playerSensed = true;
			}
		}else{
			centerDistance = int.MaxValue;
		}
		if(rightCast){
			rightDistance = (rightHitInfo.point - transform.position).magnitude;
			if(rightHitInfo.collider.gameObject.tag == "Player"){
				playerSensed = true;
			}
		}else{
			rightDistance = int.MaxValue;
		}
		
		if((leftCast || centerCast || rightCast)){
			return true;
		}
		return false;
		
	}
	
	/*
	navigate based on distance of raycast contacts
	param rotMagnitude:	turn speed
	*/
	void turnTowardsMostOpenPath(float rotMagnitude){
	
		Quaternion leftRot = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.left)*1f, Vector3.up);
		Quaternion rightRot = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward) + transform.TransformDirection(Vector3.right)*1f, Vector3.up);
		
		if(leftDistance < rightDistance){
			transform.rotation = Quaternion.Slerp(transform.rotation, rightRot, rotMagnitude);
		} else{
			transform.rotation = Quaternion.Slerp(transform.rotation, leftRot, rotMagnitude);
		}
		
		//print(leftDistance + " " + centerDistance + " " + rightDistance);
	}
	
	/*
	run away from player if between minFleeDistance and maxFleeDistance
	*/
	void flee (GameObject obj){
		
		distanceVec = transform.position - obj.transform.position;
		distanceVec.y = 0;
		
		if(distanceVec.magnitude < maxFleeDistance){
			
			// if obstacle in front
			if(senseObstacle()){
				
				// if obstacle can't be jumped over, navigate around it
				if(!canClearObstacle()){
					turnTowardsMostOpenPath(rotMagnitude);
					rotateAway(player.transform.position, (rotMagnitude * .5f) );
				}
				else{
					
					// if close enough to obstacle and on the ground, jump
					if(getObstacleDistance(leftDistance, centerDistance, rightDistance) < maxJumpFromDistance){
						if(GROUNDTOUCH_THISFRAME && GROUNDTOUCH_LASTFRAME){					
							jump();
						}
					}
					rotateAway(player.transform.position, rotMagnitude);
				}
			}
		
			// move forward
			isRunning = true;
			move(Vector3.forward, 1f);
		} else {
			isRunning = false;
			rotateToward(player.transform.position, .1f);
		}
		
		while(rotMultiplier >= 1f){
			rotMultiplier -= .1f;
		}
	}
	
	// FINISH
	/*
	// follow player if between minFollowDistance and maxFollowDistance
	void follow (GameObject obj){
		
		distanceVec = transform.position - obj.transform.position;
		distanceVec.y = 0;
		
		if(distanceVec.magnitude < maxFollowDistance && distanceVec.magnitude > minFollowDistance){
			
			// if obstacle in front and it's not the player object
			if(senseObstacle() && !playerSensed){
				
				// if obstacle can't be jumped over, navigate around it
				if(!canClearObstacle()){
					navigateObstacle();
					rotateToward(player.transform.position, .05f);
				}
				else{	
				
					// if close enough to obstacle and on the ground, jump
					if(getObstacleDistance(leftDistance, centerDistance, rightDistance) < maxJumpFromDistance){
						if(GROUNDTOUCH_THISFRAME && GROUNDTOUCH_LASTFRAME){
							jump();
						}
					}
					rotateToward(player.transform.position, .5f);
				}
			}
		
			// move forward
			isRunning = true;
			move(Vector3.forward, 1f);
		} else {
			isRunning = false;
		}
	}
	*/
	
	void OnTriggerEnter(Collider other){
		GROUNDTOUCH_THISFRAME = true;
		surfaceAngle = Vector3.Lerp(surfaceAngle, other.gameObject.transform.TransformDirection(Vector3.up), 1f);
	}
	void OnTriggerStay(Collider other){
		GROUNDTOUCH_THISFRAME = true;
		//surfaceAngle = Vector3.Lerp(surfaceAngle, other.gameObject.transform.TransformDirection(Vector3.up),.1f);
	
	}
	void OnTriggerExit(Collider other){
		GROUNDTOUCH_THISFRAME = false;
	}
}	
