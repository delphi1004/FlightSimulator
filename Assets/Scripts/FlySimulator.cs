////////////// Private Member Propery ///////////////////////////////////


////////////// Public Member Propery ///////////////////////////////////


////////////// Private Member Method ///////////////////////////////////


////////////// Public Member Method ///////////////////////////////////


////////////// Event Handling Method //////////////////////////////////



using UnityEngine;
using System.Collections;

public class FlightPointInfo
{
	public Vector3 Position;
	public float Angle;


	public FlightPointInfo(Vector3 v , float a)
	{
		Position = v;

		Angle = a;
	}
}



public class PathGenerator : Object
{

	////////////// Private Member Propery ///////////////////////////////////


	private float MaxSteps; 
	private float x0,x1,x2,x4;
	private float y0,y1,y2,y3;
	private ArrayList Controls;
	
	

	
	
	////////////// Public Member Propery ///////////////////////////////////
	
	
	
	public float MinX,MinZ;
	public float MaxX,MaxZ;
		
		
	////////////// Private Member Method ///////////////////////////////////




	float CalcCatmulRoomSplines(float t, float p0, float p1, float p2, float p3)
	{
		float t2, t3;
		float b1, b2, b3, b4;
		float Result;
		
		t2 = Mathf.Pow(t, 2);
		t3 = Mathf.Pow(t, 3);
		
		b1 = 0.5f*(  -t3+2*t2-t);
		b2 = 0.5f*( 3*t3-5*t2+2);
		b3 = 0.5f*(-3*t3+4*t2+t);
		b4 = 0.5f*(   t3-  t2);
		
		Result = (p0 * b1 + p1*b2 + p2*b3 + p3*b4);
		
		return Result;
	}
	
	
	////////////// Public Member Method ///////////////////////////////////
	

	public PathGenerator()
	{
		MaxSteps = 1000.0f;

		MinX = 100000;
		MinZ = 100000;

		MaxX = -1;
		MaxZ = -1;

		Controls = new ArrayList();

		Debug.Log("PathGenerator created.");
	}

	public int ControlCount()
	{
		return Controls.Count;
	}

	public Vector3 PointAtIndex(int Index)
	{
		return (Vector3) Controls[Index];
	}

	public void AddControl(Vector3 Position)
	{
		Controls.Add(Position);

		Debug.Log("AddControl , added  = "+Position);
	}

	public void UpdateControl(int Index , Vector3 Position)
	{
		Vector3 OrgPosition;

		OrgPosition = (Vector3) Controls[Index];

		Debug.Log("UpdateControl : Org : "+OrgPosition);

		Controls[Index] = Position;

		Debug.Log("UpdateControl : After : "+(Vector3) Controls[Index]);
	}

	public void DeleteControl()
	{
		if (Controls.Count > 0)
		{
			Debug.Log("Deleted "+Controls[Controls.Count-1]);

			Controls.RemoveAt(Controls.Count-1);
		}
	}

	
	public void ClearControls()
	{
		Controls.Clear();
	}
	
