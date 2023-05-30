using System;
using Sirenix.OdinInspector;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SmartMissile : MonoBehaviour
{
    [Header("编辑")]
    public Transform target;
    public float          maxSpeed = 20;
    public float          maxAngleSpeed;

    public float          toMaxSpeedTime = 2;
    public float          toMaxAngleSpeedTime = 2;
    public AnimationCurve toMaxSpeedCurve;
    public AnimationCurve toMaxAngleSpeedCurve;

    public float   initSpeed     = 10;
    public Vector3 initTransform = Vector3.up;
    public float   randomAngleRange = 30;
    
    [Header("runtime")]
    public float speed;
    public float angleSpeed;
    public float elapsedTime;

    private void Start()
    {
        var actorManager = FindObjectOfType<ActorsManager>();
        if (actorManager != null)
        {
            target = actorManager.Player.transform;
        }
        else
        {
            enabled = false;
            return;
        }
        
        Shoot();
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }

    [Button]
    public void Shoot()
    {
        speed = initSpeed;
        transform.forward = initTransform;
        transform.Rotate(new Vector3(GetRandomAngle(), GetRandomAngle(), 0));

        float GetRandomAngle()
        {
            return Random.Range(-randomAngleRange, randomAngleRange);
        }
        
    }
    private void Update()
    {
        // Note Calculate Speed
        var deltaTime = Time.deltaTime;
        elapsedTime += deltaTime;

        var speedLerp = toMaxSpeedCurve.Evaluate(elapsedTime / toMaxSpeedTime);
        speed = Mathf.Lerp(speed, maxSpeed, speedLerp);
        
        var angleSpeedLerp = toMaxAngleSpeedCurve.Evaluate(elapsedTime / toMaxAngleSpeedTime);
        angleSpeed = Mathf.Lerp(angleSpeed, maxAngleSpeed, angleSpeedLerp);
        
        // Modify position and Rotation;

        var targetDir  = target.position - transform.position;
        // Vector3.SignedAngle(transform.up, diff)

        //transform.up = Vector3.Slerp(transform.up, targetDiff, angleSpeedLerp);
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDir, Mathf.Deg2Rad * angleSpeed * deltaTime, 0));

            
        transform.position += transform.forward * speed * deltaTime;

    }
}