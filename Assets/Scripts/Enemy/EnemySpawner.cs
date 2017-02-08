using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {


    public List<GameObject> Enemies;
    bool EnemyActive = false;

    private float Spawn_Timer;              //Timer to handle spawning
    [SerializeField]
    private float SpawnTimer = 20f;

    System.Random rand = new System.Random();


    // Use this for initialization
    void Start ()
    {
        Spawn_Timer = rand.Next(0, (int)SpawnTimer);




    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!EnemyActive && Spawn_Timer >= SpawnTimer)
        {
            GameObject enemy = GameObject.Instantiate(Enemies[rand.Next(0, 4)]);
            enemy.transform.position = this.transform.position;
            enemy.GetComponent<Enemy>().owner = this.gameObject;
            EnemyActive = true;
        }

        if (!EnemyActive)
            Spawn_Timer += Time.deltaTime;
        else
            Spawn_Timer = 0;

    }

    public void EnemyDesrtroyed()
    {
        EnemyActive = false;
    }


}
