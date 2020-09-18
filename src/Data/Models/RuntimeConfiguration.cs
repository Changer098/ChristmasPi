﻿using System;
using ChristmasPi.Util.Arguments;

namespace ChristmasPi.Data.Models {
    public class RuntimeConfiguration {
        [HelpSection("Debug")]
        [Argument("tr", "Allows the test renderer to be selected during setup", true)]
        public bool AllowTestRenderer;

        [HelpSection("Debug")]
        [Argument("ignorepriv", "Ignore admin privilege requirement", true)]
        public bool IgnorePrivileges;

        [HelpSection("Debug")]
        [Argument("debug", "Sets the logging level to debug", true)]
        public bool DebugLogging;

        [HelpSection("Debug")]
        [Argument("log-asp", "Additionally logs ASP.NET data to a seperate log file", true)]
        public bool ASPLogging;

        [HelpSection("Debug")]
        [Argument("ignore-restart", "Ignore any restart attempts", true)]
        public bool IgnoreRestarts;

        [HelpSection("Debug")]
        [Argument("daemon-log-console", "Allow console logging in daemon mode", true)]
        public bool DaemonLogToConsole;

        [Argument("daemon", "Starts the server in a daemon configuration", true)]
        public bool DaemonMode;

        public RuntimeConfiguration() {
            AllowTestRenderer = false;
            IgnorePrivileges = false;
            DebugLogging = false;
            ASPLogging = false;
            IgnoreRestarts = false;
            DaemonMode = false;
            DaemonLogToConsole = false;
        }
    }
}
