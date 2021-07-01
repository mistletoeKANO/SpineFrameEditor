using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ActionFrame.Runtime
{
    /// <summary>
    /// DrawUtility
    /// </summary>
    public abstract class DrawUtility
    {
        #region implement

        public readonly static Color defualtColor = Color.white;
        public readonly static GizmosDrawer G = new GizmosDrawer();
        public readonly static DebugDrawer D = new DebugDrawer();

        #endregion implement

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public abstract void DrawLine(Vector3 start, Vector3 end);

        public virtual Color color { get; set; }
        public int subdivide = 30;
        public bool fillPolygon = false;
        public bool useFillPolygonOutline = true;
        public Color outlineColor => new Color(1, 1, 1, color.a);

        protected Stack<Color> _colorStack = new Stack<Color>();

        [Conditional("UNITY_EDITOR")]
        public void PushColor(Color color)
        {
            _colorStack.Push(this.color);
            this.color = color;
        }

        [Conditional("UNITY_EDITOR")]
        public void PopColor()
        {
            this.color = _colorStack.Count > 0 ? _colorStack.Pop() : defualtColor;
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void ClearColor()
        {
            _colorStack.Clear();
            color = defualtColor;
        }

        [Conditional("UNITY_EDITOR")]
        public void DrawLines(params Vector3[] vertices)
        {
            for (int i = 1; i < vertices.Length; i++)
            {
                DrawLine(vertices[i - 1], vertices[i]);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public void DrawPolygon(Vector3[] vertices)
        {
            if (fillPolygon)
            {
                FillPolygon(vertices);

                if (useFillPolygonOutline)
                {
                    PushColor(outlineColor);
                    for (int i = vertices.Length - 1, j = 0; j < vertices.Length; i = j, j++)
                    {
                        DrawLine(vertices[i], vertices[j]);
                    }
                    PopColor();
                }
            }
            else
            {
                for (int i = vertices.Length - 1, j = 0; j < vertices.Length; i = j, j++)
                {
                    DrawLine(vertices[i], vertices[j]);
                }
            }
        }

        [Conditional("UNITY_EDITOR")]
        protected virtual void FillPolygon(Vector3[] vertices)
        {
            for (int i = vertices.Length - 1, j = 0; j < vertices.Length; i = j, j++)
            {
                DrawLine(vertices[i], vertices[j]);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public void DrawBox(Vector3 size, Matrix4x4 matrix)
        {
            Vector3[] points = MathUtility.CalcBoxVertex(size, matrix);
            int[] indexs = MathUtility.GetBoxSurfaceIndex();
            for (int i = 0; i < 6; i++)
            {
                Vector3[] polygon = new Vector3[] {
                    points[indexs[i * 4]],
                    points[indexs[i * 4 + 1]],
                    points[indexs[i * 4 + 2]],
                    points[indexs[i * 4 + 3]] };
                DrawPolygon(polygon);
            }
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public virtual void DrawSphere(float radius, Matrix4x4 matrix)
        {
            Matrix4x4 lookMatrix = Matrix4x4.identity;

#if UNITY_EDITOR
            UnityEditor.SceneView sceneView = UnityEditor.SceneView.currentDrawingSceneView;
            if (sceneView != null)
            {
                Camera cam = sceneView.camera;
                var cameraTrans = cam.transform;
                var rotation = Quaternion.LookRotation(cameraTrans.position - matrix.MultiplyPoint(Vector3.zero));
                lookMatrix = Matrix4x4.TRS(matrix.MultiplyPoint(Vector3.zero), rotation, matrix.lossyScale);
                DrawCircle(radius, lookMatrix);
            }
#endif

            //绘制边界
            bool oldFillColor = fillPolygon;
            fillPolygon = false;
            PushColor(outlineColor);
            DrawCircle(radius, matrix);
            DrawCircle(radius, matrix * Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0)));
            DrawCircle(radius, matrix * Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0)));
            PopColor();
            fillPolygon = oldFillColor;
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCross(float size, Matrix4x4 matrix)
        {
            float halfSize = size / 2f;

            Vector3[] points = new Vector3[6];
            points[0] = matrix.MultiplyPoint(Vector3.up * halfSize);
            points[1] = matrix.MultiplyPoint(Vector3.down * halfSize);
            points[2] = matrix.MultiplyPoint(Vector3.left * halfSize);
            points[3] = matrix.MultiplyPoint(Vector3.right * halfSize);
            points[4] = matrix.MultiplyPoint(Vector3.forward * halfSize);
            points[5] = matrix.MultiplyPoint(Vector3.back * halfSize);

            DrawLine(points[0], points[1]);
            DrawLine(points[2], points[3]);
            DrawLine(points[4], points[5]);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawArrow(Vector3 start, Vector3 end, float size)
        {
            DrawCross(size, Matrix4x4.Translate(start));
            DrawLine(start, end);

            Matrix4x4 endMatrix = Matrix4x4.TRS(end, Quaternion.FromToRotation(Vector3.up, end - start), Vector3.one);

            Vector3[] leftRight = new Vector3[3];
            Vector3[] forwardBack = new Vector3[3];
            float angle = 60;
            forwardBack[0] = endMatrix.MultiplyPoint(Quaternion.AngleAxis(angle, Vector3.right) * Vector3.forward * size);
            forwardBack[1] = end;
            forwardBack[2] = endMatrix.MultiplyPoint(Quaternion.AngleAxis(angle, Vector3.left) * Vector3.back * size);
            leftRight[0] = endMatrix.MultiplyPoint(Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.left * size);
            leftRight[1] = end;
            leftRight[2] = endMatrix.MultiplyPoint(Quaternion.AngleAxis(angle, Vector3.back) * Vector3.right * size);
            DrawLines(forwardBack);
            DrawLines(leftRight);
            DrawLines(forwardBack[0], leftRight[0], forwardBack[2], leftRight[2], forwardBack[0]);
        }

        [Conditional("UNITY_EDITOR")]
        public void DrawRect(Vector2 size, Matrix4x4 matrix)
        {
            Vector3[] vertices = MathUtility.CalcRectVertex(size, matrix);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCircle(float radius, Matrix4x4 matrix)
        {
            Vector3[] vertices = MathUtility.CalcCircleVertex(radius, matrix, subdivide);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawArc(float radius, float angle, float rotation, Matrix4x4 matrix)
        {
            Vector3[] vertices = MathUtility.CalcArcVertex(radius, angle, rotation, matrix, true, subdivide);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCapsule2D(float height, float radius, Matrix4x4 matrix)
        {
            Vector3[] vertices = MathUtility.CalcCapsuleVertex2D(height, radius, matrix, subdivide);
            DrawPolygon(vertices);
        }

        #region Extersion

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawPolygon(Vector3[] vertices, Matrix4x4 matrix)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = matrix.MultiplyPoint(vertices[i]);
            }
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawPolygon(Vector3[] vertices, Color color)
        {
            PushColor(color);
            DrawPolygon(vertices);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawPolygon(Vector3[] vertices, Matrix4x4 matrix, Color color)
        {
            PushColor(color);
            DrawPolygon(vertices, matrix);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawBox(Vector3 position, Vector3 size, Color color)
        {
            PushColor(color);
            DrawBox(size, Matrix4x4.Translate(position));
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawSphere(Vector3 position, float radius, Color color)
        {
            PushColor(color);
            DrawSphere(radius, Matrix4x4.Translate(position));
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCross(Vector3 position, float size, Color color)
        {
            PushColor(color);
            DrawCross(size, Matrix4x4.Translate(position));
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawArc(Vector3 position, float radius, float angle, float rotation)
        {
            Vector3[] vertices = MathUtility.CalcArcVertex(radius, angle, rotation, Matrix4x4.Translate(position), true, subdivide);
            DrawPolygon(vertices);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawArc(Vector3 position, float radius, float angle, float rotation, Color color)
        {
            PushColor(color);
            DrawArc(position, radius, angle, rotation);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        public void DrawRect(Vector3 position, Vector2 size, Vector3 angle, Vector3 scale)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.Euler(angle), scale);
            DrawRect(size, matrix);
        }

        [Conditional("UNITY_EDITOR")]
        public void DrawRect(Vector3 position, Vector2 size, Vector3 angle, Vector3 scale, Color color)
        {
            PushColor(color);
            DrawRect(position, size, angle, scale);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        public void DrawRect(Vector3 position, Vector2 size)
        {
            DrawRect(position, size, Vector3.zero, Vector3.one);
        }

        [Conditional("UNITY_EDITOR")]
        public void DrawRect(Vector3 position, Vector2 size, Color color)
        {
            PushColor(color);
            DrawRect(position, size);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawRect(Rect rect, Color color)
        {
            DrawRect(rect.center, rect.size, color);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCircle(Vector3 position, float radius)
        {
            DrawCircle(radius, Matrix4x4.Translate(position));
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCircle(Vector3 position, float radius, Color color)
        {
            PushColor(color);
            DrawCircle(position, radius);
            PopColor();
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void DrawCameraFrustumCorners(Camera camera, Rect viewRect, Vector3 target, Color color)
        {
            //float distance = Vector3.Distance(camera.transform.position, target);
            Transform transform = camera.transform;
            Vector3[] vts = new Vector3[4];
            camera.CalculateFrustumCorners(viewRect, Vector3.Distance(transform.position, target), Camera.MonoOrStereoscopicEye.Mono, vts);
            DrawPolygon(vts, transform.localToWorldMatrix, Color.red);
            //DrawCross(transform.position + transform.forward * distance, 0.5f, color);
        }

        #endregion Extersion
    }

    /// <summary>
    /// GizmosUtility
    /// </summary>
    public class GizmosDrawer : DrawUtility
    {
        public override Color color { get => Gizmos.color; set => Gizmos.color = value; }

        [DebuggerStepThrough]
        public override void DrawLine(Vector3 start, Vector3 end)
        {
            Gizmos.DrawLine(start, end);
        }

        [DebuggerStepThrough]
        protected override void FillPolygon(Vector3[] vertices)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.DrawAAConvexPolygon(vertices);
#endif
        }
    }

    /// <summary>
    /// DebugUtility
    /// </summary>
    public class DebugDrawer : DrawUtility
    {
        /// <summary>
        /// 持续时间
        /// </summary>
        public float duration { get; set; } = 0.033f;

        public override Color color { get; set; } = DrawUtility.defualtColor;

        protected Stack<float> _durationStack = new Stack<float>();

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void PushDuration(float duration)
        {
            _durationStack.Push(this.duration);
            this.duration = duration;
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerStepThrough]
        public void PopDuration()
        {
            this.duration = _durationStack.Count > 0 ? _durationStack.Pop() : this.duration;
        }

        [DebuggerStepThrough]
        public override void DrawLine(Vector3 start, Vector3 end)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// HandlesDrawer
    /// </summary>
    public class HandlesDrawer : DrawUtility
    {
        public readonly static HandlesDrawer H = new HandlesDrawer();

        public override Color color { get => UnityEditor.Handles.color; set => UnityEditor.Handles.color = value; }
        
        public override void DrawLine(Vector3 start, Vector3 end)
        {
            UnityEditor.Handles.DrawLine(start, end);
        }
        
        protected override void FillPolygon(Vector3[] vertices)
        {
            UnityEditor.Handles.DrawAAConvexPolygon(vertices);
        }
    }
#endif
}