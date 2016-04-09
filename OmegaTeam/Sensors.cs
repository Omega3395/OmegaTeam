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

		private const short OBSTACLE_DISTANCE = 5;
		// Distanza a cui si riconosce un ostacolo, in cm
		public static sbyte[] BLACK = { 60, 60 };
		//25 20
		// Valore per cui viene attivato "nero"
		public static sbyte[] WHITE = { 80, 80 };

		//################################################################################
		//################################################################################

		public EV3ColorSensor colL;
		public EV3ColorSensor colR;
		public EV3TouchSensor Touch;
		public MSSensorMUXBase IR;
		public MSSensorMUXBase IR2;

		Motors M = new Motors();

		public Sensors() {

			colL = new EV3ColorSensor(SensorPort.In1, ColorMode.Reflection);
			colR = new EV3ColorSensor(SensorPort.In2, ColorMode.Reflection);

			Touch = new EV3TouchSensor(SensorPort.In3);

			IR = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C1, IRMode.Proximity); // Infrarossi anteriore inferiore
			IR2 = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C2, IRMode.Proximity); // Infrarossi anteriore superiore

		}

		/// <summary>
		/// Gets the distance value of a specified sensor.
		/// </summary>
		/// <returns>The distance in centimeters.</returns>
		/// <param name="sensor">Sensor (0: down, 1: up)</param>
		public int getDist(sbyte sensor) {

			switch (sensor) {

				case 1:
					return IR.Read();

				case 2:
					return IR2.Read();

				default:
					return 0;

			}

		}

		public void setSensorsMode(ColorMode Mode) {

			colL.Mode = Mode;
			colR.Mode = Mode;

		}

		/// <summary>
		/// Notices obstacles.
		/// </summary>
		public bool obstacleNoticed() {

			if (getDist(0) < OBSTACLE_DISTANCE)
				return true;

			return false;

		}

		/// <summary>
		/// Gets the color value of a specified sensor.
		/// </summary>
		/// <returns>The color value.</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public sbyte getColor(sbyte sensor) {

			switch (sensor) {
				
				case 0:
					return (sbyte)(colL.Read());

				case 1:
					return (sbyte)(colR.Read());

				default:
					return -1;

			}

		}

		/// <summary>
		/// Gets the state of a specified sensor.
		/// </summary>
		/// <returns><c>true</c>, if sensor is on black, <c>false</c> otherwise.</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public bool getState(sbyte sensor) {

			sbyte colorValue = getColor(sensor);

			if (colorValue <= BLACK[sensor])
				return true; // Sono sul nero, necessito di correzione

			return false; // Sono a metà, non necessito di correzione

		}

		/*
		/// <summary>
		/// Notices the green.
		/// </summary>
		/// <returns>The array with green values (true if green, false if not)</returns>
		public int checkGreen() {

			setSensorsMode(ColorMode.Color);

			bool greenL = colL.ReadColor() == Color.Green;
			bool greenR = colR.ReadColor() == Color.Green;

			if (greenL)
				return 0;
			if (greenR)
				return 1;

			M.goStraight(M.Speed, 0.2, true);

			greenL = colL.ReadColor() == Color.Green;
			greenR = colR.ReadColor() == Color.Green;

			if (greenL)
				return 0;
			if (greenR)
				return 1;

			M.goStraight((sbyte)-M.Speed, 0.4, true);

			greenL = colL.ReadColor() == Color.Green;
			greenR = colR.ReadColor() == Color.Green;

			if (greenL)
				return 0;
			if (greenR)
				return 1;

			M.goStraight(M.Speed, 0.2, true);

			return -1;

		}
		*/



		/// <summary>
		/// Checks the green.
		/// </summary>
		/// <returns>The green.</returns>
		public int checkGreen() {

			M.Brake();

			setSensorsMode(ColorMode.Color);

			M.goStraight(M.Speed);

			DateTime Init = DateTime.Now;

			do {
					
				if (colL.ReadColor() == Color.Green) {

					M.Brake();
					return 0;

				}

				if (colR.ReadColor() == Color.Green) {

					M.Brake();
					return 1;

				}
					
			} while(DateTime.Now.Millisecond - Init.Millisecond <= 200);

			M.goStraight((sbyte)-M.Speed);

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
					
			} while(DateTime.Now.Millisecond - Init.Millisecond <= 200);

			return -1;

		}



	}
}

