using System;
using System.Linq;
using System.Management;
using System.Diagnostics;
using SharpDX.Direct3D;
using SharpDX.DXGI;

namespace SystemInfoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Fetching CPU Info...");
                GetCpuInfo();

                Console.WriteLine("\nFetching Motherboard Info...");
                GetMotherboardInfo();

                Console.WriteLine("\nFetching RAM Info...");
                GetRamInfo();

                Console.WriteLine("\nFetching GPU Info...");
                GetDedicatedVideoMemory();

                Console.WriteLine("\nFetching Hidden Processes...");
                GetHiddenProcesses();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }

        static void GetCpuInfo()
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (var item in searcher.Get())
            {
                Console.WriteLine($"  Name: {item["Name"]}");
                Console.WriteLine($"  Manufacturer: {item["Manufacturer"]}");
                Console.WriteLine($"  Cores: {item["NumberOfCores"]}");
                Console.WriteLine($"  Logical Processors: {item["NumberOfLogicalProcessors"]}");
                Console.WriteLine($"  Max Clock Speed: {item["MaxClockSpeed"]} MHz");
            }
        }

        static void GetMotherboardInfo()
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_BaseBoard");
            foreach (var item in searcher.Get())
            {
                Console.WriteLine($"  Manufacturer: {item["Manufacturer"]}");
                Console.WriteLine($"  Product: {item["Product"]}");
                Console.WriteLine($"  Version: {item["Version"]}");
                Console.WriteLine($"  Serial Number: {item["SerialNumber"]}");
            }
        }

        static void GetRamInfo()
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            foreach (var item in searcher.Get())
            {
                Console.WriteLine($"  Total Physical Memory: {Math.Round(Convert.ToDouble(item["TotalPhysicalMemory"]) / (1024 * 1024 * 1024), 2)} GB");
            }
        }

        static void GetHiddenProcesses()
        {
            var hiddenProcesses = Process.GetProcesses()
                .Where(p => IsProcessHidden(p))
                .ToList();

            if (hiddenProcesses.Any())
            {
                Console.WriteLine("Hidden Processes:");
                foreach (var process in hiddenProcesses)
                {
                    Console.WriteLine($"  Name: {process.ProcessName}");
                    Console.WriteLine($"  ID: {process.Id}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No hidden processes found.");
            }
        }

        static bool IsProcessHidden(Process process)
        {
            try
            {
                if (IsSystemProcess(process))
                {
                    return false;
                }

                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(process.MainWindowTitle) && process.Threads.Count == 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        static bool IsSystemProcess(Process process)
        {
            string[] systemProcesses = { "conhost", "svchost", "csrss", "wininit", "services", "lsass", "lsm", "winlogon", "explorer", "taskmgr", "taskhostw", "taskkill", "tasklist", "taskeng", "taskswitch", "taskhook", "taskswitcher", "taskswitching", "taskman" };
            return systemProcesses.Contains(process.ProcessName, StringComparer.OrdinalIgnoreCase);
        }

        // Removed the unnecessary external method declaration
        static void GetDedicatedVideoMemory()
        {
            var factory = new Factory1();
            foreach (var adapter in factory.Adapters)
            {
                var desc = adapter.Description;
                Console.WriteLine($"  Name: {desc.Description}");
                Console.WriteLine($"  Adapter RAM: {desc.DedicatedVideoMemory / (1024 * 1024), 2} GB");
            }
        }
    }
}
