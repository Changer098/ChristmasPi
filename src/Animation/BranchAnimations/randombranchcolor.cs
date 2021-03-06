﻿using System;
using System.Drawing;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Animation.Interfaces;

namespace ChristmasPi.Animation.BranchAnimations {
    public class randombranchcolor : BaseBranchAnimation {
        public override string Name => "Random Color Branch";
        private readonly float SLEEP_TIME = 0.25f;

        public randombranchcolor() : base() { }

        public override void constructbranch(int fps, ref BranchData branch) {
            base.constructbranch(fps, ref branch);
            objectstate state = new objectstate(branch.LightCount);

            branch.Add(new ColorFrame(new RandomColor(generator, state), branch.LightCount));
            branch.Add(new SleepFrame(SLEEP_TIME));
            branch.Add(new ColorFrame(new RandomColor(generator, state), branch.LightCount));
            branch.Add(new SleepFrame(SLEEP_TIME));
        }

        public Color generator(object state) {
            objectstate objState = (objectstate)state;
            if (objState.count == objState.lightcount) {
                objState.count = 0;
                objState.color = RandomColor.RandomKnownColorGenerator();
            }
            objState.count += 1;
            return objState.color;
        }
    }

    // must use classes and not structs!
    // structs are passed by copy
    public class objectstate {
        public int count;
        public int lightcount;
        public Color color;

        public objectstate(int lightcount) {
            this.count = 0;
            this.lightcount = lightcount;
            this.color = Constants.COLOR_OFF;
        }
    }
}
