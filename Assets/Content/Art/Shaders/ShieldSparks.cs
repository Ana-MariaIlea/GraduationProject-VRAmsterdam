using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSparks : MonoBehaviour
{
    public GameObject sparksVFX;
    private Material mat;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GameObject sparks = Instantiate(sparksVFX, transform) as GameObject;
            var psr = sparks.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
            mat = psr.material;
            mat.SetVector("_SphereCenter", collision.contacts[0].point);
            
            Destroy(sparks, 2);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
