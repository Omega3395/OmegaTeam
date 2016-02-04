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

		public const short OBSTACLE_DISTANCE = 5; // Distanza a cui si riconosce un ostacolo, in cm

		//################################################################################
		//################################################################################

		public static EV3ColorSensor colL;
		public static EV3UltrasonicSensor sonic;
		public static EV3ColorSensor colR;
		public static EV3TouchSensor touch;

		public Sensors() {

			colL = new EV3ColorSensor (SensorPort.In1, ColorMode.Reflection);
			sonic = new EV3UltrasonicSensor (SensorPort.In2, UltraSonicMode.Centimeter);
			touch = new EV3TouchSensor (SensorPort.In3);
			colR = new EV3ColorSensor (SensorPort.In4, ColorMode.Reflection);

		}

		public static int getDist() {

			return sonic.Read ();

		}

		public static bool obstacle() {

			if (getDist() < OBSTACLE_DISTANCE * 10)
				return true;

			return false;


		}

		public static bool getMaxColor() { // Restituisci il sensore più sul bianco

			int value = getColors ().ToList ().IndexOf (getColors ().Max ());

			if (value == 0) {
				return false;
			} else {
				return true;
			}

		}

		public static sbyte[] getColors(bool getMaxValue=false) {

			sbyte[] colors = new sbyte[] { (sbyte)colL.Read (), (sbyte)colR.Read () }; // Invia i rilevamenti dei due sensori
			return colors;

		}

		public static bool[] isGreen() {

			Thread.Sleep (500); // Prenditi un pò di tempo per analizzare il colore... Abbondiamo con gli sleep

			colL.Mode = ColorMode.Color;
			colR.Mode = ColorMode.Color;

			bool greenL = colL.ReadColor () == Color.Green;
			bool greenR = colR.ReadColor () == Color.Green;

			colL.Mode = ColorMode.Reflection;
			colR.Mode = ColorMode.Reflection;

			Thread.Sleep (400);

			bool[] green = { greenL, greenR };

			return green;

		}
			

	}
}

