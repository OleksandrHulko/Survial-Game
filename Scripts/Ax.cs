using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Ax : MonoBehaviour , IDamage
{
    #region Serialize Fields
    [SerializeField]
    private Collider damageCollider = null;
    #endregion
    
    #region Private Fields
    private static readonly int _attack      = Animator.StringToHash("Attack");
    private static readonly int _stopAttack  = Animator.StringToHash("StopAttack");
    private static readonly int _attackSpeed = Animator.StringToHash("AttackSpeed");
    
    private Animator _playerAnimator = null;
    private DamageType _damageType = DamageType.Ax;
    private bool _canDamage = false;
    #endregion
    
    
    #region Private Methods
    private void Start()
    {
        _playerAnimator = Player.GetInstance().GetComponent<Animator>();
        
        PlayerAnimationEventsManager.OnHoldingStart += OnHoldingStartHandler;
        PlayerAnimationEventsManager.OnFellingStart += OnFellingStartHandler;
        PlayerAnimationEventsManager.OnFellingDamage += OnFellingDamageHandler;
    }

    private void OnDisable()
    {
        PlayerAnimationEventsManager.OnHoldingStart -= OnHoldingStartHandler;
        PlayerAnimationEventsManager.OnFellingStart -= OnFellingStartHandler;
        PlayerAnimationEventsManager.OnFellingDamage -= OnFellingDamageHandler;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable iDamageable))
        {
            _playerAnimator.SetFloat(_attackSpeed, -0.5f);
            _playerAnimator.SetTrigger(_stopAttack);
            
            damageCollider.enabled = false;
            
            if (_canDamage)
                iDamageable.GetDamaged(_damageType);
        }
    }

    private void OnHoldingStartHandler()
    {
        damageCollider.enabled = false;
        
        _playerAnimator.ResetTrigger(_attack);
        _playerAnimator.ResetTrigger(_stopAttack);
    }

    private void OnFellingStartHandler()
    {
        _canDamage = false;
        _playerAnimator.SetFloat(_attackSpeed, 1.0f);
        damageCollider.enabled = true;
    }

    private void OnFellingDamageHandler()
    {
        _canDamage = true;
    }
    #endregion

    #region Public Methods
    public void Attack()
    {
        _playerAnimator.SetTrigger(_attack);
    }
    #endregion
}

interface IDamage
{
    public void Attack();
}

interface IDamageable
{
    public void GetDamaged( DamageType damageType );
}

public struct DamageDamageablePair
{
    public readonly DamageType     damageType;
    public readonly DamageableType damageableType;

    public DamageDamageablePair(DamageType damageType, DamageableType damageableType)
    {
        this.damageType     = damageType;
        this.damageableType = damageableType;
    }

    public int GetDamage()
    {
        return Helper.DamageValues[this];
    }
    
    public static bool operator == (DamageDamageablePair a, DamageDamageablePair b)
    {
        return a.damageType == b.damageType && a.damageableType == b.damageableType;
    }

    public static bool operator !=(DamageDamageablePair a, DamageDamageablePair b)
    {
        return a.damageType != b.damageType || a.damageableType != b.damageableType;
    }
}

public enum DamageType : byte
{
    None = 0 ,
    Ax
}

public enum DamageableType : byte
{
    None = 0 ,
    Tree
}