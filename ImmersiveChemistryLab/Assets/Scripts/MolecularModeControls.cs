using UnityEngine;
using UnityEngine.InputSystem;

public class MolecularModeControls : MonoBehaviour
{
    public MolecularScaleGesture molecularScaleGesture;

    public Transform molecularWorld;
    public Transform headCamera;

    public float moveSpeed = 1.0f;

    private bool previousBButton = false;

    void Update()
    {
        if (molecularScaleGesture == null)
            return;

        // Controls only operate while inside molecular mode.
        if (!molecularScaleGesture.IsInMolecularWorld)
        {
            previousBButton = false;
            return;
        }

        // ====================================================
        // EXIT MOLECULAR MODE
        // ====================================================

        bool questBPressed = false;

        UnityEngine.XR.InputDevice rightController =
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(
                UnityEngine.XR.XRNode.RightHand
            );

        bool questBButton = false;

        if (rightController.isValid)
        {
            rightController.TryGetFeatureValue(
                UnityEngine.XR.CommonUsages.secondaryButton,
                out questBButton
            );

            questBPressed =
                questBButton && !previousBButton;
        }

        // Keyboard fallback for testing in Unity Editor.
        bool keyboardBPressed =
            Keyboard.current != null &&
            Keyboard.current.bKey.wasPressedThisFrame;

        if (questBPressed || keyboardBPressed)
        {
            Debug.Log("EXIT MOLECULAR MODE requested");

            molecularScaleGesture.ExitMolecularWorld();

            previousBButton = questBButton;

            return;
        }

        previousBButton = questBButton;

        // ====================================================
        // MOVEMENT INPUT
        // ====================================================

        Vector2 movementInput = Vector2.zero;

        // ---------------------------
        // QUEST LEFT THUMBSTICK
        // ---------------------------

        UnityEngine.XR.InputDevice leftController =
            UnityEngine.XR.InputDevices.GetDeviceAtXRNode(
                UnityEngine.XR.XRNode.LeftHand
            );

        Vector2 questThumbstick = Vector2.zero;

        if (leftController.isValid)
        {
            leftController.TryGetFeatureValue(
                UnityEngine.XR.CommonUsages.primary2DAxis,
                out questThumbstick
            );

            movementInput += questThumbstick;
        }

        // ---------------------------
        // KEYBOARD FALLBACK
        // ---------------------------

        if (Keyboard.current != null)
        {
            // UP / DOWN
            if (Keyboard.current.upArrowKey.isPressed)
                movementInput.y += 1f;

            if (Keyboard.current.downArrowKey.isPressed)
                movementInput.y -= 1f;

            // LEFT / RIGHT
            if (Keyboard.current.leftArrowKey.isPressed)
                movementInput.x -= 1f;

            if (Keyboard.current.rightArrowKey.isPressed)
                movementInput.x += 1f;
        }

        // Prevent diagonal movement being faster.
        if (movementInput.magnitude > 1f)
        {
            movementInput.Normalize();
        }

        // ====================================================
        // MOVE THROUGH MOLECULAR WORLD
        // ====================================================

        if (molecularWorld == null ||
            headCamera == null)
            return;

        // Move relative to the direction the user's head is facing.
        Vector3 forward = headCamera.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = headCamera.right;
        right.y = 0f;
        right.Normalize();

        Vector3 desiredMovement =
            forward * movementInput.y +
            right * movementInput.x;

        // Move the molecular environment opposite the desired
        // movement direction. To the user this appears as walking
        // through the molecular environment.
        molecularWorld.position -=
            desiredMovement *
            moveSpeed *
            Time.deltaTime;
    }
}