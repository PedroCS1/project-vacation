using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaManager : MonoBehaviour
{
    [SerializeField] private FirstPersonController _PlayerController;
    [Header("Stamina")]
    [SerializeField] private Image _StaminaBar;
    [SerializeField] private float _StaminaConsuption;
    [SerializeField] private float _StaminaRecharge;
    [SerializeField] private float _MaxStamina = 100.0f;

    private float _CurrentStamina = 0;
    private bool _IsFastChargingStamina = false;

    public float CurrentStamina { get => _CurrentStamina; }
    public bool IsFastChargingStamina { get => _IsFastChargingStamina; set => _IsFastChargingStamina = value; }
    public float StaminaRechargeRate { get => _StaminaRecharge; set => _StaminaRecharge = value; }

    private void Start()
    {
        _CurrentStamina = _MaxStamina;
    }
    void Update()
    {
        HandleStamina();
    }

    private void HandleStamina()
    {
        float recharge = _IsFastChargingStamina ? _StaminaRecharge * 5 : _StaminaRecharge;
        if (!_PlayerController.IsWalking && _PlayerController.PlayerIsMoving && !_IsFastChargingStamina)
        {
            _CurrentStamina -= _StaminaConsuption * Time.deltaTime;
            if (_CurrentStamina < 0f)
                _CurrentStamina = 0;
        }
        else
        {
            _CurrentStamina += recharge * Time.deltaTime;
            if (_CurrentStamina > _MaxStamina)
                _CurrentStamina = _MaxStamina;
        }
        UpdateStaminaBar();
    }
    public void UpdateStaminaBar()
    {
        Vector3 scale = _StaminaBar.transform.localScale;
        scale.x = (_CurrentStamina / _MaxStamina);
        _StaminaBar.transform.localScale = scale;
    }
}
