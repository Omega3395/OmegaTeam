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
		static Claw C = new Claw();

		public static void Main(string[] args) {
            
			M.Brake();

			while (!Brain.stop) {

				Brain.lineFollower();

			}

			Thread.Sleep(2000);

			M.Off();

		}
	}

}