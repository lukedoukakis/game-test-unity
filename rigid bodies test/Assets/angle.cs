using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angle : MonoBehaviour
{
	
	public GameObject player;
	public Vector3 orientation;
	
	Component[] boxColliders;
	Component groundTrigger;
	int i;
	
	
	
	
    // Start is called before the first frame update
    void Start()
    {
		
		boxColliders = player.GetComponents(typeof(BoxCollider));
		
		// initialize array element where groundTrigger is to be found
		i = 0;
		foreach ( BoxCollider bc in boxColliders ){
			if(bc.isTrigger){ break; }
			i++;
		}
    }

    // Update is called once per frame
    void Update()
    {
        
		// update groundTrigger status
		boxColliders = player.GetComponents(typeof(BoxCollider));
		groundTrigger = boxColliders[i];
		
		orientation = transform.rotation.eulerAngles;
	
    }
	
	
	void OnTriggerEnter(Collider other){
		transform.rotation = other.gameObject.transform.rotation;
	}
}
