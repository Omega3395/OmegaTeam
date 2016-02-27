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

		private const short OBSTACLE_DISTANCE = 5; // Distanza a cui si riconosce un ostacolo, in cm

		//################################################################################
		//################################################################################

		public static EV3ColorSensor colL;
		public static EV3UltrasonicSensor sonic;
		public static EV3IRSensor IR;
		public static EV3ColorSensor colR;

		public Sensors() {

			colL = new EV3ColorSensor (SensorPort.In1, ColorMode.Reflection);
			sonic = new EV3UltrasonicSensor (SensorPort.In2, UltraSonicMode.Centimeter);
			IR = new EV3IRSensor (SensorPort.In3, IRMode.Proximity);
			colR = new EV3ColorSensor (SensorPort.In4, ColorMode.Reflection);

		}

		public int getDist(bool infrared=false) {

			if (infrared)
				return IR.ReadDistance ();
			return sonic.Read ();

		}

		public bool obstacle() {

			if (getDist() < OBSTACLE_DISTANCE * 10)
				return true;

			return false;


		}

		public bool getMaxColor() { // Restituisci il sensore più sul bianco

			int value = getColors ().ToList ().IndexOf (getColors ().Max ());

			if (value == 0) {
				return false;
			} else {
				return true;
			}

		}

		public sbyte[] getColors() {

			sbyte[] colors = new sbyte[] { (sbyte)colL.Read (), (sbyte)colR.Read () }; // Invia i rilevamenti dei due sensori
			return colors;

		}

		public bool[] isGreen() {

			Thread.Sleep (200); // Prenditi un pò di tempo per analizzare il colore... Abbondiamo con gli sleep

			colL.Mode = ColorMode.Color;
			colR.Mode = ColorMode.Color;

			bool greenL = colL.ReadColor () == Color.Green;
			bool greenR = colR.ReadColor () == Color.Green;

			Thread.Sleep (200);

			colL.Mode = ColorMode.Reflection;
			colR.Mode = ColorMode.Reflection;

			bool silverL = colL.Read () >= 90;
			bool silverR = colR.Read () >= 90;

			bool[] green = { greenL, greenR, silverL && silverR };

			return green;

		}
			

	}
}

