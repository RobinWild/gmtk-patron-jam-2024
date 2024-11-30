using UnityEngine;
using Spine;

namespace Spine.Unity {
    public class GenericSpineAnimation : MonoBehaviour {
        #region Inspector
        [SpineAnimation] public string nullAnimation;
        [SpineAnimation] public string rootAnimation;
        #endregion

        [Header("Skin")]
        [SpineSkin]
        public string skinName;

        [Header("Animation Tracks")]
        private int rootTrack = 0;

        // Components
        public SkeletonAnimation skeletonAnimation;
        public Spine.AnimationState spineAnimationState;
        public Spine.Skeleton skeleton;

        // Callbacks

        protected virtual void Start() {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            skeleton = skeletonAnimation.Skeleton;
            spineAnimationState = skeletonAnimation.AnimationState;

            skeletonAnimation.Initialize(false);
            if (rootAnimation != ""){
                SetAnimation(rootAnimation, rootTrack, 0, 0, false, false);
            }

            SetSkin();
        }

        private void SetSkin()
        {
            if (string.IsNullOrEmpty(skinName))
                return;

            var skin = skeleton.Data.FindSkin(skinName);
            if (skin != null)
            {
                skeleton.SetSkin(skin);
                skeleton.SetSlotsToSetupPose();
            }
            else
            {
                Debug.LogError("Skin with name '" + skinName + "' not found!");
            }
        }

        public void SetAnimation(
            string animationName,
            int trackIndex,
            float playbackSpeed = 1f,
            float blendTime = 0.2f,
            bool loop = true,
            bool additive = true,
            bool overwrite = false,
            bool clearAfterCompleting = false,
            float alpha = 1.0f)
        {
            TrackEntry currentAnimation = spineAnimationState.GetCurrent(trackIndex);

            if (ShouldSetNewAnimation(currentAnimation, animationName, overwrite) && !string.IsNullOrEmpty(animationName))
            {
                ApplyAnimationMix(currentAnimation, animationName, blendTime);
                spineAnimationState.SetAnimation(trackIndex, animationName, loop);
            }

            ConfigureAnimation(trackIndex, playbackSpeed, alpha, additive, clearAfterCompleting);
        }

        private bool ShouldSetNewAnimation(TrackEntry currentAnimation, string animationName, bool overwrite)
        {
            return currentAnimation == null || currentAnimation.Animation.Name != animationName || overwrite;
        }

        private void ApplyAnimationMix(TrackEntry currentAnimation, string animationName, float blendTime)
        {
            if (currentAnimation != null)
            {
                spineAnimationState.Data.SetMix(currentAnimation.Animation.Name, animationName, blendTime);
            }
        }

        private void ConfigureAnimation(int trackIndex, float playbackSpeed, float alpha, bool additive, bool clearAfterCompleting)
        {
            TrackEntry newAnimation = spineAnimationState.GetCurrent(trackIndex);
            newAnimation.TimeScale = playbackSpeed;
            newAnimation.Alpha = alpha;
            newAnimation.MixBlend = additive ? MixBlend.Add : MixBlend.Replace;

            if (clearAfterCompleting)
            {
                newAnimation.Complete += ClearTrackAfterCompleting;
            }
        }


        private void ClearTrackAfterCompleting(TrackEntry trackEntry)
        {
            int trackIndex = trackEntry.TrackIndex;
            spineAnimationState.ClearTrack(trackIndex);
        }

        public float CalculatePlaybackSpeedForDuration(string animationName, float desiredDuration)
        {
            Spine.Animation animation = skeleton.Data.FindAnimation(animationName);
            if (animation != null)
            {
                float animationDuration = animation.Duration;
                if (animationDuration > 0f)
                {
                    return animationDuration / desiredDuration;
                }
                else
                {
                    Debug.LogWarning("Animation '" + animationName + "' has a duration of 0. Please check your Spine animation.");
                    return 1f; // Default playback speed if animation duration is 0.
                }
            }
            else
            {
                Debug.LogError("Animation with name '" + animationName + "' not found!");
                return 1f; // Default playback speed if animation is not found.
            }
        }

    }
}


