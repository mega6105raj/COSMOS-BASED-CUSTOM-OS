using System;
using System.Collections.Generic;

namespace CosmosKernel1
{
    public static class PageReplacement
    {
        private static LinkedList<uint> pageList = new LinkedList<uint>();
        private static Dictionary<uint, LinkedListNode<uint>> pageMap = new Dictionary<uint, LinkedListNode<uint>>();
        private static Dictionary<uint, int> pageOwners = new Dictionary<uint, int>();

        public static uint ReplacePage(int processId)
        {
            if (pageList.Count == 0)
            {
                Console.WriteLine("[PAGE] No pages available to replace.");
                return 0;
            }

            foreach (var page in pageList)
            {
                if (pageOwners[page] == processId)
                {
                    pageList.Remove(page);
                    pageMap.Remove(page);
                    pageOwners.Remove(page);

                    Console.WriteLine($"[PAGE] Replacing LRU page {page} of Process {processId}");

                    MemoryManager.FreePage(page, processId);
                    uint allocatedPage = MemoryManager.AllocatePage(processId);

                    pageList.AddLast(allocatedPage);
                    pageMap[allocatedPage] = new LinkedListNode<uint>(allocatedPage);
                    pageOwners[allocatedPage] = processId;

                    return allocatedPage;
                }
            }

            Console.WriteLine("[PAGE] No pages owned by process, performing global replacement.");
            uint oldPage = pageList.First.Value;
            pageList.RemoveFirst();
            pageMap.Remove(oldPage);

            int oldOwner = pageOwners[oldPage];
            pageOwners.Remove(oldPage);

            MemoryManager.FreePage(oldPage, oldOwner);
            uint newAllocatedPage = MemoryManager.AllocatePage(processId);

            pageList.AddLast(newAllocatedPage);
            pageMap[newAllocatedPage] = new LinkedListNode<uint>(newAllocatedPage);
            pageOwners[newAllocatedPage] = processId;

            return newAllocatedPage;
        }

        public static void AccessPage(uint page, int processId)
        {
            if (!MemoryManager.CheckAccess(page, processId))
            {
                Console.WriteLine($"[SECURITY] Process {processId} attempted unauthorized access to page {page}");
                return;
            }

            if (pageMap.ContainsKey(page))
            {
                LinkedListNode<uint> node = pageMap[page];
                pageList.Remove(node);
                pageList.AddLast(node);
            }
        }
    }
}
