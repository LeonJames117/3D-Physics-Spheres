using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Physics_Controller : MonoBehaviour
{
    //Variables
    [SerializeField] Physics_Sphere[] Spheres = new Physics_Sphere[2];
    [SerializeField] Physics_Plane[] Planes = new Physics_Plane[10];

	//Maths Functions
	static float Length_Of_Vector(Vector3 Vect)
		{
			
			return Mathf.Sqrt(Mathf.Pow(Vect.x, 2) + Mathf.Pow(Vect.y, 2) + Mathf.Pow(Vect.z, 2)); 
		}

	static float Vector_Dot(Vector3 Vect_1, Vector3 Vect_2)
    {
		return (Vect_1.x * Vect_2.x) + (Vect_1.y * Vect_2.y) + (Vect_1.z * Vect_2.z);
    }

	static bool Vector_Less_Than(Vector3 Vect_1, Vector3 Vect_2)
    {
		return (Vect_1.x < Vect_2.x && Vect_1.y < Vect_2.y && Vect_1.z < Vect_2.z);

	}

    //Physics Functions
    void Stationary_Sphere_Colision_Update()
    {
        for (int i = 0; i < Spheres.Length; i++)
		{
			for (int t = i + 1; t < Spheres.Length; t++)
			{
				if (i <= Spheres.Length)
                {
					Physics_Sphere Sphere_1 = Spheres[i];
					Physics_Sphere Sphere_2 = Spheres[t];
				
					Vector3 Sphere_1_Loc = Sphere_1.GetLocation();
					Vector3 Sphere_2_Loc = Sphere_2.GetLocation();

					// Calculate relative position
					Vector3 Sphere_To_Sphere = Sphere_1_Loc - Sphere_2_Loc;

					Vector3 Sphere_1_Vel = Sphere_1.Velocity;

					// Find angle between Sphere_1_Vel and Sphere_to_Sphere
					float QAngle = Mathf.Acos(Vector_Dot(Sphere_1_Vel, Sphere_To_Sphere) / (Length_Of_Vector(Sphere_1_Vel) * Length_Of_Vector(Sphere_To_Sphere)));
					QAngle *= (Mathf.PI / 180) ;

					//Find the closest distance to the other sphere
					float Closest_To_Other_Sphere = Mathf.Sin(QAngle) * Length_Of_Vector(Sphere_To_Sphere);
					float Sum_Of_Radi = Sphere_1.Radius + Sphere_2.Radius;

					if (Closest_To_Other_Sphere < Sum_Of_Radi)
					{
						//Sphere can collide with other Sphere

						float V_Distance_Remaining = Mathf.Sqrt(Mathf.Pow(Sum_Of_Radi, 2) - Mathf.Pow(Closest_To_Other_Sphere, 2));
						float Length_Remaining = Mathf.Cos(QAngle) * Length_Of_Vector(Sphere_To_Sphere) - V_Distance_Remaining;

						Vector3 Distance_To_Colision = Length_Remaining * Sphere_1_Vel / Length_Of_Vector(Sphere_1_Vel);

						if (Mathf.Abs(Length_Remaining) < Mathf.Abs(Length_Of_Vector(Sphere_1_Vel * Time.fixedDeltaTime)))
						{
							
							if(!Sphere_1.Recently_Colided)
                            {
								print("Spheres colliding");
								Sphere_1.Recently_Colided = true;
								Vector3 POC = Sphere_1_Loc + Distance_To_Colision;

								//Response
								Vector3 Force_Direction = POC - Sphere_2.GetLocation();
								float Force_Proportion_Angle = Mathf.Acos(Vector_Dot(Force_Direction, Sphere_1_Vel) / (Length_Of_Vector(Force_Direction) * Length_Of_Vector(Sphere_1_Vel)));
								Force_Proportion_Angle *= (Mathf.PI / 180);

								Vector3 S1_New_Vel = Mathf.Cos(Force_Proportion_Angle) * Force_Direction;
								Vector3 S2_New_Vel = Sphere_1_Vel - S1_New_Vel;

								Sphere_1.Velocity = S1_New_Vel;
								Sphere_2.Velocity = S2_New_Vel;
								return;
							}
							
						}
						else
						{
							// Spheres not colliding
							return;
						}
					}
                    else
                    {
						// Spheres cannot collide
						return;
                    }
				}
			}
		}
	}

	void Moving_Sphere_Colision_Update()
    {
		for (int i = 0; i < Spheres.Length; i++)
		{
			// Moving sphere Detected
			if (Spheres[i].Velocity.x != 0 || Spheres[i].Velocity.y != 0 || Spheres[i].Velocity.z != 0)
            {
				for (int t = i + 1; t < Spheres.Length; t++)
				{
					if(Spheres[t].Velocity.x != 0 || Spheres[t].Velocity.y != 0 || Spheres[t].Velocity.z != 0)
                    {
						Moving_Sphere_Collision(Spheres[i], Spheres[t]);
					}
					
				}

			}
            else
            {
				// No moving spheres
            }

		}
	}


void Sphere_Plane_Colision (Physics_Sphere Sphere, Physics_Plane Plane)
    {
		float Apporach_Angle = Mathf.Acos(Vector_Dot(Sphere.Velocity, -Plane.Get_Normal()) / (Length_Of_Vector(Sphere.Velocity) * Length_Of_Vector(-Plane.Get_Normal())));
		
		if (Apporach_Angle <= 90)
        {
			//Colision Possbile
			Apporach_Angle *= (Mathf.PI / 180);
			Vector3 Point_on_Plane = Plane.Get_Point();
			Vector3 Point_to_Sphere = Point_on_Plane - Sphere.GetLocation();
			
			float Angle_Between_Normal_PtS = Mathf.Acos(Vector_Dot(Point_to_Sphere, - Plane.Get_Normal()) / (Length_Of_Vector(Point_to_Sphere) * Length_Of_Vector(-Plane.Get_Normal())));
			Angle_Between_Normal_PtS *= (Mathf.PI / 180);
			float Angle_Between_Plane_PtS = 90 - Angle_Between_Normal_PtS;
			Angle_Between_Plane_PtS *= (Mathf.PI / 180);
			float Closest_Distance = Mathf.Sin(Angle_Between_Plane_PtS) * Length_Of_Vector(Point_to_Sphere);

			float Distance_to_Colision = (Closest_Distance - Sphere.Radius) / Mathf.Cos(Apporach_Angle);

			if (Distance_to_Colision <= Length_Of_Vector(Sphere.Velocity))
            {
				//Colision Occurs this frame
				if(!Sphere.Recently_Colided)
                {
					Vector3 Colision_Point = (Distance_to_Colision * Sphere.Velocity) / Length_Of_Vector(Sphere.Velocity);
					print("Sphere Colided with Plane");

					Sphere.transform.position += Colision_Point;
					Sphere.Recently_Colided = true;

					Vector3 Starting_Velocity = Sphere.Velocity;

					Vector3 Unit_Min = -Sphere.Velocity / Length_Of_Vector(Sphere.Velocity);
					
					Vector3 New_Vel_Unit = ((2 * Plane.Get_Normal()) * Vector_Dot(Plane.Get_Normal(), Unit_Min)) + Unit_Min;
					Vector3 New_Vel = New_Vel_Unit * Length_Of_Vector(-Sphere.Velocity);

					float Restitution = Length_Of_Vector(Starting_Velocity)/Length_Of_Vector(New_Vel);

					Sphere.Velocity = New_Vel * Restitution;
					return;
				}
			}
            else
            {
				// Sphere to plane colision out of bounds of this frame
            }
		}
        else
        {
			// Angle too low
        }
	}
	void SPhere_To_Plane_Update()
	{
		for (int i = 0; i < Spheres.Length; i++)
		{
			for (int t = 0; t < Planes.Length; t++)
			{
				Sphere_Plane_Colision(Spheres[i], Planes[t]);
			}

		}
	}

	void Moving_Sphere_Collision(Physics_Sphere sphere1, Physics_Sphere sphere2)
	{
		Vector3 Sphere_1_Loc = sphere1.GetLocation();
		Vector3 Sphere_2_Loc = sphere2.GetLocation();

		// Calculate the relative position between the two spheres
		float Relative_Pos_X = Sphere_1_Loc.x - Sphere_2_Loc.x;
		float Relative_Pos_Y = Sphere_1_Loc.y - Sphere_2_Loc.y;
		float Relative_Pos_Z = Sphere_1_Loc.z - Sphere_2_Loc.z;

		// Calculate the relative velocityies
		float Relative_Vel_X = sphere1.Velocity.x - sphere2.Velocity.x;
		float Relative_Vel_Y = sphere1.Velocity.y - sphere2.Velocity.y;
		float Relative_Vel_Z = sphere1.Velocity.z - sphere2.Velocity.z;

		float Sum_of_Radi = sphere1.Radius + sphere2.Radius;
		
        // Calculate components of the quadratic equation
        float a = (Relative_Vel_X * Relative_Vel_X) + (Relative_Vel_Y * Relative_Vel_Y) + (Relative_Vel_Z * Relative_Vel_Z);
        float b = (2 * Relative_Pos_X * Relative_Vel_X) + (2 * Relative_Pos_Y * Relative_Vel_Y) + (2 * Relative_Pos_Z * Relative_Vel_Z);
        float c = (Relative_Pos_X * Relative_Pos_X) + (Relative_Pos_Y * Relative_Pos_Y) + (Relative_Pos_Z * Relative_Pos_Z) - (Sum_of_Radi * Sum_of_Radi);

		float Quad_Result = (b * b) - (4 * a * c);

		// If the Result is negative, there is no collision
		if (Quad_Result < 0)
		{
			return;
		}

		float t1 = (-b + Mathf.Sqrt(Quad_Result)) / (2 * a);
		float t2 = (-b - Mathf.Sqrt(Quad_Result)) / (2 * a);

		float Collision_Time = Mathf.Min(t1, t2);


		if (Collision_Time > 0 && Collision_Time <= Time.fixedDeltaTime)
        {
			// Collision detected
			sphere1.transform.position += sphere1.Velocity * Collision_Time;
			sphere2.transform.position += sphere2.Velocity * Collision_Time;
			sphere1.Velocity = Vector3.zero;
			sphere2.Velocity = Vector3.zero;
		}
		return;
	}
	
	



	// Start is called before the first frame update
	void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
		Stationary_Sphere_Colision_Update();
        if (Length_Of_Vector(Spheres[0].Velocity) > 0 && Length_Of_Vector(Spheres[1].Velocity) > 0)
        {
			Moving_Sphere_Colision_Update();
		}
		SPhere_To_Plane_Update();
		//print("Update");
	}
}
