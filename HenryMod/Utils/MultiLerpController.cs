﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HenryMod.Utils
{
    class AutoLerpController : MultiLerpController
    {
        float currentValue;

        public AutoLerpController(float startValue = 0)
        {
            currentValue = startValue;
        }

        public void PushValue(float newValue, float duration)
        {
            Add(AnimationCurve.EaseInOut(0, currentValue, 1, newValue), duration);
            currentValue = newValue;
        }
    }

    class MultiLerpController
    {
        class Lerp
        {
            public float startTime;
            public float duration;
            public AnimationCurve curve;

            public Lerp(AnimationCurve curve, float startTime, float duration)
            {
                this.startTime = startTime;
                this.duration = duration;
                this.curve = curve;
            }

            public float GetSlide(float stopwatch)
            {
                return Mathf.Clamp01((stopwatch - startTime) / duration);
            }

            public float Evaluate(float stopwatch)
            {
                return curve.Evaluate(GetSlide(stopwatch));
            }
        }

        float stopwatch;
        List<Lerp> lerps = new List<Lerp>();

        public float finalSlide;

        public void Add(AnimationCurve curve, float duration)
        {
            Add(curve, 0, duration);
        }

        public void Add(AnimationCurve curve, float delay, float duration)
        {
            lerps.Add(new Lerp(curve, stopwatch + delay, duration));
        }

        public void Update()
        {
            CalculateFinalSlide();
            lerps.RemoveAll(lerp => lerp.GetSlide(stopwatch) >= 1);

            stopwatch += Time.deltaTime;
        }

        private void CalculateFinalSlide()
        {
            float finalValue = 0;
            float totalWeight = 0;

            foreach(var lerp in lerps)
            {
                var timeSlide = lerp.GetSlide(stopwatch);
                var weight = Mathf.Sin(timeSlide * Mathf.PI);
                finalValue += lerp.Evaluate(stopwatch) * weight;
                totalWeight += weight;
            }

            if (totalWeight > 0)
                finalSlide = finalValue / totalWeight;
        }
    }
}
