using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 战争迷雾
/// 挂载到主摄像机上
/// 参考：http://www.ceeger.com/forum/read.php?tid=22738&fid=2
/// </summary>
public class FogOfWar : MonoBehaviour {
    public LayerMask fogCoverLayer = 1;

    [Range(0, 2)]
    public float brushSize = 0.5f; //viewer影响空白的范围
    [Range(0, 1)]
    public float edgeSmoothValue = 0.1f; //影响黑雾的过渡效果
    [Range(0, 1)]
    public float fogDensity = 0.9f; //黑雾的深浅
    
    private Matrix4x4 mViewMatrix = Matrix4x4.identity;
    private Matrix4x4 mProjMatrix = Matrix4x4.identity;

    private RenderTexture mWarFogRTT = null;
    private Material mWarFogMaterial = null;
    private Material mWarFogCastMaterial = null;
    private Projector mWarFogProjector = null;
    private Texture2D mPerlinNoise = null;

    private const int WAR_FOG_RTT_SIZE = 128;
    private float mAspectRatio = 1.0f;

    void Start () {
        CreateMaterial();
        CreateWarFogRTT();
        CreateCastProjector();
        CreateProjectorMatrix();
        NoiseTool.CreatePerlinNoise(ref mPerlinNoise, WAR_FOG_RTT_SIZE, WAR_FOG_RTT_SIZE, 10, Vector2.zero);

        Matrix4x4 matVP = GL.GetGPUProjectionMatrix(mProjMatrix, true) * mViewMatrix;
        mWarFogProjector.material.SetTexture("_FogOfWarTex", mWarFogRTT);
        mWarFogProjector.material.SetMatrix("_MatCastViewProj", matVP);

        mWarFogMaterial.SetTexture("_NoiseTex", mPerlinNoise);
        mWarFogMaterial.SetFloat("_AspectRatio", mAspectRatio);
        mWarFogMaterial.SetFloat("_FogDensity", fogDensity);
    }
	
    //TODO：这里可能影响效率，有效率问题时来这看看
    void OnPreRender()
    {
        List<RobotData> viewerList = Player.Instance.RobotList;
        if (mWarFogMaterial == null || viewerList.Count == 0)
            return;

        for(int i = 0; i < viewerList.Count; i++)
        {
            Vector3 viewerWorldPos = viewerList[i].transform.position;
            Vector4 viewCenter = ConvertWorldToUV(viewerWorldPos);

            mWarFogMaterial.SetVector("_ViewCenter", viewCenter);
            mWarFogMaterial.SetFloat("_BrushSize", brushSize);
            mWarFogMaterial.SetFloat("_EdgeSmoothValue", edgeSmoothValue);

            RenderTexture ac = RenderTexture.active;
            RenderTexture.active = mWarFogRTT;
            GL.Clear(false, false, Color.black); //这里设置false来不清理深度和颜色
            Graphics.Blit(null, mWarFogRTT, mWarFogMaterial); //用Shader将RTT渲染到屏幕上
            RenderTexture.active = ac;
        }
    }

	void OnDestroy()
    {
        ClearRes(mWarFogRTT);
        ClearRes(mPerlinNoise);
        if(mWarFogProjector != null)
        {
            Destroy(mWarFogProjector.gameObject);
            mWarFogProjector = null;
        }
    }
    
    void CreateMaterial()
    {
        mWarFogMaterial = new Material(Shader.Find("Kaima/FogOfWar"));
        mWarFogCastMaterial = new Material(Shader.Find("Kaima/FogOfWarCast"));
    }

    void CreateWarFogRTT()
    {
        int w = WAR_FOG_RTT_SIZE;
        int h = WAR_FOG_RTT_SIZE;
        mWarFogRTT = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
        mWarFogRTT.wrapMode = TextureWrapMode.Clamp;
        mWarFogRTT.name = "War Fog RTT";
        mWarFogRTT.antiAliasing = 1;

        RenderTexture mainRTT = RenderTexture.active;
        RenderTexture.active = mWarFogRTT;
        GL.Clear(true, true, new Color(0, 0, 0, 1), 1.0f); //清理新创建的RTT，涂上黑色
        RenderTexture.active = mainRTT;
    }

    void CreateCastProjector()
    {
        Transform projectorTrans = new GameObject("WarFogCastProjector").transform;
        projectorTrans.SetParent(transform);
        projectorTrans.localPosition = Vector3.zero;
        projectorTrans.localRotation = new Quaternion(0, 0, 0, 1);
        projectorTrans.localScale = Vector3.one;

        mWarFogProjector = projectorTrans.gameObject.AddComponent<Projector>();
        mWarFogProjector.orthographic = true;
        mWarFogProjector.orthographicSize = 100f;
        mWarFogProjector.nearClipPlane = -100f;
        mWarFogProjector.farClipPlane = 100f;
        mWarFogProjector.material = mWarFogCastMaterial;
        mWarFogProjector.ignoreLayers = ~fogCoverLayer;
        mWarFogProjector.enabled = true;
    }

    void CreateProjectorMatrix()
    {
        Vector3 look = mWarFogProjector.transform.rotation * new Vector3(0, 0, -1);
        Vector3 up = mWarFogProjector.transform.rotation * new Vector3(0, 1, 0);
        Vector3 right = Vector3.Cross(look, up);
        GraphicsTool.CreateViewMatrix(ref mViewMatrix, look, up, right, mWarFogProjector.transform.position);

        //计算Projector视图空间的AABB
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(GameConfig.Instance.TopLeft());
        vertices.Add(GameConfig.Instance.BottomLeft());
        vertices.Add(GameConfig.Instance.BottomRight());
        vertices.Add(GameConfig.Instance.TopRight());

        Vector3 maxPosition = Vector3.one * float.MinValue;
        Vector3 minPosition = Vector3.one * float.MaxValue;

        for(int vertId = 0; vertId < 4; vertId++)
        {
            Vector3 pos = mViewMatrix.MultiplyPoint3x4(vertices[vertId]); //将边界世界坐标转到Projector的视图空间里
            maxPosition.x = Mathf.Max(maxPosition.x, pos.x);
            maxPosition.y = Mathf.Max(maxPosition.y, pos.y);
            maxPosition.z = Mathf.Max(maxPosition.z, pos.z);
            minPosition.x = Mathf.Min(minPosition.x, pos.x);
            minPosition.y = Mathf.Min(minPosition.y, pos.y);
            minPosition.z = Mathf.Min(minPosition.z, pos.z);
        }

        GraphicsTool.CreateOrthogonalProjectMatrix(ref mProjMatrix, maxPosition, minPosition);
        mAspectRatio = (maxPosition.x - minPosition.x) / (maxPosition.y - minPosition.y);
    }

    Vector4 ConvertWorldToUV(Vector3 worldPos)
    {
        Vector4 ret = Vector4.zero;
        Matrix4x4 matVP = GL.GetGPUProjectionMatrix(mProjMatrix, true) * mViewMatrix;
        Vector3 projPos = matVP.MultiplyPoint3x4(worldPos);
        float xBetween0And1 = projPos.x * 0.5f + 0.5f;
        float yBetween0And1 = projPos.y * 0.5f + 0.5f;
        ret = new Vector4(xBetween0And1, yBetween0And1, 0, 0);
        return ret;
    }

    void ClearRes(Object res)
    {
        if(res != null)
        {
            Destroy(res);
            res = null;
        }
    }
}
