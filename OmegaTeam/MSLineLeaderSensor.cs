using System;

namespace MonoBrickFirmware.Sensors
{

	/// <summary>
	/// Class for Mindsensors LineLeader Sensor
	/// </summary>
	public class MSLineLeaderSensor :I2CSensor
	{
		private static byte PowerOnCommand = (byte)'P';
		private static byte PowerOffCommand = (byte)'D';
		private static byte CalibrateWhiteCommand = (byte)'W';
		private static byte CalibrateBlackCommand = (byte)'B';

		private enum LineLeaderRegister : byte {
			Command = 0x41,
			LightCalibrata = 0x49,
			LightRaw = 0x74,
			Steering = 0x42,
			Kp = 0x46,
			Ki = 0x47,
			Kd = 0x48}

		;


		public MSLineLeaderSensor(SensorPort port)
			: base(port, (byte)0x02, I2CMode.LowSpeed9V) {	
			base.Initialise();
		}

		/// <summary>
		/// Gets calibrated light values.
		/// </summary>
		public byte[] GetLight() {
			return ReadRegister((byte)LineLeaderRegister.LightCalibrata, 8);
		}

		/// <summary>
		/// Gets the steering value calculated by the PID integrated software.
		/// </summary>
		public int GetCorrection() {
			return (int)BitConverter.ToInt16(ReadRegister((byte)LineLeaderRegister.Steering, 2), 0);
		}

		/// <summary>
		/// Gets the line trace values.
		/// </summary>
		public override string ReadAsString() {

			byte[] a = GetLight();
			byte[] c = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

			for (int i = 0; i < 8; i++)
				if (a[i] <= 15)
					c[i] = 1;
				else
					c[i] = 0;

			return ("" + c[0] + c[1] + c[2] + c[3] + c[4] + c[5] + c[6] + c[7]);
		}

		/// <summary>
		/// Gets raw light values.
		/// </summary>
		public Int16[] GetLightRaw() {
			Int16[] arr = new Int16[8];
			for (byte i = 0; i < 8; i++) {
				byte address = (byte)LineLeaderRegister.LightRaw;
				address += (byte)(i * 2);
				arr[i] = BitConverter.ToInt16(ReadRegister(address, 2), 0);
			}
			return arr;
		}

		/// <summary>
		/// Powers the sensor on.
		/// </summary>
		public void PowerOn() {
			WriteRegister((byte)LineLeaderRegister.Command, (byte)PowerOnCommand);
		}

		/// <summary>
		/// Powers the sensor off.
		/// </summary>
		public void PowerOff() {
			WriteRegister((byte)LineLeaderRegister.Command, (byte)PowerOffCommand);

		}

		/// <summary>
		/// Calibrates white.
		/// </summary>
		public void WhiteCal() {
			WriteRegister((byte)LineLeaderRegister.Command, (byte)CalibrateWhiteCommand);

		}

		/// <summary>
		/// Calibrates black.
		/// </summary>
		public void BlackCal() {
			WriteRegister((byte)LineLeaderRegister.Command, (byte)CalibrateBlackCommand);

		}

		public override string GetSensorName() {
			return "Mindsensors " + " LineLeader";
		}

		public override int NumberOfModes() {
			return 1;
		}

		public override void SelectNextMode() {
			return;
		}

		public override void SelectPreviousMode() {
			return;
		}

		public override string SelectedMode() {
			return ("LineLeader Mode");
		}
	}
}

