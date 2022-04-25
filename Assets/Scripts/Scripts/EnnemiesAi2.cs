using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemiesAi2 : MonoBehaviour
{

    public float speed;
    public float stoppingDistance;
    public float retreatDistance;

    public GameObject bullet;
    private Transform player;

    private float timeBtwShots;
    public float startTimeBtwShots;
    [SerializeField]
    private TrailRenderer bulletTrail;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        timeBtwShots = startTimeBtwShots;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = player.position - transform.position;
        Debug.DrawRay(transform.position, dir);
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 20f))
        {
            if(hit.transform.tag == "Player")
            {
                StartCoroutine(Shoot(dir, hit, bulletTrail));
            }
            
        }
    }

    private IEnumerator Shoot(Vector3 dir, RaycastHit hit, TrailRenderer bulletTrail)
    {

        //yield return new WaitForSeconds(0.2f);
        //var trail = Instantiate(bulletTrail, transform.position, Quaternion.identity);


        float time = 0;
        Vector3 startPosition = Trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        Destroy(trail.gameObject, trail.time);
    }
}