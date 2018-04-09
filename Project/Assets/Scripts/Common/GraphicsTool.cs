using UnityEngine;
using System.Collections;

public class GraphicsTool {
    //创建正交投影矩阵
    public static void CreateOrthogonalProjectMatrix(ref Matrix4x4 projectMatrix, Vector3 maxInViewSpace, Vector3 minInViewSpace)
    {
        float scaleX, scaleY, scaleZ;
        float offsetX, offsetY, offsetZ;
        scaleX = 2.0f / (maxInViewSpace.x - minInViewSpace.x);
        scaleY = 2.0f / (maxInViewSpace.y - minInViewSpace.y);
        offsetX = -0.5f * (maxInViewSpace.x + minInViewSpace.x) * scaleX;
        offsetY = -0.5f * (maxInViewSpace.y + minInViewSpace.y) * scaleY;
        scaleZ = 1.0f / (maxInViewSpace.z - minInViewSpace.z);
        offsetZ = -minInViewSpace.z * scaleZ;

        //列矩阵
        projectMatrix.m00 = scaleX; projectMatrix.m01 = 0.0f; projectMatrix.m02 = 0.0f; projectMatrix.m03 = offsetX;
        projectMatrix.m10 = 0.0f; projectMatrix.m11 = scaleY; projectMatrix.m12 = 0.0f; projectMatrix.m13 = offsetY;
        projectMatrix.m20 = 0.0f; projectMatrix.m21 = 0.0f; projectMatrix.m22 = scaleZ; projectMatrix.m23 = offsetZ;
        projectMatrix.m30 = 0.0f; projectMatrix.m31 = 0.0f; projectMatrix.m32 = 0.0f; projectMatrix.m33 = 1.0f;
    }

    //创建视图矩阵
    public static void CreateViewMatrix(ref Matrix4x4 viewMatrix, Vector3 look, Vector3 up, Vector3 right, Vector3 pos)
    {
        look.Normalize();
        up.Normalize();
        right.Normalize();

        float x = -Vector3.Dot(right, pos);
        float y = -Vector3.Dot(up, pos);
        float z = -Vector3.Dot(look, pos);

        viewMatrix.m00 = right.x; viewMatrix.m10 = up.x; viewMatrix.m20 = look.x; viewMatrix.m30 = 0.0f;
        viewMatrix.m01 = right.y; viewMatrix.m11 = up.y; viewMatrix.m21 = look.y; viewMatrix.m31 = 0.0f;
        viewMatrix.m02 = right.z; viewMatrix.m12 = up.z; viewMatrix.m22 = look.z; viewMatrix.m32 = 0.0f;
        viewMatrix.m03 = x; viewMatrix.m13 = y; viewMatrix.m23 = z; viewMatrix.m33 = 1.0f;
    }

    //画矩形
    public static void DrawRect(Vector2 leftTop, Vector2 rightDown, bool isFill, ref Material mat)
    {
        mat.SetPass(0);

        GL.PushMatrix();
        GL.LoadPixelMatrix(); //转成屏幕坐标

        if(isFill)
        {
            GL.Begin(GL.QUADS);
            GL.Vertex3(leftTop.x, leftTop.y, 0);
            GL.Vertex3(rightDown.x, leftTop.y, 0);
            GL.Vertex3(rightDown.x, rightDown.y, 0);
            GL.Vertex3(leftTop.x, rightDown.y, 0);
        }

        
        GL.Begin(GL.LINES);

        //上边
        GL.Vertex3(leftTop.x, leftTop.y, 0);
        GL.Vertex3(rightDown.x, leftTop.y, 0);
        //下边
        GL.Vertex3(leftTop.x, rightDown.y, 0);
        GL.Vertex3(rightDown.x, rightDown.y, 0);
        //左边
        GL.Vertex3(leftTop.x, leftTop.y, 0);
        GL.Vertex3(leftTop.x, rightDown.y, 0);
        //右边
        GL.Vertex3(rightDown.x, leftTop.y, 0);
        GL.Vertex3(rightDown.x, rightDown.y, 0);

        GL.End();

        GL.PopMatrix();
    }
}
