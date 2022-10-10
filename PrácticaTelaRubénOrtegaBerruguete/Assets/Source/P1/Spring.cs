using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring 
{

    public Node nodeA, nodeB;

    public float Length0;
    public float Length;
    private float damp;

    public float stiffness;

    public Spring(Node nodeA, Node nodeB, float stiffness, float damping)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        this.stiffness = stiffness;
        this.damp = damping;

        UpdateLength();
        Length0 = Length;
    }

    public void UpdateLength()
    {
        Length = (nodeA.pos - nodeB.pos).magnitude;
    }
 
    public void ComputeForces()
    {
        Vector3 u = nodeA.pos - nodeB.pos;
        u.Normalize();
        Vector3 force = -stiffness * (Length - Length0) * u;
        force += -(damp * Vector3.Dot(u, nodeA.vel - nodeB.vel)) * u;//amortiguamiento de deformación
        nodeA.force += force;
        nodeB.force -= force;
    }
}
