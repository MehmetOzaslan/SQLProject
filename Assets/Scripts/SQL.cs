using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityNpgsql;



public class SQL : MonoBehaviour
{
    // Start is called before the first frame update
    
    GraphicsBuffer positionsBuffer;
    const int pointCount = 20;
    int effectBufferID = Shader.PropertyToID("GraphicsBuffer");
    int effectTexture = Shader.PropertyToID("Texture2D");


    [SerializeField]
    VisualEffect visualEffect;


    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    struct CrimePoint
    {
        public Vector3 position;
        public int year;

        public CrimePoint(Vector3 position, int year)
        {
            this.position = position;
            this.year = year;
        }
    }
    public const int CrimePointSize = 16;
    CrimePoint[] vertexData = new CrimePoint[pointCount]; 

    private void Start()
    {
        positionsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured , pointCount, CrimePointSize);
        visualEffect.SetGraphicsBuffer(effectBufferID, positionsBuffer);
        for (int i = 0; i < pointCount; i++)
        {
            vertexData[i] = new CrimePoint(new Vector3(0, i, i), i % 20 + 2000);
        }


        positionsBuffer.SetData(vertexData);

    }


    // Update is called once per frame
    void Update()
    {
     
    }
}
