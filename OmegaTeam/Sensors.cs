using System;
using System.Threading;
using System.Linq;

using MonoBrickFirmware;
using MonoBrickFirmware.Sensors;

namespace OmegaTeam
{
	public class Sensors
	{

		//################################################################################
		//################################################################################

		const short OBSTACLE_DISTANCE = 12; // Distanza a cui si riconosce un ostacolo, in cm
		public static sbyte[] BLACK = { 30, 31 }; // Valore per cui viene attivato "nero" -23
		public static sbyte[] WHITE = { 53, 54 };

		//################################################################################
		//################################################################################

		public MSSensorMUXBase colL;
	    public MSSensorMUXBase colR;
		public MSSensorMUXBase LL; // Doesn't work mother fucker!

		public EV3TouchSensor Touch;

		public MSDistanceSensor IR;
		public MSDistanceSensor IR2;
        //public MSSensorMUXBase IR;
        //public MSSensorMUXBase IR2;

		Motors M = new Motors();

		public Sensors() {

            colL = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C2, ColorMode.Reflection);
            colR = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C1, ColorMode.Reflection);
			LL = new MSSensorMUXBase (SensorPort.In4, MSSensorMUXPort.C3);
			
            Touch = new EV3TouchSensor(SensorPort.In3);

            IR = new MSDistanceSensor(SensorPort.In2);
            IR2 = new MSDistanceSensor(SensorPort.In1);
            //IR = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C1, MSDistanceSensor); // Infrarossi anteriore inferiore
			//IR2 = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C2, MSDistanceSensor); // Infrarossi anteriore superiore
			
		}

		/// <summary>
		/// Gets the distance value of a specified sensor.
		/// </summary>
		/// <returns>The distance in centimeters.</returns>
		/// <param name="sensor">Sensor (0: down, 1: up)</param>
		public int GetDist(sbyte sensor) {

			switch (sensor) {

			case 0:
				return IR.GetDistance ();

			case 1:
				return IR2.GetDistance ();

			default:
				return 0;

			}

		}

		public void SetSensorsMode(ColorMode Mode) {

            colL = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C2, Mode);
            colR = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C1, Mode);

		}

		/// <summary>
		/// Notices obstacles.
		/// </summary>
		public bool ObstacleNoticed() {

			if (GetDist (1) < OBSTACLE_DISTANCE * 10)
				return true;

			return false;

		}

		/// <summary>
		/// Gets the color value of a specified sensor.
		/// </summary>
		/// <returns>The color value.</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public sbyte GetColor(sbyte sensor) {

			switch (sensor) {
				
			case 0:
				return (sbyte)(colL.Read ());

			case 1:
				return (sbyte)(colR.Read ());

			default:
				return -1;

			}

		}

		/// <summary>
		/// Gets the state of a specified sensor.
		/// </summary>
		/// <returns><c>true</c>, if sensor is on black, <c>false</c> otherwise.</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public bool GetState(sbyte sensor) {

			sbyte colorValue = GetColor(sensor);

			if (colorValue <= BLACK[sensor])
				return true; // Sono sul nero, necessito di correzione

			return false; // Sono a metà, non necessito di correzione

		}

		/// <summary>
		/// Notices the green.
		/// </summary>
		/// <returns>The green.</returns>
		public int CheckGreen() {

			SetSensorsMode(ColorMode.Color);

            bool greenL = colL.Read() == (byte)Color.Green;
            bool greenR = colR.Read() == (byte)Color.Green;

			if (greenL)
				return 0;
			if (greenR)
				return 1;

			M.GoStraight(M.Speed, 0.3, true);

            greenL = colL.Read() == (byte)Color.Green;
            greenR = colR.Read() == (byte)Color.Green;

			if (greenL)
				return 0;
			if (greenR)
				return 1;

			M.GoStraight((sbyte)-M.Speed, 0.6, true);

			greenL = colL.Read() == (byte)Color.Green;
			greenR = colR.Read() == (byte)Color.Green;

			if (greenL)
				return 0;
			if (greenR)
				return 1;

            M.GoStraight(M.Speed, 0.5, true);

			return -1;

		}


		/*
		/// <summary>
		/// Checks the green.
		/// </summary>
		/// <returns>The green.</returns>
		public int CheckGreen() {

			M.Brake();

			SetSensorsMode(ColorMode.Color);

			M.GoStraight(M.Speed);

			DateTime Init = DateTime.Now;
			TimeSpan Span;

			do {
					
				if (colL.ReadColor() == Color.Green) {

					M.Brake();
					return 0;

				}

				if (colR.ReadColor() == Color.Green) {

					M.Brake();
					return 1;

				}

				Span=DateTime.Now-Init;
					
			} while(Span.Milliseconds <= 200);

			M.GoStraight((sbyte)-M.Speed);

			Init = DateTime.Now;

			do {

				if (colL.ReadColor() == Color.Green) {

					M.Brake();
					return 0;

				}

				if (colR.ReadColor() == Color.Green) {

					M.Brake();
					return 1;

				}

				Span=DateTime.Now-Init;
					
			} while(Span.Milliseconds <= 200);

			return -1;

		}
		*/


	}
}