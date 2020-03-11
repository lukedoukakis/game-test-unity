using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class direction : MonoBehaviour
{

	public float sensitivity;
	public bool showCursor;
	
	
    // Start is called before the first frame update
    void Start(){
		
		if(showCursor == false){
			Cursor.visible = false;
		}
    }

    // Update is called once per frame
    void Update()
    {
		
		//float rotX = transform.localEulerAngles.x + Input.GetAxis("Mouse Y") * sensitivity;
		float rotY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
		
        // Dampen towards the target rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,rotY,0), Time.deltaTime * 100);
		
			
    }
}