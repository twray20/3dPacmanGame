using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellets : MonoBehaviour {

    public GameObject owner = null;             //Owner of the pellet. Used to keep track of which enemy spawned the pellet.


	void Start () {
		
	}
	
	void Update () {
        this.gameObject.transform.Rotate(15 * Time.deltaTime, 30 * Time.deltaTime, 45 * Time.deltaTime);
        
	}

    

    void OnTriggerEnter(Collider obj)
    {
        if(gameObject.tag == "GoodPellet")
        {
            if(obj.gameObject.tag == "Player")
            {
                GameManager.score += 100;
                //Debug.Log(gameObject.name + "Collided with " + obj.gameObject.name);
                Destroy(gameObject);
            }
        }
        if (gameObject.tag == "BadPellet")
        {
            if (obj.gameObject.tag == "Player")
            {
                //Debug.Log(gameObject.name + "Collided with " + obj.gameObject.name);

                if (owner != null)
                    owner.GetComponent<Enemy>().DeletePellet(this.gameObject);
                if(PlayerBehavior.invincible == false)
                    Destroy(gameObject);
            }
        }
        if (gameObject.tag == "Fruit")
        {
            if (obj.gameObject.tag == "Player")
            {
                GameManager.score += 1000;
                //Debug.Log(gameObject.name + "Collided with " + obj.gameObject.name);
                Destroy(gameObject);
            }
        }
        if (gameObject.tag == "Super")
        {
            if (obj.gameObject.tag == "Player" && PlayerBehavior.mega == true)
            {
                GameManager.score += 500;
                //Debug.Log(gameObject.name + "Collided with " + obj.gameObject.name);
                Destroy(gameObject);
            }
        }
    }
}
