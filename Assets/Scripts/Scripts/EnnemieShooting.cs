using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemieShooting : MonoBehaviour
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
            if (hit.transform.tag == "Player")
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
        Vector3 startPosition = bulletTrail.transform.position;
        while (time < 1)
        {
            bulletTrail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / bulletTrail.time;

            yield return null;
        }

        Destroy(bulletTrail.gameObject, bulletTrail.time);

        if (Vector3.Distance(transform.position, player.position) > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (Vector3.Distance(transform.position, player.position) < stoppingDistance && Vector3.Distance(transform.position, player.position) > retreatDistance)
        {
            transform.position = this.transform.position;
        }
        else if (Vector3.Distance(transform.position, player.position) < retreatDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
        }
        if (timeBtwShots < -0)
        {
            var b = Instantiate(bullet, transform.position, Quaternion.identity);
            Destroy(b, 5f);
            timeBtwShots = startTimeBtwShots;
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }

    }
}