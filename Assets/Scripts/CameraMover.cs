using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraMover : MonoBehaviour
{
    [Header("Swap Params")]
    [SerializeField] private float swapSpeed;
    [SerializeField] private float swapRoatationSpeed;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Transform secondTransform;
    
    private bool _cameraPos = false;
    
    

    public void SwapCamera() 
    {
        if(_cameraPos)
        {
            StopAllCoroutines();
            StartCoroutine(CameraInterpolate(transform, mainTransform));
            _cameraPos = false;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(CameraInterpolate(transform, secondTransform));
            _cameraPos = true;
        }
    }

    public void SwapCamera(bool pos) 
    {
        if (pos)
        {
            StopAllCoroutines();
            StartCoroutine(CameraInterpolate(transform, mainTransform));
            _cameraPos = false;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(CameraInterpolate(
                transform, secondTransform));
            _cameraPos = true;
        }
    }

    private IEnumerator CameraInterpolate(Transform trans, Transform to) 
    {
        while(trans.position != to.position && trans.rotation != to.rotation)
        {
            trans.position = Vector3.Lerp(trans.position, to.position, Time.deltaTime * swapSpeed);
            trans.rotation = Quaternion.Slerp(trans.rotation, to.rotation, Time.deltaTime * swapRoatationSpeed);
            yield return null;
        }
    }
}
