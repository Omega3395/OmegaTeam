using System;
using System.Threading;

using MonoBrickFirmware;
using MonoBrickFirmware.Display;

// Da rimuovere
using MonoBrickFirmware.UserInput;

// Da rimuovere

namespace OmegaTeam
{
	class MainClass
	{
		
		static Sensors S = new Sensors();
		static Motors M = new Motors();
		//static Claw C = new Claw();

		public static void Main(string[] args) {
            
			M.Brake();

			while (!S.Touch.IsPressed ()) {

				//LcdConsole.WriteLine(S.GetColor(0) + " " + S.GetColor(1));
				//LcdConsole.WriteLine (S.GetDist (0) + "  " + S.GetDist (1));
				Brain.lineFollower();

			}

            Thread.Sleep(2000);

            M.Off();

		}
	}

}