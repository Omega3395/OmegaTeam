using MonoBrickFirmware.Sensors;
using System.Threading;

namespace OmegaTeam {
	public class S {

		//################################################################################
		//################################################################################

		const short OBSTACLE_DISTANCE = 7; // Distanza a cui si riconosce un ostacolo, in cm
		public static sbyte [] WHITE = { 60, 50 }; //60 50
		public static sbyte [] BLACK = { 28, 28 }; //28 28
		public static sbyte [] BLACK_OBSTACLE = { 21, 21 }; //21 21

		//################################################################################
		//################################################################################

		public static MSSensorMUXBase colL;
		public static MSSensorMUXBase colR;

		public static MSSensorMUXBase gyro;

		public static EV3UltrasonicSensor IR;
		public static EV3UltrasonicSensor IR2;
		public static EV3UltrasonicSensor IR3;

		public S () {

			colL = new MSSensorMUXBase (SensorPort.In4, MSSensorMUXPort.C1, ColorMode.Reflection);
			colR = new MSSensorMUXBase (SensorPort.In4, MSSensorMUXPort.C2, ColorMode.Reflection);

			IR = new EV3UltrasonicSensor (SensorPort.In1, UltraSonicMode.Centimeter);
			IR2 = new EV3UltrasonicSensor (SensorPort.In2, UltraSonicMode.Centimeter);
			IR3 = new EV3UltrasonicSensor (SensorPort.In3, UltraSonicMode.Centimeter);

		}

		/// <summary>
		/// Gets the distance value of a specified sensor.
		/// </summary>
		/// <returns>The distance in centimeters.</returns>
		/// <param name="sensor">Sensor (1: left, 2: right, 3: center)</param>
		public static int GetDist (sbyte sensor) {

			switch (sensor) {

			case 1:
				return IR.Read ();

			case 2:
				return IR2.Read ();

			case 3:
				return IR3.Read ();

			default:
				return 0;

			}

		}

		public static void SetSensorsMode (ColorMode Mode) {

			//colL = new EV3ColorSensor (SensorPort.In2, Mode);
			//colR = new EV3ColorSensor (SensorPort.In1, Mode);

			colL = new MSSensorMUXBase (SensorPort.In4, MSSensorMUXPort.C1, Mode);
			colR = new MSSensorMUXBase (SensorPort.In4, MSSensorMUXPort.C2, Mode);

		}

		/// <summary>
		/// Notices obstacles.
		/// </summary>
		public static bool ObstacleNoticed () {

			if (GetDist (3) < OBSTACLE_DISTANCE * 10)
				return true;

			return false;

			/*if (Touch.IsPressed())
				return true;
			
			return false;*/

		}

		/// <summary>
		/// Gets the color value of a specified sensor.
		/// </summary>
		/// <returns>The color value.</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public static sbyte GetColor (sbyte sensor) {

			switch (sensor) {

			case 0:
				return (sbyte)(colL.Read ());

			case 1:
				return (sbyte)(colR.Read ());

			default:
				return -1;

			}

		}

		public static byte GetAngle () {
			return gyro.Read ();
		}

		/// <summary>
		/// Gets the state of a specified sensor.
		/// </summary>
		/// <returns><c>true</c>, if sensor is on black, <c>false</c> otherwise.</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public static bool GetState (sbyte sensor, bool obstacle = false) {

			if (!obstacle) { // Caso qualunque

				sbyte colorValue = GetColor (sensor);

				if (colorValue <= BLACK [sensor])
					return true; // Sono sul nero

				return false; // Sono a metà, non mi si può considerare nero

			} else { // Sto cercando il nero dopo l'ostacolo

				sbyte colorValue = GetColor (sensor);

				// Voglio essere sicuro di essere bene sul nero, in modo da correggermi in modo appropriato
				if (colorValue <= BLACK_OBSTACLE [sensor])
					return true;

				return false;

			}

		}

	}
}