	public ArrayList CalcPath()
	{
		int p0,p1,p2,p3;
		float PosX,PosZ;
		float Angle;
		float NextPosX,NextPosZ;
		ArrayList RetVector;
		Vector3 []TempVector;

		Angle = 0;

		PosX = 0;
		PosZ = 0;
	
		RetVector = new ArrayList();

		TempVector = new Vector3[4];

		for (int i=0;i<Controls.Count;i++)
		{
			p0 = (i-1 + Controls.Count) % Controls.Count;
			p1 = (i   + Controls.Count) % Controls.Count;
			p2 = (i+1 + Controls.Count) % Controls.Count;
			p3 = (i+2 + Controls.Count) % Controls.Count;

			Debug.Log(p0+" , "+p1+" , "+p2+" , "+p3);

			TempVector[0] = (Vector3) Controls[p0];
			TempVector[1] = (Vector3) Controls[p1];
			TempVector[2] = (Vector3) Controls[p2];
			TempVector[3] = (Vector3) Controls[p3];

			for (int j=0;j<MaxSteps;j++)
			{
				PosX = CalcCatmulRoomSplines(j/MaxSteps,TempVector[0].x,TempVector[1].x,TempVector[2].x,TempVector[3].x);
				PosZ = CalcCatmulRoomSplines(j/MaxSteps,TempVector[0].z,TempVector[1].z,TempVector[2].z,TempVector[3].z);

				if (j < MaxSteps-1)
				{
					NextPosX = CalcCatmulRoomSplines((j+1)/MaxSteps,TempVector[0].x,TempVector[1].x,TempVector[2].x,TempVector[3].x);
					NextPosZ = CalcCatmulRoomSplines((j+1)/MaxSteps,TempVector[0].z,TempVector[1].z,TempVector[2].z,TempVector[3].z);

					Angle = -Mathf.Atan2( (NextPosZ - PosZ) , (NextPosX - PosX)) * Mathf.Rad2Deg+90.0f;

					//Angle = Vector2.Angle(new Vector2(PosX,PosZ) , new Vector2(NextPosX,NextPosZ));
				}

				//Debug.Log("Angle :"+Angle);
				
				RetVector.Add (new FlightPointInfo(new Vector3(PosX,TempVector[1].y,PosZ) , Angle));

				if (PosX > MaxX)
				{
					MaxX = PosX;
				}

				if (PosZ > MaxZ)
				{
					MaxZ = PosZ;
				}

				if (PosX < MinX)
				{
					MinX = PosX;
				}

				if (PosZ < MinZ)
				{
					MinZ = PosZ;
				}
			}

			Debug.Log("--------------------------------------------------------");
	
		}

		Debug.Log("Path created. "+RetVector.Count+ " points");
		Debug.Log("MinX = "+MinX+" MinZ = "+MinZ+" MaxX = "+MaxX+" );  = "+MaxZ);
		
		return RetVector;
	}

}


		
    ////////////// Event Handling Method //////////////////////////////////
	



public class FlySimulator : MonoBehaviour 
{


	////////////// Private Member Propery ///////////////////////////////////


	private float LastDist;
	private float DistDiff;
	private PathGenerator FlightPathGen;
	private PathGenerator RoiPathGen;
	private ArrayList FlightPath;
	private ArrayList RoiFlightPath;
	private float FlightStep;
	private float CurAngle;
	private float RoiAutoLevel;
	private float FlyAutoLevel;
	private bool DrawInitPass;
	private bool ShouldGo;
	private MapViewEditor MapView;





	
	////////////// Public Member Propery ///////////////////////////////////

	
	public GameObject RoiIndicator;
	public GameObject FlightIndicator;

	
	////////////// Private Member Method ///////////////////////////////////





	private void InitDefaultData()
	{
	
		MapView = (MapViewEditor)GameObject.Find("MiniMap Camera").GetComponent("MapViewEditor");

		DrawInitPass = false;

		ShouldGo = true;

		CurAngle = 180;

		FlightStep = 0;

		FlightPathGen = new PathGenerator();

		RoiPathGen = new PathGenerator();

		FlightPath = new ArrayList();
		RoiFlightPath = new ArrayList();
	
		/*

		FlightPathGen.AddControl(new Vector3(2396.88f,1813.03f,10315.0f));
		FlightPathGen.AddControl(new Vector3(3063.99f,4197.01f,3280.4f));
		FlightPathGen.AddControl(new Vector3(9319.31f,3188.51f,3001.1f));
		FlightPathGen.AddControl(new Vector3(11022.0f,2521.09f,8715.8f));*/

		/*
		RoiPathGen.AddControl(new Vector3(3097.2f,2184.8f,9034.6f));
		RoiPathGen.AddControl(new Vector3(3574.2f,2328.5f,7408.8f));
		RoiPathGen.AddControl(new Vector3(4447.2f,2328.5f,6395.5f));
		RoiPathGen.AddControl(new Vector3(5110.4f,3752f,4496.5f));
		RoiPathGen.AddControl(new Vector3(8747.2f,2795.8f,5803.8f));
		RoiPathGen.AddControl(new Vector3(8747.2f,1293.9f,8800f));*/


		//FlightPath = FlightPathGen.CalcPath();

		//RoiFlightPath = RoiPathGen.CalcPath();


		//FlightInfo  = (FlightPointInfo) RoiFlightPath[0];
		//RoiAutoLevel = FlightInfo.Position.y; // first initialize

		//FlightInfo = (FlightPointInfo) FlightPath[0];
		//FlyAutoLevel = FlightInfo.Position.y; // first initialize
	}

