using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public GameObject impactParticle;
    public GameObject projectileParticle;
    public GameObject muzzleParticle;
    //public GameObject[] trailParticles;
    [HideInInspector]
    public Vector3 impactNormal; //Used to rotate impactparticle.

    private GameObject impactObj;
    private GameObject projectileObj;
    private GameObject muzzleObj;
    //private GameObject[] trailObjs;

    public bool hasFired = false;
    public bool hasCollided = false;

    public void StartFire()
    {
        hasCollided = false;
        projectileObj = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
        projectileObj.transform.parent = transform;
        if (muzzleParticle)
        {
            muzzleObj = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
            muzzleObj.transform.rotation = transform.rotation * Quaternion.Euler(180, 0, 0);
            Destroy(muzzleObj, 1.5f); // Lifetime of muzzle effect.
        }
        hasFired = true;
    }

    void OnCollisionEnter(Collision hit)
    {
        if (!hasCollided)
        {
            hasCollided = true;
            impactObj = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;

            /*
            foreach (GameObject trail in trailParticles)
            {
                GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                curTrail.transform.parent = null;
                Destroy(curTrail, 3f);
            }*/
            Destroy(projectileObj, 3f);
            Destroy(impactObj, 5f);
            gameObject.SetActive(false);
            hasFired = false;
            ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
            //Component at [0] is that of the parent i.e. this object (if there is any)
            for (int i = 1; i < trails.Length; i++)
            {
                ParticleSystem trail = trails[i];

                if (trail.gameObject.name.Contains("Trail"))
                {
                    trail.transform.SetParent(null);
                    Destroy(trail.gameObject, 2f);
                }
            }
        }
    }
}
