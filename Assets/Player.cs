using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Movement
    private Rigidbody2D rb;
    private Vector3 input_vec = Vector3.zero;
    public float speed = 300f;

    //Bullet
    public GameObject bullet;
    private float rate_of_fire = 0.05f; //In seconds.
    private float shoot_timer = 0f;
    [HideInInspector] public int bullet_count = 0;

    //States
    enum States
    {
        Move,
        Downed
    }
    States state = States.Move;

    //Downed State
    private int pressed_needed_for_revive = 5;
    private int current_presses = 0;

    //invincibility after revive
    private bool is_invincible = false;
    private float invincible_timer = 0;

    //animator reference
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //Get reference to rigidbody.
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //State machine
        switch (state)
        {
            case States.Move:
                StateMove();
                break;
            case States.Downed:
                StateDowned();
                break;
        }
    }

    private void FixedUpdate()
    {
       //Set velocity.
        rb.velocity = input_vec * speed * Time.fixedDeltaTime;
    }

    void SpawnBullet()
    {
        GameObject new_bullet = Instantiate(bullet);
        new_bullet.transform.position = transform.position;
        new_bullet.GetComponent<Bullet>().player = this;
        new_bullet.GetComponent<Bullet>().SetUp(Vector3.up);
        bullet_count += 1;
    }

    private void StateMove()
    {
        //Set input vector.
        input_vec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);

        //Shooting
        if (Input.GetKeyDown(KeyCode.Space) && shoot_timer <= 0 && bullet_count < 5)
        {
            shoot_timer = rate_of_fire;

            SpawnBullet();
        }

        //Countdown shoot timer.
        if (shoot_timer > 0)
        {
            shoot_timer -= Time.deltaTime;
        }

        //reset things for invincibility
        if(is_invincible)
        {
            invincible_timer -= Time.deltaTime;
            if(invincible_timer <= 0)
            {
                is_invincible = false;
                //GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Shell" && !is_invincible)
        {
            //change state
            state = States.Downed;

            //sstop player
            rb.velocity = Vector3.zero;
            input_vec = Vector3.zero;

            //visual cue/ change color of sprite
            //GetComponent<SpriteRenderer>().color = Color.gray;

            //reset shoot timer
            shoot_timer = 0;

            //animator
            animator.SetBool("was_hit", true);
        }
    }

    private void StateDowned()
    {
        //mash space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            current_presses++;
        }

        //reviving player
        if(current_presses >= pressed_needed_for_revive)
        {
            //reset presses
            current_presses = 0;

            //up number of presses need to revive
            pressed_needed_for_revive += 2;
            pressed_needed_for_revive = Mathf.Min(pressed_needed_for_revive, 25);

            //invincibility
            is_invincible = true;
            invincible_timer = 1.5f;

            //show the player is revived by changing color back
            //GetComponent<SpriteRenderer>().color = Color.cyan;

            animator.SetBool("was_hit", false);

            //change states
            state = States.Move;
        }
    }
}
