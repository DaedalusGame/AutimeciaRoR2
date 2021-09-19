using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    class WaveBeamComponent : MonoBehaviour
    {
        public float distance;
        public float initialAngle;
        public float angularVelocity;
        public float oscillateSpeed;

        float stopwatch;

        public void Update()
        {
            var angle = initialAngle + stopwatch * angularVelocity;
            transform.localPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0) * Mathf.Sin(stopwatch * oscillateSpeed * Mathf.PI * 2f) * distance;

            stopwatch += Time.deltaTime;
        }
    }
}
