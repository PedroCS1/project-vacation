using System;
using UnityEngine;

[Serializable]
public class CurveControlledBob
{
    public float _HorizontalBobRange = 0.33f;
    public float _VerticalBobRange = 0.33f;
    public AnimationCurve _Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                        new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                        new Keyframe(2f, 0f)); // sin curve for head bob
    public float _VerticaltoHorizontalRatio = 1f;

    private float _CyclePositionX;
    private float _CyclePositionY;
    private float _BobBaseInterval;
    private Vector3 _OriginalCameraPosition;
    private float _Time;


    public void Setup(Camera camera, float bobBaseInterval)
    {
        _BobBaseInterval = bobBaseInterval;
        _OriginalCameraPosition = camera.transform.localPosition;

        // get the length of the curve in time
        _Time = _Bobcurve[_Bobcurve.length - 1].time;
    }


    public Vector3 DoHeadBob(float speed)
    {
        float xPos = _OriginalCameraPosition.x + (_Bobcurve.Evaluate(_CyclePositionX) * _HorizontalBobRange);
        float yPos = _OriginalCameraPosition.y + (_Bobcurve.Evaluate(_CyclePositionY) * _VerticalBobRange);

        _CyclePositionX += (speed * Time.deltaTime) / _BobBaseInterval;
        _CyclePositionY += ((speed * Time.deltaTime) / _BobBaseInterval) * _VerticaltoHorizontalRatio;

        if (_CyclePositionX > _Time)
        {
            _CyclePositionX = _CyclePositionX - _Time;
        }
        if (_CyclePositionY > _Time)
        {
            _CyclePositionY = _CyclePositionY - _Time;
        }

        return new Vector3(xPos, yPos, 0f);
    }
}

