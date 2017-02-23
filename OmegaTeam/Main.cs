using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;
using System.ComponentModel.Design;

namespace OmegaTeam {
	class MainClass {

		//static Sensors S = new Sensors();
		static Motors M = new Motors ();

		public static void Main (string [] args) {

			M.Brake ();

			while (!Brain.stop) {

				//LcdConsole.WriteLine(S.GetColor(0) + " " + S.GetColor(1));
				//LcdConsole.WriteLine(S.GetDist(0) + "  " + S.GetDist(1));
				Brain.LineFollower ();

			}

			M.Brake ();

			LcdConsole.WriteLine ("Inizio Rescue");

			//Brain.Rescue();

			Thread.Sleep (2000);

			M.Off ();

		}
	}
}