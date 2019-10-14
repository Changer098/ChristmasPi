﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Util;
using System.Drawing;

namespace ChristmasPi.Animation {
    public class Animator : IDisposable {
        private IAnimatable animation;
        private AnimationState _currentState;
        private IRenderer renderer;
        private bool useWorker;                 // Whether or not to use a background worker or render events
        private Thread workerThread;            // only if renderer does not support AutoRender
        private object locker;
        private int fps;
        private int lightcount;
        private int branchcount;
        private bool disposed;
        private bool updatedRenderer;           // Whether or not we have written data to the renderer
        private RenderFrame[] currentFrames;
        private RenderFrame[] nextFrames;
        private int currentFrameIndex;          // What frame we're on
        private CancellationTokenSource currentToken;


        public IAnimatable CurrentAnimation => animation;
        public AnimationState CurrentState => _currentState;

        public Animator(IAnimatable animation, int fps, int lightcount, int branchcount) {
            this.animation = animation;
            this.fps = fps;
            this.lightcount = lightcount;
            this.branchcount = branchcount;
            disposed = false;
            locker = new object();
            currentFrames = animation.isBranchAnimation ? null : (animation as IAnimation).GetFrames(fps, lightcount);
            _currentState = AnimationState.Stopped;
            renderer = RenderFactory.GetRenderer();
            if (renderer.AutoRender) {
                useWorker = false;
            }
            else {
                useWorker = true;
                workerThread = new Thread(work);
            }
        }

        /// <summary>
        /// Starts the animation
        /// </summary>
        public void Start() {
            lock (locker) {
                if (CurrentState == AnimationState.Animating)
                    throw new InvalidAnimationActionException("Can't start an animation that's already playing");
                else if (CurrentState == AnimationState.Paused) {
                    _currentState = AnimationState.Animating;
                    return;
                }
                _currentState = AnimationState.Animating;
                currentFrameIndex = 0;
                if (useWorker) {
                    workerThread.Start();
                }
                else {
                    // subscribe to events
                    renderer.BeforeRenderEvent += OnBeforeRender;
                    renderer.AfterRenderEvent += OnAfterRender;
                }
                renderer.Start();
            }
        }

        /// <summary>
        /// Stops the current animation
        /// </summary>
        public void Stop() {
            lock (locker) {
                if (CurrentState != AnimationState.Animating && CurrentState != AnimationState.Paused)
                    throw new InvalidAnimationActionException("Can't stop an animation that's not playing");
                _currentState = AnimationState.Stopped;
                if (useWorker) {
                    ThreadHelpers.WakeUpThread(currentToken);
                    workerThread.Join(250);
                }
                else {
                    renderer.BeforeRenderEvent -= OnBeforeRender;
                    renderer.AfterRenderEvent -= OnAfterRender;
                }
                renderer.Stop();
                updatedRenderer = false;
            }
        }

        /// <summary>
        /// Pauses an animation if possible
        /// </summary>
        public void Pause() {
            if (CurrentState != AnimationState.Animating)
                throw new InvalidAnimationActionException("Can't pause an animation that's not playing");
            if (useWorker)
                throw new InvalidAnimationActionException("Can't pause an animation while using worker threads");
            _currentState = AnimationState.Paused;
        }

        /// <summary>
        /// Resumes a paused animation
        /// </summary>
        public void Resume() {
           if (CurrentState != AnimationState.Paused)
                throw new InvalidAnimationActionException("Can't resume an animation that isn't paused");
            _currentState = AnimationState.Animating;
        }

        /// <summary>
        /// background worker thread
        /// </summary>
        /// <seealso cref="Hardware.Renderers.RenderThread.work"/>
        private async void work() {
            bool doRender = false;
            int waitTime = ThreadHelpers.CalculateWaitTime(fps);
            lock (locker) {
                doRender = (CurrentState == AnimationState.Animating);
                currentToken = ThreadHelpers.RegisterWakeUp();
            }
            while (doRender) {
                DateTime beforeRender = DateTime.Now;
                RenderFrame frame = currentFrames[currentFrameIndex];
                if (frame.Action == FrameAction.Update) {
                    Color[] colors = frame.Colors.GetColors();
                    for (int  i = 0; i < colors.Length; i++) {
                        renderer.SetLEDColor(i, colors[i]);
                    }
                    try {
                        renderer.Render(renderer);
                    }
                    catch (Exception e) {
                        Console.WriteLine("LOGTHIS An exception occurred within the animator's worker thread");
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                        Console.WriteLine("Render Thread exiting");
                        doRender = false;
                        _currentState = AnimationState.Error;
                        return;
                    }
                    TimeSpan renderTime = DateTime.Now - beforeRender;
                    int newWaitTime = waitTime;
                    if (renderTime.TotalMilliseconds > waitTime) {
                        Console.WriteLine("LOGTHIS Took longer to render frame than fps waittime");
                        Console.WriteLine($"waitTime: {waitTime}, renderTime: {renderTime}");
                    }
                    else {
                        newWaitTime = waitTime - (int)renderTime.TotalMilliseconds;
                    }
                    bool slept = await ThreadHelpers.SafeSleep(currentToken, newWaitTime);
                    if (!slept) {
                        // sleep was cancelled, exit thread
                        doRender = false;
                        return;
                    }
                }
                else if (frame.Action == FrameAction.Sleep || frame.Action == FrameAction.Blank) { }
                incrementFrame();
            }
        }

        private void OnBeforeRender(object sender, RenderArgs args) {
            if (!updatedRenderer && CurrentState == AnimationState.Animating) {
                var frame = currentFrames[currentFrameIndex];
                if (frame.Action == FrameAction.Update) {
                    Color[] colors = frame.Colors.GetColors();
                    for (int i = 0; i < colors.Length; i++) {
                        renderer.SetLEDColor(i, colors[i]);
                    }
                }
                else if (frame.Action == FrameAction.Sleep || frame.Action == FrameAction.Blank) { }
            }
        }

        private void OnAfterRender(object sender, RenderArgs args) {
            if (CurrentState == AnimationState.Animating) {
                var frame = currentFrames[currentFrameIndex];
                if (frame.Action == FrameAction.Update) {
                    Color[] colors = frame.Colors.GetColors();
                    for (int i = 0; i < colors.Length; i++) {
                        renderer.SetLEDColor(i, colors[i]);
                    }
                }
                else if (frame.Action == FrameAction.Sleep || frame.Action == FrameAction.Blank) { }
                
                incrementFrame();
            }
        }

        /// <summary>
        /// Updates the current frame index and handles wrap around
        /// </summary>
        private void incrementFrame() {
            currentFrameIndex++;
            if (currentFrameIndex >= currentFrames.Length)
                currentFrameIndex = 0;
        }

        public void Dispose() {
            if (!disposed) {
                /// TODO implement
                renderer.Dispose();
                disposed = true;
            }
        }
    }
}
