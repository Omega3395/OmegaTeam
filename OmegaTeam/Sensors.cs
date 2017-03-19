using MonoBrickFirmware.Sensors;
using System.Threading;

namespace OmegaTeam {
	public class Sensors {

		//################################################################################
		//################################################################################

		const short OBSTACLE_DISTANCE = 7; // Distanza a cui si riconosce un ostacolo, in cm
		public static sbyte [] WHITE = { 60, 50 }; //60 50
		public static sbyte [] BLACK = { 28, 28 }; //28 28
		public static sbyte [] BLACK_OBSTACLE = { 21, 21 }; //21 21

		//################################################################################
		//################################################################################

		public MSSensorMUXBase colL;
		public MSSensorMUXBase colR;

		public MSSensorMUXBase gyro;

		public EV3UltrasonicSensor IR;
		public EV3UltrasonicSensor IR2;
		public EV3UltrasonicSensor IR3;

		Motors M = new Motors ();

		public Sensors () {

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
		public int GetDist (sbyte sensor) {

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

		public void SetSensorsMode (ColorMode Mode) {

			//colL = new EV3ColorSensor (SensorPort.In2, Mode);
			//colR = new EV3ColorSensor (SensorPort.In1, Mode);

			colL = new MSSensorMUXBase (SensorPort.In4, MSSensorMUXPort.C1, Mode);
			colR = new MSSensorMUXBase (SensorPort.In4, MSSensorMUXPort.C2, Mode);

		}

		/// <summary>
		/// Notices obstacles.
		/// </summary>
		public bool ObstacleNoticed () {

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
		public sbyte GetColor (sbyte sensor) {

			switch (sensor) {

			case 0:
				return (sbyte)(colL.Read ());

			case 1:
				return (sbyte)(colR.Read ());

			default:
				return -1;

			}

		}

		public byte GetAngle () {
			return gyro.Read ();
		}

		/// <summary>
		/// Gets the state of a specified sensor.
		/// </summary>
		/// <returns><c>true</c>, if sensor is on black, <c>false</c> otherwise.</returns>
		/// <param name="sensor">Sensor (0: left, 1: right)</param>
		public bool GetState (sbyte sensor, bool obstacle = false) {
			if (!obstacle) {

				sbyte colorValue = GetColor (sensor);

				if (colorValue <= BLACK [sensor])
					return true; // Sono sul nero, necessito di correzione

				return false; // Sono a metà, non necessito di correzione
			} else {

				sbyte colorValue = GetColor (sensor);

				if (colorValue <= BLACK_OBSTACLE [sensor])
					return true;

				return false;

			}

		}

		/// <summary>
		/// Notices the green.
		/// </summary>
		/// <returns>The green.</returns>
		public int CheckGreen () {

			SetSensorsMode (ColorMode.Color);

			//Brain.Angles.Add (Math.Abs (Math.Abs (GetAngle ()) - Math.Abs (MainClass.Angle)));
			bool greenL = colL.Read () == (byte)Color.Green;
			bool greenR = colR.Read () == (byte)Color.Green;
			bool bb = ((colL.Read () == colR.Read ()) && (colL.Read () == (byte)Color.Black));

			if (greenL)
				return 0;
			if (greenR)
				return 1;

			M.GoStraight (M.Speed, 0.3, true);

			//Brain.Angles.Add (Math.Abs (Math.Abs (GetAngle ()) - Math.Abs (MainClass.Angle)));
			greenL = colL.Read () == (byte)Color.Green;
			greenR = colR.Read () == (byte)Color.Green;
			bool bb2 = ((colL.Read () == colR.Read ()) && (colL.Read () == (byte)Color.White));

			if (greenL)
				return 0;
			if (greenR)
				return 1;

			M.GoStraight ((sbyte)-M.Speed, 0.6, true);

			//Brain.Angles.Add (Math.Abs (Math.Abs (GetAngle ()) - Math.Abs (MainClass.Angle)));
			greenL = colL.Read () == (byte)Color.Green;
			greenR = colR.Read () == (byte)Color.Green;
			bool bb3 = ((colL.Read () == colR.Read ()) && (colL.Read () == (byte)Color.White));

			if (greenL)
				return 0;
			if (greenR)
				return 1;

			M.GoStraight (M.Speed, 0.42, true);

			if (bb && bb2 && bb3)
				return -2;
			return -1;

		}

		public int CheckGreen2 () {

			SetSensorsMode (ColorMode.Color);
			bool greenL, greenR, bb = false, bb2 = false, bb3 = false;

			byte c1 = colL.Read ();
			byte c2 = colR.Read ();
			bb = (c1 == c2 && c1 == (byte)Color.Black);

			//New cicle function
			M.SetSpeed (M.Speed, M.Speed);
			for (int i = 0; i < 10; i++) {
				c1 = colL.Read ();
				c2 = colR.Read ();
				greenL = c1 == (byte)Color.Green;
				greenR = c2 == (byte)Color.Green;
				bb2 = (c1 == c2 && c1 == (byte)Color.White);
				if (greenL) return 0;
				if (greenR) return 1;
				Thread.Sleep (10);
			}
			M.Brake ();

			M.SetSpeed ((sbyte)-M.Speed, (sbyte)-M.Speed);
			for (int i = 0; i < 20; i++) {
				c1 = colL.Read ();
				c2 = colR.Read ();
				greenL = c1 == (byte)Color.Green;
				greenR = c2 == (byte)Color.Green;
				bb3 = (c1 == c2 && c1 == (byte)Color.White);
				if (greenL) return 0;
				if (greenR) return 1;
				Thread.Sleep (10);
			}
			M.Brake ();

			M.SetSpeed (M.Speed, M.Speed);
			for (int i = 0; i < 15; i++) {
				c1 = colL.Read ();
				c2 = colR.Read ();
				Thread.Sleep (10);
			}
			M.Brake ();

			if (bb && bb2 && bb3)
				return -2;
			return -1;

		}

	}
}