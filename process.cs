using System;
using System.Collections.Generic;

namespace CosmosKernel1
{
    public class Process
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public uint MemorySize { get; set; }
        public bool IsRunning { get; set; }
        private int Iteration;
        private List<uint> allocatedPages;

        public Process(int id, string name, uint memorySize)
        {
            Id = id;
            Name = name;
            MemorySize = memorySize;
            IsRunning = false;
            Iteration = 0;
            allocatedPages = new List<uint>();
        }

        public void Start()
        {
            if (IsRunning) return;

            IsRunning = true;
            Console.WriteLine($"[PROCESS] {Name} started (ID: {Id}, Memory: {MemorySize} bytes).");

            uint pagesNeeded = (MemorySize + MemoryManager.GetPageSize() - 1) / MemoryManager.GetPageSize();
            for (uint i = 0; i < pagesNeeded; i++)
            {
                allocatedPages.Add(MemoryManager.AllocatePage(Id));
            }

            SystemCallHandler.HandleSysCall((uint)SysCallNumbers.Print, (uint)Id);
        }

        public void ExecuteStep()
        {
            if (!IsRunning) return;

            Console.WriteLine($"[PROCESS] {Name} is executing. Iteration: {Iteration + 1}");
            Iteration++;

            if (Iteration == 2)
            {
                SystemCallHandler.HandleSysCall((uint)SysCallNumbers.SendIPCMessage, (uint)Id);
            }

            if (Iteration == 3)
            {
                string msg = IPCManager.ReceiveMessage();
                Console.WriteLine($"[PROCESS] {Name} received IPC: {msg}");
            }

            if (Iteration >= 5)
            {
                Console.WriteLine($"[PROCESS] {Name} completed.");
                IsRunning = false;
                FreeMemory();
            }
        }

        private void FreeMemory()
        {
            foreach (var page in allocatedPages)
            {
                MemoryManager.FreePage(page, Id);
            }
            allocatedPages.Clear();
            Console.WriteLine($"[PROCESS] {Name} has released its allocated memory.");
        }
    }
}
