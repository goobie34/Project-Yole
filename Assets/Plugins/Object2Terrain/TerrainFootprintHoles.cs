
using UnityEngine;
using UnityEditor;

public class TerrainFootprintHoles : EditorWindow
{
    private Terrain terrain;
    private MeshFilter grassMesh;

    [Header("Sampling")]
    private int samplesPerCell = 3;
    private float rayPadding = 10f;

    [Header("Edge Settings")]
    private int edgeInsetCells = 2;

    [MenuItem("Tools/Terrain/Generate Holes From Mesh")]
    public static void OpenWindow()
    {
        GetWindow<TerrainFootprintHoles>(
            "Terrain Footprint"
        );
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Generate Terrain Holes From Mesh",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        terrain = (Terrain)EditorGUILayout.ObjectField(
            "Target Terrain",
            terrain,
            typeof(Terrain),
            true
        );

        grassMesh = (MeshFilter)EditorGUILayout.ObjectField(
            "Grass Mesh",
            grassMesh,
            typeof(MeshFilter),
            true
        );

        EditorGUILayout.Space();

        GUILayout.Label(
            "Sampling Settings",
            EditorStyles.boldLabel
        );

        samplesPerCell = EditorGUILayout.IntSlider(
            "Samples Per Cell",
            samplesPerCell,
            1,
            5
        );

        // Ensure odd sample counts.
        if (samplesPerCell % 2 == 0)
            samplesPerCell++;

        rayPadding = EditorGUILayout.FloatField(
            "Ray Padding",
            rayPadding
        );

        EditorGUILayout.Space();

        GUILayout.Label(
            "Edge Settings",
            EditorStyles.boldLabel
        );

        edgeInsetCells = EditorGUILayout.IntSlider(
            "Edge Inset (Cells)",
            edgeInsetCells,
            0,
            10
        );

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Keeps Terrain where the grass mesh exists. " +
            "The Edge Inset shrinks the Terrain footprint " +
            "to help hide its boundary.",
            MessageType.Info
        );

        GUI.enabled = terrain != null && grassMesh != null;

        if (GUILayout.Button("Generate Terrain Holes"))
        {
            GenerateHoles();
        }

        GUI.enabled = true;
    }

    private void GenerateHoles()
    {
        if (terrain.terrainData == null ||
            grassMesh.sharedMesh == null)
        {
            Debug.LogError(
                "Missing TerrainData or source mesh."
            );
            return;
        }

        TerrainData data = terrain.terrainData;
        Mesh mesh = grassMesh.sharedMesh;

        int resolution = data.holesResolution;

        if (resolution <= 0)
        {
            Debug.LogError("Invalid Terrain holes resolution.");
            return;
        }

        // Calculate world-space mesh bounds.
        Bounds localBounds = mesh.bounds;

        Vector3 worldMin = new Vector3(
            float.PositiveInfinity,
            float.PositiveInfinity,
            float.PositiveInfinity
        );

        Vector3 worldMax = new Vector3(
            float.NegativeInfinity,
            float.NegativeInfinity,
            float.NegativeInfinity
        );

        for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
                for (int z = 0; z < 2; z++)
                {
                    Vector3 corner = new Vector3(
                        x == 0 ? localBounds.min.x : localBounds.max.x,
                        y == 0 ? localBounds.min.y : localBounds.max.y,
                        z == 0 ? localBounds.min.z : localBounds.max.z
                    );

                    corner = grassMesh.transform.TransformPoint(corner);

                    worldMin = Vector3.Min(worldMin, corner);
                    worldMax = Vector3.Max(worldMax, corner);
                }

        float rayStartY = worldMax.y + rayPadding;

        float rayDistance =
            worldMax.y - worldMin.y + rayPadding * 2f;

        // Create a temporary MeshCollider.
        MeshCollider tempCollider = null;

        try
        {
            tempCollider =
                grassMesh.gameObject.AddComponent<MeshCollider>();

            tempCollider.sharedMesh = mesh;

            Physics.SyncTransforms();

            bool[,] holes = new bool[resolution, resolution];

            RaycastHit[] hitBuffer = new RaycastHit[32];

            // Sample the Terrain.
            for (int z = 0; z < resolution; z++)
            {
                EditorUtility.DisplayProgressBar(
                    "Generating Terrain Holes",
                    $"Processing row {z + 1}/{resolution}",
                    (float)z / resolution
                );

                for (int x = 0; x < resolution; x++)
                {
                    bool covered = false;

                    for (int sz = 0; sz < samplesPerCell; sz++)
                    {
                        for (int sx = 0; sx < samplesPerCell; sx++)
                        {
                            float sampleX =
                                (x + (sx + 0.5f) / samplesPerCell)
                                / resolution;

                            float sampleZ =
                                (z + (sz + 0.5f) / samplesPerCell)
                                / resolution;

                            Vector3 localPosition = new Vector3(
                                sampleX * data.size.x,
                                0f,
                                sampleZ * data.size.z
                            );

                            Vector3 worldPosition =
                                terrain.transform.TransformPoint(
                                    localPosition
                                );

                            Vector3 rayOrigin = new Vector3(
                                worldPosition.x,
                                rayStartY,
                                worldPosition.z
                            );

                            Ray ray = new Ray(
                                rayOrigin,
                                Vector3.down
                            );

                            int hitCount = Physics.RaycastNonAlloc(
                                ray,
                                hitBuffer,
                                rayDistance,
                                ~0,
                                QueryTriggerInteraction.Ignore
                            );

                            for (int i = 0; i < hitCount; i++)
                            {
                                if (hitBuffer[i].collider == tempCollider)
                                {
                                    covered = true;
                                    break;
                                }
                            }

                            if (covered)
                                break;
                        }

                        if (covered)
                            break;
                    }

                    // True = visible Terrain.
                    // False = Terrain hole.
                    holes[z, x] = covered;
                }
            }

            // Apply edge inset.
            bool[,] insetHoles =
                new bool[resolution, resolution];

            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    bool keep = holes[z, x];

                    if (!keep)
                        continue;

                    for (int dz = -edgeInsetCells;
                         dz <= edgeInsetCells; dz++)
                    {
                        for (int dx = -edgeInsetCells;
                             dx <= edgeInsetCells; dx++)
                        {
                            // Circular inset.
                            if (dx * dx + dz * dz >
                                edgeInsetCells * edgeInsetCells)
                                continue;

                            int nx = x + dx;
                            int nz = z + dz;

                            if (nx < 0 || nx >= resolution ||
                                nz < 0 || nz >= resolution ||
                                !holes[nz, nx])
                            {
                                keep = false;
                                break;
                            }
                        }

                        if (!keep)
                            break;
                    }

                    insetHoles[z, x] = keep;
                }
            }

            // Register undo before changing TerrainData.
            Undo.RegisterCompleteObjectUndo(
                data,
                "Generate Terrain Holes"
            );

            data.SetHoles(0, 0, insetHoles);

            EditorUtility.SetDirty(data);

            Debug.Log(
                "Terrain holes successfully generated!"
            );
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();

            if (tempCollider != null)
            {
                DestroyImmediate(tempCollider);
            }
        }

        SceneView.RepaintAll();
    }
}
