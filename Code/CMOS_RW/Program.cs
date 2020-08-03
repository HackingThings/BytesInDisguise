using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices;

namespace CMOS_RW
{
    class Program
    {
        #region imports from asmedia dll
        [DllImport("asmiodll", CallingConvention = CallingConvention.StdCall, EntryPoint = "_LoadAsmIODriver@0", ExactSpelling = true)]
        public static extern UInt32 LoadAsmIODriver();

        [DllImport("asmiodll", CallingConvention = CallingConvention.StdCall, EntryPoint = "_UnloadAsmIODriver@0", ExactSpelling = true)]
        public static extern UInt32 UnloadAsmIODriver();

        [DllImport("asmiodll", CallingConvention = CallingConvention.StdCall, EntryPoint = "_ReadPortByte@4", ExactSpelling = true)]
        public static extern byte ReadPortByte(UInt16 port);

        [DllImport("asmiodll", CallingConvention = CallingConvention.StdCall, EntryPoint = "_ReadPortLong@4", ExactSpelling = true)]
        public static extern UInt32 ReadPortLong(UInt16 port);

        [DllImport("asmiodll", CallingConvention = CallingConvention.StdCall, EntryPoint = "_WritePortByte@8", ExactSpelling = true)]
        public static extern byte WritePortByte(UInt16 port, byte data);

        [DllImport("asmiodll", CallingConvention = CallingConvention.StdCall, EntryPoint = "_WritePortLong@8", ExactSpelling = true)]
        public static extern byte WritePortLong(UInt16 port, UInt32 data);

        #endregion imports from asmedia dll

        static UInt16 CMOS_ADDR_PORT_LOW = 0x70;
        static UInt16 CMOS_DATA_PORT_LOW = 0x71;
        static UInt16 CMOS_ADDR_PORT_HIGH = 0x72;
        static UInt16 CMOS_DATA_PORT_HIGH = 0x73;

