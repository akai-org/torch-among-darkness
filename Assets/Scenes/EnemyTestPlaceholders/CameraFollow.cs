using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;
    public float followSpeed = 2f;
    public string targetTag = "Player";

    void Start()
    {

        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);

        if (targetObject != null){
            target = targetObject.transform;
        }
        else{
            Debug.LogWarning("No GameObject " + targetTag + " found");
        }
    }

    // offset is not set for now
    public float offsetY = 1f;
    public float offsetX = 1f;

    void Update()
    {
        Vector3 changedPosition = new Vector3(target.position.x, target.position.y, -10f);
        transform.position = Vector3.Slerp(transform.position, changedPosition, followSpeed * Time.deltaTime);

    }
}
