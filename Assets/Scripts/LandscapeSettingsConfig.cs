using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "LandscapeSettingsConfig", menuName = "ScriptableObjects/CreateNewLandscapeSettingsConfig")]

public class LandscapeSettingsConfig : ScriptableObject
{
    #region Serialize Fields
    [SerializeField]
    private PerlinNoiseSettings[] settings = null;
    #endregion

    #region Public Fields
    public float[,] Heights
    {
        get => _heights;
        private set => _heights = value;
    }
    #endregion

    #region Private Fields
    private float[,] _heights;
    #endregion


    #region Public Methods
    public float[,] GetHeightsSimple(int positionX, int positionY, int dimension) // simple landscape
    {
        Heights = new float[dimension, dimension];
        int arrayLenght = settings.Length;

        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                float height = 0.0f;
                
                for (int i = 0; i < arrayLenght; i++)
                {
                    float scale = settings[i].scale;

                    height += Mathf.PerlinNoise((x + positionX + settings[i].offsetX) * scale, (y + positionY + settings[i].offsetY) * scale) * settings[i].weight;
                }
                
                Heights[x, y] = height;
            }
        }

        return Heights;
    }
    
    public float[,] GetHeights(int positionX, int positionY, int dimension) // complex landscape
    {
        Heights = new float[dimension, dimension];
        int arrayLenght = settings.Length;
        float[] heightInOctaves = new float[arrayLenght];

        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                for (int i = 0; i < arrayLenght; i++)
                {
                    float scale = settings[i].scale;

                    heightInOctaves[i] = Mathf.PerlinNoise((x + positionX + settings[i].offsetX) * scale, (y + positionY + settings[i].offsetY) * scale) * settings[i].weight;
                }

                float noise = Mathf.PerlinNoise((x + positionX - settings[1].offsetX) * settings[1].scale, (y + positionY - settings[1].offsetY) * settings[1].scale) * settings[1].weight;
                heightInOctaves[0] -= 0.6f;
                heightInOctaves[1] = Mathf.Pow(heightInOctaves[1],2.0f);
                heightInOctaves[2] *= Mathf.Clamp01(2.0f * noise);
                heightInOctaves[3] *= Mathf.Clamp01(heightInOctaves[3] / settings[3].weight * (heightInOctaves[2]) / settings[2].weight);
                
                Heights[x, y] = heightInOctaves.Sum();
            }
        }

        return Heights;
    }

    public float GetHeight(Vector2Int position)
    {
        int arrayLenght = settings.Length;
        float[] heightInOctaves = new float[arrayLenght];

        for (int i = 0; i < arrayLenght; i++)
        {
            float scale = settings[i].scale;
            heightInOctaves[i] = Mathf.PerlinNoise((position.x + settings[i].offsetX) * scale, (position.y + settings[i].offsetY) * scale) * settings[i].weight;
        }

        float noise = Mathf.PerlinNoise((position.x - settings[1].offsetX) * settings[1].scale, (position.y - settings[1].offsetY) * settings[1].scale) * settings[1].weight;
        heightInOctaves[0] -= 0.6f;
        heightInOctaves[1] = Mathf.Pow(heightInOctaves[1],2.0f);
        heightInOctaves[2] *= Mathf.Clamp01(2.0f * noise);
        heightInOctaves[3] *= Mathf.Clamp01(heightInOctaves[3] / settings[3].weight * (heightInOctaves[2]) / settings[2].weight);

        return heightInOctaves.Sum();;
    }

    public IEnumerator InitHeights(int positionX, int positionY, int dimension)
    {
        Thread thread = new Thread(() => GetHeights(positionX, positionY, dimension));
        thread.Start();
        yield return new WaitWhile(() => thread.IsAlive);
    }
    #endregion
}

[Serializable]
public class PerlinNoiseSettings
{
    #region Public Methods
    public int offsetX = 0;
    public int offsetY = 0;
    [Range(0.0001f,0.3f)]
    public float scale = 1f;
    [Range(0,1)]
    public float weight = 1;
    #endregion
}
