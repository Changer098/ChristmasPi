using System;
using System.Linq;
using System.Collections.Generic;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Animation.Animations;
using ChristmasPi.Animation.Branch;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;

namespace ChristmasPi.Animation {
    public class AnimationManager {
        #region Singleton Methods
        private static readonly AnimationManager _instance = new AnimationManager();
        public static AnimationManager Instance { get { return _instance; } }
        #endregion

        public Dictionary<string, IAnimatable> Animations;

        public void Init() {
            Animations = new Dictionary<string, IAnimatable>();
            string[] animationsClasses = getAnimationClasses();
            foreach (string classname in animationsClasses) {
                /// TODO Handle if exception occurs when creating instance or casting
                IAnimatable anim = (IAnimatable)Activator.CreateInstance(Type.GetType(classname));
                Animations.Add(anim.Name, anim);
            }
        }

        /// <summary>
        /// Gets a list of all animations in the executing assembly that implement the IAnimatable interface
        /// </summary>
        /// <returns>List of animations to instantiate</returns>
        private string[] getAnimationClasses() {
            /// TODO use reflection to get classes
            return new string[] {
                typeof(toggleeachbranch).FullName,
                typeof(flash).FullName
            };
        }

        public string[] GetAnimations() {
            ICollection<string> keys = Animations.Keys;
            return keys.ToArray<string>();
        }

    }
}