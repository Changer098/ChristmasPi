﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    /// <summary>
    /// Color is an invalid format
    /// </summary>
    public class InvalidColorFormatException : Exception {
        public InvalidColorFormatException() {
        }

        public InvalidColorFormatException(string message)
            : base(message) {
        }

        public InvalidColorFormatException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