	private void InitPassDraw()
	{
		int PointIndex;
		FlightPointInfo FlightInfo;

		DrawInitPass = true;

		if (FlightPath.Count > 0)
		{
			for (float i=0.0f;i<=1.0f;i+=0.0009f)
			{
				PointIndex = (int)Mathf.Lerp(0,FlightPath.Count-1,i);

				FlightInfo = (FlightPointInfo) FlightPath[PointIndex];

				FlyAutoLevel = DoAutoLevel(FlightInfo.Position,FlyAutoLevel,Color.blue);

				FlightInfo.Position.y = 10000;
			}
		}

		if (RoiFlightPath.Count > 0)
		{
			for (float i=0.0f;i<=1.0f;i+=0.0009f)
			{
				PointIndex = (int)Mathf.Lerp(0,RoiFlightPath.Count-1,i);
				
				FlightInfo = (FlightPointInfo) RoiFlightPath[PointIndex];
				
				RoiAutoLevel = DoAutoLevel(FlightInfo.Position,RoiAutoLevel,Color.red);
			}
		}

		DrawInitPass = false;
	}

	private float DoAutoLevel(Vector3 SrcPosition,float CurLevel,Color DebugLineColor)
	{
		Vector3 Position;
		RaycastHit Hit;
		float AutoLevelY;

		Hit = new RaycastHit();

		Position = new Vector3(SrcPosition.x,CurLevel,SrcPosition.z-200);

		AutoLevelY = 0;

		if (Physics.Raycast(Position, Vector3.down, out Hit) == true)
		{
			//print ("There is something in front of the object! "+Hit.distance);

			AutoLevelY = (CurLevel - (Hit.distance-1000));
		
			Position = new Vector3(SrcPosition.x , AutoLevelY , SrcPosition.z); 

			if (DrawInitPass == true)
			{
				Debug.DrawRay(Position, Vector3.down, DebugLineColor,10000000.0f);
			}

			//Debug.Log("AutoLevelY :"+AutoLevelY);
		}

		return AutoLevelY;
	}
	
	private void DoAutoFlight()
	{
		int PointIndex;
		FlightPointInfo FlightInfo;
		FlightPointInfo RoiFlightInfo;

		if (FlightStep <= 1.0f && FlightPath.Count > 0)
		{
			MapView.SetFlightStep(FlightStep);

			// Get air plane flight pass information ///////////////////////////////////

			PointIndex = (int)Mathf.Lerp(0,FlightPath.Count-1,FlightStep);

			FlightInfo = (FlightPointInfo) FlightPath[PointIndex];

			FlyAutoLevel = DoAutoLevel(FlightInfo.Position,FlyAutoLevel,Color.blue);

			FlightInfo.Position.y = 6000; // for temp test

			transform.position = FlightInfo.Position;

			CurAngle -= FlightInfo.Angle;


		
			//transform.eulerAngles = new Vector3(0, FlightInfo.Angle, 0);


			// Get Roi flight pass information ///////////////////////////////////

			PointIndex = (int)Mathf.Lerp(0,RoiFlightPath.Count-1,FlightStep);

			RoiFlightInfo = (FlightPointInfo) RoiFlightPath[PointIndex];

			RoiAutoLevel = DoAutoLevel(RoiFlightInfo.Position,RoiAutoLevel,Color.red);

			RoiFlightInfo.Position.y = RoiAutoLevel;

			transform.LookAt(RoiFlightInfo.Position);

	
			FlightStep += 0.0005f;

			Debug.DrawLine(FlightInfo.Position, RoiFlightInfo.Position);

			Debug.DrawLine(new Vector3(FlightInfo.Position.x,FlyAutoLevel,FlightInfo.Position.z), RoiFlightInfo.Position);


			FlightInfo.Position.y = 7000;
			FlightIndicator.transform.position = FlightInfo.Position;


			RoiFlightInfo.Position.y = 7000;
			RoiIndicator.transform.position = RoiFlightInfo.Position;
		}
		else
		{
			FlightStep = 0.0f;
		}
	}

