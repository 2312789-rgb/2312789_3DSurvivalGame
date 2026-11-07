using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float YRotation = 0f;

    void Start()
    {
        // Luôn hiển thị chuột khi chạy game
        GameplayUI.UpdateCursor();
        xRotation = transform.localEulerAngles.x;
        if (xRotation > 180f) xRotation -= 360f;
        YRotation = transform.localEulerAngles.y;
    }

    void Update()
    {
        // Nếu Inventory đang đóng thì vẫn cho camera xoay
        if (!GameplayUI.IsOpen)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Xoay lên xuống
            xRotation -= mouseY;

            // Giới hạn góc nhìn
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Xoay trái phải
            YRotation += mouseX;

            // Áp dụng xoay
            transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);
        }
    }
}