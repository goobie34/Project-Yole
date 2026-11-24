using UnityEngine;

#if UNITY_EDITOR 
using UnityEditor;

public class MeshToTerrainConverter : EditorWindow
{
    private GameObject sourceObject;

    private int resolution = 1025;

    private float verticalPadding = 0.05f;
    private float surfaceOffset = 0.02f;

    // Reject vertical and nearly vertical faces.
    // 0.25 means the surface normal must point
    // at least partially upward.
    private float minimumUpDot = 0.25f;

    [MenuItem("Tools/Mesh To Terrain Converter")]
    public static void ShowWindow()
    {
        GetWindow<MeshToTerrainConverter>(
            "Mesh To Terrain"
        );
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Mesh To Terrain Converter",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        sourceObject = (GameObject)EditorGUILayout.ObjectField(
            "Source Island",
            sourceObject,
            typeof(GameObject),
            true
        );

        EditorGUILayout.Space();

        resolution = EditorGUILayout.IntPopup(
            "Heightmap Resolution",
            resolution,
            new string[]
            {
                "513",
                "1025",
                "2049"
            },
            new int[]
            {
                513,
                1025,
                2049
            }
        );

        EditorGUILayout.Space();

        GUILayout.Label(
            "Terrain Settings",
            EditorStyles.boldLabel
        );

        verticalPadding = EditorGUILayout.Slider(
            "Vertical Padding",
            verticalPadding,
            0f,
            0.25f
        );

        surfaceOffset = EditorGUILayout.FloatField(
            "Surface Offset",
            surfaceOffset
        );

        EditorGUILayout.Space();

        GUILayout.Label(
            "Surface Filtering",
            EditorStyles.boldLabel
        );

        minimumUpDot = EditorGUILayout.Slider(
            "Minimum Upward Normal",
            minimumUpDot,
            0f,
            1f
        );

