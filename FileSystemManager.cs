using System;
using System.Collections.Generic;

namespace CosmosKernel1
{
    public static class FileSystemManager
    {
        private static Dictionary<string, string> files = new Dictionary<string, string>();
        private static Dictionary<string, List<string>> directories = new Dictionary<string, List<string>>();
        private static Dictionary<int, string> processSandboxes = new Dictionary<int, string>(); // Mapping process ID to sandbox directory

        public static void AssignSandbox(int processId, string sandboxDirectory)
        {
            if (!directories.ContainsKey(sandboxDirectory))
            {
                CreateDirectory(sandboxDirectory);
            }
            processSandboxes[processId] = sandboxDirectory;
        }

        private static bool IsAccessAllowed(int processId, string filename)
        {
            if (!processSandboxes.ContainsKey(processId))
            {
                Console.WriteLine("[FS] Error: Process does not have a sandbox.");
                return false;
            }

            string sandbox = processSandboxes[processId];
            return filename.StartsWith(sandbox + "/");
        }

        public static void CreateFile(int processId, string filename, string content)
        {
            if (!IsAccessAllowed(processId, filename))
            {
                Console.WriteLine("[FS] Error: Access denied.");
                return;
            }

            if (files.ContainsKey(filename))
            {
                Console.WriteLine($"[FS] Error: File '{filename}' already exists.");
                return;
            }

            files[filename] = content;
            Console.WriteLine($"[FS] File '{filename}' created.");
            MemoryManager.AllocateFileMemory(content.Length);
        }

        public static string ReadFile(int processId, string filename)
        {
            if (!IsAccessAllowed(processId, filename))
            {
                Console.WriteLine("[FS] Error: Access denied.");
                return null;
            }

            if (files.ContainsKey(filename))
            {
                Console.WriteLine($"[FS] Reading file '{filename}'.");
                return files[filename];
            }

            Console.WriteLine($"[FS] Error: File '{filename}' not found.");
            return null;
        }

        public static void WriteFile(int processId, string filename, string content)
        {
            if (!IsAccessAllowed(processId, filename))
            {
                Console.WriteLine("[FS] Error: Access denied.");
                return;
            }

            if (files.ContainsKey(filename))
            {
                MemoryManager.AllocateFileMemory(content.Length - files[filename].Length);
                files[filename] = content;
                Console.WriteLine($"[FS] File '{filename}' updated.");
            }
            else
            {
                Console.WriteLine($"[FS] Error: File '{filename}' not found.");
            }
        }

        public static void DeleteFile(int processId, string filename)
        {
            if (!IsAccessAllowed(processId, filename))
            {
                Console.WriteLine("[FS] Error: Access denied.");
                return;
            }

            if (files.ContainsKey(filename))
            {
                MemoryManager.DeallocateFileMemory(files[filename].Length);
                files.Remove(filename);
                Console.WriteLine($"[FS] File '{filename}' deleted.");
            }
            else
            {
                Console.WriteLine($"[FS] Error: File '{filename}' not found.");
            }
        }

        public static void CreateDirectory(string dirname)
        {
            if (directories.ContainsKey(dirname))
            {
                Console.WriteLine($"[FS] Error: Directory '{dirname}' already exists.");
                return;
            }

            directories[dirname] = new List<string>();
            Console.WriteLine($"[FS] Directory '{dirname}' created.");
        }

        public static void ListDirectories()
        {
            Console.WriteLine("[FS] Directories:");
            foreach (var dir in directories.Keys)
            {
                Console.WriteLine(" - " + dir);
            }
        }

        public static void ListFiles()
        {
            Console.WriteLine("[FS] Files:");
            foreach (var file in files.Keys)
            {
                Console.WriteLine(" - " + file);
            }
        }
    }
}
