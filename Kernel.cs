using CosmosKernel1;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network;
using static Cosmos.HAL.PCIDevice;
using Cosmos.HAL;

namespace CosmosKernel1
{
    public class Kernel : Sys.Kernel
    {
        private Queue<Process> processQueue = new Queue<Process>();

        protected override void BeforeRun()
        {
            Console.WriteLine("[KERNEL] Initializing memory manager...");
            MemoryManager.Initialize();

            Console.WriteLine("[KERNEL] Initializing file system...");
            Console.WriteLine("[KERNEL] Initializing network stack...");
            NetworkStack.Init();

            if (NetworkDevice.Devices.Count > 0)
            {
                var nic = NetworkDevice.Devices[0]; // Select first network device
                nic.Enable();

                // Manually configure the static IP
                IPConfig.Enable(nic, new Address(192, 168, 1, 100), new Address(255, 255, 255, 0), new Address(192, 168, 1, 1));

                Console.WriteLine($"[KERNEL] Assigned Static IP: 192.168.1.100");

                // ✅ Perform a test UDP packet send
                TestUDPConnection();
                Console.WriteLine("[KERNEL] Sending ICMP Echo Request (Ping) to 8.8.8.8...");

                if (nic != null)
                {
                    // Construct ICMP Echo Request Packet (Manually)
                    byte[] icmpPacket = new byte[]
                    {
                        0x08, 0x00, 0xF7, 0xFF, // Type (8 = Echo Request), Code (0), Checksum (Dummy)
                        0x00, 0x01, 0x00, 0x01  // Identifier, Sequence Number
                    };

                    // Send ICMP request as raw data
                    nic.QueueBytes(icmpPacket);

                    Console.WriteLine("[KERNEL] ICMP Packet Sent!");
                }
                else
                {
                    Console.WriteLine("[KERNEL] No network device found. Cannot send ICMP.");
                }

                System.Threading.Thread.Sleep(3000); // Wait to simulate response time
            }
            else
            {
                Console.WriteLine("[KERNEL] No network device found!");
            }

            Console.WriteLine("[KERNEL] Network stack initialized.");
            Console.WriteLine("[KERNEL] Checking available network devices...");

            if (NetworkDevice.Devices.Count > 0)
            {
                for (int i = 0; i < NetworkDevice.Devices.Count; i++)
                {
                    Console.WriteLine($"[KERNEL] Found network device {i}: {NetworkDevice.Devices[i].Name}");
                }
            }
            else
            {
                Console.WriteLine("[KERNEL] No network devices found!");
            }

            int testProcessId = 999; // Simulated process ID for testing file system operations
            FileSystemManager.AssignSandbox(testProcessId, "sandbox");

            FileSystemManager.CreateFile(testProcessId, "sandbox/test.txt", "Hello, Cosmos! This file is using memory.");
            Console.WriteLine(FileSystemManager.ReadFile(testProcessId, "sandbox/test.txt"));
            FileSystemManager.WriteFile(testProcessId, "sandbox/test.txt", "Updated content. This should modify memory allocation.");
            FileSystemManager.ListFiles();
            FileSystemManager.DeleteFile(testProcessId, "sandbox/test.txt");

            Console.WriteLine("[KERNEL] Creating processes...");
            processQueue.Enqueue(new Process(1, "Process A", 2048));
            processQueue.Enqueue(new Process(2, "Process B", 4096));
            processQueue.Enqueue(new Process(3, "Process C", 1024));

            Console.WriteLine($"[KERNEL] Loaded {processQueue.Count} processes.");
            MemoryManager.PrintMemoryMap();
        }

        protected override void Run()
        {
            Console.WriteLine("[KERNEL] Running processes...");

            while (processQueue.Count > 0)
            {
                Process currentProcess = processQueue.Dequeue();
                currentProcess.Start();

                if (!currentProcess.IsRunning)
                {
                    Console.WriteLine($"[KERNEL] Skipping process {currentProcess.Name} due to memory allocation failure.");
                    continue;
                }

                while (currentProcess.IsRunning)
                {
                    currentProcess.ExecuteStep();
                    System.Threading.Thread.Sleep(1000);
                }

                Console.WriteLine($"[KERNEL] Process {currentProcess.Name} finished.");
                MemoryManager.PrintMemoryMap();
            }

            Console.WriteLine("[KERNEL] All processes completed.");

            if (!AuthenticateUser())
            {
                Console.WriteLine("[KERNEL] Too many failed login attempts. System halted.");
                return;
            }

            Console.WriteLine("[KERNEL] Starting Command-Line Interface (CLI)...");
            CommandLoop();
        }

        private bool AuthenticateUser()
        {
            Console.WriteLine("[KERNEL] User authentication required.");

            int attempts = 3;

            while (attempts > 0)
            {
                Console.Write("Username: ");
                string username = Console.ReadLine();

                Console.Write("Password: ");
                string password = ReadPassword();

                if (UserManager.Authenticate(username, password))
                {
                    Console.WriteLine("\n[KERNEL] Login successful! Welcome, " + username);
                    return true;
                }
                else
                {
                    Console.WriteLine("\n[KERNEL] Invalid credentials. Try again.");
                    attempts--;
                }
            }

            return false;
        }

        private string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        private void CommandLoop()
        {
            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                switch (input.ToLower())
                {
                    case "help":
                        Console.WriteLine("Available commands: help, clear, exit");
                        break;

                    case "clear":
                        Console.Clear();
                        break;

                    case "exit":
                        Console.WriteLine("Shutting down...");
                        return;

                    default:
                        Console.WriteLine("Unknown command. Type 'help' for a list of commands.");
                        break;
                }
            }
        }

        private void TestUDPConnection()
        {
            Console.WriteLine("[KERNEL] Testing UDP Connection...");

            var destination = new Address(8, 8, 8, 8);
            var udpClient = new UdpClient(53);

            byte[] data = System.Text.Encoding.ASCII.GetBytes("Cosmos UDP Test");

            try
            {
                udpClient.Send(data, destination, 53);
                Console.WriteLine("[KERNEL] UDP Packet Sent to 8.8.8.8:53");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KERNEL] UDP Send Failed: {ex.Message}");
            }
        }
    }
}