        EditorGUILayout.HelpBox(
            "Rejects steep side-facing triangles. " +
            "Higher values reject more steep surfaces. " +
            "Start with 0.25.",
            MessageType.Info
        );

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Terrain holes are generated conservatively. " +
            "A cell must have valid surface samples at " +
            "all four corners and its center.",
            MessageType.Info
        );

        GUI.enabled = sourceObject != null;

        if (GUILayout.Button(
            "Generate Terrain",
            GUILayout.Height(35)))
        {
            GenerateTerrain();
        }

        GUI.enabled = true;
    }

    // ==================================================
    // SURFACE VALIDATION
    // ==================================================

    private bool TrySampleSurface(
        MeshCollider collider,
        float worldX,
        float worldZ,
        float rayStartY,
        float rayDistance,
        out RaycastHit hit)
    {
        Vector3 origin = new Vector3(
            worldX,
            rayStartY,
            worldZ
        );

        Ray ray = new Ray(
            origin,
            Vector3.down
        );

        if (!collider.Raycast(
            ray,
            out hit,
            rayDistance))
        {
            return false;
        }

        // Get the world-space surface normal.
        Vector3 normal = hit.normal.normalized;

        // Compare surface normal against world up.
        float upwardDot = Vector3.Dot(
            normal,
            Vector3.up
        );

        // Reject vertical and downward-facing surfaces.
        if (upwardDot < minimumUpDot)
        {
            return false;
        }

        return true;
    }

    // ==================================================
    // MAIN GENERATOR
    // ==================================================

    private void GenerateTerrain()
    {
        // ----------------------------------------------
        // 1. VALIDATE SOURCE
        // ----------------------------------------------

        MeshFilter meshFilter =
            sourceObject.GetComponent<MeshFilter>();

        MeshCollider meshCollider =
            sourceObject.GetComponent<MeshCollider>();

        if (meshFilter == null ||
            meshFilter.sharedMesh == null)
        {
            Debug.LogError(
                "Source object has no valid MeshFilter."
            );
            return;
        }

        if (meshCollider == null ||
            meshCollider.sharedMesh != meshFilter.sharedMesh)
        {
            Debug.LogError(
                "MeshCollider must use the same mesh " +
                "as the MeshFilter."
            );
            return;
        }

        if (!meshCollider.enabled || meshCollider.convex)
        {
            Debug.LogError(
                "MeshCollider must be enabled and non-convex."
            );
            return;
        }

        Bounds bounds = meshCollider.bounds;

        if (bounds.size.x <= 0f ||
            bounds.size.z <= 0f ||
            bounds.size.y <= 0f)
        {
            Debug.LogError("Invalid mesh bounds.");
            return;
        }

        // ----------------------------------------------
        // 2. TERRAIN DIMENSIONS
        // ----------------------------------------------

        float padding = Mathf.Max(
            0.01f,
            bounds.size.y * verticalPadding
        );

        float terrainBottom =
            bounds.min.y - padding;

        float terrainTop =
            bounds.max.y + padding;

        float terrainHeight =
            terrainTop - terrainBottom;

        float rayStartY =
            terrainTop + padding;

        float rayDistance =
            rayStartY - terrainBottom + 1f;

        // ----------------------------------------------
        // 3. CREATE ARRAYS
        // ----------------------------------------------

        float[,] heights =
            new float[resolution, resolution];

        int holesResolution = resolution - 1;

        bool[,] holes =
            new bool[holesResolution, holesResolution];

        int validHeightSamples = 0;
        int rejectedHeightSamples = 0;
        int validHoleCells = 0;
        int removedHoleCells = 0;

        // ----------------------------------------------
        // 4. GENERATE HEIGHTMAP
        // ----------------------------------------------

        try
        {
            for (int z = 0; z < resolution; z++)
            {
                if (z % 16 == 0)
                {
                    float progress =
                        (float)z / resolution;

                    if (EditorUtility.DisplayCancelableProgressBar(
                        "Generating Terrain",
                        "Sampling valid island surfaces...",
                        progress * 0.5f))
                    {
                        Debug.LogWarning(
                            "Terrain generation cancelled."
                        );
                        return;
                    }
                }

                float normalizedZ =
                    (float)z / (resolution - 1);

                float worldZ =
                    bounds.min.z +
                    normalizedZ * bounds.size.z;

                for (int x = 0; x < resolution; x++)
                {
                    float normalizedX =
                        (float)x / (resolution - 1);

                    float worldX =
                        bounds.min.x +
                        normalizedX * bounds.size.x;

                    if (TrySampleSurface(
                        meshCollider,
                        worldX,
                        worldZ,
                        rayStartY,
                        rayDistance,
                        out RaycastHit hit))
                    {
                        float height =
                            hit.point.y - surfaceOffset;

                        heights[z, x] = Mathf.Clamp01(
                            (height - terrainBottom) /
                            terrainHeight
                        );

                        validHeightSamples++;
                    }
                    else
                    {
                        // Invalid samples are not treated
                        // as actual terrain.
                        heights[z, x] = 0f;

                        rejectedHeightSamples++;
                    }
                }
            }

            // ------------------------------------------
            // 5. GENERATE CONSERVATIVE HOLES MASK
            // ------------------------------------------

            for (int z = 0; z < holesResolution; z++)
            {
                if (z % 16 == 0)
                {
                    float progress =
                        (float)z / holesResolution;

                    if (EditorUtility.DisplayCancelableProgressBar(
                        "Generating Terrain",
                        "Checking island boundaries...",
                        0.5f + progress * 0.5f))
                    {
                        Debug.LogWarning(
                            "Terrain generation cancelled."
                        );
                        return;
                    }
                }

                for (int x = 0; x < holesResolution; x++)
                {
                    // Terrain cell corners.
                    float x0 = (float)x / holesResolution;
                    float x1 = (float)(x + 1) / holesResolution;

                    float z0 = (float)z / holesResolution;
                    float z1 = (float)(z + 1) / holesResolution;

                    // Sample all four corners and center.
                    float centerX = (x0 + x1) * 0.5f;
                    float centerZ = (z0 + z1) * 0.5f;

                    bool corner1 = IsValidSample(
                        meshCollider, bounds, x0, z0,
                        rayStartY, rayDistance
                    );

                    bool corner2 = IsValidSample(
                        meshCollider, bounds, x1, z0,
                        rayStartY, rayDistance
                    );

                    bool corner3 = IsValidSample(
                        meshCollider, bounds, x0, z1,
                        rayStartY, rayDistance
                    );

                    bool corner4 = IsValidSample(
                        meshCollider, bounds, x1, z1,
                        rayStartY, rayDistance
                    );

                    bool center = IsValidSample(
                        meshCollider, bounds, centerX, centerZ,
                        rayStartY, rayDistance
                    );

                    // Only keep cells fully supported
                    // by valid island surface samples.
                    bool validCell =
                        corner1 &&
                        corner2 &&
                        corner3 &&
                        corner4 &&
                        center;

                    holes[z, x] = validCell;

                    if (validCell)
                        validHoleCells++;
                    else
                        removedHoleCells++;
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        // ----------------------------------------------
        // 6. SAVE TERRAIN DATA
        // ----------------------------------------------

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Terrain Data",
            sourceObject.name + "_TerrainData",
            "asset",
            "Choose where to save the TerrainData."
        );

        if (string.IsNullOrEmpty(path))
            return;

        path = AssetDatabase.GenerateUniqueAssetPath(path);

        TerrainData terrainData = new TerrainData();

        terrainData.heightmapResolution = resolution;

        terrainData.size = new Vector3(
            bounds.size.x,
            terrainHeight,
            bounds.size.z
        );

        // ----------------------------------------------
        // 7. APPLY HEIGHTMAP
        // ----------------------------------------------

        terrainData.SetHeights(
            0,
            0,
            heights
        );

        // ----------------------------------------------
        // 8. APPLY HOLES
        // ----------------------------------------------

        terrainData.SetHoles(
            0,
            0,
            holes
        );

        // ----------------------------------------------
        // 9. SAVE ASSET
        // ----------------------------------------------

        AssetDatabase.CreateAsset(
            terrainData,
            path
        );

        AssetDatabase.SaveAssets();

        // ----------------------------------------------
        // 10. CREATE TERRAIN OBJECT
        // ----------------------------------------------

        GameObject terrainObject =
            Terrain.CreateTerrainGameObject(terrainData);

        terrainObject.name =
            sourceObject.name + "_Terrain";

        terrainObject.transform.position = new Vector3(
            bounds.min.x,
            terrainBottom,
            bounds.min.z
        );

        Undo.RegisterCreatedObjectUndo(
            terrainObject,
            "Generate Terrain From Mesh"
        );

        Selection.activeGameObject = terrainObject;

        // ----------------------------------------------
        // 11. REPORT
        // ----------------------------------------------

        Debug.Log(
            "========== TERRAIN COMPLETE ==========\n" +
            "Source: " + sourceObject.name + "\n" +
            "Resolution: " + resolution + "\n" +
            "Valid height samples: " + validHeightSamples + "\n" +
            "Rejected height samples: " + rejectedHeightSamples + "\n" +
            "Valid Terrain cells: " + validHoleCells + "\n" +
            "Removed Terrain cells: " + removedHoleCells + "\n" +
            "Terrain dimensions: " + terrainData.size + "\n" +
            "======================================"
        );
    }

    // ==================================================
    // SAMPLE VALIDATION FOR HOLES
    // ==================================================

    private bool IsValidSample(
        MeshCollider collider,
        Bounds bounds,
        float normalizedX,
        float normalizedZ,
        float rayStartY,
        float rayDistance)
    {
        float worldX =
            bounds.min.x + normalizedX * bounds.size.x;

        float worldZ =
            bounds.min.z + normalizedZ * bounds.size.z;

        return TrySampleSurface(
            collider,
            worldX,
            worldZ,
            rayStartY,
            rayDistance,
            out RaycastHit hit
        );
    }
}

#endif