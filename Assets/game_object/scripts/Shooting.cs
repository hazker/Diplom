using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Shooting : MonoBehaviour
{
    [HideInInspector] public static Shooting instance;

    private PhotonView phV;
    private CharacterController iam;
    CharacterController characterController;
    public InGameUI ingameui;
    public Camera cam;
    public float power = 50f;
    public float distance = 100;
    public GameObject CastPoint;

    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        phV = this.GetComponent<PhotonView>();
        ingameui = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<InGameUI>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
           // if (phV.IsMine)
                //Shot();
        }
        Vector3 forward = transform.TransformDirection(Vector3.forward) * distance;
        //Debug.DrawLine(cam.transform.position, new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + distance),new Color(255,0,0));
        Debug.DrawRay(transform.position, forward, Color.green);
    }
    /*
    public void Shot()
    {
        //Ray ray = cam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        Ray ray = new Ray(CastPoint.transform.position, transform.TransformDirection(Vector3.forward) * distance);
        if (Physics.Raycast(ray, out hit, distance))
        {
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic && rb.constraints == RigidbodyConstraints.None)
            {

                rb.AddForceAtPosition(ray.direction * power, hit.point);

            }
            Debug.Log("Shoot: " + hit.collider.name);
            if (hit.collider.tag == "Player" )
            {
                ingameui.DoHitMarker();
                Debug.Log(hit.collider.name);
                hit.transform.root.GetComponent<PhotonView>().RPC("ApplyPlayerDamage", RpcTarget.All, 34f, PhotonNetwork.LocalPlayer,"some weapon");
                
                
            }
        }
        else
        {
            Debug.Log("Miss");
        }
        //shell = Instantiate(currentshellPref, currentshellspawn.position, currentshellspawn.rotation);
        //shell.GetComponent<Rigidbody>().AddForce(shell.transform.right * shellpower, ForceMode.Impulse);
        //Destroy(shell, 2f);

    }*/

}
