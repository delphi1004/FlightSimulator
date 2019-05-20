using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
	private int CurTouchPoint;
	private int TouchPointCount;
	private float PositionX;
	private float PositionY;
	private float PositionZ;

	private float RotationY;
	private float RotationX;
	private float Speed;

	private float ForwardSpeed;
	private float RotationSpeed;
	private Vector3 []TouchPoints;

	private float MovedPercentage;
	private float TurnPercentage;

	private Vector3 bending = Vector3.up;
	private float timeToTravel = 10.0f;

	private bool ShouldTurn;

  


	// Private member method ----------------------------------------------------//

	void InitDefaultData()
	{
		ShouldTurn = true;

		RotationX = 10.0f;

		//transform.Translate(2236.60f,2195.44f,8977.77f);

		transform.Rotate(RotationX,0,0);
		 
		TouchPoints = new Vector3[100];

		for (int i=0;i<100;i++)
		{
			TouchPoints[i] = new Vector3(0,0);
		}
	
		TouchPoints[0].x = 2396.88f;
		TouchPoints[0].y = 1813.03f;
		TouchPoints[0].z = 10315.0f;


		TouchPoints[1].x = 3063.99f;
		TouchPoints[1].y = 4197.01f;
		TouchPoints[1].z = 3280.4f;

		TouchPoints[2].x = 9319.31f;
		TouchPoints[2].y = 3188.51f;
		TouchPoints[2].z = 3001.1f;

		TouchPoints[3].x = 11022.0f;
		TouchPoints[3].y = 2521.09f;
		TouchPoints[3].z = 8715.8f;

		TouchPoints[4].x = 2396.88f;
		TouchPoints[4].y = 1813.03f;
		TouchPoints[4].z = 10315.0f;

		TouchPointCount = 5;

		CurTouchPoint = 0;

		MovedPercentage = 0.0f;

		transform.position = TouchPoints[0];

		for (int i=0;i<TouchPointCount;i++)
		{
			TouchPoints[i].y -= 100.0f;
		}

		CalcTurnPath();
    }

	private void CheckKeyInput()
	{
		if (Input.GetKey("up") == true)
		{
			transform.Translate(0,0,2);
		}
		
		if (Input.GetKey("down") == true)
		{
			transform.Translate(0,0,-2);
		}
		
		if (Input.GetKey("q") == true)
		{
			transform.Translate(0,1,0);
		}
		
		if (Input.GetKey("a") == true)
		{
			transform.Translate(0,-1,0);
		}
		
		if (Input.GetKey("w") == true)
		{
			RotationX -= RotationSpeed;

			transform.rotation = Quaternion.Euler(RotationX , RotationY,0);
		}
		
		if (Input.GetKey("s") == true)
		{
			RotationX += RotationSpeed;

			transform.rotation = Quaternion.Euler(RotationX , RotationY,0);
		}
		
		if (Input.GetKey("right") == true)
		{
			RotationY += RotationSpeed;

			transform.rotation = Quaternion.Euler(RotationX , RotationY,0);
		}
		
		if (Input.GetKey("left") == true)
		{
			RotationY -= RotationSpeed;

			transform.rotation = Quaternion.Euler(RotationX , RotationY,0);
		}
	}

	private void CalcTurnPath()
	{
		float Angle;
		float PosX;
		float PosY;
		float Radius;
		float OffSetR;
		Vector3 Temp;
		Vector3 Temp2;

		/*

		TouchPoints[0].x = 2396.88f;
		TouchPoints[0].y = 1813.03f;
		TouchPoints[0].z = 10315.0f;


		TouchPoints[1].x = 3063.99f;
		TouchPoints[1].y = 4197.01f;
		TouchPoints[1].z = 3280.4f;

		*/

		OffSetR = 20;


		for (int i=0;i<4;i++)
		{
			Debug.DrawLine(TouchPoints[i] , TouchPoints[i+1],Color.green,100);
		}

		Radius = 500;

		Angle = -Mathf.Atan2(TouchPoints[1].z - TouchPoints[0].z , TouchPoints[1].x - TouchPoints[0].x);

		//Angle = Vector3.Angle(TouchPoints[0] , TouchPoints[1]);

		/*
		PosX = Mathf.Cos(Mathf.Deg2Rad*Angle) * Radius;
		PosY = Mathf.Sin(Mathf.Deg2Rad*Angle) * Radius;
		*/

		PosX = Mathf.Cos(Angle) * Radius;
		PosY = Mathf.Sin(Angle) * Radius;

		//Debug.DrawRay(Temp, Vector3.down, Color.red , 1000000.0f);

		Temp = new Vector3(TouchPoints[1].x+PosX , TouchPoints[1].y , TouchPoints[1].z+PosY);

		//Debug.DrawLine(TouchPoints[1] , Temp,Color.blue, 20, false);
	
		Debug.Log("TurnAngle = "+Angle+" PosX = "+PosX+" PosY = "+PosY);

		Debug.Log(Temp.ToString());




		//-----------------------------------------------------------------------//


		for (float i=0;i<0.2f;i+=0.0015f)
		{
			PosX = Mathf.Cos(Angle) * Radius;
			PosY = Mathf.Sin(Angle) * Radius;

			Temp = Vector3.Lerp(TouchPoints[1] , TouchPoints[2],i);

			Temp2 = new Vector3(Temp.x+PosX , Temp.y , Temp.z+PosY);

			Debug.DrawLine(Temp , Temp2,Color.blue, 200, false);

			Radius -= OffSetR;

			OffSetR -= 0.420f;

			if (OffSetR <= 0.0f)
			{
				OffSetR = 1.0f;
			}

			if (Radius <= 0)
			{
				Radius = 0.0f;
			}

			Debug.Log(OffSetR);
			
		}
	}

	
	// Public Member Method ----------------------------------------------------//


	// Use this for initialization

	void Start () 
	{
		Speed = 100.0f;

		ForwardSpeed = 2.0f;

		RotationSpeed = 0.5f;

		RotationY = 179.7702f;

		RotationX = 0;

		/*
		PositionZ = 1738.0f;

		PositionY = 764.3639f;

		RotationY = 179.7702f;*/

		//PositionX = transform.position.x;

		PositionZ = 0.0f;
		PositionY = 0.0f;

		InitDefaultData();

		transform.LookAt(TouchPoints[CurTouchPoint+1]);

		//LeanTween.moveSplineLocal(transform.gameObject , TouchPoints , 1000);
	}

	
	// Update is called once per frame
	void Update () 
	{
		Vector3 dir;

		CheckKeyInput();


		//transform.localEulerAngles = new Vector3(RotationX , RotationY ,  transform.rotation.z);

		//dir = TouchPoints[CurTouchPoint+1] - transform.position;
				
		//Quaternion rot = Quaternion.LookRotation(dir);

		//rot.x = 100.0f;
		//rot.z = 0.0f;

		//transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

		//transform.localEulerAngles = new Vector3(RotationX , transform.rotation.y ,  transform.rotation.z);

		/*
		if (MovedPercentage < 0.8f)
		{
			MovedPercentage += 0.001f;
		}
		else
		{
			MovedPercentage += 0.0005f;
		}

		transform.position = Vector3.Lerp(TouchPoints[CurTouchPoint],TouchPoints[CurTouchPoint+1],MovedPercentage);

		if (MovedPercentage >= 0.8f)
		{
			Vector3 Temp;

			ShouldTurn = false;

			dir = TouchPoints[CurTouchPoint+2] - transform.position;

			Temp = Vector3.Lerp(TouchPoints[CurTouchPoint+1] , TouchPoints[CurTouchPoint+2],TurnPercentage);

			Debug.DrawRay (Temp, Vector3.down, Color.red , 1000000.0f);
			
			Quaternion rot = Quaternion.LookRotation(dir);

			transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime/2.0f);

			TurnPercentage += 0.001f;
		}

		if (MovedPercentage >= 1.0f)
		{
			CurTouchPoint++;

			TurnPercentage = 0.0f;

			MovedPercentage = 0.0f;

			if (CurTouchPoint >= (TouchPointCount-1))
			{
				CurTouchPoint = 0;
			}

			float Angle = Vector3.Angle(TouchPoints[CurTouchPoint+1], transform.forward);

			Debug.Log("Target Angle = "+Angle);

			//transform.Rotate(0,Angle,0);
		}

*/
	}


	/*
	void Update () 
	{
		Vector3 dir;
		
		CheckKeyInput();

		MovedPercentage += 0.001f;
		
		//Debug.Log("MovedPercentage = " + MovedPercentage);

		//transform.RotateAround(TouchPoints[CurTouchPoint+1] , new Vector3(0,1,0) , 1);

		transform.position = Vector3.Lerp(TouchPoints[CurTouchPoint], TouchPoints[CurTouchPoint+1], MovedPercentage);
		
		if (MovedPercentage >= 1.0f)
		{
			CurTouchPoint++;

			MovedPercentage = 0.0f;
			
			if (CurTouchPoint >= (TouchPointCount-1))
			{
				CurTouchPoint = 0;
			}

			transform.LookAt(TouchPoints[CurTouchPoint+1]);

			//transform.Rotate(TouchPoints[CurTouchPoint+1]);
		}
		
	}

*/


}
