using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponType
{
    Projectile,
    Raycast,
}

public enum Auto
{
    Full,
    Semi
}



public class Weapon : MonoBehaviour
{
    public WeaponType type = WeaponType.Raycast;
    public Auto type2 = Auto.Full;

    [Header("WeaponPreferences")]
    public string WeaponName;
    public int ammoCapacity;
    private int currentAmmo;
    public GameObject CastPoint;
    public float distance = 9999f;
    public float power = 50f;
    public float damage = 34f;
    public bool recoil = true;
    public float timeOnDestroy = 5f;
    public float shootDelay = 0.1f;

    [Header("Recoil")]
    public float upRecoil = 10f;
    public float sideRecoil = 5f;

    [Header("Projectile")]
    public GameObject projectile;
    public Transform Projectilepoint;

    [Header("Shell")]
    private GameObject shell;
    public Transform currentshellspawn;
    public GameObject currentshellPref;
    public float shellPower;

    public InGameUI ingameui;
    public GameObject hit_particles;

    private RaycastHit hit;
    private bool reload, inaim, shoot;
    private Animator myanim;
    private GameObject hit_p;
    private Transform startPosition;
    private Transform castPointStart;

    float time = 0f;
    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = ammoCapacity;
        ingameui = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<InGameUI>();
        myanim = gameObject.GetComponent<Animator>();
        castPointStart = CastPoint.transform;
        ingameui.SetAmmo(currentAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        ingameui.SetAmmo(currentAmmo);
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Input.GetButton("Run"))
        {
            if (vertical == 1)
            {
                vertical = 2;
            }
        }
        myanim.SetFloat("walk", vertical / 2);

