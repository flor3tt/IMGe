using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour {

    ParticleSystem particle;

    void Start()
    {
        particle = gameObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!particle.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
