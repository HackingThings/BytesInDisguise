using System;
using System.Runtime.InteropServices;

namespace PMXWrapper
{
    public class PMXStructs
    {
        public enum PCIType
        {
            DO_NOT_USE, //IN == GET //OUT == SET
            PMX_PCI_IN8,
            PMX_PCI_IN16,
            PMX_PCI_IN32,
            PMX_PCI_OUT8,
            PMX_PCI_OUT16,
            PMX_PCI_OUT32
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public unsafe class PMX_PCI_t
        {
            public uint Size;
            public PCIType PCIType;
            public uint PCICFAddr; 
            public uint AndMask;
            public uint OrMask;
            public uint OldValue;
            public uint RetValue;
        }

        public enum IOType
        {
            DO_NOT_USE, //IN == GET //OUT == SET
            PMX_IO_IN8,
            PMX_IO_IN16,
            PMX_IO_IN32,	
            PMX_IO_OUT8,
            PMX_IO_OUT16,
            PMX_IO_OUT32
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public unsafe class PMX_IO_t
        {
            public uint Size;
            public IOType Type;
            public uint IOPort;
            public uint Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct PMX_REG64_t
        {
            [FieldOffset(0x0)]   public uint Size;
            [FieldOffset(0x4)]   public UInt64 ProcessorMask;
            [FieldOffset(0xC)]   public UInt64 SuccessMask;
            [FieldOffset(0x14)]  public uint MsrNum;
            [FieldOffset(0x18)]  public UInt64 MsrMask;
            [FieldOffset(0x20)]  public UInt64 MsrValue;
            [FieldOffset(0x28)]  public fixed UInt64 _64OldValue[64];
            [FieldOffset(0x228)] public fixed UInt64 _64RetValue[64];
        }


        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct PMX_REG32_t
        {
            [FieldOffset(0x0)] public uint Size;
            [FieldOffset(0x4)] public uint ProcessorMask;
            [FieldOffset(0xC)] public uint SuccessMask;
            [FieldOffset(0x14)] public uint MsrNum;
            [FieldOffset(0x18)] public uint MsrMask;
            [FieldOffset(0x20)] public uint MsrValue;
            [FieldOffset(0x28)] public fixed uint _32OldValue[32];
            [FieldOffset(0x228)] public fixed uint _32RetValue[32];
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public unsafe struct PMX_Init_t
        {
            public uint Size;
            public fixed Byte Buffer[0x4310-4];
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct PMX_MAPPHYS64_t
        {
            [FieldOffset(0x0)] public uint Size;
            [FieldOffset(0x4)] public UInt64 PhysAddr;
            [FieldOffset(0xC)] public uint NumPages;
            [FieldOffset(0x10)] public UIntPtr VirtAddr;
        }
    }
}
