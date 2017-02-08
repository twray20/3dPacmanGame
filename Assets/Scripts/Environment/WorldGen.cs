using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldGen : MonoBehaviour
{

    public int seed = 312;

    public int Width;

    public int Height;

    public float tileSize = 40;

    public List<GameObject> WorldTiles;

    public List<GameObject> OuterTiles;

    public List<GameObject> CornerTiles;

    public GameObject Ceiling;

    public bool genRandomOnStart = true;
    System.Random rand = new System.Random();

    // Use this for initialization
    void Start()
    {
        if (genRandomOnStart)
        {
            seed = rand.Next();
        }
        rand = new System.Random(seed);
        GenWorld();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenWorld()
    {
        if (Width <= 0 || Height <= 0)
        {
            Debug.Log("Width/Height must be larger than 0!");
            return;
        }

        if (WorldTiles.Count <= 0 || OuterTiles.Count <= 0 || CornerTiles.Count <= 0)
        {
            Debug.Log("There must be elements in WorldTiles, OuterTiles, and CornerTiles!");
            return;
        }

        //Generates inner area
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {

                GameObject tile = GameObject.Instantiate(WorldTiles[rand.Next(0, WorldTiles.Count)]);
                tile.transform.position = new Vector3(j * tileSize, 0, -i * tileSize);

                float rotation = 0;

                //rotation = GetRandRotation();
                //tile.transform.Rotate(0, rotation, 0);
                //TerrainRotate(tile.GetComponentInChildren<TerrainCollider>().gameObject, rotation);

                if (rotation == 90)
                    tile.GetComponentInChildren<TerrainCollider>().gameObject.transform.localPosition = new Vector3(20, .01f, -20);
                else if(rotation == 180)
                    tile.GetComponentInChildren<TerrainCollider>().gameObject.transform.localPosition = new Vector3(20, .01f, 20);
                else if (rotation == 270)
                    tile.GetComponentInChildren<TerrainCollider>().gameObject.transform.localPosition = new Vector3(-20, .01f, 20);
            }
        }

        //Gennerates outer area (top and bottom)
        for (int i = 0; i < Width; i++)
        {
            GameObject tileUpper = GameObject.Instantiate(OuterTiles[rand.Next(0, OuterTiles.Count)]);
            tileUpper.transform.position = new Vector3(i * tileSize, -10, 1 * tileSize);

            GameObject tileLower = GameObject.Instantiate(OuterTiles[rand.Next(0, OuterTiles.Count)]);
            tileLower.transform.position = new Vector3(i * tileSize, -10, -Height * tileSize);
            tileLower.transform.Rotate(0, 180, 0);
        }

        //Gennerates outer area (left and right)
        for (int i = 0; i < Height; i++)
        {
            GameObject tileLeft = GameObject.Instantiate(OuterTiles[rand.Next(0, OuterTiles.Count)]);
            tileLeft.transform.position = new Vector3(-1 * tileSize, -10, -i * tileSize);
            tileLeft.transform.Rotate(0, -90, 0);

            GameObject tileRight = GameObject.Instantiate(OuterTiles[rand.Next(0, OuterTiles.Count)]);
            tileRight.transform.position = new Vector3(Width * tileSize, -10, -i * tileSize);
            tileRight.transform.Rotate(0, 90, 0);
        }

        //Generates the corner tiles
        GameObject UpperLeft = GameObject.Instantiate(CornerTiles[rand.Next(0, CornerTiles.Count)]);
        UpperLeft.transform.position = new Vector3(-1 * tileSize, -10, 1 * tileSize);
        GameObject UpperRight = GameObject.Instantiate(CornerTiles[rand.Next(0, CornerTiles.Count)]);
        UpperRight.transform.position = new Vector3(Width * tileSize, -10, 1 * tileSize);

        GameObject LowerLeft = GameObject.Instantiate(CornerTiles[rand.Next(0, CornerTiles.Count)]);
        LowerLeft.transform.position = new Vector3(-1 * tileSize, -10, -Height * tileSize);
        LowerLeft.transform.Rotate(0, GetRandRotation(), 0);

        GameObject LowerRight = GameObject.Instantiate(CornerTiles[rand.Next(0, CornerTiles.Count)]);
        LowerRight.transform.position = new Vector3(Width * tileSize, -10, -Height * tileSize);
        LowerRight.transform.Rotate(0, GetRandRotation(), 0);

        GameObject TopCeiling = GameObject.Instantiate(Ceiling);
        TopCeiling.transform.position = new Vector3(((Width - 1) * tileSize) / 2, 189.5f, -((Height - 1) * tileSize) / 2);
        TopCeiling.transform.localScale = new Vector3(Width * tileSize, 1, Height * tileSize);







    }

    private float GetRandRotation()
    {
        int i = rand.Next(0, 4);

        switch (i)
        {
            case 0:
                return 0;
            case 1:
                return 90;
            case 2:
                return 180;
            case 3:
                return 270;
            default:
                return 0;
        }
    }




    private float[,] origHeightMap; // original heightmap, unrotated
    private int[][,] origDetailLayer; // original detail layer, unrotated
    private float[,,] origAlphaMap; // original alphamap, unrotated
    private TreeInstance[] origTrees; // original trees, unrotated

    // rotate terrain
    void TerrainRotate(GameObject go, float angle = 0)
    {
        Terrain t = go.GetComponent<Terrain>();
        origHeightMap = t.terrainData.GetHeights(0, 0, t.terrainData.heightmapWidth, t.terrainData.heightmapHeight);
        origDetailLayer = new int[t.terrainData.detailPrototypes.Length][,];
        for (int n = 0; n < t.terrainData.detailPrototypes.Length; n++)
        {
            origDetailLayer[n] = t.terrainData.GetDetailLayer(0, 0, t.terrainData.detailWidth, t.terrainData.detailHeight, n);
        }
        origAlphaMap = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        origTrees = t.terrainData.treeInstances;


        if (origHeightMap == null)
        {
            //grabOriginal = false;
            Debug.LogWarning("Cannot rotate terrain, array has been cleared..");
            return;
        }

        //isRotating = true;

        Terrain terrain = go.GetComponent<Terrain>();

        int nx, ny;
        float cs, sn;

        // heightmap rotation
        int tw = terrain.terrainData.heightmapWidth;
        int th = terrain.terrainData.heightmapHeight;
        float[,] newHeightMap = new float[tw, th];
        float angleRad = angle * Mathf.Deg2Rad;
        float heightMiddle = (terrain.terrainData.heightmapResolution) / 2.0f; // pivot at middle

        for (int y = 0; y < th; y++)
        {
            for (int x = 0; x < tw; x++)
            {
                cs = Mathf.Cos(angleRad);
                sn = Mathf.Sin(angleRad);

                nx = (int)((x - heightMiddle) * cs - (y - heightMiddle) * sn + heightMiddle);
                ny = (int)((x - heightMiddle) * sn + (y - heightMiddle) * cs + heightMiddle);

                if (nx < 0) nx = 0;
                if (nx > tw - 1) nx = tw - 1;
                if (ny < 0) ny = 0;
                if (ny > th - 1) ny = th - 1;

                newHeightMap[x, y] = origHeightMap[nx, ny];
            } // for x
        } // for y



        // detail layer (grass, meshes)
        int dw = terrain.terrainData.detailWidth;
        int dh = terrain.terrainData.detailHeight;
        float detailMiddle = (terrain.terrainData.detailResolution) / 2.0f; // pivot at middle
        int numDetails = terrain.terrainData.detailPrototypes.Length;
        int[][,] newDetailLayer = new int[numDetails][,];

        // build new layer arrays
        for (int n = 0; n < numDetails; n++)
        {
            newDetailLayer[n] = new int[dw, dh];
        }

        for (int z = 0; z < numDetails; z++)
        {
            for (int y = 0; y < dh; y++)
            {
                for (int x = 0; x < dw; x++)
                {
                    cs = Mathf.Cos(angleRad);
                    sn = Mathf.Sin(angleRad);

                    nx = (int)((x - detailMiddle) * cs - (y - detailMiddle) * sn + detailMiddle);
                    ny = (int)((x - detailMiddle) * sn + (y - detailMiddle) * cs + detailMiddle);


                    if (nx < 0) nx = 0;
                    if (nx > dw - 1) nx = dw - 1;
                    if (ny < 0) ny = 0;
                    if (ny > dh - 1) ny = dh - 1;

                    newDetailLayer[z][x, y] = origDetailLayer[z][nx, ny];
                } // for x
            } // for y
        } // for z


        // alpha layer (texture splatmap) rotation
        dw = terrain.terrainData.alphamapWidth;
        dh = terrain.terrainData.alphamapHeight;
        int dz = terrain.terrainData.alphamapLayers;
        float alphaMiddle = (terrain.terrainData.alphamapResolution) / 2.0f; // pivot at middle
        float[,,] newAlphaMap = new float[dw, dh, dz];

        for (int z = 0; z < dz; z++)
        {
            for (int y = 0; y < dh; y++)
            {
                for (int x = 0; x < dw; x++)
                {
                    cs = Mathf.Cos(angleRad);
                    sn = Mathf.Sin(angleRad);

                    nx = (int)((x - alphaMiddle) * cs - (y - alphaMiddle) * sn + alphaMiddle);
                    ny = (int)((x - alphaMiddle) * sn + (y - alphaMiddle) * cs + alphaMiddle);

                    if (nx < 0) nx = 0;
                    if (nx > dw - 1) nx = dw - 1;
                    if (ny < 0) ny = 0;
                    if (ny > dh - 1) ny = dh - 1;

                    newAlphaMap[x, y, z] = origAlphaMap[nx, ny, z];
                } // for x
            } // for y
        } // for z



        // trees rotation, one by one..
        // TODO: use list instead, then can remove trees outside the terrain
        int treeCount = terrain.terrainData.treeInstances.Length;
        TreeInstance[] newTrees = new TreeInstance[treeCount];
        Vector3 newTreePos = Vector3.zero;
        float tx, tz;

        for (int n = 0; n < treeCount; n++)
        {

            cs = Mathf.Cos(angleRad);
            sn = Mathf.Sin(angleRad);

            tx = origTrees[n].position.x - 0.5f;
            tz = origTrees[n].position.z - 0.5f;

            newTrees[n] = origTrees[n];

            newTreePos.x = (cs * tx) - (sn * tz) + 0.5f;
            newTreePos.y = origTrees[n].position.y;
            newTreePos.z = (cs * tz) + (sn * tx) + 0.5f;

            newTrees[n].position = newTreePos;
        } // for treeCount




        // Apply new data to terrain

        //Undo.RecordObject(terrain.terrainData,"Rotate terrain ("+angle+")"); // Undoing this kills unity..

        TerrainData oldData = terrain.terrainData;

        terrain.terrainData = new TerrainData();

        terrain.terrainData.size = new Vector3(2.5f, oldData.size.y, 2.5f);

        terrain.terrainData.heightmapResolution = oldData.heightmapResolution;
        terrain.terrainData.SetDetailResolution(oldData.detailResolution, 8);
        terrain.terrainData.baseMapResolution = 1024;

        terrain.terrainData.alphamapResolution = oldData.alphamapResolution;

        terrain.terrainData.SetHeights(0, 0, newHeightMap);
        Debug.Log("Trying");

        

        terrain.terrainData.splatPrototypes = oldData.splatPrototypes;
        terrain.terrainData.SetAlphamaps(0, 0, newAlphaMap);


        Debug.Log("Trying");
        terrain.terrainData.treeInstances = newTrees;
        for (int n = 0; n < terrain.terrainData.detailPrototypes.Length; n++)
        {
            terrain.terrainData.SetDetailLayer(0, 0, n, newDetailLayer[n]);
        }

        // we are done..
        //isRotating = false;

    } //TerrainRotate


}
