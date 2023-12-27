using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleCarTriggerHandler : MonoBehaviour
{
    private float timer = 0;
    private ZombieKillbox[] killboxes;
    private Rigidbody rb;
    private float massOfZombiesSinceTimerStart = 0f;
    public float timeBetweenCollisionsToReset = .5f;
    public float scalingAmountLinear = 1.0f;
    public float scalingAmountAngular = 1.0f;
    
    public Vector3 centerOfRotation = Vector3.zero;
    public float velocityToDamageFactor = 1.0f;
    public float driftFactor = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        killboxes = GetComponentsInChildren<ZombieKillbox>();
        //Debug.Log(killboxes.Length);
        foreach(ZombieKillbox killbox in killboxes)
        {
            killbox.mHandler = this;
        }
        rb = transform.parent.parent.gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.Log("No parent RigidBody found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > timeBetweenCollisionsToReset)
        {
            timer = 0;
            massOfZombiesSinceTimerStart = 0;
        }
    }
    public int ZombieHit(Collider other, Vector3 faceDir, EnemyStats enemy, bool currentlyStopped = false)
    {
        timer = 0;
        Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();
        massOfZombiesSinceTimerStart += otherRB.mass;
        
        if(Vector3.Dot(faceDir, Vector3.down) > .9 && Vector3.Dot(Vector3.Normalize(rb.velocity), Vector3.down) > .5 && Vector3.Magnitude(rb.velocity) > 5)
        {
            int kill = enemy.Damage(1000);
            return kill;
        }
        /*if (CarManager.Instance.isSpinning)
        {
            return enemy.Damage(60);
        }*/
        //Get relevant variables in local coordinates for accurate math
        Vector3 localizedVel = transform.InverseTransformDirection(rb.velocity);
        Vector3 localizedOtherPos = transform.InverseTransformPoint(rb.position);


        //Amount of velocity you are running into zombies with linearly = the amount of velocity you have total in the direction of the normal of the face they collided with
        float linearHitboxEffectiveness = Vector3.Dot(faceDir, localizedVel);

        //A perpendicular vector to the direction of angular velocity and the normal of the face they collided with (positive is the direction where the car is spinning towards the zombie, negative the car is spinning away from it)
        //This vector's magnitude is approximately the angular velocity in the plane of the car
        Vector3 perpendicularBasedOnAngularVel = Vector3.Cross(faceDir, rb.angularVelocity);
        //The amount of velocity going into the zombie rotationally depends on the angular velocity, wherether the car is rotating into or away from the zombie at that point, and how long the lever arm is
        // This is accomplished by projecting the vector from the center of rotation to the zombie on to the vector onto the perpendicular from the last step
        float rotationalHitboxEffectiveness = Vector3.Dot(perpendicularBasedOnAngularVel, localizedOtherPos - centerOfRotation);
        //The total velocity going into the zombie at a point is the amount of velocity going into it from linear motion and the amount going from rotational motion, if the net movement is away from the zombie, clamp it to 0 so it isnt healed.
        float totalEffectiveness = Mathf.Clamp(linearHitboxEffectiveness + rotationalHitboxEffectiveness, 0f, Mathf.Infinity);
        float drifting = CarManager.currentState == CarManager.CarState.DriftingRight || CarManager.currentState == CarManager.CarState.DriftingLeft ? driftFactor : 1.0f;
        float spinning = CarManager.Instance.isSpinning ? 5.0f : drifting;
        Vector3 hitDir = -1.0f * (linearHitboxEffectiveness * Vector3.Normalize(rb.velocity));
        float calculatedDamage = totalEffectiveness * velocityToDamageFactor * spinning;
        int killed = 0;
        if (calculatedDamage > 2 || (CarInputManager.Instance.boost && !CarInputManager.Instance.boostRefreshing))
        {
            if (CarInputManager.Instance.boost && !CarInputManager.Instance.boostRefreshing)
                calculatedDamage = 999;
            
            killed = enemy.Damage(calculatedDamage);
        }
            
        if (enemy.isSwole)
        {
            enemy.Push(5 * Vector3.Dot((localizedOtherPos - CarManager.Instance.gameObject.transform.position).normalized, hitDir), true);
            if (!currentlyStopped)
            {
                if (CarManager.carSpeed > 60)
                {
                    enemy.Push(5000, true);
                    var vel = rb.velocity;
                    vel = vel * .5f;
                    rb.velocity = vel;
                }
            }
        }
        else
        {
            enemy.Push(Vector3.Dot((localizedOtherPos - CarManager.Instance.gameObject.transform.position).normalized, hitDir), true);
        }
        /*
        //if the car will slow down, slow it down based on the scaling factor-- This helps prevent physics glitches
        if(Vector3.Dot(hitDir,rb.velocity) <= 0)
        {
            //Debug.Log(hitDir * GetHitScaling() / Time.deltaTime);
            rb.AddForce(hitDir * GetHitScaling() / Time.deltaTime, ForceMode.Impulse);
            rb.AddTorque(angularChange * GetHitScalingAngular() / Time.deltaTime, ForceMode.Impulse);
        }
        */

        return killed;
    }

    public float GetHitScaling()
    {
        return scalingAmountLinear * massOfZombiesSinceTimerStart;
    }
    public float GetHitScalingAngular()
    {
        return scalingAmountAngular * massOfZombiesSinceTimerStart;
    }
}
