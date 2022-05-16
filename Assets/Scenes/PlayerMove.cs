using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private MousePoint _mousePoint;

    private const float SpeedCoefficient = 0.01f;
    private const float ScaleFromCenterXZDirection = 0.5f;
    private const float ScaleFromCenterYDirection = 1.0f;
    private EdgePoint _edgePointFromCenter;

    private void Start()
    {
        _mousePoint = GameObject.Find("MousePoint").GetComponent<MousePoint>();

        // 自分のXZ方向のサイズ(中心からの座標の差分)
        var pointToForwardEdge = new Vector3(0, 0, ScaleFromCenterXZDirection);
        var pointToBackEdge = new Vector3(0, 0, -ScaleFromCenterXZDirection);
        var pointToRightEdge = new Vector3(ScaleFromCenterXZDirection, 0, 0);
        var pointToLeftEdge = new Vector3(-ScaleFromCenterXZDirection, 0, 0);
        var pointToUpEdge = new Vector3(0, ScaleFromCenterYDirection, 0);
        var pointToDownEdge = new Vector3(0, -ScaleFromCenterYDirection, 0);
        _edgePointFromCenter = new EdgePoint(pointToForwardEdge, pointToBackEdge, pointToRightEdge, pointToLeftEdge,
            pointToUpEdge, pointToDownEdge);
    }

    private void Update()
    {
        // マウスクリックされている間、クリックされている地点に向かって移動する
        if (!Input.GetMouseButton(0)) return;
        var targetPoint = _mousePoint.GetTargetPointForObject(ScaleFromCenterYDirection);

        var position = this.transform.position;
        var currentEdgePoint = CalcEdgePoint(position);

        // x方向とz方向を分けたほうがオブジェクトにぶつかったときの動きが自然
        // x方向の目標地点
        var targetPointX = new Vector3(targetPoint.x, position.y, position.z);
        // x方向の当たり判定
        var targetEdgePointX = CalcEdgePoint(targetPointX);
        var isCollideForwardEdge = CheckCollide(currentEdgePoint.Forward, targetEdgePointX.Forward,
            ScaleFromCenterXZDirection);
        var isCollideLeftBackEdge =
            CheckCollide(currentEdgePoint.Back, targetEdgePointX.Back, ScaleFromCenterXZDirection);
        // x方向の移動
        if (!isCollideForwardEdge && !isCollideLeftBackEdge)
        {
            MoveFromHere(targetPointX);
            // } else if (this.transform.position.y != targetPoint.y) {
            //     moveFromHere(new Vector3(targetPointX.x, targetPoint.y, targetPointX.z));
        }

        // z方向の目標地点
        var targetPointZ = new Vector3(position.x, position.y, targetPoint.z);
        // z方向の当たり判定
        var targetEdgePointZ = CalcEdgePoint(targetPointZ);
        var isCollideRightEdge =
            CheckCollide(currentEdgePoint.Right, targetEdgePointZ.Right, ScaleFromCenterXZDirection);
        var isCollideLeftEdge =
            CheckCollide(currentEdgePoint.Left, targetEdgePointZ.Left, ScaleFromCenterXZDirection);
        // z方向の移動
        if (!isCollideRightEdge && !isCollideLeftEdge)
        {
            MoveFromHere(targetPointZ);
            // } else if (this.transform.position.y != targetPoint.y) {
            //     moveFromHere(new Vector3(targetPointZ.x, targetPoint.y, targetPointZ.z));
        }


        // // y方向の目標地点(常に地面上)
        // Vector3 targetPointY = new Vector3(this.transform.position.x, 0.0f, this.transform.position.z);
        // // y方向の当たり判定
        // EdgePoint targetEdgePointY = calcEdgePoint(targetPointY);
        // bool isCollideDownEdge = checkCollide(currentEdgePoint.Down, targetEdgePointY.Down, ScaleFromCenterYDirection);
        // Debug.Log(isCollideDownEdge);
        // // y方向の移動
        // if (!isCollideDownEdge) {
        //     this.transform.position = Vector3.MoveTowards(this.transform.position, targetPointY, 1.0f);
        // }
        // y方向の目標地点(常に地面上)
        // Vector3 pointToVector = new Vector3(0, -1, 0).normalized;
        // Physics.Raycast (new Vector3(this.transform.position.x, 100.0f, this.transform.position.z), pointToVector, out RaycastHit hit);
        // if (hit.collider != null && hit.collider.tag == "Field") {
        //     // Debug.Log(hit.collider.transform.position.y + ScaleFromCenterYDirection);
        //     Vector3 targetPointY = new Vector3(this.transform.position.x, hit.collider.transform.position.y + ScaleFromCenterYDirection, this.transform.position.z);
        //     this.transform.position = Vector3.MoveTowards(this.transform.position, targetPointY, 1.0f);
        //     // this.transform.position = new Vector3(this.transform.position.x, hit.collider.transform.position.y + ScaleFromCenterYDirection, this.transform.position.z);
        // }
    }

    /// <summary>
    /// ある座標を中心としたときの、オブジェクトの端の座標を算出する
    /// </summary>
    /// <param name="point">座標</param>
    /// <returns>XZ方向のオブジェクトの端の座標</returns>
    private EdgePoint CalcEdgePoint(Vector3 point)
    {
        return new EdgePoint(point + _edgePointFromCenter.Forward, point + _edgePointFromCenter.Back,
            point + _edgePointFromCenter.Right, point + _edgePointFromCenter.Left, point + _edgePointFromCenter.Up,
            point + _edgePointFromCenter.Down);
    }

    /// <summary>
    /// 座標2点間で、始点から検索距離の範囲内にぶつかるオブジェクトがあるかどうかを判定する
    /// </summary>
    /// <param name="pointFrom">始点の座標</param>
    /// <param name="pointTo">終点の座標</param>
    /// <param name="searchDistance">検索距離の範囲</param>
    /// <returns>ぶつかるオブジェクトがあるかどうか</returns>
    private static bool CheckCollide(Vector3 pointFrom, Vector3 pointTo, float searchDistance)
    {
        var pointToVector = (pointTo - pointFrom).normalized;
        Physics.Raycast(pointFrom, pointToVector, out RaycastHit hit, searchDistance);
        return hit.collider != null;
    }

    /// <summary>
    /// 目標座標に向かって移動する
    /// </summary>
    /// <param name="pointTo">目標の座標</param>
    private void MoveFromHere(Vector3 pointTo)
    {
        // 距離を算出し、遠いほど早くなる
        var position = transform.position;
        var speed = Vector3.Distance(position, pointTo) * SpeedCoefficient;
        position = Vector3.MoveTowards(position, pointTo, speed);
        transform.position = position;
    }

    /// <summary>
    /// ある座標を中心としたときの、オブジェクトの端の座標
    /// </summary>
    private struct EdgePoint
    {
        public readonly Vector3 Forward;
        public readonly Vector3 Back;
        public readonly Vector3 Right;
        public readonly Vector3 Left;
        public readonly Vector3 Up;
        public readonly Vector3 Down;

        public EdgePoint(Vector3 forward, Vector3 back, Vector3 right, Vector3 left, Vector3 up, Vector3 down)
        {
            this.Forward = forward;
            this.Back = back;
            this.Right = right;
            this.Left = left;
            this.Up = up;
            this.Down = down;
        }
    }
}