using UnityEngine;
using System.Collections;


public class MapViewEditor : MonoBehaviour
{

	////////////// Private Member Propery ///////////////////////////////////

	public Material mat;
	private Vector3 startVertex;
	private Vector3 mousePos;
	private PathGenerator FlightPathGen;
	private PathGenerator RoiPathGen;
	private FlightPointInfo FlightInfo;
	private ArrayList FlightPath;
	private ArrayList RoiPath;

	private Rect RectHud;
	private float ControlPointWidth;
	private float ControlPointHeight;
	private float RoiControlPoint;
	private bool IsMouseDown;
	private int CurSelControlPoint;
	private float LastMousePosX;
	private float LastMousePosY;
	private bool IsAddFlightControlMode;
	private bool IsFightControlPointSelected;
	private FlySimulator Simulator;
	private float FlightStep;






	////////////// Public Member Propery ///////////////////////////////////








	////////////// Private Member Method ///////////////////////////////////





	private void InitDefaultData()
	{
		Simulator = (FlySimulator)GameObject.Find("Main Camera").GetComponent("FlySimulator");

		IsAddFlightControlMode = false;

		IsFightControlPointSelected = false;

		ControlPointWidth = 0.01f;
		ControlPointHeight = 0.013f;

		RoiControlPoint = Mathf.Sqrt(Mathf.Pow(ControlPointWidth ,2)+Mathf.Pow(ControlPointHeight/2,2))*2;

		Debug.Log("RoiControlPoint = "+RoiControlPoint);
	}

	private Vector3 ConvertToViewportPoint(float PosX,float PosY)
	{
		Vector3 TempVect;
		Vector3 PointVect;
		
		TempVect = transform.camera.ScreenToViewportPoint(new Vector3(PosX,PosY,0));
		
		PointVect = new Vector3(TempVect.x,0,TempVect.y);

		Debug.Log("Before ConvertToGLCoord , PosX = "+PosX+" PosY = "+PosY);

		Debug.Log("After ConvertToGLCoord , PosX = "+PointVect.x+" PosY = "+PointVect.y);
		
		return PointVect;
	}
	
	private int GetShortestControlPoint(PathGenerator PathGen, Vector3 Point)
	{
		float Distance;
		int RetIndex;
		Vector3 FlightPoint;
	
		RetIndex = -1;

		for (int i=0;i<PathGen.ControlCount();i++)
		{
			FlightPoint = PathGen.PointAtIndex(i);

			Distance = Vector3.Distance(Point,FlightPoint);

			if (Distance <= RoiControlPoint)
			{
				RetIndex = i;

				Debug.Log("Selected ShortestControlPoint : "+Distance+" and index is "+RetIndex);

				break;
			}
		}
	
		return RetIndex;
	}

	private void MouseMovedWithMouseDown(float PosX,float PosY)
	{
		Vector3 PointVect;
	
		PointVect = ConvertToViewportPoint(PosX,PosY);

		if (CurSelControlPoint >= 0)
		{
			if (IsFightControlPointSelected == true)
			{
				UpdateFlightPoint(CurSelControlPoint , PointVect);
			}
			else
			{
				UpdateROIPoint(CurSelControlPoint , PointVect);
			}
		}
	}

	private void DecideRelatedAction(float PosX,float PosY)
	{
		int TempIndex1;
		int TempIndex2;

		Vector3 PointVect;

		PointVect = ConvertToViewportPoint(PosX,PosY);

		if (IsMouseDown == true)
		{
			CurSelControlPoint = -1;

			TempIndex1 = GetShortestControlPoint(FlightPathGen , PointVect);
			TempIndex2 = GetShortestControlPoint(RoiPathGen , PointVect);

			if (TempIndex1 == -1 && TempIndex2 == -1)
			{
				if (IsAddFlightControlMode == true)
				{
					AddFlightPoint(PointVect);
				}
				else
				{
					AddROIPoint(PointVect);
				}
			}

			Debug.Log("TempIndex1 = "+TempIndex1+" TempIndex2 = "+TempIndex2);

			if (TempIndex1 != -1)
			{
				IsFightControlPointSelected = true;

				CurSelControlPoint = TempIndex1;
			}

			if(TempIndex2 != -1)
			{
				IsFightControlPointSelected = false;
				
				CurSelControlPoint = TempIndex2;
			}
		}
	}	

	private void DrawHudArea()
	{
		GL.PushMatrix ();
		
		GL.LoadOrtho();
		
		mat.SetPass (0);
		
		GL.Begin (GL.LINES);
		
		GL.Color (Color.green);

		GL.Vertex(new Vector2(RectHud.x,RectHud.y));
		GL.Vertex(new Vector2(RectHud.x+RectHud.width,RectHud.y));

		GL.Vertex(new Vector2(RectHud.x+RectHud.width,RectHud.y));
		GL.Vertex(new Vector2(RectHud.x+RectHud.width,RectHud.y-RectHud.height));

		GL.Vertex(new Vector2(RectHud.x+RectHud.width,RectHud.y-RectHud.height));
		GL.Vertex(new Vector2(RectHud.x,RectHud.y-RectHud.height));

		GL.Vertex(new Vector2(RectHud.x,RectHud.y-RectHud.height));
		GL.Vertex(new Vector2(RectHud.x,RectHud.y));

		GL.End ();
		
		GL.PopMatrix ();
	}

