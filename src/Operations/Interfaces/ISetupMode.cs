﻿namespace ChristmasPi.Operations.Interfaces {
    public interface ISetupMode {
        string GetNext(string current);
        void SetCurrentStep(string currentstep);
    }
}
