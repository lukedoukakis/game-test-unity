using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation : MonoBehaviour
{
	
	public Animator animator;
	public movement movementScript;
	public BoxCollider hitBox;
	
	bool groundTouch_lastFrame;
	bool groundTouch_thisFrame;
	
	
    // Start is called before the first frame update
    void Start()
    {
		animator.SetInteger("condition", 0);
    }

    // Update is called once per frame
    void Update()
    {
		//animator.SetInteger("condition", 0);
		
		//groundTouch_thisFrame = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), .5f);
		//groundTouch_thisFrame = Physics.SphereCast(transform.position, .5f, transform.TransformDirection(Vector3.down), out hitInfo, .5f);
		//groundTouch_thisFrame = Physics.BoxCast(transform.position + transform.TransformDirection(Vector3.up), new Vector3(boxCollider.size.x,.005f,boxCollider.size.z), transform.TransformDirection(Vector3.down), out hitInfo, transform.rotation, 1.5f);
		groundTouch_thisFrame = movementScript.GROUNDTOUCH_THISFRAME;
		
		int airTime = movementScript.airTime;
		
		if(groundTouch_thisFrame){
			
			animator.SetInteger("condition", 0);
			
			if(Input.GetKey(KeyCode.A)){
				animator.SetInteger("condition", -1);
			}
			if(Input.GetKey(KeyCode.D)){
				animator.SetInteger("condition", -1);
			}
			if(Input.GetKey(KeyCode.W)){
				animator.SetInteger("condition", 1);
				if(Input.GetKey(KeyCode.LeftShift)){
					animator.SetInteger("condition", 2);
				}
			}
			if(Input.GetKey(KeyCode.S)){
				animator.SetInteger("condition", -1);
			}
			if(Input.GetKey(KeyCode.Space)){
				animator.SetInteger("condition", 3);
			}
			if(!groundTouch_lastFrame && movementScript.airTime > 25){
				animator.SetInteger("condition", 5);
			}
		}
		
		groundTouch_lastFrame = groundTouch_thisFrame;
    }
}