       /// <summary>
       /// This method returns the location of the longest sequence of ByteToFind in a Byte array.
       /// This is useful when you are looking for unused image location to hide your data in.
       /// </summary>
       /// <param name="bArr"></param>
       /// <param name="byteToFind"></param>
       /// <returns></returns>
        public static Dictionary<int, int> getIndexOfLargestSequance(byte[] bArr, byte byteToFind)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = 0; i < bArr.Length; i++)
            {
                if (bArr[i] == byteToFind)
                {
                    int count = 0;
                    while ((i + count < bArr.Length - 1) & (bArr[i + count] == byteToFind))
                    {
                        count++;
                    }
                    dict.Add(i, count);
                    i = i + count;
                }
            }
            return dict;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Available Arguments:");
            Console.WriteLine("1 - read CMOS");
            Console.WriteLine("2 - write to CMOS");
            Console.WriteLine("3 - Find longest ZERO chains.");
            Console.WriteLine("4 - write 00 to CMOS at index 72 length 55");
            Console.WriteLine("5 - write DATA to CMOS at index 72 length 55");
        }

        static void Main(string[] args)
        {
            LoadAsmIODriver();
            int switchvalue = 0;
            if ((args.Length == 0))
            {
                PrintUsage();
            }
            else
            {
                switchvalue = int.Parse(args[0]);

                switch (switchvalue)
                {
                    case (1):
                        {
                            #region READ CMOS
                            Console.WriteLine("Reading CMOS Contents,");
                            Console.WriteLine("\r\nCMOS Lower 128 bytes:");
                            for (int i = 0; i < 128; i++)
                            {
                                WritePortByte(CMOS_ADDR_PORT_LOW, Convert.ToByte(i));
                                byte res = ReadPortByte(CMOS_DATA_PORT_LOW);
                                Console.Write("{0:x2}", res);
                                if ((i + 1) % 16 == 0) Console.WriteLine();

                            }
                            Console.WriteLine("\r\nCMOS Upper 128 bytes:");
                            byte[] upperbytes = new byte[128];
                            for (int i = 0; i < 128; i++)
                            {
                                WritePortByte(CMOS_ADDR_PORT_HIGH, Convert.ToByte(i));
                                upperbytes[i] = ReadPortByte(CMOS_DATA_PORT_HIGH);
                                Console.Write("{0:x2}", upperbytes[i]);
                                if ((i + 1) % 16 == 0) Console.WriteLine();
                            }
                            #endregion
                            break;
                        }
                    case (2):
                        {
                            #region WRITE CMOS 

                            Console.WriteLine("\r\nCMOS Lower bytes:");
                            for (int i = 0; i < 128; i++)
                            {
                                WritePortByte(CMOS_ADDR_PORT_LOW, Convert.ToByte(i));
                                byte res = ReadPortByte(CMOS_DATA_PORT_LOW);
                                Console.Write("{0:x2}", res);

                            }
                            Console.WriteLine("\r\nCMOS Upper bytes:");
                            byte[] upperbytes = new byte[128];
                            for (int i = 0; i < 128; i++)
                            {
                                WritePortByte(CMOS_ADDR_PORT_HIGH, Convert.ToByte(i));
                                upperbytes[i] = ReadPortByte(CMOS_DATA_PORT_HIGH);
                                Console.Write("{0:x2}", upperbytes[i]);
                            }
                            #endregion
                            break;
                        }
                    case (3):
                        {
                            byte[] lowerbytes = new byte[128];
                            byte[] upperbytes = new byte[128];
                            Console.WriteLine("\r\nCMOS Lower bytes:");
                            for (int i = 0; i < 128; i++)
                            {
                                WritePortByte(CMOS_ADDR_PORT_LOW, Convert.ToByte(i));
                                lowerbytes[i] = ReadPortByte(CMOS_DATA_PORT_LOW);

                            }
                            Console.WriteLine("\r\nCMOS Upper bytes:");
                            for (int i = 0; i < 128; i++)
                            {
                                WritePortByte(CMOS_ADDR_PORT_HIGH, Convert.ToByte(i));
                                upperbytes[i] = ReadPortByte(CMOS_DATA_PORT_HIGH);
                            }

                            Dictionary<int, int> lowerRes = getIndexOfLargestSequance(lowerbytes, 0);
                            List<int> lowerList = new List<int>(lowerRes.Keys);
                            lowerRes = lowerRes.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                            Dictionary<int, int> UpperRes = getIndexOfLargestSequance(upperbytes, 0);
                            UpperRes = UpperRes.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                            Console.WriteLine("Lower longest index and count:" + lowerRes.Last().Key + " " + lowerRes.Last().Value);
                            Console.WriteLine("Upper longest index and count:" + UpperRes.Last().Key + " " + UpperRes.Last().Value);
                            //last index of each dict is the longest series of bytes we are looking for
                            break;
                        }
                    case (4):
                        {
                            //Write 0 to LOWER CMOS
                            //Brix SKL results are: Lower longest index and count:72 55
                            Console.WriteLine("\r\nRestoring Original CMOS");
                            for (int i = 72; i < 128; i++)
                            {
                                WritePortByte(CMOS_ADDR_PORT_LOW, Convert.ToByte(i));
                                WritePortByte(CMOS_DATA_PORT_LOW, Convert.ToByte(0));
                                //Console.Write("{0:x2}", res);
                            }
                            break;
                        }
                    case (5):
                        {
                            //Write simulated payload to LOWER CMOS
                            //Brix SKL results are: Lower longest index and count:72 55
                            Console.WriteLine("\r\nSaving 'Malicious' Bytes to CMOS");
                            for (int i = 72; i < 128; i++)
                            {
                                WritePortByte(CMOS_ADDR_PORT_LOW, Convert.ToByte(i));
                                WritePortByte(CMOS_DATA_PORT_LOW, Convert.ToByte(i));
                                //Console.Write("{0:x2}", res);
                            }
                            break;
                        }
                    default:
                        {
                            PrintUsage();
                            break;
                        }
                }
            }
            UnloadAsmIODriver();
        }
       
    }
}

