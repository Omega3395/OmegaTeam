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
		public static sbyte[] BLACK = { 25, 20 };
		// Valore per cui viene attivato "nero"
		public static sbyte[] WHITE = { 90, 70 };

		//################################################################################
		//################################################################################

		public EV3ColorSensor colL;
		public EV3ColorSensor colR;
		public EV3TouchSensor Touch;
		public MSSensorMUXBase IR;
		public MSSensorMUXBase IR2;

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


		/// <summary>
		/// Notices the green.
		/// </summary>
		/// <returns>The array with green values (true if green, false if not)</returns>
		public bool[] isGreen() {

			Thread.Sleep(200); // Prenditi un pò di tempo per analizzare il colore... Abbondiamo con gli sleep

			colL.Mode = ColorMode.Color;
			colR.Mode = ColorMode.Color;

			bool greenL = colL.ReadColor() == Color.Green;
			bool greenR = colR.ReadColor() == Color.Green;

			Thread.Sleep(200);

			colL.Mode = ColorMode.Reflection;
			colR.Mode = ColorMode.Reflection;

			bool[] green = { greenL, greenR };

			return green;

		}
			

	}
}

