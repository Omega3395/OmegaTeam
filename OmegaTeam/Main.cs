using System.Threading;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display;

namespace OmegaTeam {
	class MainClass {

		static readonly ButtonEvents Buttons = new ButtonEvents ();

		public static void Main (string [] args) {

			M.Brake ();

			while (!Brain.stop) {

				//LcdConsole.WriteLine (S.GetColor (0) + " " + S.GetColor (1));

				Brain.LineFollower ();
                
				Buttons.EscapePressed += () => {
					Brain.stop = true;
				};
			}

			M.Brake ();

            //Brain.Rescue ();
            
            M.Off ();

		}
	}
}