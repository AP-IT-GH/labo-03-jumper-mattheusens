using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class JumpAgent : Agent
{
    public GameObject obstacle;
    private ObstacleMovement obstacleM;
    private Rigidbody rb;

    public LayerMask groundLayer;
    
    private float jumpForce = 8f;
    private bool isGrounded = true;
    private int jumpCount = 0;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        obstacleM = obstacle.GetComponent<ObstacleMovement>();
    }
    public override void OnEpisodeBegin()
    {
        jumpCount = 0;

        this.transform.localPosition = new Vector3(0, 0.5f, 0); // Reset Player Position
        rb.velocity = Vector3.zero; // Reset Player Velocity

        // Choosing Obstacle Direction
        int dir = Random.Range(1, 3);
        if (dir == 1) {
            obstacleM.xMov = true;
            obstacle.transform.localPosition = new Vector3(-15, 0.5f, 0);  
        }
        else { 
            obstacleM.xMov = false;
            obstacle.transform.localPosition = new Vector3(0, 0.5f, 15);
        }

        obstacleM.speed = Random.Range(8, 20); // Choosing Obstacle Speed
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition.y); // Agent knows own Y position
        sensor.AddObservation(rb.velocity.y); // Agent knows own Y velocity
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(0.001f); // Reward for staying alive

        float jumpAction = actions.ContinuousActions[0];

        if (jumpAction > 0.5f && isGrounded && jumpCount == 0) // Jump in the air
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
            jumpCount++;
        }
        if((obstacle.transform.localPosition.x > 15 && obstacleM.xMov) || (obstacle.transform.localPosition.z < -15 && !obstacleM.xMov))
        {
            SetReward(1f); // Reward for jumping over obstacle
            EndEpisode();
        } 
    }
    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.6f, groundLayer); 
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-1f); // Reward for touching obstacle
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut) // If controls are with arrow keys
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }
}
