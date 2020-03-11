using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

	public BoxCollider hitBox;
	public BoxCollider groundTrigger;
	public Rigidbody rigidBody;
	
	public bool GROUNDTOUCH_THISFRAME;
	public bool GROUNDTOUCH_LASTFRAME;
	public int airTime;
	
	public float moveSpeed_run;
	public float moveSpeed_sprint;
	float movementX;
	float movementZ;
	Vector3 movementTot;
	
	float moveForce;
	public float jumpForce;
	Vector3 jumpVec;
	bool jumpInput;
	
	Vector3 surfaceAngle;
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
			moveForce = moveSpeed_sprint;
		}else{
			moveForce = moveSpeed_run;
		}
		
        movementX = Input.GetAxis("Horizontal");
		movementZ = Input.GetAxis("Vertical");
		movementTot = new Vector3(movementX, 0, movementZ);
		movementTot *= moveForce;
		movementTot = transform.TransformDirection(movementTot);
		rigidBody.AddForce(movementTot * Time.deltaTime, ForceMode.Impulse);
		
		Vector3 surfaceVec = new Vector3(surfaceAngle.x, 0f, surfaceAngle.z);
		rigidBody.AddForce(surfaceVec * moveForce * Time.deltaTime, ForceMode.Impulse);
		
		if(GROUNDTOUCH_THISFRAME && GROUNDTOUCH_LASTFRAME){
			if(Input.GetKey(KeyCode.Space)){
				jumpInput = true;
			}
		}

		GROUNDTOUCH_LASTFRAME = GROUNDTOUCH_THISFRAME;
	
    }
	
	
	void FixedUpdate(){
		if(jumpInput){
			jump();
			jumpInput = false;
		}
	}
	
	
	void jump(){
		transform.position = transform.position + (surfaceAngle.normalized)*.1f;
		jumpVec = surfaceAngle * jumpForce * Time.deltaTime;
		rigidBody.AddForce(jumpVec, ForceMode.Impulse);
	}
	
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
	
	public float getMoveSpeed(){
		return moveSpeed_run;
	}
}
