using UnityEngine;

public class CanvasRotation : MonoBehaviour
{
    public RotationAxis rotationAxis = RotationAxis.Y;

    private void LateUpdate()
    {
        Vector3 eulerRotation = transform.rotation.eulerAngles;

        switch (rotationAxis)
        {
            case RotationAxis.X:
                eulerRotation.x = Camera.main.transform.rotation.eulerAngles.x;
                break;
            case RotationAxis.Y:
                eulerRotation.y = Camera.main.transform.rotation.eulerAngles.y;
                break;
            case RotationAxis.Z:
                eulerRotation.z = Camera.main.transform.rotation.eulerAngles.z;
                break;
        }

        transform.rotation = Quaternion.Euler(eulerRotation);
    }
}

public enum RotationAxis
{
    X,
    Y,
    Z
}
