using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Chessborder : MonoBehaviour

{

    [Header("Art stuff ")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize=1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 borderCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.3f;
    [SerializeField] private float deathSpacing = 0.3f;
    [SerializeField] private float dragoffset = 1.5f;
    [SerializeField] private GameObject VictoryScreen;

    [Header("Prefabs && Materials  ")]

    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;




    private ChessPiece[,] chessPiece;
    private const int TILES_COUNT_X = 8;
    private const int TILES_COUNT_Y = 8;
    private Camera currentCamera;

    private Vector2Int currentHover;

    private ChessPiece currentlyDragging; 
    private Vector3 bounds ;
    private bool isWhiteTurn;

    private int team;



    private List<ChessPiece> deadWhite = new List<ChessPiece>();
    private List<ChessPiece> deadBlack = new List<ChessPiece>();
    private List<Vector2Int> availabelMove = new List<Vector2Int>();


    private GameObject[,] tiles;



    private void Start()
    {
        transform.rotation= Quaternion.Euler((team==0 )? Vector3.zero: new Vector3(0,180,0));
    }
    private void Awake()
    {
        isWhiteTurn = true;
        GeneratAllTiles(tileSize, TILES_COUNT_X, TILES_COUNT_Y);

        // SpawningSinglePiece(ChessPiecesType.King, 0);

        SpawningAllPiece();
        PosationAllPieces();
    }


    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availabelMove, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }




            // if the press down on the mouse 
            if (Input.GetMouseButtonDown(0))
            {
                if (chessPiece[hitPosition.x, hitPosition.y] != null)

                {
                    if ((chessPiece[hitPosition.x, hitPosition.y].team== 0&& isWhiteTurn)|| (chessPiece[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn))
                    {
                        currentlyDragging = chessPiece[hitPosition.x, hitPosition.y];



                        // Get a list of where i con go ,highlight tiles as well
                        availabelMove = currentlyDragging.GetAvailableMove(ref chessPiece, TILES_COUNT_X,TILES_COUNT_Y);

                        HighlightTiles();




                    }
                }
            }


            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosation = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                 
                if (!validMove)
               

                    currentlyDragging.setPosition ( GetTileCenter(previousPosation.x, previousPosation.y));
                    currentlyDragging = null;
                RemoveHighlightTiles();



            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availabelMove ,currentHover)) ?LayerMask.NameToLayer("Highlight"): LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.setPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));

                currentlyDragging = null;
                RemoveHighlightTiles();

            }

        }




        // if wa're dargging  a piece 

        if(currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;

            if (horizontalPlane.Raycast(ray, out distance))
                currentlyDragging.setPosition(ray.GetPoint(distance) +Vector3.up * dragoffset);
        }
        }

  

    // Generate Border 

    private void GeneratAllTiles(float tilesSize ,int tileCountX,int tileCountY)
    {
        yOffset += transform.position.y;

        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + borderCenter;

        tiles = new GameObject[tileCountX, tileCountY];
         for(int x=0; x < tileCountX; x++)

        
            for (int y = 0; y < tileCountY; y++)

            
                tiles[x, y] = GeneratSingelTiles(tilesSize, x, y);

            
       
    }


    private GameObject GeneratSingelTiles(float tilesSize, int x, int y)
    {

        GameObject tilesObject = new GameObject(string.Format("X:{0},Y:{1}", x, y));
        tilesObject.transform.parent = transform;


        Mesh mesh = new Mesh();
        tilesObject.AddComponent<MeshFilter>().mesh = mesh;
        tilesObject.AddComponent<MeshRenderer>().material = tileMaterial;


        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(x*tilesSize ,yOffset ,y*tilesSize) - bounds;
        vertices[1] = new Vector3(x * tilesSize, yOffset, (y+1) * tilesSize) - bounds;
        vertices[2] = new Vector3((x+1) * tilesSize, yOffset, y  * tilesSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tilesSize, yOffset, (y+1) * tilesSize) - bounds;


        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        tilesObject.layer = LayerMask.NameToLayer("Tile");

        tilesObject.AddComponent<BoxCollider>();



        return tilesObject;
    }

    // Spawning  of the Pieces 
    private void SpawningAllPiece()
    {
        chessPiece = new ChessPiece[TILES_COUNT_X, TILES_COUNT_Y];
        int whiteTeam = 0, BlackTeam = 1;

        chessPiece[0, 0] = SpawningSinglePiece(ChessPiecesType.Rook, whiteTeam);
        chessPiece[1, 0] = SpawningSinglePiece(ChessPiecesType.Knight, whiteTeam);
        chessPiece[2, 0] = SpawningSinglePiece(ChessPiecesType.Bishop, whiteTeam);
        chessPiece[3, 0] = SpawningSinglePiece(ChessPiecesType.King, whiteTeam);
        chessPiece[4, 0] = SpawningSinglePiece(ChessPiecesType.Queen, whiteTeam);
        chessPiece[5, 0] = SpawningSinglePiece(ChessPiecesType.Bishop, whiteTeam);
        chessPiece[6, 0] = SpawningSinglePiece(ChessPiecesType.Knight, whiteTeam);
        chessPiece[7, 0] = SpawningSinglePiece(ChessPiecesType.Rook, whiteTeam);

        for (int i = 0; i < TILES_COUNT_X; i++)

            chessPiece[i, 1] = SpawningSinglePiece(ChessPiecesType.Pawn, whiteTeam);


        //Black Team 
        chessPiece[0, 7] = SpawningSinglePiece(ChessPiecesType.Rook, BlackTeam);
        chessPiece[1, 7] = SpawningSinglePiece(ChessPiecesType.Knight, BlackTeam);
        chessPiece[2, 7] = SpawningSinglePiece(ChessPiecesType.Bishop, BlackTeam);
        chessPiece[3, 7] = SpawningSinglePiece(ChessPiecesType.Queen, BlackTeam);
        chessPiece[4, 7] = SpawningSinglePiece(ChessPiecesType.King, BlackTeam);

        chessPiece[5, 7] = SpawningSinglePiece(ChessPiecesType.Bishop, BlackTeam);
        chessPiece[6, 7] = SpawningSinglePiece(ChessPiecesType.Knight, BlackTeam);
        chessPiece[7, 7] = SpawningSinglePiece(ChessPiecesType.Rook, BlackTeam);

        for (int i = 0; i < TILES_COUNT_X; i++)

            chessPiece[i, 6] = SpawningSinglePiece(ChessPiecesType.Pawn, BlackTeam);



    }

    private ChessPiece SpawningSinglePiece(ChessPiecesType type, int team )
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();


        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];
        return cp;


    }


    private void PosationAllPieces()
    {
        for (int x = 0; x < TILES_COUNT_X; x++)
            for (int y = 0; y < TILES_COUNT_Y; y++)
                if (chessPiece[x, y] != null)
                    PosationSinglePieces(x, y, true);

    }

    private void PosationSinglePieces(int x, int y, bool force = false)
    {
        //if (chessPiece[x, y] == null)
        //{
        //    Debug.LogError($"Chess piece at position ({x}, {y}) is null.");
        //    return;
        //}

        chessPiece[x, y].currentX = x;
        chessPiece[x, y].currentY = y;
        chessPiece[x, y].setPosition( GetTileCenter(x, y ),force);
    }



    private Vector3 GetTileCenter(int x, int y)
    {

        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    private void HighlightTiles()
    {
        for (int i = 0; i < availabelMove.Count; i++)
        {
            tiles[availabelMove[i].x, availabelMove[i].y].layer = LayerMask.NameToLayer("Highlight");

        }
    }

    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availabelMove.Count; i++)
        {
            tiles[availabelMove[i].x, availabelMove[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availabelMove.Clear();
    }

    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {

        for (int i = 0; i < moves.Count; i++)

            if (moves[i].x == pos.x && moves[i].y == pos.y)

                return true;
                return false;
    }
    private bool MoveTo(ChessPiece cp, int x, int y)
    {


        if (!ContainsValidMove(ref availabelMove, new Vector2(x, y)))

            return false;

        Vector2Int previousPosation = new Vector2Int(cp.currentX, cp.currentY);



        //if there another  piece on the target posation   

        if (chessPiece[x, y] != null)
        {
            ChessPiece ocp = chessPiece[x, y];
            if (cp.team == ocp.team) 
            return false;


            if (cp.team == 0)
            {

                if (ocp.type == ChessPiecesType.King)

                    checkMate(1);
                deadWhite.Add(ocp);
                ocp.setScale(Vector3.one * deathSize);

                ocp.setPosition(
                    new Vector3(8 * tileSize, yOffset, -1 * tileSize)
                    - bounds
                   + new Vector3(tileSize / 2, 0, tileSize / 2)
                    + (Vector3.forward * deathSpacing) * deadWhite.Count);
            }
            else
            {

                if (ocp.type == ChessPiecesType.King)

                    checkMate(0);
                deadBlack.Add(ocp);
                ocp.setScale(Vector3.one * deathSize);
                ocp.setPosition(
                   new Vector3(-1 * tileSize, yOffset, 8 * tileSize)
                   - bounds
                  + new Vector3(tileSize / 2, 0, tileSize / 2)
                   + (Vector3.back * deathSpacing) * deadBlack.Count);
            }


        }
 
        chessPiece[x, y] = cp;
        chessPiece[previousPosation.x, previousPosation.y] = null;

        PosationSinglePieces(x, y);

        isWhiteTurn = !isWhiteTurn;

        return true;


    }


     private void checkMate(int team)
    {
        DisplayVictory(team);

    }


    private void DisplayVictory(int WinningTeam)
    {
        VictoryScreen.SetActive(true);
        VictoryScreen.transform.GetChild(WinningTeam).gameObject.SetActive(true);
    }

    public void OnResetButton()
    {
        VictoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        VictoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        VictoryScreen.SetActive(true);



        currentlyDragging = null;
        availabelMove = new List<Vector2Int>();

        for (int x=0; x < TILES_COUNT_X; x++)
        {
            for(int y=0; y<TILES_COUNT_Y;y++ )

            {
                if (chessPiece[x, y] != null)
                    Destroy(chessPiece[x, y].gameObject);
                chessPiece[x, y] = null;

            }
        }


        for (int i = 0; i < deadWhite.Count; i++)
            Destroy(deadWhite[i].gameObject);

        for (int i = 0; i < deadBlack.Count; i++)
            Destroy(deadBlack[i].gameObject);

        deadBlack.Clear();
        deadWhite.Clear();

        SpawningAllPiece();
        PosationAllPieces();
        isWhiteTurn= true;

    }

    public void OnEixtButton()
    {
        Application.Quit();

    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for( int x=0; x<TILES_COUNT_X;x++)
            for(int y=0;y<TILES_COUNT_Y;y++)
            
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }


  
}