        if (currentAmmo <= 0 && reload != true && shoot != true)/////////////фикс
        {

            Reload();

        }
        if (GameManager.instance.IsAlive && GameManager.instance.menuOn == false)
            UserInput();

    }

    void UserInput()
    {
        time += Time.deltaTime;
        if (type == WeaponType.Raycast)
        {
            /*if (Input.GetButton("Fire1") && Input.anyKeyDown && currentAmmo > 0 && reload == false && time>shootDelay)
            {
                time = 0;
                currentAmmo--;
                ingameui.SetAmmo(currentAmmo);
                Fire();
                
            }*/
            if (Input.GetButton("Fire1") && currentAmmo > 0 && reload == false && time > shootDelay)
            {
                time = 0;
                currentAmmo--;
                ingameui.SetAmmo(currentAmmo);
                Fire();

            }
        }
        if (Input.GetButton("Fire1") && currentAmmo > 0 && reload == false && type == WeaponType.Projectile)
        {
            Launch();
        }

        if (Input.GetAxis("Fire2") > 0 && Input.anyKeyDown)
        {
            myanim.SetBool("aim", inaim = !inaim);
        }

        if (Input.GetButton("Reload") && Input.anyKeyDown && currentAmmo != ammoCapacity)
        {
            Reload();
        }
    }

    void Launch()
    {
        currentAmmo--;
        MouseLook.GetInstanse().Recoil(Random.Range(0, upRecoil), Random.Range(-sideRecoil, sideRecoil));
        myanim.SetBool("shoot", true);
        //GameObject proj = Instantiate(projectile, Projectilepoint.position, Projectilepoint.rotation) as GameObject;
        PhotonNetwork.Instantiate("Projectile", Projectilepoint.position, Projectilepoint.rotation, 0);
    }

    void Fire()
    {

        shoot = true;

        if (WeaponName.Equals("Triple Threat"))
        {
            myanim.SetBool("shoot", true);
            MouseLook.GetInstanse().Recoil(Random.Range(10, upRecoil), Random.Range(-sideRecoil, sideRecoil));
            float dist;
            float currentdamage = 0;
            for (int bullet_counter = 10; bullet_counter > 0; bullet_counter--)
            {
                CastPoint.transform.localRotation = Quaternion.identity;
                CastPoint.transform.localRotation = Quaternion.Euler(castPointStart.localRotation.x + Random.Range(-6, 6), castPointStart.localRotation.y + Random.Range(-6, 6), castPointStart.localRotation.z + Random.Range(-6, 6));
                Vector3 fwd = CastPoint.transform.TransformDirection(Vector3.forward);
                if (Physics.Raycast(CastPoint.transform.position, fwd, out hit, distance))
                {
                    //Debug.Log(hit.point);
                    //hit_p = Instantiate(hit_particles, hit.point, Quaternion.FromToRotation(Vector3.forward, castPointStart.forward));
                    hit_p = PhotonNetwork.Instantiate("Bullethole", hit.point, Quaternion.identity, 0);
                    StartCoroutine(DestroyHits(timeOnDestroy, hit_p.GetPhotonView()));

                    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                    if (rb != null && !rb.isKinematic && rb.constraints == RigidbodyConstraints.None)
                    {

                        rb.AddForceAtPosition(CastPoint.transform.forward * power, hit.point);

                    }


                    if (hit.collider.tag == "Player")
                    {
                        ingameui.DoHitMarker();
                        dist = Vector3.Distance(transform.position, hit.collider.transform.position);

                        currentdamage += Mathf.Round(damage / dist / 5);



                        hit.transform.root.GetComponent<PhotonView>().RPC("ApplyPlayerDamage", RpcTarget.All, currentdamage, PhotonNetwork.LocalPlayer, WeaponName);
                    }
                }
            }
            //Debug.Log("uron " + currentdamage);

        }
        else
        {
            myanim.SetBool("shoot", true);
            MouseLook.GetInstanse().Recoil(Random.Range(0, upRecoil), Random.Range(-sideRecoil, sideRecoil));
            // MouseLook.GetInstanse().Recoil(upRecoil, sideRecoil);
            //Ray ray = new Ray(CastPoint.transform.position, transform.TransformDirection(Vector3.forward) * distance);

            CastPoint.transform.localRotation = Quaternion.identity;
            if (WeaponName != "PM")
                CastPoint.transform.localRotation = Quaternion.Euler(castPointStart.localRotation.x + Random.Range(-3, 3), castPointStart.localRotation.y + Random.Range(-4, 4), castPointStart.localRotation.z + Random.Range(-6, 6));
            else
                CastPoint.transform.localRotation = Quaternion.Euler(castPointStart.localRotation.x + Random.Range(-3, 3), castPointStart.localRotation.y + Random.Range(-2, 2), castPointStart.localRotation.z + Random.Range(-6, 6));
            Vector3 fwd = CastPoint.transform.TransformDirection(Vector3.forward);
            if (Physics.Raycast(CastPoint.transform.position, fwd, out hit, distance))
            {
                //hit_p = Instantiate(hit_particles, hit.point, Quaternion.FromToRotation(Vector3.forward, castPointStart.forward));
                hit_p = PhotonNetwork.Instantiate("Bullethole", hit.point, Quaternion.identity, 0);
                StartCoroutine(DestroyHits(timeOnDestroy, hit_p.GetPhotonView()));
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                if (rb != null && !rb.isKinematic && rb.constraints == RigidbodyConstraints.None)
                {

                    rb.AddForceAtPosition(CastPoint.transform.forward * power, hit.point);

                }
                Debug.Log("Shoot: " + hit.collider.name);
                if (hit.collider.tag == "Player")
                {
                    ingameui.DoHitMarker();
                    Debug.Log(hit.collider.name);
                    hit.transform.root.GetComponent<PhotonView>().RPC("ApplyPlayerDamage", RpcTarget.All, damage, PhotonNetwork.LocalPlayer, WeaponName);


                }
            }
            else
            {
                Debug.Log("Miss");
            }

            shell = Instantiate(currentshellPref, currentshellspawn.position, currentshellspawn.rotation);
            shell.GetComponent<Rigidbody>().AddForce(shell.transform.right * shellPower, ForceMode.Impulse);
            Destroy(shell, 2f);
        }
        // if (recoil)
        //Recoil();
    }

    void PtrdShell()
    {
        shell = Instantiate(currentshellPref, currentshellspawn.position, currentshellspawn.rotation);
        shell.GetComponent<Rigidbody>().AddForce(-shell.transform.up * shellPower, ForceMode.Impulse);
        Destroy(shell, 2f);
    }

    IEnumerator DestroyHits(float time, PhotonView target)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(target);
    }
    void Reload()
    {
        reload = true;
        myanim.SetBool("reload", reload);

    }

    void EndOfShoot()
    {
        shoot = false;
        myanim.SetBool("shoot", false);
    }

    void EndOfRealod()
    {
        currentAmmo = ammoCapacity;
        reload = false;
        myanim.SetBool("reload", false);
    }

}
