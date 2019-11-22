﻿using System;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Animations {
    public class random : BaseAnimation {

        public override string Name => "Random";

        public override void construct(int lightcount, int fps) {
            base.construct(lightcount, fps);
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomColorGenerator), lightcount));
            list.Add(new SleepFrame(0.2f));
            list.Add(new ColorFrame(new RandomColor(RandomColor.RandomColorGenerator), lightcount));
            list.Add(new SleepFrame(0.2f));
        }
    }
}