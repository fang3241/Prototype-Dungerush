﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Graphs;

public class Generator2D : MonoBehaviour {
    enum CellType {
        None,
        Room,
        Hallway,
        RDoor,
        HDoor
    }

    class Room {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int size) {
            bounds = new RectInt(location, size);
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }
    }

    [SerializeField]
    Vector2Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector2Int roomMaxSize;
    [SerializeField]
    Vector2Int roomMinSize;
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Material greenMaterial;
    [SerializeField]
    Material yellowMaterial;
    [SerializeField]
    Material purpleMaterial;
    [SerializeField]
    Transform wallParent;
    [SerializeField]
    float scaling;
    [SerializeField]
    Transform player;

    Grid2D<CellType> grid;
    List<Room> rooms;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    void Start() {
        Generate();
    }

    void Generate() {
        grid = new Grid2D<CellType>(size, Vector2Int.zero);
        rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
        drawFloor();
        drawWall();
    }

    void PlaceRooms() {
        for (int i = 0; i < roomCount; i++) {
            Vector2Int location = new Vector2Int(
                Random.Range(0, size.x),
                Random.Range(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                Random.Range(roomMinSize.x, roomMaxSize.x + 1),
                Random.Range(roomMinSize.y, roomMaxSize.y + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y) {
                add = false;
            }
            if (add) {
                rooms.Add(newRoom);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
            }
        }
    }

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            if (Random.Range(1,8) == 8) {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways() {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(size);

        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();
                
                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[b.Position] == CellType.Room) {
                    pathCost.cost += 10;
                } else if (grid[b.Position] == CellType.None) {
                    pathCost.cost += 5;
                } else if (grid[b.Position] == CellType.Hallway) {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[current] == CellType.None) {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0) {
                        var prev = path[i - 1];
                        if(grid[current] == CellType.Hallway && grid[prev] == CellType.Room){
                            grid[prev] = CellType.RDoor;
                            grid[current] = CellType.HDoor;
                        }
                        var delta = current - prev;
                    }

                    if(i < path.Count - 1){
                        var next = path[i + 1];
                        if(grid[current] == CellType.Hallway && grid[next] == CellType.Room){
                            grid[next] = CellType.RDoor;
                            grid[current] = CellType.HDoor;
                        }
                    }
                }

            }
        }
    }

    void drawFloor(){
        for(int x = 0; x < size.x; x++){
            for(int y = 0; y < size.y; y++){
                var pos = new Vector2Int((int)x, (int)y);
                //floor + ceiling
                if (grid[pos] == CellType.Room) {
                    PlaceRoomz(pos);
                }
                if (grid[pos] == CellType.Hallway) {
                    PlaceHallway(pos);
                }
                if (grid[pos] == CellType.RDoor) {
                    PlaceRDoor(pos);
                }
                if (grid[pos] == CellType.HDoor) {
                    PlaceHDoor(pos);
                    //cek xy+-1 apa ada yang ROOM, kalo ada paksa ubah jadi RDoor, di lantai ngga keliatan keganti tapi
                    if((grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.RDoor) || (grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.Room) || (grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.RDoor) || (grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.RDoor)){
                        //d
                    }
                    else{
                        if(grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.Room){
                            grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] = CellType.RDoor;
                        }
                        if(grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.Room){
                            grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] = CellType.RDoor;
                        }
                        if(grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.Room){
                            grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] = CellType.RDoor;
                        }
                        if(grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.Room){
                            grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] = CellType.RDoor;
                        }
                    }
                }
            }
        }
    }

    void drawWall(){
        for(int x = 0; x < size.x; x++){
            for(int y = 0; y < size.y; y++){
                var pos = new Vector2Int((int)x, (int)y);
                //floor + ceiling
                if (grid[pos] == CellType.Room) {
                    cekArch('R', pos);
                }
                if (grid[pos] == CellType.Hallway) {
                    cekArch('H', pos);
                }
                if (grid[pos] == CellType.RDoor) {
                    cekArch('D', pos);
                }
                if (grid[pos] == CellType.HDoor) {
                    cekArch('F', pos);
                }
            }
        }
    }

    void PlaceCube(Vector2Int location, Vector2Int size, Material material) {
        GameObject go = Instantiate(cubePrefab, new Vector3(location.x * scaling, 0, location.y * scaling), Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3(size.x * scaling, 0.1f, size.y * scaling);
        go.GetComponent<MeshRenderer>().material = material;
    }

    void cekArch(char gridType, Vector2Int pos) {
//        var posi = new Vector2Int((int)pos.x, (int)pos.y);
    //
    int randSpawn;
    //Vector2Int spawnPoint;

    randSpawn = Random.Range(0, (size.x + size.y)/2);
    //Debug.Log("RandSpawn: " + randSpawn);
        if(gridType == 'R'){
//            posi.x = posi.x + 1;
            //Debug.Log(": " + randSpawn);
            randSpawn--;
            if(randSpawn == 0){
                player.position = new Vector3((pos.x + 0.5f) * scaling, 1.2f, (pos.y + 0.5f) * scaling);
                //Debug.Log(player.position);
            }
            if(pos.x > 0){
                if(grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.Room || grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.RDoor){
                   //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x - 1) + " " + pos.y);
                }
                else{
                    placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.x == 0){
                placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
            }
            if(pos.x <= size.x){
                if(grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.Room || grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x + 1) + " " + pos.y);
                }
                else{
                    placeWallR(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.y > 0){
                if(grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.Room || grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                }
                else{
                    placeWallU(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
                }
            }
            if(pos.y == 0){
                placeWallU(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
            }
            if(pos.y <= size.y){
                if(grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.Room || grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                }
                else{
                    placeWallD(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
                }
            }
        }
        if(gridType == 'H'){
//            posi.x = posi.x + 1;
            if(pos.x > 0){
                if(grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.Hallway || grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.HDoor){
                   //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x - 1) + " " + pos.y);
                }
                else{
                    placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.x == 0){
                placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
            }
            if(pos.x <= size.x){
                if(grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.Hallway || grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x + 1) + " " + pos.y);
                }
                else{
                    placeWallR(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.y > 0){
                if(grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.Hallway || grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                }
                else{
                    placeWallU(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
                }
            }
            if(pos.y == 0){
                placeWallU(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
            }
            if(pos.y <= size.y){
                if(grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.Hallway || grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                }
                else{
                    placeWallD(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
                }
            }
        }
        if(gridType == 'D'){
//            posi.x = posi.x + 1;
            if(pos.x > 0){
                if(grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.HDoor){
                   //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x - 1) + " " + pos.y);
                   //door
                }
                if(grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.None){
                    placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.x == 0){
                placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
            }
            if(pos.x <= size.x){
                if(grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x + 1) + " " + pos.y);
                    //door
                }
                if(grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.None){
                    placeWallR(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.y > 0){
                if(grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                    //door
                }
                if(grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.None){
                    placeWallU(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
                }
            }
            if(pos.y == 0){
                placeWallU(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
            }
            if(pos.y <= size.y){
                if(grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                    //door
                }
                if(grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.None){
                    placeWallD(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
                }
            }
        }
        if(gridType == 'F'){    //!cek +-xy, kalo ada merah force ganti jadi ijo!!!![] -> kalo ketemu merah apus semwa wall(wall jadiin 1 child yang sama dulu), terus ulangi generate wall
//            posi.x = posi.x + 1;
            if(pos.x > 0){
                if(grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.RDoor){
                   //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x - 1) + " " + pos.y);
                   //door
                }
                if(grid[new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.None){
                    placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.x == 0){
                placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
            }
            if(pos.x <= size.x){
                if(grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x + 1) + " " + pos.y);
                    //door
                }
                if(grid[new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.None){
                    placeWallR(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.y > 0){
                if(grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                    //door
                }
                if(grid[new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.None){
                    placeWallU(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
                }
            }
            if(pos.y == 0){
                placeWallU(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
            }
            if(pos.y <= size.y){
                if(grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                    //door
                }
                if(grid[new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.None){
                    placeWallD(pos, new Vector2(1.0f, 0.1f), purpleMaterial);
                }
            }
        }
    }
    
    void PlaceRoom(Vector2Int location, Vector2Int size) {
        PlaceCube(location, size, redMaterial);
    }
    void PlaceHallway(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), blueMaterial);
    }
    void PlaceRDoor(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), greenMaterial);
    }
    void PlaceHDoor(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), yellowMaterial);
    }
    void PlaceRoomz(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), redMaterial);
    }

    void placeWallR(Vector2Int location, Vector2 size, Material material){
        GameObject go = Instantiate(cubePrefab, new Vector3(((float)location.x + 0.9f) * scaling, 0, location.y * scaling), Quaternion.identity, wallParent);
        go.GetComponent<Transform>().localScale = new Vector3(size.x * scaling, scaling, size.y * scaling);
        go.GetComponent<MeshRenderer>().material = material;
    }
    void placeWallL(Vector2Int location, Vector2 size, Material material){
        GameObject go = Instantiate(cubePrefab, new Vector3(location.x * scaling, 0, location.y * scaling), Quaternion.identity, wallParent);
        go.GetComponent<Transform>().localScale = new Vector3(size.x * scaling, scaling, size.y * scaling);
        go.GetComponent<MeshRenderer>().material = material;
    }
    void placeWallD(Vector2Int location, Vector2 size, Material material){
        GameObject go = Instantiate(cubePrefab, new Vector3(location.x * scaling, 0, ((float)location.y + 0.9f) * scaling), Quaternion.identity, wallParent);
        go.GetComponent<Transform>().localScale = new Vector3(size.x * scaling, scaling, size.y * scaling);
        go.GetComponent<MeshRenderer>().material = material;
    }
    void placeWallU(Vector2Int location, Vector2 size, Material material){
        GameObject go = Instantiate(cubePrefab, new Vector3(location.x * scaling, 0, location.y * scaling), Quaternion.identity, wallParent);
        go.GetComponent<Transform>().localScale = new Vector3(size.x * scaling, scaling, size.y * scaling);
        go.GetComponent<MeshRenderer>().material = material;
    }
}
