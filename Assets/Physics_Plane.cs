using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics_Plane : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Vector3 Get_Normal()
    {
        return transform.up;
    }

    public Vector3 Get_Point()
    {
        return transform.position;
    }
        

    // Update is called once per frame
    void Update()
    {
      
    }
}