	private Vector3 ConvertToTerrainCoord(Vector3 Point)
	{
		float PosX;
		float PosY;
		Terrain TempTerrain;
		Vector3 TempVect;
		Vector3 RetVect;
		
		TempVect = new Vector3(1.0f - Point.x , 1.0f , 1.0f - Point.z);
		
		Debug.Log("TempVect = "+TempVect);
		
		TempTerrain = Terrain.activeTerrain;
		
		PosX = Mathf.Lerp(0,TempTerrain.terrainData.size.x,TempVect.x);
		PosY = Mathf.Lerp(0,TempTerrain.terrainData.size.z,TempVect.z);
		
		Debug.Log("TempTerrain.terrainData.size = "+TempTerrain.terrainData.size);
		Debug.Log("PosX = "+PosX+" PosY = "+PosY);
		
		Debug.Log("TempVect = "+TempVect);
		
		RetVect = new Vector3(PosX,4000,PosY);

		return RetVect;
	}

	

	////////////// Public Member Method ///////////////////////////////////
		


	// Use this for initialization

	void Start () 
	{
		InitDefaultData();

		InitPassDraw();
	}
	
	public void StartFlight()
	{
		ShouldGo = true;

		Debug.Log("Start Flight Clicked!");
	}

	public void AddFlightControlPoint(Vector3 Point)
	{
		Vector3 TempVect;

		TempVect = ConvertToTerrainCoord(Point);

		FlightPathGen.AddControl(TempVect);

		CalcFlightPath();

		InitPassDraw();
	}

	public void UpdateFlightControlPoint(int Index , Vector3 Point)
	{
		Vector3 TempVect;
		
		TempVect = ConvertToTerrainCoord(Point);

		FlightPathGen.UpdateControl(Index,TempVect);
		
		CalcFlightPath();
	}

	public void CalcFlightPath()
	{
		FlightPointInfo FlightInfo;

		FlightPath.Clear();

		FlightPath = FlightPathGen.CalcPath();
		
		FlightInfo = (FlightPointInfo) FlightPath[0];
		
		FlyAutoLevel = FlightInfo.Position.y; // first initialize
	}

	public void AddRoiControlPoint(Vector3 Point)
	{
		Vector3 TempVect;
		
		TempVect = ConvertToTerrainCoord(Point);
		
		RoiPathGen.AddControl(TempVect);
		
		CalcRoiPath();
		
		InitPassDraw();
	}
	
	public void UpdateRoiControlPoint(int Index , Vector3 Point)
	{
		Vector3 TempVect;
		
		TempVect = ConvertToTerrainCoord(Point);
		
		RoiPathGen.UpdateControl(Index,TempVect);
		
		CalcRoiPath();
	}
	
	public void CalcRoiPath()
	{
		FlightPointInfo FlightInfo;
		
		RoiFlightPath.Clear();
		
		RoiFlightPath = RoiPathGen.CalcPath();
		
		FlightInfo = (FlightPointInfo) RoiFlightPath[0];
		
		RoiAutoLevel = FlightInfo.Position.y; // first initialize
	}

	public void DeleteFlightControlPoint()
	{
		FlightPathGen.DeleteControl();
	}

	public void DeleteRoiControlPoint()
	{
		RoiPathGen.DeleteControl();
	}
		
	// Update is called once per frame
	void Update () 
	{
		if (ShouldGo == true)
		{
			DoAutoFlight();
		}

	}

}


