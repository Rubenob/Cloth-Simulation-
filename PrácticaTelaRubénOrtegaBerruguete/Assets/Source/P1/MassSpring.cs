using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Sample code for accessing MeshFilter data.
/// </summary>
public class MassSpring : MonoBehaviour 
{
    /// <summary>
    /// Default constructor. Zero all. 
    /// </summary>
    public MassSpring()
    {
        this.Paused = true;
        this.TimeStep = 0.01f;
        this.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        this.IntegrationMethod = Integration.Symplectic;
        this.mass = 1.0f;
        this.stiffness = 50.0f;
        this.flexStiffness = 10.0f;
        this.damping = 1.0f;
        this.activateWind = false;
        this.windVelocity = new Vector3(-20.0f, 0.0f, 0.0f);
        this.windFriction = 0.5f;
        this.nodes = new List<Node> { };
        this.springs = new List<Spring> { };
        this.edges = new List<Edge> { };
        this.windTriangles = new List<Triangle> { };
    }

    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };


    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public float mass;
    public float stiffness;
    public float flexStiffness;
    public float damping;
    public bool activateWind;
    public Vector3 windVelocity;
    public float windFriction;
    public Vector3 Gravity;
    public Integration IntegrationMethod;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private List<Node> nodes;
    private List<Spring> springs;
    private List<Edge> edges;
    private List<Triangle> windTriangles;

    #endregion

    #region OtherVariables

    #endregion

    #region MonoBehaviour

    public void Awake()
    {
        this.mesh = this.GetComponent<MeshFilter>().mesh;
        this.vertices = mesh.vertices;
        this.triangles = mesh.triangles;

        createNodes(this.vertices);
        createSprings(this.triangles);
    }

    public void Update()
    {
        //Procedure to update vertex positions
        this.mesh = this.GetComponent<MeshFilter>().mesh;
        this.vertices = new Vector3[mesh.vertexCount];

        int i = 0;
        foreach (Node node in nodes)
        {
            Vector3 pos = node.pos;
            this.vertices[i] = transform.InverseTransformPoint(pos);//volvemos a coordenadas locales para darle bien los valores al mesh y hacer bien el pintado
            i++;
        }
        mesh.vertices = this.vertices;
        mesh.RecalculateNormals();//Para que no se vea la geometria tanto y lo pinte bien 

    }

    public void FixedUpdate()
    {
        if (this.Paused) // Si esta parado no se simula nada
            return;

        // Seleccionar el metodo de integración
        switch (this.IntegrationMethod)
        {
            case Integration.Explicit: this.stepExplicit(); break;
            case Integration.Symplectic: this.stepSymplectic(); break;
            default:
                throw new System.Exception("Esto no deberia pasar nunca");
        }
    }

    public void createNodes(Vector3[] v)
    {
        for (int i = 0; i < v.Length; i++)
        {
            Vector3 pos = transform.TransformPoint(v[i]);

            Node newNode = new Node(pos, this.mass, this.Gravity, this.damping);
            nodes.Add(newNode);
        }
    }

    public void createSprings(int[] t)
    {
        float numTriangles = t.Length / 3;
        for (int i = 0; i < numTriangles; i++)
        {
            int j = i * 3;
            int vertex1 = t[j];
            int vertex2 = t[j + 1];
            int vertex3 = t[j + 2];

            Edge newEdge1 = new Edge(vertex1, vertex2, vertex3);
            edges.Add(newEdge1);

            Edge newEdge2 = new Edge(vertex2, vertex3, vertex1);
            edges.Add(newEdge2);

            Edge newEdge3 = new Edge(vertex3, vertex1, vertex2);
            edges.Add(newEdge3);

            Triangle triangle = new Triangle(nodes[vertex1], nodes[vertex2], nodes[vertex3], windFriction, windVelocity);
            windTriangles.Add(triangle);
        }
        edges.Sort(); //Ordenamos los elementos 

        Spring newSpring1 = new Spring(nodes[edges[0].vertexA], nodes[edges[0].vertexB], this.stiffness, this.damping);
        springs.Add(newSpring1);

        float springsTrac = 1;
        float springsFlex = 0;
        for (int i = 1; i < edges.Count; i++)
        {
            if (edges[i].Compare(edges[i - 1])) //si los muelles son iguales
            {
                //Se crean los muelles de flexión 
                Spring newSpring2 = new Spring(nodes[edges[i].vertexC], nodes[edges[i - 1].vertexC], flexStiffness, damping);
                springs.Add(newSpring2);
                springsFlex++;
            }
            else
            {
                Spring newSpring3 = new Spring(nodes[edges[i].vertexA], nodes[edges[i].vertexB], stiffness, damping);
                springs.Add(newSpring3);
                springsTrac++;
            }
        }
        print("Muelles de tracción " + springsTrac);
        print("Muelles de flexión " + springsFlex);
    }
    #endregion
    private void stepExplicit()
    {
        foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces();
        }
        foreach (Spring spring in springs)
        {
            spring.ComputeForces();
        }

        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                node.pos += TimeStep * node.vel;
                node.vel += TimeStep / this.mass * node.force;
            }
        }

        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }
    }

    private void stepSymplectic()
    {
        foreach (Node node in nodes)
        {
            node.force = Vector3.zero;
            node.ComputeForces();
        }
        foreach (Spring spring in springs)
        {
            spring.ComputeForces();
        }

        if (activateWind)
        {
            foreach (Triangle triangle in windTriangles)
            {
                triangle.ComputeForces();
            }
        }

        foreach (Node node in nodes)
        {
            if (!node.isFixed)
            {
                node.vel += TimeStep / this.mass * node.force;
                node.pos += TimeStep * node.vel;
            }
        }

        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }

    }

    public List<Node> getNodes()
    {
        return this.nodes;
    }

}
