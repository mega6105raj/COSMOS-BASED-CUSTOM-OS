using System;

namespace CosmosKernel1
{
    public static class SystemCallHandler
    {
        public static void HandleSysCall(uint callNumber, uint param)
        {
            switch ((SysCallNumbers)callNumber)
            {
                case SysCallNumbers.Print:
                    Console.WriteLine($"[SYSCALL] User process says: {param}");
                    break;

                case SysCallNumbers.AllocateMemory:
                    uint allocatedPage = MemoryManager.AllocatePage((int)param);
                    Console.WriteLine($"[SYSCALL] Allocated page {allocatedPage}");
                    break;

                case SysCallNumbers.GetProcessID:
                    Console.WriteLine("[SYSCALL] Returning process ID (Mocked: 42)");
                    break;

                case SysCallNumbers.SendIPCMessage:
                    IPCManager.SendMessage($"Message from Process {param}");
                    break;

                default:
                    Console.WriteLine("[SYSCALL] Unknown system call");
                    break;
            }
        }
    }
}
