using System;
using UnityEngine;

[Serializable]
public class MouseLook
{
    public float _XSensitivity = 2f;
    public float _YSensitivity = 2f;
    public bool _ClampVerticalRotation = true;
    public float _MinimumX = -90F;
    public float _MaximumX = 90F;
    public bool _Smooth;
    public float _SmoothTime = 5f;
    public bool _LockCursor = true;


    private Quaternion _CharacterTargetRot;
    private Quaternion _CameraTargetRot;
    private bool m_cursorIsLocked = true;

    public void Init(Transform character, Transform camera)
    {
        _CharacterTargetRot = character.localRotation;
        _CameraTargetRot = camera.localRotation;
    }


    public void LookRotation(Transform character, Transform camera)
    {
        float yRot = Input.GetAxis("Mouse X") * _XSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * _YSensitivity;

        _CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        _CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (_ClampVerticalRotation)
            _CameraTargetRot = ClampRotationAroundXAxis(_CameraTargetRot);

        if (_Smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, _CharacterTargetRot,
                _SmoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, _CameraTargetRot,
                _SmoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = _CharacterTargetRot;
            camera.localRotation = _CameraTargetRot;
        }

        UpdateCursorLock();
    }

    public void SetCursorLock(bool value)
    {
        _LockCursor = value;
        if (!_LockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (_LockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, _MinimumX, _MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}

