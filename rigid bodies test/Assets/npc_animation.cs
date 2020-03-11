using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_animation : MonoBehaviour
{
	
	public Animator animator;
	npc_ai movementScript;
	
	bool groundTouch_lastFrame;
	bool groundTouch_thisFrame;
	
    // Start is called before the first frame update
    void Start()
    {
		
		movementScript = GetComponent<npc_ai>();
		animator.SetInteger("condition", 0);
		
    }

    // Update is called once per frame
    void Update()
    {
		
        groundTouch_thisFrame = movementScript.GROUNDTOUCH_THISFRAME;
		
		// if on the ground
		if(groundTouch_thisFrame){
			if(groundTouch_lastFrame){
				if(movementScript.isRunning){
					animator.SetInteger("condition", 1);
				}
				else{
					animator.SetInteger("condition", 0);
				}
			}
			else{
				animator.SetInteger("condition", 5);
			}
		}
		
		//if in the air
		else{
			animator.SetInteger("condition", 3);
		}
		
		
		
		
		
		groundTouch_lastFrame = groundTouch_thisFrame;
    }
}
