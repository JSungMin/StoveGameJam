using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem particle;
	// Use this for initialization
	void Start ()
    {
        particle = GetComponent<ParticleSystem>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!particle.IsAlive())
            Destroy(gameObject);
	}
}
