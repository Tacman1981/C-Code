using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

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
                GetGpuInfo();

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

        static void GetGpuInfo()
        {
            var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (var item in searcher.Get())
            {
                Console.WriteLine($"  Name: {item["Name"]}");
                Console.WriteLine($"  Adapter RAM: {Math.Round(Convert.ToDouble(item["AdapterRAM"]) / (1024 * 1024 * 1024), 2)} GB");
                Console.WriteLine($"  Driver Version: {item["DriverVersion"]}");
                Console.WriteLine($"  Video Processor: {item["VideoProcessor"]}");
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
                // Check if the process is a system process
                if (IsSystemProcess(process))
                {
                    // If it's a system process, it's not hidden
                    return false;
                }

                // Check if the process has a main window
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    // If it has a main window, it's not hidden
                    return false;
                }

                // Check if the process is hidden by other means (e.g., no main window)
                if (string.IsNullOrWhiteSpace(process.MainWindowTitle) && process.Threads.Count == 0)
                {
                    // If the process has no threads and no visible title, it's likely hidden
                    return true;
                }

                // If none of the conditions above are met, the process is not hidden
                return false;
            }
            catch (Exception)
            {
                // Unable to determine if the process is hidden, so consider it hidden
                return true;
            }
        }

        static bool IsSystemProcess(Process process)
        {
            string[] systemProcesses = { "conhost", "svchost", "csrss", "wininit", "services", "lsass", "lsm", "winlogon", "explorer", "taskmgr", "taskhostw", "taskkill", "tasklist", "taskeng", "taskswitch", "taskhook", "taskswitcher", "taskswitching", "taskman" };
            return systemProcesses.Contains(process.ProcessName, StringComparer.OrdinalIgnoreCase);
        }

        // Import Windows API functions
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);
    }
}
