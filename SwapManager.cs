using System;
using System.Collections.Generic;

namespace CosmosKernel1
{
    public static class SwapManager
    {
        private static List<uint> swappedPages = new List<uint>();
        private static Dictionary<uint, string> swapFile = new Dictionary<uint, string>();

        public static uint SwapOut(uint page)
        {
            swappedPages.Add(page);
            swapFile[page] = $"swap_page_{page}.bin";
            Console.WriteLine($"[SWAP] Swapped out page {page} to disk.");
            return page;
        }

        public static void SwapIn(uint page)
        {
            if (swapFile.ContainsKey(page))
            {
                Console.WriteLine($"[SWAP] Swapped in page {page} from disk.");
                swapFile.Remove(page);
                swappedPages.Remove(page);
            }
        }
    }
}
