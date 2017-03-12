using System.Threading;
using MonoBrickFirmware.Display;

namespace OmegaTeam {
	class MainClass {

		static readonly Sensors S = new Sensors ();
		static readonly Motors M = new Motors ();
		public static int Angle = S.GetAngle ();


		public static void Main (string [] args) {

			M.Brake ();

			LcdConsole.WriteLine (Angle + "");

			Thread.Sleep (1000);

			while (!Brain.stop) {

				//LcdConsole.WriteLine (S.GetColor (0) + " " + S.GetColor (1) + "     " + S.GetAngle ());
				//LcdConsole.WriteLine(S.GetDist(0) + "  " + S.GetDist(1));
				Brain.LineFollower ();

			}

			M.Brake ();

			Brain.Rescue ();

			Thread.Sleep (2000);

			M.Off ();

		}
	}
}