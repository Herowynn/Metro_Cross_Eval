using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float NormalSpeed;
    public float RunSpeed;
    public float SlowSpeed;
    public float JumpHeight;
    public float Gravity = -15.0f;

    public LayerMask SlowCubeLayerMask;
    public LayerMask BarrierLayerMask;
    public LayerMask GroundLayerMask;

    public Animator PlayerAnimator;

    CharacterController _characterController;
    float _targetSpeed;
    bool _inTheBarrier = false;
    int BarrierLayer;
    [SerializeField]Vector3 _characterMovement;
    

    private void Awake()
    {
        BarrierLayer = (int)Mathf.Log(1f * BarrierLayerMask.value, 2f);

        _characterController = GetComponent<CharacterController>();
        _targetSpeed = NormalSpeed;
    }

    private void Update()
    {
        Move();
        Jump();
    }

    void Jump()
    {
        if (Grounded() && _characterMovement.y < 0f)
        {
            _characterMovement.y = 0f;
        }

        if (Grounded() && Input.GetButtonDown("Jump"))
        {
            PlayerAnimator.SetTrigger("Jump");
            _characterMovement.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        _characterMovement.y += Gravity * Time.deltaTime;

        _characterController.Move(_characterMovement * Time.deltaTime);
    }

    private void Move()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0 && !_inTheBarrier && !SlowGround())
        {
            _targetSpeed = RunSpeed;
            PlayerAnimator.SetBool("Runing", true);
            PlayerAnimator.SetBool("Slowing", true);
        }


        else if (SlowGround() || _inTheBarrier || horizontalInput < 0)
        {
            _targetSpeed = SlowSpeed;
            PlayerAnimator.SetBool("Slowing", true);
            PlayerAnimator.SetBool("Runing", false);

        }


        else
        {
            _targetSpeed = NormalSpeed;
            PlayerAnimator.SetBool("Runing", false);
            PlayerAnimator.SetBool("Slowing", false);

        }


        _characterMovement.x = 1;
        _characterMovement.z = verticalInput;

        _characterController.Move(_targetSpeed * Time.deltaTime * _characterMovement);
    }

    bool SlowGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.y * 2, SlowCubeLayerMask)) return true;
        return false;
    }
    
    bool Grounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.y * .5f, GroundLayerMask)) return true;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == BarrierLayer)
        {
            _inTheBarrier = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == BarrierLayer)
        {
            _inTheBarrier = false;
        }
    }
}
