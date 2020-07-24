using System;
using System.Collections;
using CoreSystem.Additional;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreSystem.Utility
{
    public class ShakeManager : MonoBehaviour
    {
        [SerializeField]
        private NoiseSettings[] profiles;

        public bool isPlaying = false;
        public bool usePosNoise = true;
        public bool useRotNoise = true;
        [Tooltip("Gain to apply to the amplitudes defined in the NoiseSettings asset.  1 is normal.  Setting this to 0 completely mutes the noise.")]
        public float mAmplitudeGain = 1f;
        [Tooltip("Scale factor to apply to the frequencies defined in the NoiseSettings asset.  1 is normal.  Larger magnitudes will make the noise shake more rapidly.")]
        public float mFrequencyGain = 1f;

        private bool mInitialized = false;
        private float mNoiseTime = 0f;
        [SerializeField][HideInInspector]
        private Vector3 mNoiseOffsets = Vector3.zero;

        public BehaviourJob Shake(int index, Transform shakeTransform, float amplitude = -1f, float frequency = -1f,float duration = 0f, Action<bool> onComplete = null)
        {
            mInitialized = false;
            mAmplitudeGain = amplitude;
            mFrequencyGain = frequency;
            var job = BehaviourJob.Make(IShake(index, shakeTransform, duration));
            job.OnComplete += onComplete;
            return job;
        }

        private IEnumerator IShake(int index, Transform originTransform, float duration)
        {
            if(!mInitialized)
                Initialize();
            isPlaying = true;
            var noiseProfile = profiles[index];
            var originPos = originTransform.localPosition;
            var originRot = originTransform.localRotation;
            var correctedOrientation = originTransform.localRotation;
            var correctionPos = Vector3.zero;
            var correctionRot = Quaternion.identity;
            var timer = 0f;

            if (duration <= 0f)
                duration = float.PositiveInfinity;
            
            while (isPlaying && timer <= duration)
            {
                timer += Time.deltaTime;
                mNoiseTime += Time.deltaTime * mFrequencyGain;
                if (usePosNoise)
                {
                    correctionPos = originPos + correctedOrientation * NoiseSettings.GetCombinedFilterResults(
                                noiseProfile.PositionNoise,mNoiseTime,mNoiseOffsets)*mAmplitudeGain;
                    originTransform.localPosition = correctionPos;
                }
                if (useRotNoise)
                {
                    correctionRot = Quaternion.Euler(NoiseSettings.GetCombinedFilterResults(
                        noiseProfile.OrientationNoise, mNoiseTime, mNoiseOffsets) * mAmplitudeGain);
                    originTransform.localRotation = originRot * correctionRot;
                }
                yield return null;
            }
            originTransform.localPosition = originPos;
        }
        /// <summary>Generate a new random seed</summary>
        public void ReSeed()
        {
            mNoiseOffsets = new Vector3(
                Random.Range(-1000f, 1000f),
                Random.Range(-1000f, 1000f),
                Random.Range(-1000f, 1000f));
        }

        void Initialize()
        {
            mInitialized = true;
            mNoiseTime = 0;
            if (mNoiseOffsets == Vector3.zero)
                ReSeed();
        }
    }
}
