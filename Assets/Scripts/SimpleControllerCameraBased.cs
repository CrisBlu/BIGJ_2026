using UnityEngine;
using UnityEngine.InputSystem;
public class SimpleControllerCameraBased : MonoBehaviour
{
    private float playerSpeed = 5.0f;
    private float gravityValue = -9.81f;

    public CharacterController controller;
    private Vector3 playerVelocity;
    public Camera playerCamera;

    private InputAction moveAction;
    private InputAction pickUpAction;
    private InputAction putDownAction;
    private InputAction sprintAction;
    private GameObject itemHeld;


    private void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        pickUpAction = InputSystem.actions.FindAction("Attack");
        putDownAction = InputSystem.actions.FindAction("Drop");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        

        pickUpAction.performed += PickUp;
        putDownAction.performed += PutDown;
        sprintAction.started += ToggleSprint;
        sprintAction.canceled += ToggleSprint;


        itemHeld = null;

    }

    private void OnDisable()
    {
        pickUpAction.performed -= PickUp;
        putDownAction.performed -= PutDown;
        sprintAction.started -= ToggleSprint;
        sprintAction.canceled -= ToggleSprint;
    }


    void Update()
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





    }



    void PickUp(InputAction.CallbackContext context)
    {
        if(itemHeld)
            return;

        Collider[] items = Physics.OverlapSphere(transform.position + transform.forward, 1f);

        foreach (Collider item in items)
        {
            //Won't hold up if we need to make guards pick up able; make an interface instead, the only reason I'm not is because this because test pick up object has no functionality besides this
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


    void ToggleSprint(InputAction.CallbackContext context)
    {
        if(context.started )
        {
            //sprinting = true;
        }
        else if(context.canceled)
        {
            //sprinting = false;
        }
    }

    void Sprint()
    {
        PlayerStats stats = PlayerStats.Stats;
        if(stats.adrenaline > 0)
        {
            stats.ShiftAdrenaline(-3);
        }

        //Stop running and such
        //Dunno that I like this
        if(stats.adrenaline <= 0)
        {
            //sprinting = false;
        }
    }

    
}

