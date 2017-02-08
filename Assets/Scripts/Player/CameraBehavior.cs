using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {
    [SerializeField]
    Transform playerpos;
    RaycastHit hit;
    Vector3 desiredposition;
    Vector3 targetposition;
    Vector3 offset;

    
    LayerMask wallLayer;

	// Use this for initialization
	void Start () {
        wallLayer = LayerMask.NameToLayer("impassable");

        offset = new Vector3(playerpos.position.x -3, playerpos.position.y + 8f, playerpos.position.z-3);
        desiredposition = offset;
    }
	
	// Update is called once per frame
	void Update () {
        

		
	}


    void FollowMe()
    {
        
        desiredposition = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * 1.5f, Vector3.up) * desiredposition;
        if (Physics.Raycast(desiredposition,playerpos.position - desiredposition, out hit, (playerpos.position - desiredposition).magnitude,  1 << wallLayer))
        {
            targetposition = (hit.point - playerpos.position) * .8f + playerpos.position;
        }
        else
        {
            targetposition = desiredposition;
        }


        transform.position = targetposition;
        transform.LookAt(playerpos);
        
        
    }


    void LateUpdate()
    {
        if (playerpos != null)
        {
            FollowMe();
        }

    }
}
