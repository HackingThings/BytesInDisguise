using System;
using System.Text;

namespace PMXWrapper
{
   
    class Program
    {
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        static void Main(string[] args)
        {
            try
            {
                // PMXInit will be called automatically if a method function is used before it's been called
                // However, you can specify debug level by manually calling PMXInit
                PMXMethods.Init(0);

                #region Example PCI read
                //Exmple of reading the SPI BAR on modern Intel platforms.
                Console.WriteLine(PMXMethods.readpci(0, 0x1f, 0x5, 0x10).ToString("x"));
                #endregion Example PCI read

                #region Example read PCI Legacy
                //Example of reading the SPI BAR using legacy PCI
                Console.WriteLine(PMXMethods.readPCILegacy(0x0, 0x1f, 0x5, 0x10).ToString("x"));
                #endregion Example read PCI Legacy

                #region Example IO Read
                //Exmaple of reading the POST code from IO port 80h
                Console.WriteLine(PMXMethods.readIO(0x80, 1).ToString("x"));
                Console.WriteLine(PMXMethods.readIO(0x80, 2).ToString("x"));
                Console.WriteLine(PMXMethods.readIO(0x80, 4).ToString("x"));
                #endregion Example IO Read

                #region Example IO read and write to read content of Lower CMOS 128 bytes
                for (uint i = 0; i < 128; i++)
                {
                    PMXMethods.writeIO(0x70, i, 1);
                    Console.Write(PMXMethods.readIO(0x71, 1).ToString("x"));
                }
                Console.WriteLine();
                #endregion Example IO read and write to read content of Lower CMOS 128 bytes

                #region Example of MSR Read for each core on an 8 core system
                uint msr_to_read = 0x1f2;
                UInt64[] res64 = PMXMethods.readMSR(msr_to_read);
                for (int i = 0; i < 8; i++)
                {
                    Console.WriteLine("READ MSR 0x{0:x} for Core[" + i + "] = {1:x} ", msr_to_read, res64[i]);
                }
                #endregion Example of MSR Read for each core on an 8 core system

                #region Example of CR Read for each core on an 8 core system
                uint cr_to_read = 0;
                UInt64[] res = PMXMethods.readCR64(cr_to_read);
                for (int i = 0; i < 8; i++)
                {
                    Console.WriteLine("READ CR 0x{0:x} for Core[" + i + "] = {1:x} ", cr_to_read, res[i]);
                }
                #endregion Example of CR Read for each core on an 8 core system

                #region Example of SPI HW Sequencing reading from BIOS flash
                SPI_HW_SEQ s = new SPI_HW_SEQ();
                byte[] spi_contents = s.SPI_Read(0);
                Console.WriteLine("start of spi contents: {0}", ByteArrayToString(spi_contents));
                #endregion Example of SPI HW Sequencing reading from BIOS flash

                #region Example of Physical memory mapping and unmapping
                UIntPtr virtaddr = PMXMethods.MapPhysMem(0xfffff000, 1);
                Console.WriteLine("virtaddr = {0:x}", virtaddr);
                byte[] reset_vector = new byte[16];
                unsafe
                {
                    byte* src = (byte*)virtaddr.ToPointer();
                    for (int i = 0; i < 16; i++)
                    {
                        reset_vector[i] = src[0xff0 + i];
                    }
                }
                Console.WriteLine("reset vector: {0}", ByteArrayToString(reset_vector));
                //PMXMethods.UnMapPhysMem(virtaddr);
                #endregion Example of Physical memory mapping and unmapping

                //Print the last error that occured using the DLL and/or Driver
                PMXMethods.PrintLastError();
                PMXMethods.PMxDestroy();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        
    }
}




