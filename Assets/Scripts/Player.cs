using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event CharacterMovementEventHandler OnMovementEvent;

    /// <summary>
    /// Uses a Raycast to check if the player is connected to the ground.
    /// Needed because CharacterController´s isGrounded property does not work properly.
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded
    {
        get
        {
            return Physics.Raycast(this.transform.position, Vector3.down, _distanceToGround + 0.1f);
        }
    }

    public float MovementSpeed = 5f;
    public float JumpForce = 8f;
    public float RotationSpeed = 5f;
    public WaypointManager Waypoints;
    
    private CharacterController _cc;
    private Vector3 _previousTarget;
    private Vector3 _walkingTarget;
    private Vector3 _gravity;
    private float _distanceToGround;
    private float _horizontalInput;
    private bool _isJumping;
    private bool _isAirbourne;
    private bool _isWalkingForward;

	private void Start ()
    {
        // Initialize variables
        _cc = GetComponent<CharacterController>();
        _gravity = Vector3.zero;
        _distanceToGround = GetComponent<MeshCollider>().bounds.extents.y;
        _horizontalInput = 0f;
        _isJumping = false;
        _isAirbourne = false;
        _isWalkingForward = true;

        // Set player to first waypoint and select next waypoint as walking target
        this.transform.position = this.Waypoints.Current.position;
        _previousTarget = this.transform.position;
        _walkingTarget = this.Waypoints.Next.position;
	}
	
	private void Update ()
    {
        GetInputs();
        CheckWaypointReached();
        CheckGroundedState();
        HandleDirectionChange();
        MoveTowards(_walkingTarget - this.transform.position);  
	}

    /// <summary>
    /// Calculates the the percentual progress from the previous to the next waypoint.
    /// </summary>
    /// <returns></returns>
    public float GetDistancePercentage()
    {
        if (!IsGrounded && _horizontalInput == 0) return 1; // Camera should not move if player jumps straight
        float totalDistance = Vector3.Distance(_previousTarget, _walkingTarget);
        float distanceToNext = Vector3.Distance(this.transform.position, _walkingTarget);
        return distanceToNext / totalDistance;
    }

    /// <summary>
    /// Checks if the current target waypoint is reached by calculating the distance of the player 
    /// to the x and z axis of the waypoint.
    /// </summary>
    /// <returns></returns>
    private bool WaypointReached()
    {
        float diff1 = Mathf.Abs(_walkingTarget.x) - Mathf.Abs(this.transform.position.x);
        float diff2 = Mathf.Abs(_walkingTarget.z) - Mathf.Abs(this.transform.position.z);
        return (Math.Abs(Math.Round(diff1, 1)) <= 0.1 && Math.Abs(Math.Round(diff2, 1)) <= 0.1);
    }

    /// <summary>
    /// Resets _isWalkingForward if player changes direction. Also selects new waypoint if needed.
    /// </summary>
    private void HandleDirectionChange()
    {
        if (_horizontalInput > 0 && !_isWalkingForward)
        {
            _isWalkingForward = true;
            SetWalkingTarget();
        }
        else if (_horizontalInput < 0 && _isWalkingForward)
        {
            _isWalkingForward = false;
            SetWalkingTarget();
        }
    }

    /// <summary>
    /// Sets the next target waypoint depending on walking direction.
    /// </summary>
    private void SetWalkingTarget()
    {
        _previousTarget = _walkingTarget;
        _walkingTarget = _isWalkingForward ? this.Waypoints.Next.position : this.Waypoints.Previous.position;
        if (this.OnMovementEvent != null)
        {
            this.OnMovementEvent.Invoke(this, _isWalkingForward);
        }
    }

    /// <summary>
    /// Sets _isAirbourne and _isJumping varibles depending the result of IsGrounded().
    /// </summary>
    private void CheckGroundedState()
    {
        if (!IsGrounded)
        {
            _isAirbourne = true;
        }
        else if (_isAirbourne && IsGrounded)
        {
            // Character is landing
            _isAirbourne = false;
            _isJumping = false;
        }
    }

    /// <summary>
    /// Moves the the player under the consideration of gravity to the targed waypoint.
    /// </summary>
    /// <param name="direction"></param>
    private void MoveTowards(Vector3 direction)
    {
        HandleGravity();
        RotateToTarget(direction);
        Vector3 movement = GetMovementVector(direction) + _gravity;
        _cc.Move(movement * Time.deltaTime);
    }

    /// <summary>
    /// Make the player look at the current walking target.
    /// </summary>
    /// <param name="direction">Direction from the player to the current walking target.</param>
    private void RotateToTarget(Vector3 direction)
    {
        Vector3 lookRotation = new Vector3(direction.x, 0f, direction.z);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(lookRotation), this.RotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Gets normalized movement vector from direction to the target waypoint.
    /// </summary>
    /// <param name="direction">The Vector describing the direction from the player´s position to the position of the targeted waypoint.</param>
    /// <returns></returns>
    private Vector3 GetMovementVector(Vector3 direction)
    {
        direction.y = 0;
        Vector3 movement = direction.normalized * this.MovementSpeed;
        if (_horizontalInput == 0)
        {
            movement = Vector3.zero;
        }
        return movement;
    }

    /// <summary>
    /// Fetches user inputs for movement and jumping.
    /// </summary>
    private void GetInputs()
    {
        _horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && !_isAirbourne)
        {
            _isJumping = true;
        }
    }

    /// <summary>
    /// Checks if the player has reached the current targeted waypoint. If so, 
    /// the next waypoint gets selected.
    /// </summary>
    private void CheckWaypointReached()
    {
        if (WaypointReached() == true)
        {
            SetWalkingTarget();
        }
    }

    /// <summary>
    /// Adds gravity to player if he is not on the ground or adds jumping force if user 
    /// hit the jump button.
    /// </summary>
    private void HandleGravity()
    {
        if (!IsGrounded)
        {
            _gravity += Physics.gravity * Time.deltaTime;
        }
        else
        {
            _gravity = Vector3.zero;
            if (_isJumping)
            {
                _gravity.y = this.JumpForce;
                _isJumping = false;
            }
        }
    }
}
