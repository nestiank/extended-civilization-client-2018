using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laserbeam : MonoBehaviour
{
    public GameObject StartParticle;
    public GameObject EndParticle;
    public GameObject lineParticle;

    public Vector3 Target;
    public float shootTime;

    float distance;
    Vector3 Direction;
    Vector3 HitPoint;
    LineRenderer Line;
    GameObject StartObj;
    GameObject EndObj;
    GameObject LineObj;

    public void Fire(Vector3 Target, float shootTime)
    {
        StartObj = Instantiate(StartParticle, transform);
        StartObj.transform.localPosition = Vector3.zero;
        EndObj = Instantiate(EndParticle, transform);
        EndObj.SetActive(false);
        LineObj = Instantiate(lineParticle, transform);
        Line = LineObj.GetComponent<LineRenderer>();
        this.Target = Target;
        this.shootTime = shootTime;
        EndObj.transform.position = Target;
        distance = (Target - transform.position).magnitude;
        Direction = (Target - transform.position).normalized;
        StartCoroutine(_fire());
    }

    IEnumerator _fire()
    {
        float timer = 0;
        Vector3[] pos = new Vector3[2];
        pos[0] = transform.position;
        HitPoint = transform.position;
        while (timer < 5f)
        {
            if (Vector3.Distance(Target, HitPoint) > 0.01)
                HitPoint = Vector3.MoveTowards(HitPoint, Target, distance / shootTime * Time.deltaTime);
            else
                break;
            pos[1] = HitPoint;
            Line.SetPositions(pos);
            timer += Time.deltaTime;
            yield return null;
        }
        EndObj.SetActive(true);
        timer = 0;
        while(timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
