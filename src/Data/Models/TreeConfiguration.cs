﻿using System;
using System.Collections.Generic;
using System.Linq;
namespace ChristmasPi.Data.Models {
    public class TreeConfiguration {
        /// <summary>
        /// Customizable information about the tree
        /// </summary>
        public TreeSettings tree { get; set; }

        /// <summary>
        /// Information for the setup process to operate
        /// </summary>
        public SetupSettings setup { get; set; }

        /// <summary>
        /// Information for renderers to interact with the hardware
        /// </summary>
        public HardwareSettings hardware { get; set; }

        public AnimationInfo[] animations { get; set; }

        /// <summary>
        /// Returns the default settings for the entire tree
        /// </summary>
        /// <returns>A TreeConfiguration object</returns>
        public static TreeConfiguration DefaultSettings() {
            TreeConfiguration config = new TreeConfiguration();
            config.tree = TreeSettings.DefaultSettings();
            config.setup = SetupSettings.DefaultSettings();
            config.hardware = HardwareSettings.DefaultSettings();
            config.animations = null;
            return config;
        }

        /// <summary>
        /// Gets info for an animation if it exists in the configuration
        /// </summary>
        /// <returns>An AnimationInfo object if the animation exists, else null</returns>
        public AnimationInfo GetAnimation(string name) {
            if (animations == null)
                return null;
            foreach (AnimationInfo info in animations) {
                if (info.Name == name)
                    return info;
            }
            return null;
        }
    }
}
