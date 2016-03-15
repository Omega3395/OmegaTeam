using System;
using System.Threading;

namespace OmegaTeam {
    public class Pinza {

        private const int TachoP = 425;
        private bool isopen;
        private bool isclose;

        private static Motors M = new Motors();

        public Pinza() {

            isopen = false;
            isclose = true;

        }

        private bool isOpen() {

            return isopen;

        }

        private bool isClose() {

            return isclose;

        }

        public void apri() {

            bool p = true;

            M.resetTacho();

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

        public void chiudi() {
            bool p = true;

            M.resetTacho();

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