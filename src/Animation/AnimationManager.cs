using System;
using System.Linq;
using System.Collections.Generic;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Animation.Animations;
using ChristmasPi.Animation.BranchAnimations;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using Serilog;

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
                IAnimatable anim = null;
                try {
                    anim = (IAnimatable)Activator.CreateInstance(Type.GetType(classname));
                }
                catch (Exception e) {
                    Log.ForContext<AnimationManager>().Error(e, "An exception ocurred creating an instance of class {classname}", classname);
                    Environment.Exit(Constants.EXIT_INIT_FAILURE);
                }
                if (anim.isBranchAnimation)
                    (anim as IBranchAnimation).Init(ConfigurationManager.Instance.CurrentTreeConfig.tree.branches.ToArray());
                // resolve properties
                AnimationInfo config = ConfigurationManager.Instance.CurrentTreeConfig.GetAnimation(anim.Name);
                if (anim is IAnimation) {
                    if (config != null)
                        (anim as IAnimation).AddProperties(config.Properties);
                    (anim as IAnimation).RegisterProperties();
                }
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
                typeof(random).FullName,
                typeof(flash).FullName,
                typeof(rainbow).FullName,
                typeof(randomcolor).FullName,
                typeof(randomcolorwstate).FullName,
                typeof(randombranchcolor).FullName,
                typeof(twinkle).FullName
            };
        }

        public string[] GetAnimations(bool overrideDebugFlag = false) {
            ICollection<string> keys = Animations.Keys;
            List<string> animationList = new List<string>();
            foreach (string key in keys) {
                if (Animations[key].isDebugAnimation) {
                    // If AllowDebugAnimations is true and override debug flag is false, add the animation
                    // If override debug flag is true and AllowDebugAnimations, don't add the animation
                    if (!(ConfigurationManager.Instance.RuntimeConfiguration.AllowDebugAnimations && !overrideDebugFlag))
                        continue;
                }
                animationList.Add(key);
            }
            return animationList.ToArray();
        }
        public ActiveAnimationProperty[] GetAnimationProperties(string animation) {
            if (!Animations.ContainsKey(animation))
                return null;
            IAnimatable animationObj = Animations[animation];
            return animationObj.GetActiveAnimationProperties();
        }
    }
}