using UnityEngine;
using TNet;
using System.Collections;
using InControl;

public class CPlayer : Entity
{
    public static CPlayer instance;

    public int money = 0;

    public int bestMark = 0;
    public int meters = 0;

    public int respawnHeight = -50;

    private Game game;
    private float startSpeed = 0;

    protected override void Start()
    {
        base.Start();

        game = Game.Get();

        if (TNManager.isThisMyObject) {
            instance = this;
        }

        rigidbody = GetComponent<Rigidbody>(); 
        camera = game.playerCamera.GetComponent<CameraMovement>();

        //Set up controlled player reference
        if (tno.isMine)
        {
            game.controlledPlayer = this;
        }
        else {
            GetComponent<AudioListener>().enabled = false;
        }

        startSpeed = speedDamp;
	}
	
	void Update () {
        if (!tno.isMine || Game.state != Game.GameState.PLAYING)
            return;

        //Die
        if (transform.position.y < respawnHeight)
        {
            Game.PauseGame();
        }

        MovementUpdate();

        meters = Mathf.FloorToInt(Vector3.Distance(transform.position, game.activeSpawn.transform.position));
        if (meters > bestMark) bestMark = meters;

        //UI
        //canvas.SetMoney(money);
        //canvas.SetHP((float)(live) / maxLive);


        game.hudMeters.text = meters + "";
        game.hudBestMark.text = bestMark + "";

	}

    public void CollectMoney(Coin coin)
    {
        CollectMoneyAmount(coin.amount);
    }


    /****************
     * Remote Calls *
     ****************/

    [RFC(0)]
    public void CollectMoneyAmount(int amount) {
        if(tno && tno.isMine)
            tno.Send(0, TNet.Target.OthersSaved, amount);
        
        money += amount;
    }

    [RFC(1)]
    public void Respawn(bool sound = true)
    {
        if(tno && tno.isMine)
            tno.Send(1, TNet.Target.OthersSaved);

        transform.position = game.activeSpawn.transform.position;
        transform.rotation = game.activeSpawn.transform.rotation;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        live = maxLive;
        money = 0;
        speedDamp = startSpeed;

        if (sound) {
            game.activeSpawn.GetComponent<AudioSource>().Play();
        }
    }



    /*****************
     * Movement Code *
     *****************/

    public float turnSmoothing = 15f;	// A smoothing value for turning the player.
    public float speedDamp = 0.1f;	// The damping for the speed parameter
    public float horizontalSpeedDamp = 0.1f;	// The damping for the speed parameter
    public float jumpSpeed = 50f;

    [System.NonSerialized]
    public bool canJumpAgain = false;
    [System.NonSerialized]
    public bool isMoving = false;

    private Rigidbody rigidbody;
    private CameraMovement camera;
    
    //Audio
    [SerializeField]
    private AudioSource jumpSound;

    private void MovementUpdate()
    {
        // Cache the inputs.
        float h = 0f;
        float v = 0f;
        bool jump = false;
        //Detect Correct Input
        if(Game.IsMobile() || Application.isEditor){
            InputDevice activeDevice = InputManager.ActiveDevice;
            h = activeDevice.LeftStickX;
            v = activeDevice.LeftStickY;
            jump = activeDevice.Action1.WasPressed;
        }else{
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            jump = Input.GetKeyDown("space");
        }

        //Continous movement
        v = 1f;

        if (CanMove())
        {
            isMoving = true;
            Rotating(h, v);

            rigidbody.velocity = new Vector3(h * horizontalSpeedDamp, rigidbody.velocity.y, v * speedDamp);

        }
        else
        {
            isMoving = false;
        }

        //Jump
        if (jump && canJumpAgain)
        {
            jumpSound.Play();
            isMoving = true;
            rigidbody.AddForce(0, jumpSpeed, 0);
        }

        //Enable or disable motionBlur
        //camera.motionBlur.enabled = rigidbody.velocity.sqrMagnitude > 25*25;
    }

    void Rotating(float horizontal, float vertical)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

        rigidbody.MoveRotation(newRotation);
    }


    //Fix Wall Stuck
    private int collidersAmount = 0;

    public bool IsColliding(){
        return collidersAmount > 0;
    }

    public bool CanMove() {
        return !(IsColliding() && !canJumpAgain);
    }

    void OnCollisionEnter(Collision col) {
        collidersAmount += 1;
    }

    void OnCollisionExit(Collision col)
    {
        if(collidersAmount > 0)
            collidersAmount -= 1;
    }



    public override bool IsPlayer() { return true; }

    public virtual void IncrementSpeed(float inc){
        speedDamp += inc;
    }
}
