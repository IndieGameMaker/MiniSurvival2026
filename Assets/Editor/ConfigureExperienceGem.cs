using System.IO;
using UnityEditor;
using UnityEngine;

public static class ConfigureExperienceGem
{
    private const string EnemyPrefabPath = "Assets/03_Prefabs/Enemy.prefab";
    private const string GemPrefabPath = "Assets/03_Prefabs/ExperienceGem.prefab";
    private const string GemSpritePath = "Assets/04_Sprites/ExperienceGem.png";

    [MenuItem("Tools/Codex/Configure Experience Gem")]
    public static void Configure()
    {
        CreateGemSprite();
        GameObject gemPrefab = CreateGemPrefab();
        AssignGemToEnemy(gemPrefab);
        AssetDatabase.SaveAssets();
        Debug.Log("Experience Gem setup completed.");
    }

    private static void CreateGemSprite()
    {
        const int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = new Color(0f, 0f, 0f, 0f);
        Color gemColor = new Color(0.55f, 0.2f, 1f, 1f);
        Color highlightColor = new Color(0.85f, 0.65f, 1f, 1f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int distance = Mathf.Abs(x - 7) + Mathf.Abs(y - 7);
                Color pixelColor = clear;
                if (distance <= 7)
                {
                    pixelColor = x <= 7 && y >= 7 ? highlightColor : gemColor;
                }

                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        File.WriteAllBytes(GemSpritePath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(GemSpritePath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(GemSpritePath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 32f;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    private static GameObject CreateGemPrefab()
    {
        GameObject gemObject = new GameObject("ExperienceGem");

        SpriteRenderer spriteRenderer = gemObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(GemSpritePath);
        spriteRenderer.sortingOrder = 1;

        CircleCollider2D gemCollider = gemObject.AddComponent<CircleCollider2D>();
        gemCollider.isTrigger = true;
        gemCollider.radius = 0.22f;

        GameObject gemPrefab = PrefabUtility.SaveAsPrefabAsset(gemObject, GemPrefabPath);
        Object.DestroyImmediate(gemObject);
        return gemPrefab;
    }

    private static void AssignGemToEnemy(GameObject gemPrefab)
    {
        GameObject enemyRoot = PrefabUtility.LoadPrefabContents(EnemyPrefabPath);
        EnemyHealth enemyHealth = enemyRoot.GetComponent<EnemyHealth>();

        SerializedObject serializedHealth = new SerializedObject(enemyHealth);
        serializedHealth.FindProperty("experienceGemPrefab").objectReferenceValue = gemPrefab;
        serializedHealth.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(enemyRoot, EnemyPrefabPath);
        PrefabUtility.UnloadPrefabContents(enemyRoot);
    }
}
