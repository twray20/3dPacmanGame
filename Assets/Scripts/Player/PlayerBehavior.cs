using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField]
    public static int health = 100;
    [SerializeField]
    public static int energy = 0;

    public static bool invincible = false;
    public static bool mega = false;

    private float timer1 = 0;
    private float timer2 = 0;
    private bool crouching = false;
    private float fallSpeed = 1;

	public Animation anim;
    private Vector3 camPos;

    [SerializeField]
    Transform playerModel;

    CharacterController body;
    Vector3 inp = Vector3.zero;

    [SerializeField]
    float movespeed;

    [SerializeField]
    float jumpheight;

    [SerializeField]
    float gravity;

    // Use this for initialization
    void Start()
    {
        
		anim = GetComponent<Animation> ();
        energy = 100;
        body = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        camPos = Camera.main.transform.position;
        aniHelp();
        Debug.Log(mega);
        move();
        glide();
        crouch();
        invincibility();
        megaChomp();
        Debug.Log(energy);
        Debug.Log("Health: " + health);
        if (mega == true)
            megaTimer();
    }
    
    void aniHelp()
    {
        if (mega == true)
        {
            anim.Play("attack");
        }
        else if (inp.z != 0 && body.isGrounded)
        {
            anim.Play("run");
        }
        else if (inp.z == 0 && body.isGrounded)
        {
            anim.Play("idle");
        }
        else if (inp.y != 0 && !body.isGrounded)
        {
            anim.Play("jump");
        }
    }

    void move()
    {
		inp.z = Input.GetAxis("Vertical");
		inp.z *= movespeed;
		inp = transform.TransformDirection(0, inp.y, inp.z);



        if (body.isGrounded)
        {
            inp.y = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                jump();
            }
        }

        transform.Rotate(0, Input.GetAxis("Horizontal") * 3.0f, 0);

        if (!body.isGrounded)
            inp.y -= gravity * Time.deltaTime * fallSpeed;
        body.Move(inp * Time.deltaTime);
    }

    void jump()
    {
        inp.y += jumpheight;
    }

    void crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (crouching == false)
            {
                movespeed *= 0.75f;
                playerModel.localScale = new Vector3(.5f, .5f, .5f);
                body.height = .75f;
                body.center = new Vector3(0, .5f, 0);
                Camera.main.transform.position = camPos;
            }
            crouching = true;
        }
        else
        {
            if (crouching == true)
            {                
                movespeed /= 0.75f;
                playerModel.localScale = new Vector3(1, 1, 1);
                body.height = 1.5f;
                body.center = new Vector3(0, .75f, 0);
                Camera.main.transform.position = camPos;
            }
            crouching = false;
        }

    }

    void glide()
    {
        if (!body.isGrounded && Input.GetKey(KeyCode.Space) && inp.y < 0)
        {
            fallSpeed = 0.1f;
        }

        else
        {
            fallSpeed = 1.0f;
        }
    }

    void megaChomp()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && energy == 100 && mega == false)
        {
            mega = true;
            movespeed /= 2;
            energy = 0;
            timer2 = Time.time;
        }
    }

    void megaTimer()
    {
        if (Time.time - timer2 >= 5)
        {
            mega = false;
            movespeed *= 2;
        }
    }

    void invincibility()
    {
        if (invincible == true && Time.time - timer1 >= 10)
            invincible = false;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "GoodPellet")
        {
            healEnergy(10);
        }
        if (c.gameObject.tag == "BadPellet")
        {
            if(mega == false && invincible == false)
                damagePlayer(5);
            if (mega == true)
                GameManager.score += 100;

        }
        if (c.gameObject.tag == "Fruit")
        {
            healHealth(15);
            Destroy(c.gameObject);
        }
        if (c.gameObject.tag == "Super" && mega == true)
        {
            timer1 = Time.time;
            invincible = true;
            Destroy(c.gameObject);
        }
    }

    //Test
    public static void damagePlayer(int dmg)
    {
        health -= dmg;
        if (health < 0)
            health = 0;
    }

    public static void healEnergy(int nrg)
    {
        energy += nrg;
        if (energy > 100)
            energy = 100;
    }


    public static void healHealth(int hp)
    {
        health += hp;
        if (health > 100)
            health = 100;
    }
}