	private void DrawControlPoints(PathGenerator PathGen , Color color)
	{
		Vector3 FlightPoint;

		for (int i=0;i<PathGen.ControlCount();i++)
		{
			FlightPoint = PathGen.PointAtIndex(i);

			GL.PushMatrix ();
			
			GL.LoadOrtho();
			
			mat.SetPass(0);
			
			GL.Begin(GL.QUADS);
			
			GL.Color(new Color(color.r,color.g,color.b,0.7f));
			
			GL.Vertex3(FlightPoint.x - ControlPointWidth,FlightPoint.z+ControlPointHeight,0);
			GL.Vertex3(FlightPoint.x - ControlPointWidth,FlightPoint.z-ControlPointHeight,0);
			GL.Vertex3(FlightPoint.x + ControlPointWidth,FlightPoint.z-ControlPointHeight,0);
			GL.Vertex3(FlightPoint.x + ControlPointWidth,FlightPoint.z+ControlPointHeight,0);
		
			GL.End ();

			GL.PopMatrix ();
		}
	}

	private void DrawPath(ArrayList Path , Color color,float Alpha)
	{
		GL.PushMatrix ();
		
		GL.LoadOrtho();
		
		mat.SetPass (0);

		GL.Begin(GL.LINES);

		GL.Color(new Color(color.r,color.g,color.b,Alpha));
	
		for (int i=0;i<Path.Count-1;i++)
		{
			FlightInfo = (FlightPointInfo) Path[i];
			
			GL.Vertex3(FlightInfo.Position.x,FlightInfo.Position.z,0);
			
			FlightInfo = (FlightPointInfo) Path[i+1];
			
			GL.Vertex3(FlightInfo.Position.x,FlightInfo.Position.z,0);
		}
		
		GL.End ();
		
		GL.PopMatrix ();
	}

	private void DrawIndicator(ArrayList PathInfo , Color color)
	{
		int PointIndex;
		float PosX,PosY;
		float PosX1,PosY1;
		float PosX2,PosY2;
		float PosX3,PosY3;
		float PosX4,PosY4;
		float Angle;
		FlightPointInfo FlightInfo;
		FlightPointInfo FlightInfo1;

		Angle = 0.0f;
	
		PosX1 = 0;
		PosY1 = 0;
	
		PointIndex = (int)Mathf.Lerp(0,PathInfo.Count-1,FlightStep);

		if (PointIndex < PathInfo.Count-1)
		{
			FlightInfo = (FlightPointInfo) PathInfo[PointIndex];
			FlightInfo1 = (FlightPointInfo) PathInfo[PointIndex+1];
			
			PosX = FlightInfo.Position.x;
			PosY = FlightInfo.Position.z;

			PosX1 = FlightInfo1.Position.x;
			PosY1 = FlightInfo1.Position.z;
	
			Angle = Mathf.Atan2(FlightInfo1.Position.z-FlightInfo.Position.z, FlightInfo1.Position.x-FlightInfo.Position.x);

			PosX2 = Mathf.Cos(Angle+Mathf.Deg2Rad*90.0f) * ControlPointWidth;
			PosY2 = Mathf.Sin(Angle+Mathf.Deg2Rad*90.0f) * ControlPointWidth;

			PosX3 = Mathf.Cos(Angle+Mathf.Deg2Rad*-90.0f) * ControlPointWidth;
			PosY3 = Mathf.Sin(Angle+Mathf.Deg2Rad*-90.0f) * ControlPointWidth;

			PosX4 = Mathf.Cos(Angle) * ControlPointWidth * 2.0f;
			PosY4 = Mathf.Sin(Angle) * ControlPointWidth * 2.0f;

			GL.PushMatrix ();
			
			GL.LoadOrtho();
			
			mat.SetPass(0);

			GL.Begin(GL.QUADS);

			GL.Color(Color.white);

			GL.Vertex3(PosX+PosX2,PosY+PosY2,0);
			GL.Vertex3(PosX+PosX4,PosY+PosY4,0);

			GL.Vertex3(PosX+PosX3,PosY+PosY3,0);
			GL.Vertex3(PosX+PosX4,PosY+PosY4,0);

			GL.Vertex3(PosX+PosX2,PosY+PosY2,0);
			GL.Vertex3(PosX+PosX3,PosY+PosY3,0);
				
			GL.End ();

			// Draw Triangle Line

			GL.Begin(GL.LINES);
			
			GL.Color(color);

			GL.Vertex3(PosX+PosX2,PosY+PosY2,0);
			GL.Vertex3(PosX+PosX4,PosY+PosY4,0);
			
			GL.Vertex3(PosX+PosX3,PosY+PosY3,0);
			GL.Vertex3(PosX+PosX4,PosY+PosY4,0);
			
			GL.Vertex3(PosX+PosX2,PosY+PosY2,0);
			GL.Vertex3(PosX+PosX3,PosY+PosY3,0);
			
			GL.End ();
			
			GL.PopMatrix ();
		}
	}
	
