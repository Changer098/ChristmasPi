using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Operations.Modes {
    public class SetupMode : IOperationMode, ISetupMode {
        #region Properties
        public string Name => "SetupMode";
        #endregion
        #region Fields
        private string[] steps;
        public string CurrentStep { get; private set; }
        private TreeConfiguration newConfiguration; 
        #endregion
        public SetupMode() {
            steps = new string[] {
                "start",
                "hardware",
                "lights",
                "branches",
                "defaults",
                "finished"
            };
            SetCurrentStep(null);
        }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
            newConfiguration = ConfigurationManager.Instance.CurrentTreeConfig;
            SetCurrentStep("start");
        }
        public void Deactivate() {
            SetCurrentStep("null");
        }
        public object Info() {
            return new {};
        }
        public object GetProperty(string property) {
            return null;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets the next step in the setup process
        /// </summary>
        /// <param name="currentpage">The current step in the setup process</param>
        /// <returns>The name of the next setup step</returns>
        public string GetNext(string currentpage) {
            for (int i = 0; i < steps.Length; i++) {
                if (steps[i].Equals(currentpage, StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 >= steps.Length)
                        return null;
                    return steps[i+1];
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the current step in the setup process
        /// </summary>
        /// <param name="newstep">The new setup step</param>
        /// <remarks>This should only be called by the SetupController</remarks>
        public void SetCurrentStep(string newstep) {
            CurrentStep = newstep;
        }

        /// <summary>
        /// Complete the setup process and switch to the default operating mode
        /// </summary>
        public void Finish() {
            // set firstrun to false
            // set current configuration
            // save configuration
        }

        /// <summary>
        /// Sets the hardware info
        /// </summary>
        /// <param name="rendererType">The type of renderer to use</param>
        /// <param name="datapin">The datapin to use</param>
        /// <returns>True if the hardware settings are valid or false if not valid</returns>
        public bool SetHardware(RendererType rendererType, int datapin) {
            // test if renderer settings are correct
            if (RenderFactory.TestRender(rendererType, datapin)) {
                newConfiguration.hardware.datapin = datapin;
                newConfiguration.hardware.type = rendererType;
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Sets the info for the lights
        /// </summary>
        /// <param name="lightcount">How many lights on the tree</param>
        /// <param name="fps">How quickly animations should be rendered</param>
        /// <param name="brightness">How bright the tree should be</param>
        /// <returns>True if the info is valid, false if parameters are incorrect</returns>
        public bool SetLights(int lightcount, int fps, int brightness) {
            // limit fps to 1-Constants.FPS_MAX and brightness to 0-255, 1 - lightcount to Constants.LIGHTS_MAX
            if (lightcount < 1)
                return false;
            if (fps < 1 || fps > Constants.FPS_MAX)
                return false;
            if (brightness < 0 || brightness > 255)
                return false;
            newConfiguration.hardware.brightness = brightness;
            newConfiguration.hardware.lightcount = lightcount;
            newConfiguration.hardware.fps = fps;
            return true;
        }
        #endregion
    }
}