using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using System.Text;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Rendering;

public class CLA_TerraformingHandler : MonoBehaviour
{
    [Header("Output Textures")]
    public CLA_Texture2D colorTexture_;
    public CLA_Texture2D heightmapTexture_;

    [Header("Biome Template")]
    public SOB_BiomeTemplate biomeTemplate_;

    [Header("Biome Palette")]
    public Texture2D biomePalette_;

    [Header("Key Binding")]
    public KeyCode clearMap_;
    public KeyCode generateMap_;

    [Header("Terrain")]
    public Terrain ground_;
    public TerrainData groundData_;
    [Space(10)]
    public Terrain water_;
    public TerrainData waterData_;

    public void Start()
    {
        GenerateNoiseMap(16, new Vector2Int(heightmapTexture_.offset_.x, heightmapTexture_.offset_.y), heightmapTexture_);
        ColorizeTexture(colorTexture_, heightmapTexture_);
        UpdateHeightmap(heightmapTexture_, groundData_);
    }

    private void Update()
    {
        if (Input.GetKeyDown(generateMap_))
        {
            GenerateNoiseMap(16, new Vector2Int(heightmapTexture_.offset_.x, heightmapTexture_.offset_.y), heightmapTexture_);
            ColorizeTexture(colorTexture_, heightmapTexture_);
            UpdateHeightmap(heightmapTexture_, groundData_);
        }

        if (Input.GetKeyDown(clearMap_))
        {
            DecolorizeTexture(colorTexture_);
        }
    }

    private void ColorizeTexture(CLA_Texture2D texture, CLA_Texture2D heightmap)
    {
        Vector2 longitude = new Vector2((((uint)biomeTemplate_.coastalness_       * 18u) + 1u) / 90.0f, 
                                       ((((uint)biomeTemplate_.coastalness_ + 1u) * 18u) - 1u) / 90.0f);

        Vector2Int coordinates = Vector2Int.zero;
        Vector2Int pixel = Vector2Int.zero;

        for (int y = 0; y < texture.size_; ++y)
        {
            for (int x = 0; x < texture.size_; ++x)
            {
                if (heightmap.texture_.GetPixel(x, y).r < biomeTemplate_.seaLevel_)
                {
                    texture.texture_.SetPixel(x, y, Color.cyan);
                    continue;
                }

                coordinates.x = (int)(x / texture.size_);
                pixel.x = ((int)biomePalette_.Size().x) * coordinates.x;
                pixel.x = (int)math.remap(0.0f, biomePalette_.Size().x, biomePalette_.Size().x * longitude.x, biomePalette_.Size().x * longitude.y, pixel.x);

                coordinates.y = (int)(y / texture.size_);
                pixel.y = (int)(biomePalette_.Size().y * heightmap.texture_.GetPixel(x, y).r);
                pixel.y = (int)math.remap(0.0f, biomePalette_.Size().y, biomePalette_.Size().y * 0.0f, biomePalette_.Size().y * 1.0f, pixel.y);

                texture.texture_.SetPixel(x, y, biomePalette_.GetPixel(pixel.x, pixel.y));
            }
        }

        texture.texture_.Apply();
    }

    private void DecolorizeTexture(CLA_Texture2D texture)
    {
        for (int y = 0; y < texture.size_; ++y)
        {
            for (int x = 0; x < texture.size_; ++x)
            {
                texture.texture_.SetPixel(x, y, Color.white);
            }
        }

        texture.texture_.Apply();
    }

    private void GenerateNoiseMap(uint octaves, Vector2Int origin, CLA_Texture2D output)
    {
        UnityEngine.Random.InitState(output.seed_.Length > 0 ? output.seed_.GetHashCode() : UnityEngine.Random.Range(0, 9999));

        Vector2Int pivot = new Vector2Int(UnityEngine.Random.Range(-9999, 9999), UnityEngine.Random.Range(-9999, 9999));

        // loops through each pixel
        for (int y = 0; y < output.size_; ++y)
        {
            for (int x = 0; x < output.size_; ++x)
            {
                float noise = GetNoiseAtPosition(pivot.x + x, pivot.y + y, octaves, origin, output.size_, output.scale_);

                output.texture_.SetPixel(x, y, new Color(noise, noise, noise, 1.0f));
            }
        }

        output.texture_.Apply();
    }

    private float GetNoiseAtPosition(int x, int y, uint octaves, Vector2Int origin, uint size, float scale)
    {
        float pixel = 0.00f;
        float opcty = 1.00f;
        float mxval = 0.00f;

        for (int index = 0; index < octaves; ++index)
        {
            float x_internal = (x / (size / scale)) + origin.x;
            float y_internal = (y / (size / scale)) + origin.y;

            pixel += (noise.snoise(new float2(x_internal, y_internal)) + biomeTemplate_.terrainHeight_) * biomeTemplate_.globalHeight_ / opcty;
            mxval += 1.0f / opcty;

            scale *= 2.0f;
            opcty *= 2.0f;
        }

        return pixel / mxval;
    }

    private void UpdateHeightmap(CLA_Texture2D heightmap, TerrainData data)
    {
        Undo.RegisterCompleteObjectUndo(data, "Heightmap From Texture");
        Vector2Int heightmap_size = new Vector2Int((int)heightmap.texture_.Size().x, (int)heightmap.texture_.Size().y);
        int heightmap_resolution = data.baseMapResolution;
        float[,] heightmap_data = data.GetHeights(0, 0, heightmap_size.x, heightmap_size.y);
        Color[] heightmap_colors = heightmap.texture_.GetPixels();
        Color[] map = new Color[heightmap_resolution * heightmap_resolution];

        if (heightmap.texture_.filterMode == FilterMode.Point)
        {
            float dx = (float)heightmap_size.x / heightmap_resolution;
            float dy = (float)heightmap_size.y / heightmap_resolution;

            for (int y = 0; y < heightmap_resolution; ++y)
            {
                var thisy = (int)(dy * y) * heightmap_size.x;
                var yw = y * heightmap_resolution;

                for (int x = 0; x < heightmap_resolution; ++x)
                {
                    map[yw + x] = heightmap_colors[thisy + (int)dx * x];
                }
            }
        }

        for (int y = 0; y < heightmap_resolution; ++y)
        {
            for (int x = 0; x < heightmap_resolution; ++x)
            {
                heightmap_data[y, x] = map[y * heightmap_resolution + x].grayscale;
            }
        }

        data.SetHeights(0, 0, heightmap_data);
    }
}
