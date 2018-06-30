using UnityEngine;

public class Camera : MonoBehaviour
{
    public WaypointManager Waypoints;
    public Transform Target;
    public float RotationSpeed = 5f;

    private Player _player;
    private Vector3 _nextWaypoint;
    private Vector3 _previousWaypoint;

	private void Start ()
    {
        _player = this.Target.GetComponent<Player>();
        _player.OnMovementEvent += OnPlayerMovementEvent;

        this.transform.position = this.Waypoints.Current.position;
        _previousWaypoint = this.transform.position;
        _nextWaypoint = this.Waypoints.Next.position;
	}

    private void Update ()
    {
        Rotate();
        Move();
	}

    private void OnPlayerMovementEvent(object sender, bool isWalkingForward)
    {
        _previousWaypoint = _nextWaypoint;
        _nextWaypoint = isWalkingForward ? Waypoints.Next.position : Waypoints.Previous.position;
    }

    /// <summary>
    /// Make camera move alongside the player.
    /// </summary>
    private void Move()
    {
        float percentage = _player.GetDistancePercentage();
        if (percentage == 1 || float.IsNaN(percentage))
        {
            return;
        }
        Vector3 targetPosition = Vector3.Lerp(_previousWaypoint, _nextWaypoint, 1 - percentage);
        float speed = _player.GetComponent<CharacterController>().velocity.magnitude * Time.deltaTime;
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, speed);
    }

    /// <summary>
    /// Make camera always center the player.
    /// </summary>
    private void Rotate()
    {
        float speed = this.RotationSpeed * Time.deltaTime;
        Vector3 lookRotation = this.Target.position - transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(lookRotation), speed);
    }
}
