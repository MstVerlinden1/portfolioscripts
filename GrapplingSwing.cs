using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingSwing : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] private LayerMask grappleable;
    [SerializeField] private Transform shootPoint, camera, player;
    [SerializeField] private float time, fakeGrappleHoldTimer;
    [SerializeField] private InputActionReference _Grapple;
    [SerializeField] private bool swinging;
    public delegate void SwingingManager(Vector3 position);
    public event SwingingManager HitPosition;
    private Vector3 grapplePoint;
    public float maxDistance = 100f;
    private SpringJoint joint;
    private Movement playerMove;
    private PlayerInput _input;
    private Rigidbody rb;

    public bool Swinging
    {
        get { return swinging; }
    }
    private void Awake()
    {
        playerMove = GetComponent<Movement>();
        _input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

    }
    private void OnEnable()
    {
        _input.actions[_Grapple.action.name].started += StartGrapple;
        _input.actions[_Grapple.action.name].canceled += StopGrapple;
    }
    private void OnDisable()
    {
        _input.actions[_Grapple.action.name].started -= StartGrapple;
        _input.actions[_Grapple.action.name].canceled -= StopGrapple;
    }

    private void LateUpdate()
    {
        DrawRope();
    }
    private void StartGrapple(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, grappleable))
        {
            lr.positionCount = 2;
            swinging = true;
            playerMove.Freeze = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.5f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 5f;
            joint.damper = 7f;
            joint.massScale = 5f;

            if (HitPosition != null)
            {
                HitPosition(hit.point);
            }
        }
    }
    private void StopGrapple(InputAction.CallbackContext context)
    {
        lr.positionCount = 0;
        swinging = false;
        playerMove.Freeze = false;
        Destroy(joint);
    }
    private void DrawRope()
    {
        if (!joint) return;
        lr.SetPosition(0, shootPoint.position);
        lr.SetPosition(1, grapplePoint);
    }
}
