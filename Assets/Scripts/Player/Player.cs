using System;
using System.Collections;
using System.Collections.Generic;
using Observer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IDamageable
{

    #region HealthAndMana

    private float _maxHealth { get; set; } 
    private float _currentHealth { get; set; }

    private float _maxMana { get; set; } 
    
    private float _currentMana { get; set; }
    #endregion

    #region Components

    private Character _cc;
    private CharacterController _characterController;
    public Character_Input input;
    private Animator _animator;
    private VFXPlayerController _vfxPlayerController;
    private DamageCaster _damageCaster;
    private PlayerSkillsBarController _playerSkillsBarController;
    #endregion

    #region MovementVariables

    private float _moveSpeed;
    private readonly float _runSpeed = 2.5f;
    private readonly float _sprintSpeed = 4f;
    private readonly float _jumpHeight = 2.2f;
    
    private float _attackAnimationDuration;
    private readonly float _gravity = -9.81f;
    
    private Vector3 _verticalVelocity;
    private Vector3 _movementVelocity;
    private Vector3 _impactOnPlayer;
    
    private float _fallTimeoutDelta;
    private float _jumpTimeoutDelta;
    private bool _jumpEnd;

    public float jumpTimeout = 0.3f;
    public float fallTimeout = 0.15f;
    #endregion

    #region AimAttackVariables

    public LayerMask isEnemy;
    private bool _enemyInSightRange;
    public float sightRange;
    #endregion

    #region VisualPlayerVariables

    public GameObject visualMale;
    public GameObject visualFemale;
    
    public Avatar maleAvatar;
    public Avatar feMaleAvatar;
    
    //Set sex player: true is male, false is female
    public bool isMale;

    #endregion

    #region PlayerSlideVariables
    
    public float attackStartTime;
    public float attackSlideDuraton = 0.25f;
    public float attackSlideSpeed = 0.05f;
    
    #endregion

    #region OtherVariables

    private bool _hasAnimator;

    private bool _isInvincible;

    public GameObject ultimateCutScene;
    public GameObject guardSkillCutScene;
    public GameObject magicSkillCutScene;
    public GameObject swordSkillCutScene;
    public GameObject openGateCutScene;


    public GameObject playerDiedUI;

    private Tuple<string, string, int, int, int, string, int, Tuple<int>> _weaponEquip;
    private int _damageWeapon;
    public int damageWeapon => _damageWeapon;

    public Vector3 enemyInRangeSkill;
    private float sightRangeSkill = 10f;
    #endregion


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        input = GetComponent<Character_Input>();
        _animator = GetComponent<Animator>();
        _vfxPlayerController = GetComponent<VFXPlayerController>();
        _playerSkillsBarController = GetComponentInChildren<PlayerSkillsBarController>();
        _cc = GetComponent<Character>();
        
        int idWeapon = DataManager.Instance.GetDataPrefPlayer(DataManager.EDataPlayerEquip.WeaponId);
        _weaponEquip = DataManager.Instance.GetWeaponByID(idWeapon);
        //health setup
        _maxHealth = _weaponEquip.Item4 + (30 * DataManager.Instance.GetDataPrefPlayer(DataManager.EDataPlayerEquip.LevelPlayer)) ;
        _currentHealth = _maxHealth;
        //
        //mana setup
        _maxMana = _weaponEquip.Item5 + 10 * DataManager.Instance.GetDataPrefPlayer(DataManager.EDataPlayerEquip.LevelPlayer);
        _currentMana = _maxMana;
        //
        //dmg setup
        _damageWeapon = _weaponEquip.Item3 + (int)Mathf.Floor(_weaponEquip.Item3 * DataManager.Instance.GetDataPrefPlayer(DataManager.EDataPlayerEquip.LevelPlayer)/10f);
        //
    }

    private void Start()
    {
        _characterController.enabled = false;
        AppearPlayerInGame();
        
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _hasAnimator = TryGetComponent(out _animator);

        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
        _isInvincible = false; // tesst
        _jumpEnd = true;
    }
    

    #region Player Appear And Dissapear

    public void AppearPlayerInGame()
    {
        StartCoroutine(AppearPlayer());
    }
    
    public void DissapearPlayerInGame()
    {
        StartCoroutine(DissapearPlayer());
    }

    IEnumerator AppearPlayer()
    {
        _vfxPlayerController.PlayVfxTrailsUp();
        yield return new WaitForSeconds(1f);
        AODPlayer(true);
        _characterController.enabled = true;
    }
    
    IEnumerator DissapearPlayer()
    {
        _vfxPlayerController.PlayVfxTrailsDown();
        yield return new WaitForSeconds(0.5f);
        AODPlayer(false);
        _characterController.enabled = false;
    }

    private void AODPlayer(bool aod)
    {
        if (DataManager.Instance.GetDataPrefPlayer(DataManager.EDataPlayerEquip.PlayerSex) == 1)
        {
            visualFemale.SetActive(aod);
            _animator.avatar = feMaleAvatar;
        }
        else
        {
            visualMale.SetActive(aod);
            _animator.avatar = maleAvatar;
        }
    }

    #endregion
    
    #region Player Movement

        public void CalculateMovementPlayer()
        {
            PlayerInputImplement();
            _moveSpeed = input.sprint ? _sprintSpeed : _runSpeed;
            
            if (input.move == Vector2.zero) _moveSpeed = 0.0f;
            if (_characterController.isGrounded && _verticalVelocity.y < 0) _verticalVelocity.y = 0;
            
            float inputMagnitude = input.move.magnitude;
            
            _movementVelocity = new Vector3(input.move.x,0f,input.move.y);
            _movementVelocity.Normalize();
            _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;
            _movementVelocity *= _moveSpeed * Time.deltaTime;
            
            if (_movementVelocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_movementVelocity);
            }

            _animator.SetFloat(AnimationManager.Instance.animIDSpeed,_moveSpeed);
            _animator.SetFloat(AnimationManager.Instance.animIDMotionSpeed,inputMagnitude);
            
            if (_hasAnimator)
            {
                _animator.SetBool(AnimationManager.Instance.animIDGrounded, _characterController.isGrounded);
            }
        }

    public void PlayerMove()
    {
        if (_cc.CurrentState != Character.CharacterState.Dead)
        {
            // move the player
            _characterController.Move(_movementVelocity + _verticalVelocity * Time.deltaTime);
            _movementVelocity = Vector3.zero;
        } 
    }
    
    public void JumpAndGravity()
    {
        if (_characterController.isGrounded)
        {
            _fallTimeoutDelta = fallTimeout;
            
            if (_hasAnimator)
            {
                _animator.SetBool(AnimationManager.Instance.animIDJump, false);
                _animator.SetBool(AnimationManager.Instance.animIDFall, false);
            }
                        
            //stop dropping infinite when grounded
            if (_verticalVelocity.y < 0.0f)
            {
                _verticalVelocity.y = -2f;
            }

            if (input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravity);
                _animator.SetBool(AnimationManager.Instance.animIDJump, true);
                _jumpEnd = false;
            }
            
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = jumpTimeout;
            
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_hasAnimator)
                {
                    _animator.SetBool(AnimationManager.Instance.animIDFall, true);
                }
            }
            
            _verticalVelocity.y += _gravity * Time.deltaTime;
            input.jump = false;
        }
    }

    #endregion

    #region HealthAndMana

    public void ApplyDamage(float dmg, Vector3 posAttack = new Vector3())
    {
        _currentHealth -= dmg;
        Debug.Log("dmg player: " + dmg);
        if (_currentHealth <= 0)
        {
            Debug.Log("death");
            _cc.SwitchStateTo(Character.CharacterState.Dead);
            ActionManager.OnUpdateUIPlayerDie?.Invoke();
        }
        
        _cc.SwitchStateTo(Character.CharacterState.BeingHit);
        AddImpact(posAttack,7f);
    }

    public void AddHealth(float healVal)
    {
        _currentHealth += healVal;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }
    
    public float GetMaxHealth()
    {
        return _maxHealth;
    }
    
    public float GetCurrentHealth()
    {
        return _currentHealth;
    }
    
    public float GetMaxMana()
    {
        return _maxMana;
    }
    
    public float GetCurrentMana()
    {
        return _currentMana;
    }
    
    public void ManaRecoveryAuto()
    {
        if (_currentMana < _maxMana)
        {
            StartCoroutine(RecoveryMana(7));
        }
    }

    IEnumerator RecoveryMana(float time)
    {
        yield return new WaitForSeconds(time);
        if (_currentMana < _maxMana)
        {        
            _currentMana += _maxMana / 100;
        }
    }
    
    public void AddMana(float manaVal)
    {
        _currentMana += manaVal;
        if (_currentMana > _maxMana)
        {
            _currentMana = _maxMana;
        }
    }

    private void ManaConsumption(float manaConsump)
    {
        _currentMana -= manaConsump;
    }

    private bool ManaCanUseSkill(float manaConsump)
    {
        if (_currentMana - manaConsump >= 0)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Player Attack And Skill

    private void PlayerInputImplement()
    {
        var manaGuard = DataManager.Instance.GetSkillDataByID(_playerSkillsBarController.ItemSkillBarGuard.idSkill,
            _playerSkillsBarController.ItemSkillBarGuard.eskill).Item3; 
        var manaSword = DataManager.Instance.GetSkillDataByID(_playerSkillsBarController.ItemSkillBarGuard.idSkill,
            _playerSkillsBarController.ItemSkillBarGuard.eskill).Item3; 
        var manaMagic = DataManager.Instance.GetSkillDataByID(_playerSkillsBarController.ItemSkillBarGuard.idSkill,
            _playerSkillsBarController.ItemSkillBarGuard.eskill).Item3;
        
        _enemyInSightRange = Physics.CheckSphere(transform.position, sightRange, isEnemy);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightRange,isEnemy);

        if (input.potion1)
        {
            if (_playerSkillsBarController.finishCDPotion1)
            {
                if (_playerSkillsBarController.ItemPotion1Bar.countItem > 0)
                {
                    this.PostEvent(EventID.OnPotion1CdActivate);
                    input.ClearSkillInput();
                    Debug.Log("Use potion 1 " + "healt " + _playerSkillsBarController.ItemPotion1Bar.health + "| mana " + _playerSkillsBarController.ItemPotion1Bar.mana);
                    AddHealth(_playerSkillsBarController.ItemPotion1Bar.health);
                    AddMana(_playerSkillsBarController.ItemPotion1Bar.mana);
                    _vfxPlayerController.PlayerVfxHealing();
                    return;
                }
                else
                {
                    ActionManager.OnUpdateAnoucementText?.Invoke("Potion Is Out Of Stock");
                } 
            }
            else
            {
                ActionManager.OnUpdateAnoucementText?.Invoke("Skill Is Still On Cooldown");
            }
        }
        if (input.potion2)
        {
            if (_playerSkillsBarController.finishCDPotion2)
            {
                if (_playerSkillsBarController.ItemPotion2Bar.countItem > 0)
                {
                    this.PostEvent(EventID.OnPotion2CdActivate);
                    input.ClearSkillInput();
                    AddHealth(_playerSkillsBarController.ItemPotion2Bar.health);
                    AddMana(_playerSkillsBarController.ItemPotion2Bar.mana);
                    _vfxPlayerController.PlayerVfxHealing();
                    Debug.Log("Use potion 2 " + "healt " + _playerSkillsBarController.ItemPotion2Bar.health + "| mana " + _playerSkillsBarController.ItemPotion2Bar.mana);
                    return;
                }
                else
                {
                    ActionManager.OnUpdateAnoucementText?.Invoke("Potion Is Out Of Stock");
                }
            }
            else
            {
                ActionManager.OnUpdateAnoucementText?.Invoke("Skill Is Still On Cooldown");
            }
        }
        
        if (input.guard  && _characterController.isGrounded && _jumpEnd)
        {
            if (ManaCanUseSkill(manaGuard))
            {
                if (_playerSkillsBarController.finishCDGuard)
                {
                    guardSkillCutScene.SetActive(true);
                    guardSkillCutScene.GetComponent<PlayableDirector>().Play();
                    _cc.SwitchStateTo(Character.CharacterState.Skill);
                    InviciblePlayer(7f);
                    ManaConsumption(manaGuard);
                    return;
                }
                else
                {
                    ActionManager.OnUpdateAnoucementText?.Invoke("Skill Is Still On Cooldown");
                }
            }
            else
            {
                ActionManager.OnUpdateAnoucementText?.Invoke("Mana Is Out To Use");
            }
            return;
        }
        
        if (input.sword  && _characterController.isGrounded && _jumpEnd)
        {
            if (ManaCanUseSkill(manaSword))
            {
                if (_playerSkillsBarController.finishCDSword)
                {
                    swordSkillCutScene.SetActive(true);
                    swordSkillCutScene.GetComponent<PlayableDirector>().Play();
                    _cc.SwitchStateTo(Character.CharacterState.Skill);
                    ManaConsumption(manaSword);
                    return;
                }
                else
                {
                    ActionManager.OnUpdateAnoucementText?.Invoke("Skill Is Still On Cooldown");
                }
            }
            else
            {
                ActionManager.OnUpdateAnoucementText?.Invoke("Mana Is Out To Use");
            }
            return;
        }
        
        if (input.magic  && _characterController.isGrounded && _jumpEnd)
        {
            if (ManaCanUseSkill(manaMagic))
            {
                if (_playerSkillsBarController.finishCDMagic)
                {
                    magicSkillCutScene.SetActive(true);
                    magicSkillCutScene.GetComponent<PlayableDirector>().Play();
                    _cc.SwitchStateTo(Character.CharacterState.Skill);
                    ManaConsumption(manaMagic);
                    return;
                }
                else
                {
                    ActionManager.OnUpdateAnoucementText?.Invoke("Skill Is Still On Cooldown");

                }
            }
            else
            {
                ActionManager.OnUpdateAnoucementText?.Invoke("Mana Is Out To Use");
            }
            return;
        }
        else
        {
            input.ClearSkillInput();
        }
        
        if (input.attack && _characterController.isGrounded && _jumpEnd)
        {
            _cc.SwitchStateTo(Character.CharacterState.Attacking);
            if (_enemyInSightRange)
            {
                transform.LookAt(hitColliders[0].transform);
            }
            return;
        }
        
        if (input.roll && _characterController.isGrounded && _jumpEnd)
        {
            _cc.SwitchStateTo(Character.CharacterState.Roll);
            return;
        }
    }
    public void CheckEnemyInRangeSkill()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,sightRangeSkill, isEnemy);
        if (colliders.Length != 0)
        {
            enemyInRangeSkill = colliders[0].transform.position;
        }
        else
        {
            enemyInRangeSkill = new Vector3(transform.position.x + 1,transform.position.y,transform.position.z);
        }
    }

    public void SlidePlayerAttack()
    {                    
        if (Time.deltaTime < attackSlideDuraton + attackStartTime)
        {
            float timePassed = Time.time - attackStartTime;
            float lerpTime = timePassed / attackSlideDuraton;
            _movementVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
        }
    }

    public void PlayerAttackCombo()
    {
        if (input.attack && _characterController.isGrounded && _jumpEnd)
        {
            _attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Combo03" 
                && _attackAnimationDuration > 0.65f && _attackAnimationDuration < 0.75f)
            {
                _cc.SwitchStateTo(Character.CharacterState.Attacking);
                input.attack = false;
                CalculateMovementPlayer();
            }
            else
            {
                input.attack = false;
                _cc.SwitchStateTo(Character.CharacterState.Normal);
            }
        }
    }

    public void CancelTriggerAttack()
    {
        _animator.ResetTrigger(AnimationManager.Instance.animIDAttack);
    }

    #endregion

    #region PlayerHit

    public void PlayerBeingHit()
    {
        if (_impactOnPlayer.magnitude > 0.2f)
        {
            _movementVelocity = _impactOnPlayer * Time.deltaTime;
        }
        _impactOnPlayer = Vector3.Lerp(_impactOnPlayer, Vector3.zero, Time.deltaTime * 5);
    }
    public void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        _impactOnPlayer = impactDir * force;
    }

    public void InviciblePlayer(float duration)
    {
        _isInvincible = true;
        _characterController.detectCollisions = false;
        StartCoroutine(DelayCancelInvincible(duration));
    }
    
    IEnumerator DelayCancelInvincible(float duration)
    {
        yield return new WaitForSeconds(duration);
        _characterController.detectCollisions = true;
        _isInvincible = false;
    }

    public void Die()
    {
        _characterController.enabled = false;
    }

    public void LoadScreenLose()
    {
        // StartCoroutine(DelayToLoadScreenLose());
        playerDiedUI.SetActive(true);
    }

    IEnumerator DelayToLoadScreenLose()
    {
        yield return new WaitForSeconds(1f);
        MainSceneManager.Instance.profile.SetActive(false);
        MainSceneManager.Instance.endingScreen.LoseGame();
    }

    #endregion
    
    public void PickUpItem(DropItem item)
    {
        switch (item.type)
        {
            case DropItem.ItemType.Coin:
                var coin = DataManager.Instance.GetDataPrefGame(DataManager.EDataPrefName.Coin);
                Debug.Log(coin);
                coin += 100;
                DataManager.Instance.SaveDataPrefGame(DataManager.EDataPrefName.Coin,coin);
                Debug.Log(coin);
                SoundManager.Instance.PlaySfxObj(EnumManager.ESfxObjType.CoinObj);
                break;
            case DropItem.ItemType.HealOrb:
                AddHealth(30);
                _vfxPlayerController.PlayerVfxHealing();
                SoundManager.Instance.PlaySfxObj(EnumManager.ESfxObjType.HealObj);
                break;
        }
    }
    public void JumpToNormal()
    {
        _jumpEnd = true;
    }

    public void StopBladeAnimation()
    {
        GetComponent<VFXPlayerController>().StopBlade();
    }
    
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,sightRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,sightRangeSkill);
    }

    public void PlayerSfxSlash()
    {
        SoundManager.Instance.PlaySfxPlayer(EnumManager.ESfxSoundPlayer.SwordSlash);
    }
    
    public void PlayerSfxDefend()
    {
        SoundManager.Instance.PlaySfxPlayer(EnumManager.ESfxSoundPlayer.Defend);

    }
    
    public void PlayerSfxHit()
    {
        SoundManager.Instance.PlaySfxPlayer(EnumManager.ESfxSoundPlayer.Hit);

    }
}
