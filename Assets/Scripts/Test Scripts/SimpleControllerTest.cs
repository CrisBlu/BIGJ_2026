using UnityEngine;
using UnityEngine.InputSystem;

namespace Test_Scripts
{
    public class SimpleControllerTest : MonoBehaviour
    {
        public float playerSpeed = 5.0f;
        private float defaultPlayerSpeed;
        private float gravityValue = -9.81f;

        [Range(0f, 20f)] public float minGrindSpeed = 2f;
        [Range(0f, 20f)] public float maxGrindSpeed = 6f;
        public float fixedGrindSpeed = 12f;
        public float snapDistance = 1.2f;
        public float railHeightOffset = 0.5f;
        public float grindRotationSpeed = 10f;

        public float twirlDegreesPerSec = 720f;
        public float twirlDuration = 0.4f;

        public CharacterController controller;
        public Camera playerCamera;

        private Vector3 playerVelocity;

        private InputAction moveAction;
        private InputAction twirlAction;
        private InputAction pickUpAction;
        private InputAction putDownAction;
        private InputAction sprintAction;

        private GameObject itemHeld;

        private bool isGrinding;
        private RailSegment activeRail;
        private float grindT;
        private float grindSpeed;
        private float grindCooldown;
        private float grindDirection;

        private bool isTwirling;
        private float twirlTimer;

        private void OnEnable()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            twirlAction = InputSystem.actions.FindAction("Jump");
            pickUpAction = InputSystem.actions.FindAction("Attack");
            putDownAction = InputSystem.actions.FindAction("Drop");
            sprintAction = InputSystem.actions.FindAction("Sprint");

            twirlAction.performed += OnTwirl;
            pickUpAction.performed += PickUp;
            putDownAction.performed += PutDown;

            itemHeld = null;

            defaultPlayerSpeed = playerSpeed;
        }

        private void OnDisable()
        {
            twirlAction.performed -= OnTwirl;
            pickUpAction.performed -= PickUp;
            putDownAction.performed -= PutDown;
        }

        void Update()
        {

            if (isGrinding)
                UpdateGrinding();
            else
                UpdateNormal();

            HandleTwirl();

            if(sprintAction.ReadValue<float>() == 1)
            {
                Sprint();
            }
            else
            {
                playerSpeed = defaultPlayerSpeed;
            }
        }

        // ── normal movement ────────────────────────────────────────────

        void UpdateNormal()
        {
            if (controller.isGrounded && playerVelocity.y < -2f)
                playerVelocity.y = -2f;

            Vector2 input = moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y);
            move = Vector3.ClampMagnitude(move, 1f);

            if (move != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
                move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                transform.forward = move;
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move((move * playerSpeed + Vector3.up * playerVelocity.y) * Time.deltaTime);

            playerVelocity.x = move.x * playerSpeed;
            playerVelocity.z = move.z * playerSpeed;

            if (grindCooldown > 0f)
                grindCooldown -= Time.deltaTime;
            else
                TrySnapToRail();
        }

        // ── rail grind ─────────────────────────────────────────────────

        void TrySnapToRail()
        {
            float horizSpeed = new Vector3(playerVelocity.x, 0f, playerVelocity.z).magnitude;
            if (horizSpeed < minGrindSpeed)
                return;

            foreach (var rail in RailSegment.All)
            {
                float t = rail.GetClosestT(transform.position);
                Vector3 point = rail.GetPosition(t);
                if (Vector3.Distance(transform.position, point) <= snapDistance)
                {
                    EnterGrind(rail, t, horizSpeed);
                    return;
                }
            }
        }

        void EnterGrind(RailSegment rail, float startT, float speed)
        {
            activeRail = rail;
            grindT = Mathf.Clamp(startT, 0.01f, 0.99f);
            grindSpeed = fixedGrindSpeed;
            isGrinding = true;
            playerVelocity.y = 0f;

            Vector3 railDir = rail.GetDirection(grindT);
            grindDirection = Vector3.Dot(transform.forward, railDir) >= 0f ? 1f : -1f;
        }

        void UpdateGrinding()
        {
            grindT += grindDirection * (grindSpeed / activeRail.Length) * Time.deltaTime;
            grindT = Mathf.Clamp01(grindT);

            Vector3 pos = activeRail.GetPosition(grindT) + Vector3.up * railHeightOffset;
            Vector3 dir = activeRail.GetDirection(grindT) * grindDirection;

            controller.enabled = false;
            transform.position = pos;
            controller.enabled = true;

            if (dir.sqrMagnitude > 0.001f && !isTwirling)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up),
                    Time.deltaTime * grindRotationSpeed);

            if (grindT >= 1f || grindT <= 0f)
                ExitGrind();
        }

        void ExitGrind()
        {
            Vector3 exitDirection = activeRail.GetDirection(grindT) * grindDirection;
            playerVelocity = exitDirection * grindSpeed;
            isGrinding = false;
            activeRail = null;
            grindCooldown = 0.5f;
        }

        // ── twirl ──────────────────────────────────────────────────────

        void HandleTwirl()
        {
            if (isTwirling)
            {
                transform.Rotate(Vector3.up, twirlDegreesPerSec * Time.deltaTime, Space.World);
                twirlTimer -= Time.deltaTime;
                if (twirlTimer <= 0f)
                    isTwirling = false;
            }
        }

        void OnTwirl(InputAction.CallbackContext context)
        {
            PlayerStats.Stats.Event_Taunted.Invoke();

            if (isGrinding && !isTwirling)
            {
                isTwirling = true;
                twirlTimer = twirlDuration;
            }
        }

        // ── pick up / put down ─────────────────────────────────────────

        void PickUp(InputAction.CallbackContext context)
        {
            if (itemHeld)
                return;

            Collider[] items = Physics.OverlapSphere(transform.position + transform.forward, 1f);

            foreach (Collider item in items)
            {
                if (item.CompareTag("PickUp"))
                {
                    item.transform.SetParent(transform, true);
                    item.gameObject.SetActive(false);
                    itemHeld = item.gameObject;
                    break;
                }
            }
        }

        void PutDown(InputAction.CallbackContext context)
        {
            if (!itemHeld)
                return;

            itemHeld.transform.SetParent(null, true);
            itemHeld.SetActive(true);
            itemHeld = null;
        }

        void Sprint()
        {
            PlayerStats stats = PlayerStats.Stats;
            if (stats.ShiftAdrenaline(-1.5f))
            {
                playerSpeed = 2 * defaultPlayerSpeed;
            }
            else
            {
                playerSpeed = defaultPlayerSpeed;
            }

        }
    }
}

