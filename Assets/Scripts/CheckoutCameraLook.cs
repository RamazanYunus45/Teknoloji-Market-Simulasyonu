using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutCameraLook : MonoBehaviour
{
    public float sensitivity = 2f;
    public float smoothing = 1.5f;

    //public float xLimit1 = 120;
    //public float xLimit2 = 240;
    public float yLimit = 90f;

    Vector2 velocity;
    Vector2 frameVelocity;

    void Start()
    {
        // Mevcut kamera rotasyonunu velocity'ye çevir (Baþlangýç açýsýný koru)
        Vector3 angles = transform.localRotation.eulerAngles;

        // Unity EulerAngles 0~360 arasý döner, onu -180~180'e çeviriyoruz
        float yRotation = angles.y > 180 ? angles.y - 360 : angles.y;
        float xRotation = angles.x > 180 ? angles.x - 360 : angles.x;

        velocity = new Vector2(yRotation, -xRotation);
    }

    void Update()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;

        velocity.y = Mathf.Clamp(velocity.y, -yLimit, yLimit);
       // velocity.x = Mathf.Clamp(velocity.x, xLimit1, xLimit2);

        transform.localRotation = Quaternion.Euler(-velocity.y, velocity.x, 0);
    }
}
