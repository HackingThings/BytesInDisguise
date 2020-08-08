using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PMXWrapper
{
    class SPI_HW_SEQ
    {
        public UInt32 SPI_BAR = 0x0;
        public UIntPtr SpiBarMapping;

        public UInt16 HSFS_OFF = 0x4;
        public UInt16 HSFC_OFF = 0x6;
        public UInt16 FADDR_OFF = 0x8;
        public UInt16 FDATA0_OFF = 0x10;

        public UInt16 HSFS_FDONE = 1 << 0;
        public UInt16 HSFS_FCERR = 1 << 1;
        public UInt16 HSFS_AEL = 1 << 2;
        public UInt16 HSFS_SCIP = 1 << 5;   // spi cycle in progress

        public UInt16 HSFC_FGO = 1 << 0;    // go bit, starts flash cycle
        public UInt16 HSFC_FCYCLE_READ = 0 << 1;    // flash cycle types
        public UInt16 HSFC_FCYCLE_WRITE = 2 << 1;
        public UInt16 HSFC_FCYCLE_ERASE = 3 << 1;

        public UInt16 HSFC_MAX_DBC = 63;   // set data-byte-count to max of 64 bytes...

        public UInt32 FADDR_MASK = 0x07FFFFFF;

        public SPI_HW_SEQ()
        {
            //get spi bar
            SPI_BAR = PMXMethods.readPCILegacy(0x0, 0x1f, 0x5, 0x10);
            SpiBarMapping = PMXMethods.MapPhysMem(SPI_BAR, 1);
        }

        ~SPI_HW_SEQ()
        {
            PMXMethods.UnMapPhysMem(SpiBarMapping);
        }
        public Byte SPI_ReadReg8(UInt16 RegOffset)
        {
            Byte RegValue;

            unsafe
            {
                byte* spibar_ptr = (byte*)SpiBarMapping.ToPointer();
                RegValue = *((Byte*)(spibar_ptr + RegOffset));
            }
            return RegValue;
        }

        public UInt16 SPI_ReadReg16(UInt16 RegOffset)
        {
            UInt16 RegValue;

            unsafe
            {
                byte* spibar_ptr = (byte*)SpiBarMapping.ToPointer();
                RegValue = *((UInt16*)(spibar_ptr + RegOffset));
            }
            return RegValue;
        }

        public UInt32 SPI_ReadReg32(UInt16 RegOffset)
        {
            UInt32 RegValue;
            unsafe
            {
                byte* spibar_ptr = (byte*)SpiBarMapping.ToPointer();
                RegValue = *((UInt32*)(spibar_ptr + RegOffset));
            }
            return RegValue;
        }

        public void SPI_WriteReg8(UInt16 RegOffset, Byte RegValue)
        {
            unsafe
            {
                byte* spibar_ptr = (byte*)SpiBarMapping.ToPointer();
                *((Byte *)(spibar_ptr + RegOffset)) = RegValue;
            }
        }

        public void SPI_WriteReg16(UInt16 RegOffset, UInt16 RegValue)
        {
            unsafe
            {
                byte* spibar_ptr = (byte*)SpiBarMapping.ToPointer();
                *((UInt16 *)(spibar_ptr + RegOffset)) = RegValue;
            }
        }

        public void SPI_WriteReg32(UInt16 RegOffset, UInt32 RegValue)
        {
            unsafe
            {
                byte* spibar_ptr = (byte*)SpiBarMapping.ToPointer();
                *((UInt32*)(spibar_ptr + RegOffset)) = RegValue;
            }

        }

        public void SPI_WaitUntilDone()
        {
            UInt16 HSFS;
            do
            {
                HSFS = SPI_ReadReg16(HSFS_OFF);
            } while ((HSFS & HSFS_SCIP) != 0);
            SPI_WriteReg16(HSFS_OFF, (UInt16)(HSFS_AEL | HSFS_FCERR | HSFS_FDONE));
        }

        public Byte[] SPI_Read(UInt32 FlashLinearAddress)
        {
            UInt32[] FData = new UInt32[16];
            SPI_WaitUntilDone();
            SPI_WriteReg32(FADDR_OFF, FlashLinearAddress & FADDR_MASK);
            SPI_WriteReg8((UInt16)(HSFC_OFF + 1), (Byte)HSFC_MAX_DBC);
            SPI_WriteReg8(HSFC_OFF, (Byte)(HSFC_FCYCLE_READ | HSFC_FGO));
            SPI_WaitUntilDone();
            for (UInt16 i = 0; i < 16; i++)
            {
                FData[i] = SPI_ReadReg32((UInt16)(FDATA0_OFF + i*4));
                Console.WriteLine("FDATA[{0}] = {1:x}", i, FData[i]);
            }
            Byte[] spi_data = new byte[64];
            Buffer.BlockCopy(FData, 0, spi_data, 0, 64);
            return spi_data;
        }
    }
}
