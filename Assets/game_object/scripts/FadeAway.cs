using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAway : MonoBehaviour
{
    public float timeToFade = 6f;

    float coeff;
    float time=0;
    Light l;

    void Start()
    {
        coeff = 1;
        l = GetComponent<Light>();
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.5f)
        {
        
            time = 0;
            l.intensity -= 0.15f*coeff;
            l.range -= 0.5f*coeff;

            coeff *= 1.1f;
        }
        
    }
}
