using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
	
	// colliders
	public BoxCollider hitBox;
	public BoxCollider groundTrigger;
	public Rigidbody rigidBody;
	
	// WASD movement
	public float moveSpeed_run;
	public float moveSpeed_sprint;
	float moveSpeed;
	float movementZ;
	float movementX;
	Vector3 vec_movementDir;
	
	// jumping
	public float jumpForce;
	bool jumpInput;
	Vector3 vec_jumpDir;
	Vector3 vec_surfaceNormal;
	
	// sensing
	public bool GROUNDTOUCH_THISFRAME;
	public bool GROUNDTOUCH_LASTFRAME;
	public int airTime;
	RaycastHit hitInfo;

	
	
    // Start is called before the first frame update
    void Start()
    {
		
		jumpInput = false;
    }

    // Update is called once per frame
    void Update()
    {
		
		if(GROUNDTOUCH_THISFRAME){
			airTime = 0;
		} else { airTime++; }
		
		if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift)){
			moveSpeed = moveSpeed_sprint;
		}else{
			moveSpeed = moveSpeed_run;
		}
		
        movementX = Input.GetAxis("Horizontal");
		movementZ = Input.GetAxis("Vertical");
		vec_movementDir = new Vector3(movementX, 0, movementZ);
		vec_movementDir *= moveSpeed;
		vec_movementDir = transform.TransformDirection(vec_movementDir);
		rigidBody.AddForce(vec_movementDir * Time.deltaTime, ForceMode.Impulse);
		
		Vector3 surfaceVec = new Vector3(vec_surfaceNormal.x, 0f, vec_surfaceNormal.z);
		rigidBody.AddForce(surfaceVec * moveSpeed * Time.deltaTime, ForceMode.Impulse);
		
		if(GROUNDTOUCH_THISFRAME && GROUNDTOUCH_LASTFRAME){
			if(Input.GetKey(KeyCode.Space)){
				jumpInput = true;
			}
		}

		GROUNDTOUCH_LASTFRAME = GROUNDTOUCH_THISFRAME;
	
    }
	
	// jump in FixedUpdate() to avoid inconsistent jump height
	void FixedUpdate(){
		if(jumpInput){
			jump();
			jumpInput = false;
		}
	}
	
	
	void jump(){
		transform.position = transform.position + (vec_surfaceNormal.normalized)*.1f;
		vec_jumpDir = vec_surfaceNormal * jumpForce * Time.deltaTime;
		rigidBody.AddForce(vec_jumpDir, ForceMode.Impulse);
	}
	
	void OnTriggerEnter(Collider other){
		GROUNDTOUCH_THISFRAME = true;
		vec_surfaceNormal = Vector3.Lerp(vec_surfaceNormal, other.gameObject.transform.TransformDirection(Vector3.up), 1f);
	}
	void OnTriggerStay(Collider other){
		GROUNDTOUCH_THISFRAME = true;
		//vec_surfaceNormal = Vector3.Lerp(vec_surfaceNormal, other.gameObject.transform.TransformDirection(Vector3.up),.1f);
	
	}
	void OnTriggerExit(Collider other){
		GROUNDTOUCH_THISFRAME = false;
	}
	
	public float getMoveSpeed(){
		return moveSpeed_run;
	}
}
