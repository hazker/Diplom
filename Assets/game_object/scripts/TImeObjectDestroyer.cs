﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TImeObjectDestroyer : MonoBehaviour
{

    public float lifeTime = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

}
