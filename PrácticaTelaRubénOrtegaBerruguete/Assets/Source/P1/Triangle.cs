using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Node nodeA;
    public Node nodeB;
    public Node nodeC;

    public float friction;
    public Vector3 wind;
    public float area;
    public Vector3 vel;
    public Vector3 u;

    public Triangle(Node nodeA, Node nodeB, Node nodeC, float friction, Vector3 wind)
    {
        this.nodeA = nodeA;
        this.nodeB = nodeB;
        this.nodeC = nodeC;

        this.friction = friction;
        this.wind = wind;
        this.area = calculateArea();
        this.vel = (nodeA.vel + nodeB.vel + nodeC.vel) / 3;
    }

    public float calculateArea()
    {
        Vector3 u = (nodeB.pos - nodeA.pos);
        Vector3 v = (nodeC.pos - nodeA.pos);
        Vector3 cross = Vector3.Cross(u, v);
        float magnitude = cross.magnitude;
        float A = magnitude / 2;
        return A;
    }

    public void ComputeForces()
    {
        u = Vector3.Cross((nodeB.pos - nodeA.pos), (nodeC.pos - nodeA.pos)).normalized;
        vel = (nodeA.vel + nodeB.vel + nodeC.vel) / 3;
        nodeA.force += (friction * area * Vector3.Project((wind - vel), u)) / 3;
        nodeB.force += (friction * area * Vector3.Project((wind - vel), u)) / 3;
        nodeC.force += (friction * area * Vector3.Project((wind - vel), u)) / 3;
    }
}
