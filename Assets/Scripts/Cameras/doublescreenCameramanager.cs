using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class doublescreenCameramanager : MonoBehaviour
{

    public GameObject plane;
    public GameObject blend;
    public Color backgroundColor;

    [Range(0, 100)]
    public float percentageOfCameraOverlap;
    /// <summary>
    /// This script just open the two screens on the same allication
    /// </summary>
    void Awake()
    {
        //NB: screen indexes start from 1
        for (int i = 0; i < GameObject.FindObjectsOfType<Camera>().Length; i++)
        {
            if (i < Display.displays.Length)
            {
                Display.displays[i].Activate();
            }
        }

        correctCameraAndBlendPos();


        Camera cam1 = transform.GetChild(0).GetComponent<Camera>();
        Camera cam2 = transform.GetChild(1).GetComponent<Camera>();

        correctBlendScale();

        Vector3 lookDirection = cameraLookDirectionVector(cam1);
        float yourShiftY = GetViewMatrixShiftY(cam1, lookDirection);
     
        float originalAspect = 9f / 16f;
        float targetAspect = getTargetAspect();

        float scaleFactor = targetAspect / originalAspect;
        yourShiftY /= scaleFactor;

        Matrix4x4 projectionMatrix = cam1.projectionMatrix;

        projectionMatrix.m11 /= scaleFactor;
        projectionMatrix.m12 += yourShiftY;

        cam1.projectionMatrix = projectionMatrix;
        cam2.projectionMatrix = projectionMatrix;

        transform.GetChild(0).GetComponent<Camera>().backgroundColor = backgroundColor;
        transform.GetChild(1).GetComponent<Camera>().backgroundColor = backgroundColor;
    }

    private void correctBlendScale()
    {
        Vector3 camPos = this.transform.position;
        Vector3 blendPos = blend.transform.position;
        Vector3 planePos = plane.transform.position;

        float cameraBlendRatio = Vector3.Distance(camPos, blendPos) / Vector3.Distance(camPos, planePos);
        float multiplyer = plane.transform.lossyScale.z / 10.0f;

        float blendScale = (percentageOfCameraOverlap / 10f) * cameraBlendRatio * multiplyer;

        Vector3 newScale = blend.transform.localScale;
        newScale.z = blendScale;
        newScale.x *= multiplyer;
        blend.transform.localScale = newScale;
    }

    private void correctCameraAndBlendPos()
    {
        Vector3 plane_pos = plane.transform.position;
        Vector3 plane_scale = plane.transform.lossyScale;

        this.transform.position = new Vector3(0f, plane_scale.x * 13.9f, 0f) + plane_pos;
        blend.transform.position = new Vector3(0f, ((this.transform.position.y - plane_pos.y) / 2.0f) , 0f) + plane_pos;
    }

    private float getTargetAspect()
    {
        Vector3 planeScale = plane.transform.lossyScale * 10;
        Vector3 bottomCameraVision = new Vector3(0f, 0f, -(planeScale.z / 2));
        Vector3 topCameraVision = new Vector3(0f, 0f, (planeScale.z / 2) * (percentageOfCameraOverlap / 100));
        Vector3 leftCameraVision = new Vector3(-(planeScale.x / 2), 0f, 0f);
        Vector3 rightCameraVision = new Vector3(planeScale.x / 2, 0f, 0f);

        float scale = Vector3.Distance(topCameraVision, bottomCameraVision) / Vector3.Distance(leftCameraVision, rightCameraVision);

        return scale;
    }

    private Vector3 cameraLookDirectionVector(Camera cam)
    {
        Vector3 plane_pos = plane.transform.position;
        Vector3 planeScale = plane.transform.lossyScale * 10;
        Vector3 bottomCameraVision = new Vector3(0f, 0f, -(planeScale.z / 2)) + plane_pos;
        Vector3 topCameraVision = new Vector3(0f, 0f, (planeScale.z / 2) * (percentageOfCameraOverlap / 100)) + plane_pos;

        Vector3 middleCameraVision = (topCameraVision + bottomCameraVision) / 2;

        Vector3 lookDirection = cameraToPointDirection(cam, middleCameraVision);

        return lookDirection;
    }

    private Vector3 cameraToPointDirection(Camera cam, Vector3 posToLook)
    {
        Vector3 direction = posToLook - cam.transform.position;

        direction.Normalize();

        return direction;
    }

    private float GetViewMatrixShiftY(Camera cam, Vector3 lookDirection)
    {
        Vector3 pointInWorldSpace = cam.transform.position + lookDirection;

        Vector3 viewportPoint = cam.WorldToViewportPoint(pointInWorldSpace);

        float normalizedY = (viewportPoint.y * 2f) - 1f;

        return normalizedY;
    }
}
