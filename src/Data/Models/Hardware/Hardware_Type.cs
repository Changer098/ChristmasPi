﻿using System;
using System.IO;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Data.Models.Hardware {
    /// <summary>
    /// The types of hardware/systems recognized by ChristmasPi
    /// </summary>
    [Flags]
    public enum Hardware_Type {
        All = 0 | 1 | 2,        // Referring to all hardware (even unkown) in this enumeration
        RPI = 1,                // A Raspberry Pi
        UNKNOWN = 2             // Unknown hardware, could be a PC or a new board
    }
    public class HardwareType {
        /// <summary>
        /// Detects what hardware the system is running on
        /// </summary>
        /// <returns>Current detected type</returns>
        public static Hardware_Type GetHardwareType(bool defaultToRpi) {
            bool isRPIResult = false;
            try {
                isRPIResult = isRPI();
            }
            catch (NonLinuxOSException) {
                if (!defaultToRpi)
                    throw;
                isRPIResult = true;
            }
            if (isRPIResult)
                return Hardware_Type.RPI;
            else {
                if (defaultToRpi)
                    return Hardware_Type.RPI;
                return Hardware_Type.UNKNOWN;
            }
        }
        private static bool isRPI() {
            // read /proc/cpuinfo
            if (File.Exists("/proc/cpuinfo")) {
                using (StreamReader reader = new StreamReader(File.OpenRead("/proc/cpuinfo"))) {
                    string[] lines = reader.ReadToEnd().Split('\n');
                    foreach (string line in lines) {
                        if (line.Contains("Hardware", StringComparison.CurrentCultureIgnoreCase)) {
                            string[] tabSplit = line.Split('\t');
                            string hardware = tabSplit[1].Trim();
                            if (hardware.Equals("BCM2835", StringComparison.CurrentCultureIgnoreCase))
                                return true;
                            if (hardware.Equals("BCM2836", StringComparison.CurrentCultureIgnoreCase))
                                return true;
                            if (hardware.Equals("BCM2837", StringComparison.CurrentCultureIgnoreCase))
                                return true;
                            if (hardware.Equals("BCM2837B0", StringComparison.CurrentCultureIgnoreCase))
                                return true;
                            return false;
                        }
                    }
                    return false;
                }
            }
            throw new NonLinuxOSException();
        }
    }
}
