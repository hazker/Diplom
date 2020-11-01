using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    private PhotonView phV;
    private Animator myanim;
    CharacterController characterController;
    public Camera cam;
    public GameObject pos1,pos2,rotpoint;
    bool inaim, crouch,isLeaning;


    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    Vector3 moveDirection = Vector3.zero;
    private RaycastHit hit;
    Ray ray;

    public float LeanSpeed = 100f;
    public float MaxLeanAngle = 60f;
    private float CurrentLeanAngle = 0f;

    [HideInInspector]
    public bool canMove = true;
    public bool roof = false;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        myanim = this.GetComponent<Animator>();
        phV = this.GetComponent<PhotonView>();
        crouch = false;
    }

    void Update()
    {
        if (!phV.IsMine) return;

        if(GameManager.instance.IsAlive && GameManager.instance.menuOn==false)
            UserInput();
    }

    void UserInput()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetButton("Run");
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        myanim.SetFloat("vertical", (forward * curSpeedX).x);
        myanim.SetFloat("Horizontal", (right * curSpeedY).z);
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        Debug.DrawRay(cam.transform.position, transform.up * 100f, Color.red);
        //Debug.Log(canMove + " " + characterController.isGrounded);
        if (Input.GetAxis("Jump") > 0 && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        if (Input.GetAxis("Fire2") > 0 && Input.anyKeyDown)
        {
            myanim.SetBool("Aim", inaim = !inaim);
        }

        if (Input.GetAxis("Crouch") > 0 && Input.anyKeyDown)
        {

            if (crouch)
            {
                ray = new Ray(cam.transform.position, transform.up);
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.distance >= 2.99f)
                    {
                        characterController.Move(new Vector3(0f, 0.25f, 0f));////крауч фикс
                        //characterController.height = 7.5f;
                        characterController.height = Mathf.Lerp(5.25f, 7.5f, 0.85f);
                        cam.transform.position = new Vector3(pos1.transform.position.x, pos1.transform.position.y, pos1.transform.position.z);
                        characterController.center = new Vector3(0f, 3.7f, 0f);
                        myanim.SetBool("Сrouch", crouch = !crouch);
                    }
                }
            }
            else
            {
                characterController.center = new Vector3(0f, 2.9f, 0f);
                cam.transform.position = new Vector3(pos2.transform.position.x, pos2.transform.position.y, pos2.transform.position.z);
                //characterController.height = 5.25f;
                characterController.height = Mathf.Lerp(7.5f, 5.25f, 0.85f);
                myanim.SetBool("Сrouch", crouch = !crouch);


            }

            //characterController.radius = characterController.radius * 2;

        }
        if (Input.GetAxis("Reload") > 0 && Input.anyKeyDown)
        {
            myanim.SetBool("reload", true);
        }


        if (Input.GetButton("Q") && !isRunning)
        { //If we press Q and arent sprinting
            isLeaning = true;
            CurrentLeanAngle = Mathf.MoveTowardsAngle(CurrentLeanAngle, MaxLeanAngle, LeanSpeed * Time.deltaTime); //Smoothing translate our pivot point to a set angle
        }
        else if (Input.GetButton("E") && !isRunning)
        {
            isLeaning = true;
            CurrentLeanAngle = Mathf.MoveTowardsAngle(CurrentLeanAngle, -MaxLeanAngle, LeanSpeed * Time.deltaTime); //Smoothing translate our pivot point to a set angle
        }
        else
        {
            isLeaning = false;
            CurrentLeanAngle = Mathf.MoveTowardsAngle(CurrentLeanAngle, 0f, LeanSpeed * Time.deltaTime); //Reset the pivot point to a straight upward position
        }
        rotpoint.transform.localRotation = Quaternion.AngleAxis(CurrentLeanAngle, Vector3.forward);
        //cam.transform.localRotation = Quaternion.AngleAxis(CurrentLeanAngle, Vector3.forward);

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }
    public void endReload(){myanim.SetBool("reload",false);}
}
