using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float currentdamage = 100f;
    public float speed = 10f;
    public float initialForce = 10f;
    public string WeaponName;
    public float lifetime = 30.0f;
    public float gravity = 9.8f;

    public float explosionForce = 25.0f;
    public float explosionRadius = 50.0f;
    float damage = 10f;
    public GameObject explosion;
    private float lifeTimer = 0.0f;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(0, -10, initialForce);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
        {
            Explode(transform.position);
        }


        rb.velocity = -transform.up * speed;
        rb.transform.Translate(0, -gravity * Time.deltaTime, 0);
    }

    void OnCollisionEnter(Collision col)
    {
        Hit(col);
        Debug.Log("Hit and object named " + col.gameObject.name);
    }

    void Hit(Collision col)
    {
        Explode(col.contacts[0].point);

        if (col.collider.tag == "Player")
        {
            col.transform.root.GetComponent<PhotonView>().RPC("ApplyPlayerDamage", RpcTarget.All, currentdamage, PhotonNetwork.LocalPlayer, WeaponName);
        }
    }

    void Explode(Vector3 position)
    {

        //Instantiate(explosion, position, Quaternion.identity);
        PhotonNetwork.Instantiate("Explosion1", position, Quaternion.identity, 0);
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in cols)
        {
            if (col.transform.tag=="Player")
            {
                float damageAmount = currentdamage * (1 / Vector3.Distance(transform.position, col.transform.position));


                col.transform.root.GetComponent<PhotonView>().RPC("ApplyPlayerDamage", RpcTarget.All, damageAmount, PhotonNetwork.LocalPlayer, WeaponName);

            }

        }



        List<Rigidbody> rigidbodies = new List<Rigidbody>();

        foreach (Collider col in cols)
        {
            if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
            {
                rigidbodies.Add(col.attachedRigidbody);
            }
        }

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1, ForceMode.Impulse);
        }



        Destroy(gameObject);
    }
}
