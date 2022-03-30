using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    class ReelComponent : MonoBehaviour
    {
        public float spinRate;
        public float spinOffset;

        float stopwatch;
        public float startTime;
        public float endTime = float.PositiveInfinity;
        public float doneTime = float.PositiveInfinity;
        public AnimationCurve startCurve = AnimationCurve.Constant(0, float.PositiveInfinity, 0);
        public AnimationCurve endCurve = AnimationCurve.Constant(0, float.PositiveInfinity, 1);
        public bool done => stopwatch >= doneTime;

        List<ReelBoardComponent> boards = new List<ReelBoardComponent>();

        private void Awake()
        {
            boards = new List<ReelBoardComponent>(gameObject.GetComponentsInChildren<ReelBoardComponent>());
        }

        public void StartReel(AnimationCurve curve)
        {
            startTime = stopwatch;
            startCurve = curve;
            foreach(var board in boards)
            {
                board.StartReel(curve);
            }
        }

        public void EndReel(AnimationCurve curve, float duration, float delay = 0f)
        {
            endTime = stopwatch + delay;
            endCurve = curve;
            doneTime = stopwatch + duration + delay;
            foreach (var board in boards)
            {
                board.StartReel(curve);
            }
        }

        public float GetWrappedAngle(float angle)
        {
            return (angle % 360f + 360f) % 360f;
        }

        public ReelBoardComponent GetSelectedBoard(float angleOffset)
        {
            float fullOffset = 360f / boards.Count;
            float halfOffset = fullOffset / 2;
            for (int i = 0; i < boards.Count; i++)
            {
                float angle = GetWrappedAngle(fullOffset * i + angleOffset + halfOffset);
                if (IsSelected(angle, halfOffset))
                    return boards[i];
            }

            return null;
        }

        private static bool IsSelected(float angle, float halfOffset)
        {
            return angle > 180 - halfOffset && angle < 180 + halfOffset;
        }

        public object Select()
        {
            float angleOffset = stopwatch * spinRate + spinOffset;

            var selectBoard = GetSelectedBoard(angleOffset);

            for (int i = 0; i < boards.Count; i++)
            {
                boards[i].Select(selectBoard);
                boards[i].EndReel(AnimationCurve.EaseInOut(0f, 1f, 0.5f, 0f), 0.05f);
            }

            EndReel(AnimationCurve.EaseInOut(0f, 1f, 0.5f, 0f), 0.5f, 0.05f);

            return selectBoard.value;
        }

        public void Update()
        {
            stopwatch += Time.deltaTime;
            float fullOffset = 360f / boards.Count;
            float halfOffset = fullOffset / 2f;
            float angleOffset = stopwatch * spinRate + spinOffset;
            float distance = startCurve.Evaluate(stopwatch - startTime) * endCurve.Evaluate(stopwatch - endTime);

            for (int i = 0; i < boards.Count; i++)
            {
                float angle = GetWrappedAngle(fullOffset * i + angleOffset + halfOffset);
                boards[i].transform.localPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad)) * 1f * distance;
                boards[i].SetCursorMode(IsSelected(angle, halfOffset));
            }
        }
    }

    abstract class ReelBoardComponent : MonoBehaviour
    {
        float stopwatch;
        public float startTime;
        public float endTime = float.PositiveInfinity;
        public float selectTime = float.PositiveInfinity;
        public AnimationCurve startCurve = AnimationCurve.Constant(0, float.PositiveInfinity, 0);
        public AnimationCurve endCurve = AnimationCurve.Constant(0, float.PositiveInfinity, 1);
        public abstract object value { get; }

        SpriteRenderer spriteRenderer;
        SpriteRenderer cursorRenderer;

        private void Awake()
        {
            var locator = GetComponent<ChildLocator>();
            spriteRenderer = locator.FindChildComponent<SpriteRenderer>("Sprite");
            cursorRenderer = locator.FindChildComponent<SpriteRenderer>("Cursor");
        }

        public void SetCursorMode(bool active)
        {
            cursorRenderer.enabled = active;
        }

        public void Select(ReelBoardComponent selectBoard)
        {
            selectTime = stopwatch;
            spriteRenderer.sprite = selectBoard.spriteRenderer.sprite;
        }

        public void StartReel(AnimationCurve curve)
        {
            startTime = stopwatch;
            startCurve = curve;
        }

        public void EndReel(AnimationCurve curve, float delay = 0f)
        {
            endTime = stopwatch + delay;
            endCurve = curve;
        }

        public void Update()
        {
            stopwatch += Time.deltaTime;
            float selectBonk = 1 + Mathf.Sin(Mathf.Clamp01((stopwatch - selectTime) / 0.3f) * Mathf.PI) * 0.5f;
            float scale = startCurve.Evaluate(stopwatch - startTime) * endCurve.Evaluate(stopwatch - endTime) * selectBonk;

            transform.localScale = Vector3.one * scale;
        }
    }
}
