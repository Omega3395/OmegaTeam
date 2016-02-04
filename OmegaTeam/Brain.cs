using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace OmegaTeam
{
	public class Brain
	{

		//################################################################################
		//################################################################################

		private const sbyte CORRECTION = (sbyte)1;
		private static sbyte[] BLACK = { 20, 20 }; // Valore per cui viene attivato "nero"
		private static sbyte[] WHITE = { 60, 60 }; // Valore per cui viene attivato "bianco"

		//################################################################################
		//################################################################################

		public static bool stop = false;

		ButtonEvents buts = new ButtonEvents();

		public Brain () {
		}
			
		private static bool state(sbyte sensor) {

			sbyte white = BLACK [sensor];
			sbyte black = WHITE [sensor];

			sbyte colorValue = Sensors.getColors () [sensor];

			if (colorValue >= white) {
			
				return false; // Sono sul bianco o quasi
			
			}

			if (colorValue <= black) {

				return true; // Sono sul nero o sulla soglia del nero

			}

			return false; // Sono a metà, non mi correggo

		}

		public static sbyte correction(sbyte sensor) {

			return (sbyte)(Math.Abs (WHITE [sensor] - Sensors.getColors () [sensor]) * 0.05); // Formula per calcolare la correzione di posizione

		}

		public void print(string a) {

			LcdConsole.WriteLine (a);

		}

		public static bool[][] verify() {
			
			Motors.setSpeed (-10, 10, 0.5);

			bool[] photoLeft = { state (0), state (1) };

			Motors.setSpeed (10, -10, 1);

			bool[] photoRight = { state (0), state (1) };

			Motors.setSpeed (-10, 10, 0.5);

			Motors.Brake ();

			bool[][] values = { photoLeft, photoRight };

			return values;


		}

		public void lineFollower() {

			bool CL = state (0); // Bianco o nero?
			bool CR = state (1);

			if (!CL && !CR) { // Bianco Bianco

				print ("Bianco Bianco");
				Motors.goStraight ();

			}

			if (CL && !CR) { //Nero Bianco

				print ("Nero Bianco");
				Motors.turnLeft ();

			}

			if (!CL && CR) { //Bianco Nero

				print ("Bianco Nero");
				Motors.turnRight ();

			}

			if (Sensors.obstacle ()) { // Attenzione... Ostacolo rilevato!

				print ("Ostacolo!");
				Motors.avoidObstacle ();

			}

			if (CL && CR) { // Nero Nero, forse Verde?

				Motors.Brake ();

				bool[] green = Sensors.isGreen ();

				bool GL = green [0];
				bool GR = green [1];

				if (GL) { // Verde a sinistra

					print ("Verde sinistra");
					Motors.goStraight (10, 0.2);
					Motors.setSpeed (-2, 20, 0.5);

				}

				if (GR) { // Verde a destra

					print ("Verde sinistra");
					Motors.goStraight (10, 0.2);
					Motors.setSpeed (20, -2, 0.5);

				}

				if (!GL && !GR) { // Nero nero

					Motors.Brake ();
					Motors.goStraight (-10, 0.2);

					if (Sensors.getMaxColor ()) { // Quale sensore è più sul bianco? 0 (sinistra) o 1 (destra)
						Motors.turnLeft (); // Il sensore destra è più sul bianco
					} else {
						Motors.turnRight (); // Il sensore sinistra è più sul bianco
					}

				}
			}


			buts.EscapePressed += () => {
				
				LcdConsole.WriteLine ("FINE");
				stop = true;
				Motors.Off ();

			};

		}
			

	}
}

