using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class ThreadTest : MonoBehaviour
{
    private float[,] values = new float[32, 32];
    private float a = 0;
    private Thread MyThread;
    public Terrain _terrain;
    private TerrainData _terrainData;
    void Start()
    {
        initValues();
        _terrainData = _terrain.terrainData;
        //_terrainData.SetHeights(0, 0, values);
        MyThread = new Thread(Calculation);
        var a = Task.Factory.StartNew(Calculation);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            MyThread.Start();
    }

    private void Calculation()
    {
        Debug.Log("thread start");
        Debug.Log(transform.position.x);
        _terrainData.SetHeights(0, 0, values);
        /*for (int i = 0; i < 10000; i++)
        {
            for (int j = 0; j < 10000; j++)
            {
                //values[i, j] = Mathf.PerlinNoise(i / 100f, j / 100f);
                a =  Mathf.PerlinNoise(i / 100f, j / 100f);
            }
        }*/
        Debug.Log("thread finish");
    }

    private void initValues()
    {
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                values[i, j] = Random.Range(0.0f, 1.0f);
            }
        }
    }
}
