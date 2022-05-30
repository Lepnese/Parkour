using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowRoute : MonoBehaviour
{
    [SerializeField]
    private Transform[] routes;
    [SerializeField]
    private float speedModifier = 0.1f;

    private int routeToGo;

    private float tParam;

    private Vector3 objectPosition;

    private bool coroutineAllowed;
    public List<Vector3> pt;

    void Start()
    {
        routeToGo = 0;
        tParam = 0f;
        coroutineAllowed = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
    }
        
    private IEnumerator GoByTheRoute(int routeNum)
    {
        coroutineAllowed = false;

        Vector3 p0 = routes[routeNum].GetChild(0).position;
        Vector3 p1 = routes[routeNum].GetChild(1).position;
        Vector3 p2 = routes[routeNum].GetChild(2).position;
        Vector3 p3 = routes[routeNum].GetChild(3).position;
        Vector3[] pTab = new Vector3[] { p0, p1, p2, p3 };

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;

            transform.LookAt(objectPosition);
            transform.position = objectPosition;
           

            yield return new WaitForFixedUpdate();
        }



        tParam = 0;
        routeToGo += 1;

        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;

    }
}
