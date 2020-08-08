using System;
using System.Runtime.InteropServices;

namespace PMXWrapper
{
    public class PMXMethods
    {
        #region Dll Imports
        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PMxInit(int dbg, PMXStructs.PMX_Init_t p);

        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PMxDestroy();

        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PMxGetLastError(byte[] c, uint i);

        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PMxPCI(PMXStructs.PMX_PCI_t p);

        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PMxIO(PMXStructs.PMX_IO_t p);

        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PMxRDMSR(ref PMXStructs.PMX_REG64_t ppmxReg64);

        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PMxRDCR64(ref PMXStructs.PMX_REG64_t ppmxReg64);

        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PMxMapPhys(ref PMXStructs.PMX_MAPPHYS64_t pmxMapPhys);
        
        [DllImport("Pmxdll32e.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PMxUnmapPhys(ref PMXStructs.PMX_MAPPHYS64_t pmxMapPhys);

        #endregion
        public static void PrintLastError()
        {
            byte[] buf_PMxGetLastError = new byte[256];
            PMxGetLastError(buf_PMxGetLastError, 0x100);
            string error = System.Text.Encoding.UTF8.GetString(buf_PMxGetLastError).Replace("\0", string.Empty);
            if (error.Trim() != "")
            {
                Console.WriteLine("[!] PMxGetLastError:" + System.Text.Encoding.UTF8.GetString(buf_PMxGetLastError));
            }
        }

        public static int Init(int debug_level) {
            PMXStructs.PMX_Init_t ppmxInit = new PMXStructs.PMX_Init_t();
            ppmxInit.Size = 0x4310;
            return PMXMethods.PMxInit(debug_level, ppmxInit);
        }

        public static UInt32 readPCILegacy(UInt32 bus, UInt32 dev, UInt32 fun, UInt32 reg)
        {
            UInt32 address = 0; ;
            address = bus << 16 | dev << 11 | fun << 8 | reg & 0xFC | 0x80000000;
            writeIO(0xcf8, address,4);
            UInt32 res = readIO(0xcfc,4);
            return res;
        }

        public static void writePCILegacy(UInt32 bus, UInt32 dev, UInt32 fun, UInt32 reg, UInt32 data)
        {
            UInt32 address = 0; ;
            address = bus << 16 | dev << 11 | fun << 8 | reg & 0xFC | 0x80000000;
            writeIO(0xcf8, address, 4);
            writeIO(0xcfc, data, 4);
        }
       
        public static UInt32 readIO(uint port, int SizeInBytes)
        {
            PMXStructs.PMX_IO_t io = new PMXStructs.PMX_IO_t();
            io.Size = 16;
            io.Type = PMXStructs.IOType.PMX_IO_IN8; //placeholdser for init;
            switch (SizeInBytes)
            {
                case (1):
                    {
                        io.Type = PMXStructs.IOType.PMX_IO_IN8;
                        break;
                    }
                case (2):
                    {
                        io.Type = PMXStructs.IOType.PMX_IO_IN16;
                        break;
                    }
                case (4):
                    {
                        io.Type = PMXStructs.IOType.PMX_IO_IN32;
                        break;
                    }
                default:
                    break;
            }
            io.IOPort = port;
            io.Value = 0;
            PMxIO(io);
            return io.Value;
        }

        public static UInt32 writeIO(uint port, uint value, int SizeInBytes)
        {
            PMXStructs.PMX_IO_t io = new PMXStructs.PMX_IO_t();
            io.Size = 16;
            io.Type = PMXStructs.IOType.PMX_IO_OUT8; 
            switch (SizeInBytes)
            {
                case (1):
                    {
                        io.Type = PMXStructs.IOType.PMX_IO_OUT8;
                        break;
                    }
                case (2):
                    {
                        io.Type = PMXStructs.IOType.PMX_IO_OUT16;
                        break;
                    }
                case (4):
                    {
                        io.Type = PMXStructs.IOType.PMX_IO_OUT32;
                        break;
                    }
                default:
                    break;
            }
            io.IOPort = port;
            io.Value = value;
            PMxIO(io);
            return io.Value;

        }

        public static UInt32 readpci(uint bus, uint device, uint function, uint offset)
        {
            PMXStructs.PMX_PCI_t pci = new PMXStructs.PMX_PCI_t();
            pci.Size = 28;
            pci.PCIType = PMXStructs.PCIType.PMX_PCI_IN32;
            pci.PCICFAddr = bus << 16 | device << 11 | function << 8 | offset & 0xFC | 0x80000000;
            PMxPCI(pci);
            return pci.RetValue;
        }

        public unsafe static UInt64[] readMSR(uint msrHex)
        {
            bool err;
            PMXStructs.PMX_REG64_t ppmxReg64 = new PMXStructs.PMX_REG64_t();
            ppmxReg64.Size = 1064;
            ppmxReg64.ProcessorMask = 0xff;    // which cpu thread to request MSR from
            ppmxReg64.SuccessMask = 0;
            ppmxReg64.MsrNum = msrHex;
            err = PMxRDMSR(ref ppmxReg64);
            UInt64[] ret = new UInt64[64];
            for (int i = 0; i < 8; i++)
            {
                ret[i] = ppmxReg64._64RetValue[i];
            }
            return ret;    
        }
        public static unsafe UInt64[] readCR64(uint cr)
        {
            PMXStructs.PMX_REG64_t ppmxReg64 = new PMXStructs.PMX_REG64_t();
            ppmxReg64.Size = 1064;
            ppmxReg64.ProcessorMask = 0xff;    
            ppmxReg64.SuccessMask = 0;
            ppmxReg64.MsrNum = cr;
            PMxRDCR64(ref ppmxReg64);
            UInt64[] res = new UInt64[64];
            for (int i = 0; i < 64; i++)
            {
                res[i] = ppmxReg64._64RetValue[i];
            }
            return res;
        }

        public unsafe static UIntPtr MapPhysMem(uint physicalAddress, uint numberOfPages)
        {
            bool err;
            PMXStructs.PMX_MAPPHYS64_t ppmxMapPhys64 = new PMXStructs.PMX_MAPPHYS64_t();
            ppmxMapPhys64.Size = 24;
            ppmxMapPhys64.PhysAddr = physicalAddress;
            ppmxMapPhys64.NumPages = numberOfPages;
            err = PMxMapPhys(ref ppmxMapPhys64);
            Console.WriteLine("mapphys = {0}", err);
            return ppmxMapPhys64.VirtAddr;
        }

        public static bool UnMapPhysMem(UIntPtr virtualAddress)
        {
            bool err;
            PMXStructs.PMX_MAPPHYS64_t ppmxMapPhys64 = new PMXStructs.PMX_MAPPHYS64_t();
            ppmxMapPhys64.Size = 24;
            ppmxMapPhys64.VirtAddr = virtualAddress;
            err = PMxUnmapPhys(ref ppmxMapPhys64);
            Console.WriteLine("unmapphys = {0}", err);
            return err;
        }
    }
}
