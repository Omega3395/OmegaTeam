﻿using System;
using System.Threading;
using System.Linq;

using MonoBrickFirmware;
using MonoBrickFirmware.Sensors;

namespace OmegaTeam
{
    public class Sensors
    {

        //################################################################################
        //################################################################################

        private const short OBSTACLE_DISTANCE = 5;
        // Distanza a cui si riconosce un ostacolo, in cm

        //################################################################################
        //################################################################################

        public EV3ColorSensor colL;
        public EV3ColorSensor colR;
        public MSSensorMUXBase IR;
        public MSSensorMUXBase IR2;
        public MSSensorMUXBase IR3;
        public EV3TouchSensor Touch;

        public Sensors() {

            colL = new EV3ColorSensor(SensorPort.In1, ColorMode.Reflection);
            colR = new EV3ColorSensor(SensorPort.In2, ColorMode.Reflection);

            IR = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C1, IRMode.Proximity); // Infrarossi anteriore inferiore
            IR2 = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C2, IRMode.Proximity); // Infrarossi anteriore inferiore
            IR3 = new MSSensorMUXBase(SensorPort.In4, MSSensorMUXPort.C3, IRMode.Proximity); // Infrarossi anteriore inferiore

            Touch = new EV3TouchSensor(SensorPort.In3);

        }

        public int getDist(sbyte sensor) {

            switch (sensor) {

                case 1:
                    return IR.Read();

                case 2:
                    return IR2.Read();

                case 3:
                    return IR3.Read();

                default:
                    return 0;

            }

        }

        public bool obstacle() {

            if (getDist(1) < OBSTACLE_DISTANCE)
                return true;

            return false;


        }

        public bool getMaxColor() { // Restituisci il sensore più sul bianco

            if (getColor(0) > getColor(1)) {
                return false;
            }
            else {
                return true;
            }

        }

        public sbyte getColor(sbyte sensor) {

            if (sensor == 0) {
                return (sbyte)(colL.Read());
            }

            if (sensor == 1) {
                return (sbyte)(colR.Read());
            }

            return 0;
        }

        public bool[] isGreen() {

            Thread.Sleep(200); // Prenditi un pò di tempo per analizzare il colore... Abbondiamo con gli sleep

            colL.Mode = ColorMode.Color;
            colR.Mode = ColorMode.Color;

            bool greenL = colL.ReadColor() == Color.Green;
            bool greenR = colR.ReadColor() == Color.Green;

            Thread.Sleep(200);

            colL.Mode = ColorMode.Reflection;
            colR.Mode = ColorMode.Reflection;

            bool[] green = { greenL, greenR };

            return green;

        }
			

    }
}

