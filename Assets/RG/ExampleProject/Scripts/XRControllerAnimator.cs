using UnityEngine;
using UnityEngine.XR;

public class XRControllerAnimator : MonoBehaviour {

    [SerializeField]
    private GameObject controllerGameObject;

    [SerializeField]
    private Animator controllerAnimator;

    public XRNode controllerXRNode;

    void Update() {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(controllerXRNode);
        controllerGameObject.SetActive(controller.isValid);

        if(controller.isValid) {
            if(controller.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPrimaryButtonHeld)) {
                controllerAnimator.SetFloat("Button 1", isPrimaryButtonHeld ? 1.0f : 0.0f);
            }
            if(controller.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isSecondaryButtonHeld)) {
                controllerAnimator.SetFloat("Button 2", isSecondaryButtonHeld ? 1.0f : 0.0f);
            }
            if(controller.TryGetFeatureValue(CommonUsages.menuButton, out bool isMenuButtonHeld)) {
                controllerAnimator.SetFloat("Button 3", isMenuButtonHeld ? 1.0f : 0.0f);
            }

            if(controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue)) {
                controllerAnimator.SetFloat("Joy X", primary2DAxisValue.x);
                controllerAnimator.SetFloat("Joy Y", primary2DAxisValue.y);
            }

            if(controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) {
                controllerAnimator.SetFloat("Trigger", triggerValue);
            }
            if(controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
                controllerAnimator.SetFloat("Grip", gripValue);
            }
        }
    }
}
