using System.Collections.Generic;
using ActionFrame.Runtime;
using UnityEditor;
using UnityEngine;

namespace ActionFrame.Editor
{
    public class SceneViewDrawer
    {
        public static void DrawRect(Matrix4x4 localToWorld, List<BoxItem> ranges, Color color)
        {
            Matrix4x4 localToWorldNoScale = Matrix4x4.TRS(localToWorld.MultiplyPoint3x4(Vector2.zero), localToWorld.rotation, Vector2.one);

            Matrix4x4 oldMat = Handles.matrix;
            Handles.matrix = localToWorldNoScale;
            foreach (var box in ranges)
            {
                DrawRange(box, color);
            }
            Handles.matrix = oldMat;
        }
        
        private static void DrawRange(BoxItem config, Color color)
        {
            HandlesDrawer.H.PushColor(color);
            HandlesDrawer.H.fillPolygon = true;

            Matrix4x4 matrix = Matrix4x4.Translate(config.Offset);
            matrix = MathUtility.MatrixRotate(matrix, config.Rotation);

            HandlesDrawer.H.DrawRect(config.Size, matrix);
            
            HandlesDrawer.H.fillPolygon = false;
            HandlesDrawer.H.PopColor();
        }
    }
}