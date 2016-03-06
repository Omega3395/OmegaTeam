﻿using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display; // Da rimuovere
using MonoBrickFirmware.UserInput; // Da rimuovere

namespace OmegaTeam
{
	class MainClass
	{
		
		public static Sensors S = new Sensors ();
		public static Motors M = new Motors ();
		public static Pinza P = new Pinza ();


		public static void Main (string[] args)
		{

			/*while (!Brain.stop) {

				LcdConsole.WriteLine ("" + S.getColor (0) + "  " + S.getColor (1));
				Brain.lineFollower ();

			}*/

			P.chiudi ();
			Thread.Sleep (2000);
			P.apri ();

			//Brain.rescue ();

			M.Off ();

		}
	}

}