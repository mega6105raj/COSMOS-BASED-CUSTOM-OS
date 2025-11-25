using System;
using System.Collections.Generic;

namespace CosmosKernel1
{
    public static class MemoryManager
    {
        private static uint totalMemory = 1024 * 64;
        private static uint pageSize = 4096; 
        private static Dictionary<uint, int> pageTable = new Dictionary<uint, int>();

        public static void Initialize()
        {
            Console.WriteLine("[MEMORY] Initializing memory...");
            uint totalPages = totalMemory / pageSize;
            for (uint i = 0; i < totalPages; i++)
            {
                pageTable[i] = -1;
            }
        }

        public static uint AllocatePage(int processId)
        {
            foreach (var entry in pageTable)
            {
                if (entry.Value == -1) 
                {
                    pageTable[entry.Key] = processId;
                    Console.WriteLine($"[MEMORY] Allocated page {entry.Key} to Process {processId}");
                    return entry.Key;
                }
            }

            Console.WriteLine("[MEMORY] No free pages, using page replacement...");
            return PageReplacement.ReplacePage(processId);
        }

        public static void FreePage(uint page, int processId)
        {
            if (pageTable.ContainsKey(page) && pageTable[page] == processId)
            {
                pageTable[page] = -1;
                Console.WriteLine($"[MEMORY] Process {processId} freed page {page}");
            }
            else
            {
                Console.WriteLine($"[SECURITY] Unauthorized memory free attempt by Process {processId}");
            }
        }

        public static bool CheckAccess(uint page, int processId)
        {
            return pageTable.ContainsKey(page) && pageTable[page] == processId;
        }

        public static uint GetPageSize()
        {
            return pageSize;
        }

        public static void AllocateFileMemory(int size)
        {
            uint pagesNeeded = (uint)Math.Ceiling(size / (double)pageSize);
            for (uint i = 0; i < pagesNeeded; i++)
            {
                AllocatePage(-2); 
            }
        }

        public static void DeallocateFileMemory(int size)
        {
            uint pagesFreed = (uint)Math.Ceiling(size / (double)pageSize);
            foreach (var entry in pageTable)
            {
                if (entry.Value == -2 && pagesFreed > 0)
                {
                    pageTable[entry.Key] = -1;
                    pagesFreed--;
                }
            }
        }
        public static void PrintMemoryMap()
        {
            Console.WriteLine("[MEMORY] Current Page Allocation:");
            foreach (var entry in pageTable)
            {
                Console.WriteLine($"Page {entry.Key}: {(entry.Value == -1 ? "Free" : entry.Value == -2 ? "File System" : $"Owned by Process {entry.Value}")}");
            }
        }

    }
}
