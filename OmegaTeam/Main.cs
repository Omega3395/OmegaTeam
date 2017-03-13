using System;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace OmegaTeam {
	class MainClass {

		static readonly Sensors S = new Sensors ();
		static readonly Motors M = new Motors ();
		public static int Angle;
		static readonly ButtonEvents Buttons = new ButtonEvents ();

		public static void Main (string [] args) {

			M.Brake ();

			DateTime Initial = DateTime.Now;

			Angle = S.GetAngle ();

			LcdConsole.WriteLine ("" + Angle);

			while (!Brain.stop) {

				//LcdConsole.WriteLine (S.GetColor (0) + " " + S.GetColor (1) + "     " + S.GetAngle ());
				//LcdConsole.WriteLine ((S.GetAngle () - Angle).ToString ());

				if (DateTime.Now.Second % 5 == 0) { Angle = S.GetAngle (); LcdConsole.WriteLine ("RESET"); }

				Brain.LineFollower ();

				Buttons.EscapePressed += () => {
					Brain.stop = true;
				};

			}

			M.Brake ();

			Brain.Rescue ();

			Thread.Sleep (2000);

			M.Off ();

		}
	}
}