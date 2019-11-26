using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform[] ruta;
    public int actual;
    public float range;
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        this.actual = 0;
        this.range = 0.2f;
        this.speed = 15.0f;
        StartCoroutine(checaSiLlego());
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(ruta[actual].transform);
        transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);
    }
    
    IEnumerator checaSiLlego()
    {
        float d;
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            d = Vector3.Distance(this.transform.position, ruta[actual].transform.position);
            if (d <= range)
            {
                actual++;
                actual %= ruta.Length;
            }
        }
    }


}