	private void AddFlightPoint(Vector3 Point)
	{
		FlightPathGen.AddControl(Point);

		Simulator.AddFlightControlPoint(Point);

		Debug.Log("Flight point added :"+Point);

		CalcFlightPath();
	}

	private void CalcFlightPath()
	{
		FlightPath.Clear();
		
		FlightPath = FlightPathGen.CalcPath();

		Simulator.CalcFlightPath();
	}
	
	private void UpdateFlightPoint(int Index , Vector3 Point)
	{
		FlightPathGen.UpdateControl(Index,Point);

		CalcFlightPath();

		Simulator.UpdateFlightControlPoint(Index,Point);
	}

	private void AddROIPoint(Vector3 Point)
	{
		RoiPathGen.AddControl(Point);

		Simulator.AddRoiControlPoint(Point);
		
		CalcROIPath();
		
		Debug.Log("ROI point added :"+Point+ " Count is = "+RoiPathGen.ControlCount());
	}

	private void CalcROIPath()
	{
		RoiPath.Clear();
		
		RoiPath = RoiPathGen.CalcPath();

		Simulator.CalcRoiPath();
	}
	
	private void UpdateROIPoint(int Index , Vector3 Point)
	{
		RoiPathGen.UpdateControl(Index,Point);
		
		CalcROIPath();

		Simulator.UpdateRoiControlPoint(Index,Point);
	}

	private void DeleteControlPoint()
	{
		PathGenerator PathGen;

		PathGen = FlightPathGen;

		if (IsFightControlPointSelected == false)
		{
			PathGen = RoiPathGen;

			Simulator.DeleteRoiControlPoint();
		}
		else
		{
			Simulator.DeleteFlightControlPoint();
		}

		PathGen.DeleteControl();

		if (IsFightControlPointSelected == true)
		{
			CalcFlightPath();
		}
		else
		{
			CalcROIPath();
		}
	}










	
	
	
	
	
	
	
	
	
	
	
	
	
	
	////////////// Public Member Method ///////////////////////////////////
	
	void OnPostRender()
	{
		if (IsAddFlightControlMode == true )
		{
			DrawPath(FlightPath, Color.blue,1.0f);
		}
		else
		{
			DrawPath(FlightPath, Color.blue,1.0f);
		}


		if (IsAddFlightControlMode == false)
		{
			DrawPath(RoiPath, Color.red,1.0f);
		}
		else
		{
			DrawPath(RoiPath, Color.red,1.0f);
		}

		DrawControlPoints(FlightPathGen , Color.blue);
		DrawControlPoints(RoiPathGen , Color.red);

		if (FlightPathGen.ControlCount() > 1)
		{
			DrawIndicator(FlightPath , Color.blue);
		}

		if (RoiPathGen.ControlCount() > 1)
		{
			DrawIndicator(RoiPath , Color.red);
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		InitDefaultData();

		FlightPathGen = new PathGenerator();
		RoiPathGen = new PathGenerator();

		FlightPath = new ArrayList();
		RoiPath = new ArrayList();

		if (!mat) 
		{
			Debug.LogError ("Please Assign a material on the inspector");
			
			return;
		}
    }

	public void SetFlightStep(float Step)
	{
		FlightStep = Step;
	}

	
	// Update is called once per frame
	void Update ()
	{
		mousePos = Input.mousePosition;

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			startVertex = new Vector3 (mousePos.x / Screen.width, mousePos.y / Screen.height, 0);

			Debug.Log("startVertex "+startVertex);
		}

		if (Input.GetKeyDown (KeyCode.F) == true) 
		{
			IsAddFlightControlMode = true;
		}

		if (Input.GetKeyDown (KeyCode.R) == true) 
		{
			IsAddFlightControlMode = false;
		}

		if (Input.GetKeyDown (KeyCode.Backspace) == true) 
		{
			DeleteControlPoint();
		}

		if (IsMouseDown == true)
		{
			if (LastMousePosX != mousePos.x || LastMousePosY != mousePos.y)
			{
				LastMousePosX = mousePos.x;
				LastMousePosY = mousePos.y;

				MouseMovedWithMouseDown(mousePos.x,mousePos.y);
			}
		}

		if (Input.GetMouseButtonDown(0) == true)
		{
			IsMouseDown = true;

			Debug.Log("Pressed left click. "+mousePos.x+" "+mousePos.y);

			//SetHudArea(mousePos.x,mousePos.y,250,200);

			Debug.Log(transform.camera.ScreenToViewportPoint(new Vector3(mousePos.x,mousePos.y,0)));

			//AddFlightPoint(mousePos.x,mousePos.y);

			DecideRelatedAction(mousePos.x,mousePos.y);

			//DrawFlightPoints();
		}

		if (Input.GetMouseButtonUp(0) == true)
		{
			IsMouseDown = false;
		}
	}



	////////////// Event Handler ///////////////////////////////////

	void OnMouseDown() 
	{
		Debug.Log("OnMouseDown Occured!");
	}
}
