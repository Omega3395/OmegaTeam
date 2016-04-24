using System;
using System.Threading;

namespace OmegaTeam
{
    public class Claw
    {

        private const int TachoP = 700;
        private bool isopen;
        private bool isclose;

        private static Motors M = new Motors();

        public Claw() {

            isopen = false;
            isclose = true;

        }

        private bool isOpen() {

            return isopen;

        }

        private bool isClose() {

            return isclose;

        }

		/// <summary>
		/// Open the claw.
		/// </summary>
        public void Open() {

            bool p = true;

            M.ResetTacho();

            M.motP.SetSpeed(-50);

            do {
                if (M.motP.GetTachoCount() <= -TachoP) {

                    M.motP.Brake();
                    p = false;

                }

            } while(p);

            isopen = true;
            isclose = false;
        }

		/// <summary>
		/// Close the claw.
		/// </summary>
        public void Close() {
            bool p = true;

            M.ResetTacho();

            M.motP.SetSpeed(50);

            do {
                if (M.motP.GetTachoCount() >= TachoP) {

                    M.motP.Brake();
                    p = false;

                }

            } while (p);

            isopen = false;
            isclose = true;
        }
    }
}