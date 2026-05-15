using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{

    // references
    CharacterController controller;

    FPSInput input;

    [SerializeField] GameObject cam;
    [SerializeField] Transform gunHold;
    [SerializeField] Gun initialGun;

    // stats
    [SerializeField] float movementSpeed = 2.0f;
    [SerializeField] float lookSensitivityX = 1.0f;
    [SerializeField] float lookSensitivityY = 1.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;

    // private variables
    Vector3 origin;
    Vector3 velocity;
    bool grounded;
    float xRotation;

    List<Gun> equippedGuns = new List<Gun>();

    int gunIndex = 0;

    Gun currentGun = null;

    // properties
    public GameObject Cam { get { return cam; } }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        input = GetComponent<FPSInput>();

        Cursor.lockState = CursorLockMode.Locked;

        // start with a gun
        if (initialGun != null)
            AddGun(initialGun);

        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Look();

        HandleSwitchGun();

        FireGun();

        // always go back to "no velocity"
        // "velocity" is for movement speed that we gain in addition to our movement
        // (falling, knockback, etc.)
        Vector3 noVelocity = new Vector3(0, velocity.y, 0);

        velocity = Vector3.Lerp(
            velocity,
            noVelocity,
            5 * Time.deltaTime
        );
    }

    void Movement()
    {
        grounded = controller.isGrounded;

        if (grounded && velocity.y < 0)
        {
            velocity.y = -1;
        }

        Vector2 movement = GetPlayerMovementVector();

        Vector3 move =
            transform.right * movement.x +
            transform.forward * movement.y;

        controller.Move(
            move *
            movementSpeed *
            (GetSprint() ? 2 : 1) *
            Time.deltaTime
        );

        // NEW INPUT SYSTEM JUMP
        if (input.jumpPressed && grounded)
        {
            velocity.y += Mathf.Sqrt(
                jumpForce * -1 * gravity
            );
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        Vector2 looking = GetPlayerLook();

        float lookX = looking.x * lookSensitivityX;
        float lookY = looking.y * lookSensitivityY;

        xRotation -= lookY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookX);
    }
    /*void Look()
    {
        Vector2 looking = GetPlayerLook();

        float lookX =
            looking.x *
            lookSensitivityX;
            

        float lookY =
            looking.y *
            lookSensitivityY; 
            

        xRotation -= lookY;

        xRotation = Mathf.Clamp(
            xRotation,
            -90f,
            90f
        );

        cam.transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookX);
    }*/

    void HandleSwitchGun()
    {
        if (equippedGuns.Count == 0)
            return;

        // NEW INPUT SYSTEM SCROLL
        if (input.scrollValue > 0)
        {
            gunIndex++;

            if (gunIndex > equippedGuns.Count - 1)
                gunIndex = 0;

            EquipGun(equippedGuns[gunIndex]);
        }

        else if (input.scrollValue < 0)
        {
            gunIndex--;

            if (gunIndex < 0)
                gunIndex = equippedGuns.Count - 1;

            EquipGun(equippedGuns[gunIndex]);
        }
    }

    void FireGun()
    {
        // don't fire if we don't have a gun
        if (currentGun == null)
            return;

        // pressed fire
        if (GetPressFire())
        {
            currentGun?.AttemptFire();
        }

        // hold fire for automatic weapons
        else if (GetHoldFire())
        {
            if (currentGun.AttemptAutomaticFire())
                currentGun?.AttemptFire();
        }

        // alt fire
        if (GetPressAltFire())
        {
            currentGun?.AttemptAltFire();
        }
    }

    void EquipGun(Gun g)
    {
        // disable current gun
        currentGun?.Unequip();

        currentGun?.gameObject.SetActive(false);

        // enable new gun
        g.gameObject.SetActive(true);

        g.transform.parent = gunHold;

        g.transform.localPosition = Vector3.zero;

        currentGun = g;

        g.Equip(this);
    }

    // public methods

    public void AddGun(Gun g)
    {
        // add new gun
        equippedGuns.Add(g);

        // set index
        gunIndex = equippedGuns.Count - 1;

        // equip it
        EquipGun(g);
    }

    public void IncreaseAmmo(int amount)
    {
        currentGun.AddAmmo(amount);
    }

    public void Respawn()
    {
        transform.position = origin;
    }

    // INPUT METHODS
    // NOW USING FPSInput + UNITY EVENTS

    bool GetPressFire()
    {
        return input.firePressed;
    }

    bool GetHoldFire()
    {
        return input.fireHeld;
    }

    bool GetPressAltFire()
    {
        return input.altFirePressed;
    }

    Vector2 GetPlayerMovementVector()
    {
        return input.movement;
    }

    Vector2 GetPlayerLook()
    {
        return input.look;
    }

    bool GetSprint()
    {
        return input.sprintHeld;
    }

    // Collision methods

    // Character Controller can't use OnCollisionEnter
    private void OnControllerColliderHit(
        ControllerColliderHit hit
    )
    {
        if (hit.gameObject.GetComponent<Damager>())
        {
            var collisionPoint =
                hit.collider.ClosestPoint(transform.position);

            var knockbackAngle =
                (transform.position - collisionPoint).normalized;

            velocity = (20 * knockbackAngle);
        }

        if (hit.gameObject.GetComponent<KillZone>())
        {
            Respawn();
        }
    }
    public void TakeHit()
    {
        Debug.Log("Player Hit!");
    }
}
