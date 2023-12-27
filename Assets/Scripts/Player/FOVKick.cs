using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class FOVKick
{
    public Camera _Camera;
    [HideInInspector] public float _OriginalFov;
    public float _FOVIncrease = 3f;
    public float _TimeToIncrease = 1f;
    public float _TimeToDecrease = 1f;
    public AnimationCurve _IncreaseCurve;


    public void Setup(Camera camera)
    {
        CheckStatus(camera);

        _Camera = camera;
        _OriginalFov = camera.fieldOfView;
    }


    private void CheckStatus(Camera camera)
    {
        if (camera == null)
        {
            throw new Exception("FOVKick camera is null, please supply the camera to the constructor");
        }

        if (_IncreaseCurve == null)
        {
            throw new Exception(
                "FOVKick Increase curve is null, please define the curve for the field of view kicks");
        }
    }


    public void ChangeCamera(Camera camera)
    {
        _Camera = camera;
    }


    public IEnumerator FOVKickUp()
    {
        float t = Mathf.Abs((_Camera.fieldOfView - _OriginalFov) / _FOVIncrease);
        while (t < _TimeToIncrease)
        {
            _Camera.fieldOfView = _OriginalFov + (_IncreaseCurve.Evaluate(t / _TimeToIncrease) * _FOVIncrease);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


    public IEnumerator FOVKickDown()
    {
        float t = Mathf.Abs((_Camera.fieldOfView - _OriginalFov) / _FOVIncrease);
        while (t > 0)
        {
            _Camera.fieldOfView = _OriginalFov + (_IncreaseCurve.Evaluate(t / _TimeToDecrease) * _FOVIncrease);
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //make sure that fov returns to the original size
        _Camera.fieldOfView = _OriginalFov;
    }
}

