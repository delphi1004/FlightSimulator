using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
//using System.Web;
using UnityEngine;

/*
 {
   "results" : [
      {
         "elevation" : 1608.637939453125,
         "location" : {
            "lat" : 39.7391536,
            "lng" : -104.9847034
         },
         "resolution" : 4.771975994110107
      }
   ],
   "status" : "OK"
}

*/

namespace GoogleElevation
{
    enum QUERY_MODE
    {
        ONT_POINT = 0,
        TWO_POINTS,
        PATH
    };

    public class GooglePosition
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class GoogleElevation
    {
        public double Elevation { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public double Resolution { get; set; }

        public void Show()
        {
            Debug.Log("Elevation :"+Elevation+" Latitude :"+Latitude+" Longitude:"+Longitude+" Resolution :"+Resolution);

            //System.Console.WriteLine("Elevation :"+Elevation+" Latitude :"+Latitude+" Longitude:"+Longitude+" Resolution :"+Resolution);
        }
    }

	public class GoogleElevationPack : MonoBehaviour
    {
        ////////////// Private Member Propery ///////////////////////////////////

        private QUERY_MODE QueryMode;
        private String Query;
        private int SampleCount;
        private GooglePosition[] Position;
        private Regex RegExpElevation;
        private Regex RegExpStatus;
        private Uri GoogleUri;
        private WebResponse GoogleWebResponse;
        private StreamReader GoogleStream;
        private String GoogleResult;
		private double MaxElevation;


        ////////////// Public Member Propery ///////////////////////////////////

        public int DataWidth { get; set; }
        public int DataHeight { get; set; }

        public List<GoogleElevation> Elevations { get; set; }



        ////////////// Private Member Method ///////////////////////////////////


        private void ExtractElevation()
        {
            float SampleRatio;
            GooglePosition[] TempPosition; 

            TempPosition = new GooglePosition[2];

            TempPosition[0] = new GooglePosition();
            TempPosition[1] = new GooglePosition();

            TempPosition[0].Latitude = Position[0].Latitude;
            TempPosition[0].Longitude = Position[0].Longitude;

            TempPosition[1].Latitude = Position[1].Latitude;
            TempPosition[1].Longitude = Position[1].Longitude;
           
            if (Position[0].Latitude != Position[1].Latitude) // Area Selected , ROI
            {
                SampleRatio = ((Position[0].Longitude - Position[1].Longitude) / SampleCount); // for symmetric sampleling

                TempPosition[1].Latitude = TempPosition[0].Latitude;

                while (TempPosition[0].Latitude > Position[1].Latitude)
                {
                    GenerateQuery(TempPosition); // build query string

                   // Console.WriteLine("Find for : " + TempPosition[0].Latitude + " , " + TempPosition[0].Longitude + " , " +
                    //                                TempPosition[1].Latitude + " , " + TempPosition[1].Longitude);

                    if (RequestElevation() == true) // execute query string and validation 
                    {
                        StoreElevationData();

                        DataHeight++;

                        //Console.WriteLine("--------------------------------------------------------------------"+DataHeight);
                    }
					else
					{
						StoreRandomElevationData();

						DataHeight++;
					}


                    TempPosition[0].Latitude += SampleRatio;

                    TempPosition[1].Latitude = TempPosition[0].Latitude;
                }
               
            }

            System.Console.WriteLine("DataWidth = "+DataWidth+" + DataHeight = "+DataHeight);
        }

        private void GenerateQuery(GooglePosition[] TargetPosition)
        {
            // Build Query String ////////////////////////////////////////////////////////////

            if (QueryMode != QUERY_MODE.PATH)
            {
                Query = "http://maps.googleapis.com/maps/api/elevation/json?locations=";
            }
            else
            {
                Query = "http://maps.googleapis.com/maps/api/elevation/json?path=";
            }

            if (QueryMode == QUERY_MODE.ONT_POINT)
            {
                    Query += TargetPosition[0].Latitude + "," + TargetPosition[0].Longitude;
            }

            if (QueryMode == QUERY_MODE.TWO_POINTS || QueryMode == QUERY_MODE.PATH)
            {
                    Query += TargetPosition[0].Latitude + "," + TargetPosition[0].Longitude + "|" + TargetPosition[1].Latitude + "," + TargetPosition[1].Longitude;
            }

            if (QueryMode == QUERY_MODE.PATH)
            {
                Query += "&samples=" + SampleCount;
            }

            Query += "&sensor=true";

            //Debug.Log(Query);
        }

        private bool RequestElevation()
        {
            bool Success;
            String TempString;
            String[] TempSplit;
            MatchCollection TempMatCol;

            Success = true;

			TempSplit = null;

            try
            {
                GoogleResult = null;

                GoogleUri = new Uri(Query);

                GoogleWebResponse = WebRequest.Create(GoogleUri).GetResponse();

                GoogleStream = new StreamReader(GoogleWebResponse.GetResponseStream());

                GoogleResult = GoogleStream.ReadToEnd();

                TempMatCol = RegExpStatus.Matches(GoogleResult);

                TempString = TempMatCol[0].ToString().Replace("\"", string.Empty);

                TempSplit = TempString.Split(' ');

                //Debug.Log(TempString);

                Success = TempSplit[TempSplit.Length - 1].Equals("OK");
            }
            catch (Exception ex)
            {
                Success = false;

                Debug.Log(ex.Message);
            }
            finally
            {
                GoogleUri = null;
                GoogleStream = null;

				Debug.Log(Query+"     Status : "+TempSplit[2]);
            }

            return Success;
        }

        private void StoreElevationData()
        {
            MatchCollection TempMatCol;

            TempMatCol = RegExpElevation.Matches(GoogleResult);

            for (int i = 0; i < TempMatCol.Count; i += 4)
            {
                //Debug.Log(TempMatCol[i + 0].ToString());
                //Debug.Log(TempMatCol[i + 1].ToString());
                //Debug.Log(TempMatCol[i + 2].ToString());
                //Debug.Log(TempMatCol[i + 3].ToString());

                AddElevation(double.Parse(TempMatCol[i].ToString()), float.Parse(TempMatCol[i + 1].ToString()),
                             float.Parse(TempMatCol[i + 2].ToString()), float.Parse(TempMatCol[i + 3].ToString()));
            }

            GoogleResult = null;
        }

		private void StoreRandomElevationData()
		{
			for (int i = 0; i < DataWidth;i++)
			{
				//AddElevation(UnityEngine.Random.Range(0.0f,1000.0f),0,0,0);


				AddElevation(Mathf.PerlinNoise(UnityEngine.Random.Range(0.0f,1.0f) , 1.0f),0,0,0);


				//AddElevation(Mathf.PerlinNoise(UnityEngine.Random.Range(0.0f,1.0f) , 1.0f),0,0,0);

				//AddElevation( (i*1.0f) / DataWidth ,0,0,0);

				//AddElevation(Mathf.Sin((i*1.0f)/20.0f) , 0,0,0);

			}
			
			GoogleResult = null;
		}

        private void AddElevation(double Elv, float Lat, float Lng, float Res)
        {
            GoogleElevation TempElevation;

            TempElevation = new GoogleElevation();

            TempElevation.Elevation = Elv;
            TempElevation.Latitude = Lat;
            TempElevation.Longitude = Lng;
            TempElevation.Resolution = Res;

            Elevations.Add(TempElevation);

			if (Elv > MaxElevation)
			{
				MaxElevation = Elv;
			}

            //TempElevation.Show();
        }

       
        ////////////// Public Member Method ///////////////////////////////////


        public GoogleElevationPack()
        {
			MaxElevation = -1000;

            RegExpElevation = new Regex(@"[-+]?(\d*[.])?\d+");

            RegExpStatus = new Regex(@"\""status""\s:\s\""\D*""");

            Elevations = new List<GoogleElevation>();

            Position = new GooglePosition[2];

            Position[0] = new GooglePosition();
            Position[1] = new GooglePosition();
        }

        public void GenerateElevation(float Latitude, float Longitude)
        {
            Position[0].Latitude   = Latitude;
            Position[0].Longitude  = Longitude;

            QueryMode = QUERY_MODE.ONT_POINT;

            DataWidth = 1;
            DataHeight = 1;

            ExtractElevation();
        }

        public void GenerateElevation(float Latitude1, float Longitude1,float Latitude2, float Longitude2)
        {
            Position[0].Latitude = Latitude1;
            Position[0].Longitude = Longitude1;

            Position[1].Latitude = Latitude2;
            Position[1].Longitude = Longitude2;

            QueryMode = QUERY_MODE.TWO_POINTS;

            DataWidth = 2;
            DataHeight = 1;

            ExtractElevation();
        }

        public void GenerateElevation(float Latitude1, float Longitude1, float Latitude2, float Longitude2, int Count)
        {
            Position[0].Latitude = Latitude1;
            Position[0].Longitude = Longitude1;

            Position[1].Latitude = Latitude2;
            Position[1].Longitude = Longitude2;

            if (Count < 2)
            {
                Count = 2;
            }

            SampleCount = Count;

            DataWidth = Count;
            DataHeight = 0; // default.

            QueryMode = QUERY_MODE.PATH;

            ExtractElevation();
        }

        public float[,] GetElevationBy2DFloat()
        {
            int Count;
            float[,] Data;
            GoogleElevation TempElevation;

            Count = 0;

            Data = new float[DataHeight , DataWidth];

			try
			{
            	for (int y = 0; y < DataHeight; y++)
            	{
                	for (int x = 0; x < DataWidth; x++)
                	{
                    	TempElevation = (GoogleElevation)Elevations[Count++];

						Data[y, x] = (float)(TempElevation.Elevation / MaxElevation);
					}
				}
			}
			catch(Exception ex)
			{
				Debug.Log(ex.Message);
			}

            return Data;
        }
	
    }        
}
