using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum Direction { North, East, South, West};
    public Direction playerDirection = Direction.South;
    private Animator playerAnimator;
    public float maxSpeed = 7;
    private Vector2 targetVelocity;
    private Collider2D playerCollider;
    private ContactFilter2D movementContactFilter;

    private float minMoveDistance = 0.001f;
    private float shellRadius = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        movementContactFilter = BuildContactFilter2DForLayer(gameObject.layer);
    }

    // Update is called once per frame
    void Update()
    {
        ComputeVelocity();
        Vector2 velocityX = new Vector2();
        velocityX.x = targetVelocity.x * Time.deltaTime;
        Vector2 velocityY = new Vector2();
        velocityY.y = targetVelocity.y * Time.deltaTime;

        PlayerMove(velocityX);
        PlayerMove(velocityY);
    }

    private ContactFilter2D BuildContactFilter2DForLayer(LayerMask Layer)
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;
        contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(Layer));
        contactFilter2D.useLayerMask = true;

        return contactFilter2D;
    }

    private void ComputeVelocity() //calcule a quel vitesse le joueur souhaite aller
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");
        targetVelocity = move.normalized * maxSpeed;
    }

    private void PlayerMove(Vector2 move)
    {
        float distance = move.magnitude;
        RaycastHit2D[] hitbuffer = new RaycastHit2D[16];
        if (distance > minMoveDistance)
        {
            int movementColisionHitCount = playerCollider.Cast(move, movementContactFilter, hitbuffer, distance + shellRadius);
            List<RaycastHit2D> hitBufferList = BufferArrayHitToList(hitbuffer, movementColisionHitCount);
            for (int i = 0; i < hitBufferList.Count; i++)
            {
                float currentDistance = hitBufferList[i].distance - shellRadius;
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                }
            }
        }
        gameObject.transform.Translate(move.normalized * distance);
    }

    private List<RaycastHit2D> BufferArrayHitToList(RaycastHit2D[] hitbuffer, int count)
    {
        List <RaycastHit2D> bufferHitList = new List<RaycastHit2D>(count);
        bufferHitList.Clear();
        for (int i = 0; i < count; i++)
        {
            bufferHitList.Add(hitbuffer[i]);
        }

        return bufferHitList;
    }
}
