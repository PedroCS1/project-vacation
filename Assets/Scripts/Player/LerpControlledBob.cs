using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class LerpControlledBob
{
    public float _BobDuration;
    public float _BobAmount;

    private float _Offset = 0f;


    public float Offset()
    {
        return _Offset;
    }


    public IEnumerator DoBobCycle()
    {
        // make the camera move down slightly
        float t = 0f;
        while (t < _BobDuration)
        {
            _Offset = Mathf.Lerp(0f, _BobAmount, t / _BobDuration);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        // make it move back to neutral
        t = 0f;
        while (t < _BobDuration)
        {
            _Offset = Mathf.Lerp(_BobAmount, 0f, t / _BobDuration);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        _Offset = 0f;
    }
}
