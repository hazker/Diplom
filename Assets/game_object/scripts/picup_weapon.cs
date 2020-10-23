using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class picup_weapon : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public static picup_weapon instance;

    public int my_number;
    public bool destroyable;
    public void OnTriggerEnter(Collider other){
        //Debug.Log("pickup weapon");
        if(other.gameObject.tag == "Player"){
            //mouse_look.instance.change_wheapon(my_number);
            //other.GetComponent<mouse_look>().change_wheapon(my_number);
            other.transform.root.GetComponent<MouseLook>().change_wheapon(my_number);
            if(destroyable)
                Destroy(gameObject);
        }
    }
}
