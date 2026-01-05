using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Vector3 _moveDirection, _lookRotation;
    [SerializeField] private CharacterMovement _movement;
    [SerializeField] private CharacterRotation _rotation;
    [SerializeField] private CharacterSprint _sprint;
    [SerializeField] private CharacterJump jump;
    [SerializeField] private PlayerShootPaintball shooting;
    [SerializeField] private PlayerInteract interact;
    [SerializeField] private MouseClickStrategy currentMouseClickStrategy;
    [SerializeField] private CommandGiver commandGiver;
    [SerializeField] private PaintInventory paintInventory;
    [SerializeField] private PlayerErase erase;

    private float scrollInput;

    #region Testing
    [Header("Test Multipliers")]
    [SerializeField] private float speed;
    [SerializeField] private float size;
    [SerializeField] private float effectRadius;
    [SerializeField] private float cooldown;
    [SerializeField] private float duration;
    #endregion

    void Start()
    {
        GameManager.Instance.RegisterPlayerInput(this);

        currentMouseClickStrategy = shooting;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        _moveDirection.x = Input.GetAxisRaw("Horizontal");
        _moveDirection.z = Input.GetAxisRaw("Vertical");
        _moveDirection = _moveDirection.normalized;

        _lookRotation.x = -Input.GetAxisRaw("Mouse Y");
        _lookRotation.y = Input.GetAxisRaw("Mouse X");

        if (_movement != null)
        {
            _movement.MoveCharacter(_moveDirection);
        }
        if (_rotation != null)
        {
            _rotation.RotateByAngles(_lookRotation);
        }
        if (_sprint != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _sprint.StartSprinting();
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _sprint.StopSprinting();
            }
        }
        if (jump != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jump.Jump();
            }
        }
        if (currentMouseClickStrategy != null)
        {
            if (Input.GetMouseButton(0))
            {
                currentMouseClickStrategy.ExecuteStrategy();
            }
        }
        #region Command Giver
        //if (commandGiver != null)
        //{
        //    if (Input.GetKeyDown(KeyCode.C))
        //    {
        //        commandGiver.ExecuteStrategy();
        //    }
        //}
        #endregion

        #region Select Colours
        if (paintInventory != null)
        {
            for (int i = 1; i <= paintInventory.NumOfExistPaints; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    paintInventory.TrySelectPaintWithKeyCode(KeyCode.Alpha0 + i);
                }
            }

            scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0f)
            {
                paintInventory.CycleSelectedPaint(scrollInput);
            }
        }

        #endregion

        #region Testing
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    NpcManager.Instance.SpawnNpc(transform.position);
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    shooting.StartPowerUp(speed, size, effectRadius, cooldown, duration, true);
        //}

        if (Input.GetKeyDown(KeyCode.P))
        {
            CollectionManager.Instance.TrySpawnPowerUp(transform.position + transform.forward * 3);
        }

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    erase.ErasePaint();
        //}
        #endregion
    }
}
