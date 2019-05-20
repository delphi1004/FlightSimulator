using System;
using UnityEngine;
using System.Collections;
using GoogleElevation;

/*
 * 
Debug.Log(Application.dataPath);

		TerrainGen = new TerrainGenerator();

		byte Type = 1;  // 00000001
		byte Size = 2;  // 00000010

		byte LowPass = 15;//00001111
		byte HighPass = 240; //11110000

		byte Data;

		Data = (byte)((Type << 4) | Size);

		print(Data);

		print("Type is = "+ ((Data & HighPass) >> 4));

		print("Size is = "+ (Data & LowPass));

*/

public class TestTerrainGenerator :MonoBehaviour
{

	private TerrainGenerator TerrainGen;

	
	// Use this for initialization
	void Start () 
	{

		Debug.Log(Application.dataPath);

		TerrainGen = new TerrainGenerator();

		/*
		GoogleElevationPack GoogleElevation;
		
		GoogleElevation = new GoogleElevationPack();
		
		//ElevationPack.GenerateElevation(39.7391536f, -104.9847034f); // One Point
		
		//ElevationPack.GenerateElevation(39.7391536f, -104.9847034f, 36.455556f, -116.866667f); // Two Points
		
		//ElevationPack.GenerateElevation(36.578581f, -118.291994f, 36.23998f, -116.83171f, 50); // Path
		
		GoogleElevation.GenerateElevation(36.0f, -118f, 35f, -116f, 200); // Path

		TerrainGen.SetTerrainInfo(GoogleElevation.DataWidth ,GoogleElevation.DataHeight,150);

		TerrainGen.CreateTerrain(GoogleElevation.GetElevationBy2DFloat() , GoogleElevation.DataWidth , GoogleElevation.DataHeight);

*/

		DEMElevationPack DemElevation;

		DemElevation = new DEMElevationPack();

		DemElevation.ReadFile(Application.dataPath+"/Data/elevation_map100.dem");

		//DemElevation.ReadFile(Application.dataPath+"/Data/elevation_map000.dem");

		TerrainGen.SetTerrainInfo(DemElevation.DataWidth *30,DemElevation.DataHeight * 30,4000);

		//TerrainGen.SetTerrainInfo(DemElevation.DataWidth *10,DemElevation.DataHeight * 10,2048);

		//TerrainGen.SetTerrainInfo(10000,10000,2048);

		TerrainGen.CreateTerrain(DemElevation.GetElevationBy2DFloat() , DemElevation.DataWidth , DemElevation.DataHeight);

		TerrainGen.CreateTexture("Grass (Hill)","Grass&Rock","Cliff (Layered Rock)");

		TerrainGen.AssignTexture();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
		;

	}
}
