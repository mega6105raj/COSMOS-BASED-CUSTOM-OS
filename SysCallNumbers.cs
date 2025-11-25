using System;

namespace CosmosKernel1
{
    public enum SysCallNumbers : uint
    {
        Print = 0x01,
        AllocateMemory = 0x02,
        GetProcessID = 0x03,
        SendIPCMessage = 0x04
    }
}
