using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARDrawController : MonoBehaviour
{
    [SerializeField]
    private float distanceFromCamera = 0.3f;

    [SerializeField]
    private Material defaultColorMaterial;

    [SerializeField]
    private ARAnchorManager anchorManager = null;

    [SerializeField]
    private Camera arCamera = null;


    GameObject director;

    private LineRenderer currentLineRender;

    private List<ARAnchor> anchors = new List<ARAnchor>();//ARAnchor型のリストを宣言する
    private List<LineRenderer> lines = new List<LineRenderer>();

    private int positionCount = 0;

    private Vector3 prevPointDistance = Vector3.zero;
    [Range(0, 1.0f)]
    public float minDistanceBeforeNewPoint = 0.001f;

    [Header("Tolerance Options")]
    public bool allowSimplification = false;
    public float tolerance = 0.001f;
    public float applySimplifyAfterPoints = 20.0f;
    //public bool allowMultiTouch = true;
    private LineRenderer LineRenderer { get; set; }

    //Start前にGetComponentする
    private void Reset()
    {
        
    }

    void Start()
    {
        this.director = GameObject.Find("GameDirector");
    }

    void Update()
    {
        //タップの検出
        int tapCount = Input.touchCount;

        for (int i = 0; i < tapCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            //カメラ手前30cmの位置(x,y,z)
            Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, distanceFromCamera));

            //タップスタート
            if (touch.phase == TouchPhase.Began)
            {

                //実世界の位置と方向を持ったオブジェクト
                //Quaternion.identityとは、無回転状態のこと
                ARAnchor anchor = anchorManager.AddAnchor(new Pose(touchPosition, Quaternion.identity));

                if (anchor == null)
                    Debug.LogError("Error creating reference point");
                else
                {
                    anchors.Add(anchor);
                }
                AddNweLineRenderer(anchor, touchPosition);
                this.director.GetComponent<GameDirector>().Tapping();
            }
            // タップ中
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {

                AddPoint(touchPosition);

            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // タッチ終了
                this.director.GetComponent<GameDirector>().NoTapping();
            }
        }
    }


    private void SetLineSettings(LineRenderer currentLineRenderer)
    {
        currentLineRenderer.startWidth = 0.01f;
        currentLineRender.endWidth = 0.01f;
        currentLineRender.numCapVertices = 5;
        currentLineRender.numCapVertices = 5;
        currentLineRender.startColor = Color.white;
        currentLineRender.endColor = Color.white;

    }

    public void AddNweLineRenderer(ARAnchor anchor, Vector3 touchPosition)
    {
        positionCount = 2;//点と点を繋ぐ際の、点の数　今回は2点で固定
        GameObject go = new GameObject("LineRenderer");

        go.transform.parent = anchor?.transform ?? transform;
        go.transform.position = touchPosition;
        go.tag = "Line";

        LineRenderer goLineRenderer = go.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = 0.01f;
        goLineRenderer.endWidth = 0.01f;

        goLineRenderer.startColor = Color.white;
        goLineRenderer.endColor = Color.white;

        goLineRenderer.material = defaultColorMaterial;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.positionCount = positionCount;

        goLineRenderer.numCornerVertices = 5;
        goLineRenderer.numCapVertices = 5;

        //始点だから２点とも同じ位置
        goLineRenderer.SetPosition(0, touchPosition);
        goLineRenderer.SetPosition(1, touchPosition);

        LineRenderer = goLineRenderer;

    }

    GameObject[] GetAllLinesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Line");

    }

    public void AddPoint(Vector3 position)
    {
        if (prevPointDistance == null)
            prevPointDistance = position;

        //前回のUpdate時の位置＝prevPointDistance、今回のUpdate時の位置＝position
        //prevPointDistanceとpositionの距離がminDistanceBeforeNewPoint以上ならば、LineRendererの点を増やす
        if (prevPointDistance != null && Mathf.Abs(Vector3.Distance(prevPointDistance, position)) >= minDistanceBeforeNewPoint)
        {
            
            //positionCount(点の数)を増やしていく
            positionCount++;
            LineRenderer.positionCount = positionCount;

            //indexは0始まりなので、positionCountから１引く
            LineRenderer.SetPosition(positionCount - 1, position);

            //次のUpdateの準備
            //次のUpdate時には今回の位置を前回の位置扱いするので
            prevPointDistance = position;
        }
    }

}
