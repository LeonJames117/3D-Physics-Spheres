using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Physics_Sphere : MonoBehaviour
{
    // Variables
    [SerializeField] public Vector3 Velocity;
    public Vector3 Acceleration;
    public Vector3 Pos;
    public float Radius = 10;
    public bool Recently_Colided = false;
    int RC_Count = 0;

    public Vector3 GetLocation()
    {
        return transform.position;
    }

    public void Force_Update()
    {
        Update();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Pos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        Velocity += Acceleration * delta;

        Pos = Velocity * delta;

        this.transform.position += Pos ;
        if(Recently_Colided)
        {
            RC_Count++;
            if(RC_Count > 500)
            {
                Recently_Colided=false;
                RC_Count=0;
            }
        }
    }
}
