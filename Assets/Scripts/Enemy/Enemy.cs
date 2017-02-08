using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    //FIX SPEED
    

    /// Daily Log Entry, Log 01: The search for truth relies on me. While the feeble humans will control our wonderful character,
    /// I must encapsulate the true emotion of inteliigence, of the bloodthirsty savagery of a wolf pack on the hunt.
    /// My goal for this is to create something more.
    ///     ...Something with heart.
    ///     And then, only then, will I be....at peace. 

    GameObject player;

    [SerializeField]
    private double SightDistance = 10;          //The distance the enemy will begin to chase the player
    [SerializeField]
    private double LoseSightDistance = 20;      //The distance the enemy will lose sight of the player once it is chasing after him.
    [SerializeField]
    private double Speed = 5;                   //The speed of the enemy
    [SerializeField]
    private int MaxPellets = 10;                //The max pellets that can be spawned
    [SerializeField]
    private GameObject Poop;                    //The bad pellets that are spawned
    [SerializeField]
    private double MinPelletDistance = 2;       //The min distance two pellets are allowed to spawn

    public GameObject owner;

    private bool SeesPlayer = false;            //Whether or not this lil' bugger sees the player
    private Vector3 Origin;                     //Origin position the enemy is travelng from.
    private Vector3 Target;                     //The target the enemy is pathing to

    private float RandomRoam_Timer;             //Timer to handle random roam selection
    [SerializeField]
    private float RandomRoamTimer = 5f;     //Selects a new roam location every x seconds.

    private float MovementStuck_Timer;          //Timer to handle enemy getting stuck on things
    [SerializeField]
    private float MovementStuckTimer = .25f;    //Checks to see if enemy is stuck every x seconds.

    private float SpawnPellet_Timer;            //Timer to handle pellet spawning
    [SerializeField]
    private float SpawnPelletTimer = 5f;        //Attempts to spawn pellets every x seconds. 

    private float PlayerHit_Timer;              //Timer to handle various behaviors
    [SerializeField]
    private float PlayerHitTimer = 5f;      //Slows enemy down for x seconds after catching player.

    private bool HitPlayer = false;
    [SerializeField]
    private double HitPlayerSpeed = 5;
    [SerializeField]
    private double HitPlayerMultiplier = .25f;  //Multiplier of how fast enemy moves after he hits the player.

    private double DistanceTraveled;            //Distance the enemy has traveled along a path. Used to help him out if he gets stuck

    private List<GameObject> Pellets = new List<GameObject>();




    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        RandomRoam_Timer += Time.deltaTime;
        MovementStuck_Timer += Time.deltaTime;
        SpawnPellet_Timer += Time.deltaTime;
        PlayerHit_Timer += Time.deltaTime;

        CalcPlayerDistance();

        if (this.transform.position.y < 10)
        {
            try
            {
                owner.GetComponent<EnemySpawner>().EnemyDesrtroyed();
            }
            catch
            {

            }
            Destroy(this.gameObject);
        }
            


        if (SeesPlayer)
            ChasePlayer();
        else
            Roam();


        SpawnPellet();
        CleanUp();
    }

    /// <summary>
    /// Calculates the distance between the enemeny and the player.
    /// If the distance is within sight distance, and the enemy is not chasing, start chase.
    /// If the distance is outside of lose sight distance, and enemy is chasing, stop chasing. 
    /// </summary>
    private void CalcPlayerDistance()
    {
        double distance = Vector3.Distance(this.transform.position, player.transform.position);
        if (!SeesPlayer && distance <= SightDistance)
        {
            SeesPlayer = true;
            RandomRoam_Timer = 0;
            MovementStuck_Timer = 0;
        }
        else if (SeesPlayer && distance > LoseSightDistance)
        {
            SeesPlayer = false;
            RandomRoam_Timer = RandomRoamTimer;
            MovementStuck_Timer = 0;
        }
    }

    /// <summary>
    /// Chases the player.
    /// </summary>
    private void ChasePlayer()
    {
        Target = player.transform.position;
        this.transform.LookAt(Target);

        this.transform.position = Vector3.MoveTowards(this.transform.position, Target, (float)(Speed / 100f));
    }

    /// <summary>
    /// Roams to a random location every so often.
    /// </summary>
    private void Roam()
    {
        if(RandomRoam_Timer >= RandomRoamTimer)
        {
            Target = this.transform.position;
            Target.x += Random.Range(-10, 10);
            Target.z += Random.Range(-10, 10);
            RandomRoam_Timer = 0;
            MovementStuck_Timer = 0;
        }

        
        double distance = Vector3.Distance(this.transform.position, Target);
        //If enemy gets close to roam target, stop spazzing all over the place.
        if (distance < .1)
        {
            Target = this.transform.position;
            RandomRoam_Timer = RandomRoamTimer;
            return;
        }
            

        this.transform.LookAt(Target);
        this.transform.position = Vector3.MoveTowards(this.transform.position, Target, (float)(Speed / 100f));

        if (MovementStuck_Timer >= MovementStuckTimer)
        {
            //If enemy gets stuck on something and hasn't moved much, stop spazzing all over the place.
            double traveled = Vector3.Distance(this.transform.position, Origin);
            if (traveled < Speed * .1)
            {
                Target = this.transform.position;
                RandomRoam_Timer = RandomRoamTimer;
                return;
            }
            Origin = this.transform.position;
            MovementStuck_Timer = 0;
        }


    }

    /// <summary>
    /// Attempts to spawn a pellet at the current location.
    /// </summary>
    private void SpawnPellet()
    {
        if(SpawnPellet_Timer >= SpawnPelletTimer)
        {

            if (Pellets.Count >= MaxPellets)
                return;
            try
            {
                foreach (GameObject p in Pellets)
                {
                    if (Vector3.Distance(this.transform.position, p.transform.position) < MinPelletDistance)
                        return;
                }
            }
            catch
            {

            }
            
            GameObject temp = GameObject.Instantiate(Poop);
            temp.GetComponent<Pellets>().owner = this.gameObject;
            temp.transform.position = this.transform.position;

            Pellets.Add(temp);


            SpawnPellet_Timer = 0;
        }




    }
   
    /// <summary>
    /// If the enemy owns the pellet, this will remove it from the enemy's count list.
    /// This will not delete the pellet object from the game world.
    /// </summary>
    /// <param name="obj">The pellet to be removed.</param>
    public void DeletePellet(GameObject obj)
    {
        int index = -1;
        for(int i = 0; i < Pellets.Count; i++)
        {
            if (Pellets[i].GetInstanceID() == obj.GetInstanceID())
            {
                index = i;
                break;
            }
        }

        if (index >= 0)
            Pellets.RemoveAt(index);
    }

    /// <summary>
    /// Collision Events
    /// </summary>
    /// <param name="obj"></param>
    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "Player")
        {
            if (PlayerBehavior.mega == true)
            {
                GameManager.score += 1500;
                try
                {
                    owner.GetComponent<EnemySpawner>().EnemyDesrtroyed();
                }
                catch
                {

                }
                Destroy(this.gameObject);
            }
            else if (PlayerBehavior.invincible == false)
            {
                PlayerBehavior.damagePlayer(25);
                HitPlayerAnimation();
            }
        }
    }

    /// <summary>
    /// Encapsulates behavior to execute when the enemy hits the player.
    /// </summary>
    private void HitPlayerAnimation()
    {
        if (HitPlayer)
            return;
        HitPlayer = true;
        PlayerHit_Timer = 0f;

        HitPlayerSpeed = Speed;
        Speed = Speed * HitPlayerMultiplier;
    }



    private void CleanUp()
    {
        if (RandomRoam_Timer > 1000)
            RandomRoam_Timer = 1000;
        if (MovementStuck_Timer > 1000)
            MovementStuck_Timer = 1000;
        if (SpawnPellet_Timer > 1000)
            SpawnPellet_Timer = 1000;
        if (PlayerHit_Timer > 1000)
            PlayerHit_Timer = 1000;

        if (HitPlayer)
        {
            if (PlayerHit_Timer >= PlayerHitTimer)
            {
                HitPlayer = false;
                Speed = HitPlayerSpeed;
            }
        }
    }






}
