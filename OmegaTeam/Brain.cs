﻿using System;
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

		private static sbyte[] BLACK = { 20, 20 }; // Valore per cui viene attivato "nero"
		private static sbyte[] WHITE = { 60, 60 }; // Valore per cui viene attivato "bianco"
		public static bool stop = false;

		//################################################################################
		//################################################################################

		private static Sensors S = new Sensors ();
		private static Motors M = new Motors ();
		private static Pinza P = new Pinza ();
		private static Salvataggio Salvataggio = new Salvataggio ();

		private static ButtonEvents Buttons = new ButtonEvents();

		public Brain () {
		}
			
		private static bool state(sbyte sensor) {

			sbyte white = BLACK [sensor];
			sbyte black = WHITE [sensor];
			
			sbyte colorValue = S.getColor (sensor);

			if (colorValue >= white) {
			
				return false; // Sono sul bianco o quasi
			
			}

			if (colorValue <= black) {

				return true; // Sono sul nero o sulla soglia del nero

			}

			return false; // Sono a metà, risulto bianco

		}

		public static sbyte correction(sbyte sensor) {

			return (sbyte)(Math.Abs (WHITE [sensor] - S.getColor (sensor)) * 0.05); // Formula per calcolare la correzione di posizione

		}

		public static void print(string a) {
			 
			LcdConsole.WriteLine (a);

		}

		public static void lineFollower() {

			bool CL = state (0); // Bianco o nero?
			bool CR = state (1);
			bool SILVER = (S.getColor (0) >= 90 && S.getColor (1) >= 90);

			if (!CL && !CR) { // Bianco Bianco

				M.goStraight (15);

			}

			if (CL && !CR) { //Nero Bianco

				M.turnLeft ();

			}

			if (!CL && CR) { //Bianco Nero

				M.turnRight ();

			}

			if (S.obstacle ()) { // Attenzione... Ostacolo rilevato!

				print ("Ostacolo!");
				M.avoidObstacle ();

			}

			if (SILVER) {

				stop = true;

			}

			if (CL && CR) { // Nero Nero, forse Verde?

				M.Brake ();

				bool[] green = S.isGreen ();

				bool GL = green [0];
				bool GR = green [1];

				if (GL) { // Verde a sinistra

					print ("Verde sinistra");
					M.goStraight (10, 0, 0.2);
					M.setSpeed (-2, 20, 0.8);

				}

				if (GR) { // Verde a destra

					print ("Verde destra");
					M.goStraight (10, 0, 0.2);
					M.setSpeed (20, -2, 0.8);

				}

				if (!GL && !GR) { // Nero nero

					M.Brake ();

					if (S.getMaxColor ()) { // Quale sensore è più sul bianco? 0 (sinistra) o 1 (destra)
						M.turnLeft (0.2); // Il sensore destra è più sul bianco
					} else {
						M.turnRight (0.2); // Il sensore sinistra è più sul bianco
					}

				}

			}


			Buttons.EscapePressed += () => {

				stop=true;
				LcdConsole.WriteLine ("Fine seguilinea");

			};

		}

		public static void rescue () {

		}

	}
}

