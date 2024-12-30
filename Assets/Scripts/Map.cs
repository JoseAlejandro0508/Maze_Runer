using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Linq.Expressions;
using System.Collections;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Rendering.RenderGraphModule;
using TMPro;
using static UnityEngine.GraphicsBuffer;
public class Player

{

    public int id;
    public string name;
    public (int x, int y) seed;
    public (int x, int y) checkpoint;
    public int[,] distances;
    public string role;
    public GameObject texture;
    public GameObject instance;
    public GameObject checkpoint_texture;
    public List<GameObject>posibles_movements=new List<GameObject>();
    public int health;
    public int speed;
    public int vision=1;
    public int damage;
    public string status = "good";



    public Player((int, int) seed_, int id_ = -1, string name_ = null, GameObject instance_ = null, int[,] distances_ = null,string role_=null)
    {
        id = id_;
        name = name_;
        seed = seed_;
        instance = instance_;
        distances = distances_;
        role=role_;


    }
    public void  GenerateStistics(Dictionary<string, Dictionary<string, int>> Players_DB_)
    {
        health = Players_DB_[role]["health"];
        damage = Players_DB_[role]["damage"];
        speed = Players_DB_[role]["speed"];

    }
    public Vector3 GetActualPosition()
    {
        Vector3 actual_position = instance.transform.position;
        return actual_position;


    }





}


    
 public class Role_Details
  {
        public GameObject texture;
        public GameObject checkpoint_texture;
        public int health;
        public int speed;
        public int vision;
        public Role_Details(GameObject texture_,GameObject checkpoint_texture_,int health_,int speed_,int vision_=1)
        {
            texture = texture_;
            checkpoint_texture = checkpoint_texture_;
            health = health_;
            speed = speed_;
            vision = vision_;

        }


  }
   
public class Distances
{
    public static (int dist, (int x, int y)) GetMaxDistance(Player obj)
    {
        if (obj.distances == null) throw new Exception("Distances's matrix is null");


        (int dist, (int x, int y)) max_distance = (0, (obj.seed.x, 0));
        for (int i = 0; i < obj.distances.GetLength(0); i++)
        {
            for (int j = 0; j < obj.distances.GetLength(0); j++)
            {
                if (obj.distances[i, j] > max_distance.dist) max_distance = (obj.distances[i, j], (i, j));

            }

        }
        return max_distance;

    }
    public static List<(int dist, (int x, int y))> GetDistancesbyvalue(Player obj, int value)
    {
        if (obj.distances == null) throw new Exception("Distances's matrix is null");


        List<(int dist, (int x, int y))> finded = new List<(int dist, (int x, int y))>();
        for (int i = 0; i < obj.distances.GetLength(0); i++)
        {
            for (int j = 0; j < obj.distances.GetLength(0); j++)
            {
                if (obj.distances[i, j] == value) finded.Add((obj.distances[i, j], (i, j)));

            }

        }
        return finded;

    }


}

public class Map : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text Role;
    public TMP_Text Health_indicator;
    public TMP_Text Speed_indicator;
    public TMP_Text Status_indicator;
    public TMP_Text Damage_indicator;
    public GameObject CA_texture;
    public GameObject IM_texture;
    public GameObject Thor_texture;
    public GameObject Hulk_texture;
    public GameObject HE_texture;
    public GameObject Vision_texture;
    public GameObject CA_CHP;
    public GameObject IM_CHP;
    public GameObject Thor_CHP;
    public GameObject Hulk_CHP;
    public GameObject HE_CHP;
    public GameObject Vision_CHP;
    public GameObject Posible_Mov;
    GameObject uiImageObj=null;
    public GameObject wallPrefab;
    public Transform Display_player;

    private GameObject Player_view=null;

    public Camera mainCamera;

    static int n = 31;
    private int total_turns = 0;
    private int number_players;
    static List<((int x, int y), (int x, int y))> paths = new List<((int x, int y), (int x, int y))>();
    static Dictionary<int, (int, int)> Players_Seed = new Dictionary<int, (int, int)>    {
        {0,(1,1)},
        {1,(1,n-2)},
        {2,(n-2,1)},
        {3,(n-2,n-2)},
        {4,((n-1)/2,1)},
        {5,((n-1)/2,n-2)},

     };

    Dictionary<string, Dictionary<string, int>>Players_db = new Dictionary<string, Dictionary<string,int>>();

    Dictionary<string, Dictionary<string, GameObject>> Textures = new Dictionary<string, Dictionary<string, GameObject>>();
    Dictionary<int, Player> Players = new Dictionary<int, Player>();
   
    string[,] laberinto = new string[n, n]; 
    private static System.Random rand = new System.Random();

    private bool Block_move=false;
    private bool New_Turn = true;

    public void Start()
    {
        Init_DBS();

        number_players = PlayerPrefs.GetInt("Number_of_Players", 1);

        if (n % 2 == 0)
        {
            Debug.LogError("Las dimensiones deben ser impares.");
            return;
        }
        Init_Map();




    }

    public void Map_Fill()
    {
        for(int i = 0; i < laberinto.GetLength(0); i++)
        {
            for (int j = 0; j < laberinto.GetLength(1); j++)
            {
                laberinto[i,j] = "wall";
            }
        }
        
    }
    public void Init_DBS()
    {
        Textures["Capitan America"] = new Dictionary<string, GameObject>
        {
            { "Player", CA_texture },
             { "CHP", CA_CHP }
        };
        Textures["Iron Man"] = new Dictionary<string, GameObject>
        {
            { "Player", IM_texture },
             { "CHP", IM_CHP }
        };
        Textures["Thor"] = new Dictionary<string, GameObject>
        {
            { "Player", Thor_texture },
             { "CHP", Thor_CHP }
        };
        Textures["Vision"] = new Dictionary<string, GameObject>
        {
            { "Player", Vision_texture },
             { "CHP", Vision_CHP }
        };
        Textures["Hawk Eye"] = new Dictionary<string, GameObject>
        {
            { "Player", HE_texture },
             { "CHP", HE_CHP }
        };
        Textures["Hulk"] = new Dictionary<string, GameObject>
        {
            { "Player",Hulk_texture },
             { "CHP", Hulk_CHP }
        };
      

        Players_db["Capitan America"] = new Dictionary<string, int>
        {
            { "health",5 },
            { "speed", 3},
            { "damage", 0},
            { "referesh_time", 1},
        };
        Players_db["Iron Man"] = new Dictionary<string, int>
        {
            { "health",6 },
            { "speed", 3},
            { "damage", 0},
            { "referesh_time", 2},
        };
        Players_db["Thor"] = new Dictionary<string, int>
        {
            { "health",7 },
            { "speed", 2},
            { "damage", 4},
            { "referesh_time", 2},
        };
        Players_db["Vision"] = new Dictionary<string, int>
        {
            { "health",7 },
            { "speed", 2},
            { "damage", 0},
            { "referesh_time", 2},
        };
        Players_db["Hawk Eye"] = new Dictionary<string, int>
        {
            { "health",5 },
            { "speed", 3},
            { "damage", 3},
            { "referesh_time", 3},
        };
        Players_db["Hulk"] = new Dictionary<string, int>
        {
            { "health",9 },
            { "speed", 1},
            { "damage", 5},
            { "referesh_time", 1},
        };


    }
    public void Init_Map()
    {
        Map_Fill();
        Generarate();
        Init_Players();
        
        Imprimir();
        CentrarCamara();

        for (int i = 0; i < number_players; i++)
        {
            var seed_player= Players_Seed[i];
            int x = seed_player.Item1;
            int y = seed_player.Item2;
            Players[i].instance=Instantiate(Players[i].texture, new Vector3(x + 0.5f, y + 0.5f,-1), Quaternion.identity);
     
    
        }
        Add_Checkpoints();



    }
    public void SwitchPlayerPreview(int id)
    {
        if(uiImageObj!=null) Destroy(uiImageObj);

        uiImageObj = new GameObject("UIImage");
        uiImageObj.transform.SetParent(Display_player.transform);
        GameObject orig_txt = Players[id].texture;
        // Añadir componente Image
        Image uiImage = uiImageObj.AddComponent<Image>();
        // Asignar el sprite del GameObject original al Image
        SpriteRenderer spriteRenderer = orig_txt.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            uiImage.sprite = spriteRenderer.sprite;
        }

        // Ajustar 
        RectTransform rectTransform = uiImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 100); // Ajustar dimensiones
        rectTransform.localPosition = Vector3.zero; // Fijar posicion de manera relativa al objeto padre

    }



        public void Generarate()
    {       

        // Marcando camino en celda inicial impar
        (int x,int y)cord= ( 1, 1);
        laberinto[cord.x, cord.y] = "path";
        // Porcesa la celda inicial para determinar los posibles caminos contiguos
        Add_path(cord);

        // Procesa las coordenadas en la lista hasta que se acaben
        while (paths.Count > 0)
        {
            // Elige una cordenada al azar y la elimina de la lista
            int index = rand.Next(paths.Count);
            var (wall_cord, path_cord) = paths[index];
            paths.RemoveAt(index);

            // Si la celda conectada no ha sido visitada
            if (laberinto[path_cord.x, path_cord.y] == "wall")
            {
                
                laberinto[wall_cord.x, wall_cord.y] = "path";// Elimina la pared entre las celdas
                laberinto[path_cord.x, path_cord.y] = "path";// Marca la nueva celda como camino
                Add_path(path_cord); // Añade los posibles caminos contiguos a la celda actual
            }
        }
    }

    public void Add_path((int x, int y) cord)
    {
        int x = cord.x;
        int y = cord.y;
        // Añade la celda que sera el posible proximo camino y una celda adyacente
        if (x > 1) paths.Add(((x - 1, y), (x - 2, y))); // Arriba
        if (x < n - 2) paths.Add(((x + 1, y), (x + 2, y))); // Abajo
        if (y > 1) paths.Add(((x, y - 1),( x, y - 2))); // Izquierda
        if (y < n - 2) paths.Add(((x, y + 1), (x, y + 2))); // Derecha
    }

    public void Imprimir()
    {
        for (int i = 0; i < n; i++)
        {

            for (int j = 0; j < n; j++)
            {
                if (laberinto[i, j] == "wall")
                {
                    Instantiate(wallPrefab, new Vector3(i + 0.5f, j + 0.5f, 2), Quaternion.identity);
                }


            }
        }
    }
    public void CentrarCamara()
    {
        // Calcula la posición central del laberinto
        float x_camera = n / 2;
        float y_camera = n / 2;
        float z_camera = -10f;
        Vector3 centralPosition = new Vector3(x_camera, y_camera, z_camera); // -10 es la distancia Z para la cámara

        // Mueve la cámara a la posición central
        mainCamera.transform.position = centralPosition;

        // Ajusta la cámara para que el laberinto se vea completo
        // Si estás usando una cámara ortográfica, ajusta el tamaño
        if (mainCamera.orthographic)
        {
            float orthographic_size = n / 2 + 2;
            mainCamera.orthographicSize = orthographic_size; // Ajusta el tamaño según el tamaño del laberinto
        }
    }
    public void Update()
    {
        Turn_Simulator();

        

    }
    public void Turn_Simulator()
    {

        int total_turns_ = total_turns;
        Player player_selected = Players[total_turns % number_players];
        DisplayPosibleMovements(player_selected);
        DisplayPlayerPanel(player_selected);
        ChangePlayerVision(player_selected);
        if (!Block_move) Check_Move(player_selected);


        if (total_turns_ != 0) SwitchPlayerPreview(player_selected.id);
        if (total_turns_==0) SwitchPlayerPreview(player_selected.id);
        CheckNextTrun();

    }

    public List<(int x,int y)>GetPosibleMovements(Player player_selected_)
    {
        List<(int x, int y)> possible_celds = new List<(int x, int y)>();
        int pases = player_selected_.speed;
        Vector3 player_actual_pos = player_selected_.GetActualPosition();
      
        int[,] player_distances = BFS(Mathf.RoundToInt(player_actual_pos.x - 0.5f), Mathf.RoundToInt(player_actual_pos.y - 0.5f));
        for (int i = 0; i < player_distances.GetLength(0); i++)
        {
            for (int j = 0; j < player_distances.GetLength(1); j++)
            {
                if (player_distances[i, j] <= pases && player_distances[i, j] > 0 && laberinto[i,j]!="wall")
                {
                    possible_celds.Add((i, j));
                }

            }
        }
        return possible_celds;
    }
    public void DisplayPosibleMovements(Player player_selected_)
    {

        if (!New_Turn) return;
        if (Block_move)
        {
            CleanPossibleMovements(player_selected_);
            return;
        }
        List<(int, int)> possible_celds = GetPosibleMovements(player_selected_);
        for (int i = 0; i < possible_celds.Count; i++)
        {
            (int x, int y) cord = possible_celds[i];
            GameObject instance_mov = Instantiate(Posible_Mov, new Vector3(cord.x + 0.5f, cord.y + 0.5f, -2), Quaternion.identity);
            Players[player_selected_.id].posibles_movements.Add(instance_mov);

        }
        New_Turn = false;
    }

    public void CleanPossibleMovements(Player player_selected_)
    {

        List<GameObject> posible_movements = player_selected_.posibles_movements;
        for (int i = 0; i < posible_movements.Count; i++)
        {
            Destroy(posible_movements[i]);
        }
        player_selected_.posibles_movements.Clear();
    }

    public void ChangePlayerVision(Player player_selected_)
    {
     
        Vector3 player_pos = player_selected_.GetActualPosition();
        player_pos.z = -10f;
        // Mueve la cámara a la posición central
        mainCamera.transform.position = player_pos;
        // Ajusta la cámara para que el laberinto se vea completo
        // Si estás usando una cámara ortográfica, ajusta el tamaño
        if (mainCamera.orthographic)
        {
            float orthographic_size = n / 7;
            mainCamera.orthographicSize = orthographic_size; // Ajusta el tamaño según el tamaño del laberinto
        }



    }

    public void DisplayPlayerPanel(Player player_selected_)
    {
        Title.text = $"{player_selected_.name}(P-{player_selected_.id})";

        Role.text = player_selected_.role;
        Health_indicator.text = $"{player_selected_.health}/{Players_db[player_selected_.role]["health"]}";
        Speed_indicator.text = $"{player_selected_.speed}/{Players_db[player_selected_.role]["speed"]}";
        Damage_indicator.text = $"{player_selected_.damage}/{Players_db[player_selected_.role]["damage"]}";
        Status_indicator.text = player_selected_.status;


    }
    public void CheckNextTrun()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Block_move = false;
            New_Turn = true;
            total_turns++;

            return;

        }

    }
    public void Init_Players()
    {

        for (int i = 0; i < number_players; i++)
        {
            string player_role= PlayerPrefs.GetString($"PlayerRole_{i}");
            string player_name=PlayerPrefs.GetString($"PlayerName_{i}");

            (int, int) seed_player = Players_Seed[i];
            Players[i] = new Player(id_: i, seed_: seed_player,name_:player_name, distances_: BFS(seed_player.Item1, seed_player.Item2),role_: player_role);
            Players[i].texture = Textures[player_role]["Player"];
            Players[i].checkpoint_texture = Textures[player_role]["CHP"];
            Players[i].GenerateStistics(Players_db);

        }
    }
    public void Check_Move(Player player)
    {
        Vector3 player_pos = player.instance.transform.position;
        Vector3 movement;
        if (Input.GetMouseButtonDown(0))
        {
            // Obtener la posición del mouse en la pantalla
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            movement = new Vector3((float)Math.Floor(mousePosition.x) + 0.5f, (float)Math.Floor(mousePosition.y) + 0.5f, -2f);
            Debug.Log("Clic en: " + Math.Floor(mousePosition.x) + " " + Math.Floor(mousePosition.y));
            if (Move(movement, player))
            {
                //total_turns++;
                Block_move = true;
                CleanPossibleMovements(player);

                return;
            }
            
        }
        /*Movimiento del jugador
        if (Input.GetKeyDown(KeyCode.W))
        {
            movement = new Vector3(player_pos.x, player_pos.y + 1, player_pos.z);
            if (Move(movement, player))
            {
                //total_turns++;
                Block_move = true;

                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            movement = new Vector3(player_pos.x, player_pos.y - 1, player_pos.z);
            if (Move(movement, player))
            {
                //total_turns++;
                Block_move = true;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {

            movement = new Vector3(player_pos.x - 1, player_pos.y, player_pos.z);
            if (Move(movement, player))
            {
                //total_turns++;
                Block_move = true;
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            movement = new Vector3(player_pos.x + 1, player_pos.y, player_pos.z);
            if (Move(movement, player))
            {
                //total_turns++;
                Block_move = true;
                return;
            }
        }
        */



    }

    public bool Move(Vector3 target,Player player)
    {
        /*Calcula la nueva posición
        if (laberinto[Mathf.RoundToInt(target.x - 0.5f), Mathf.RoundToInt(target.y - 0.5f)] == "path")
        {
            player.instance.transform.position = target;
            return true;
        }
        
        */
        bool finded = false;
        List<(int x, int y)> possible_movements = GetPosibleMovements(player);
        for (int i = 0; i <possible_movements.Count; i++)
        {
            if (possible_movements[i] == (Math.Floor(target.x), Math.Floor(target.y)))finded=true;
        }
        if (finded)
        {
            player.instance.transform.position = target;
            return true;
        }
        return false;


    }
    public int[,] BFS(int x, int y)
    {
        bool[,] mask = new bool[n, n];
        int[,] Distance_matrix = new int[n, n];
        Queue<(int, int)> queue = new Queue<(int, int)>();
        int[] mov_horizontal = { 1, -1, 0, 0 };
        int[] mov_vertical = { 0, 0, 1, -1 };
        queue.Enqueue((x, y));
        while (queue.Count > 0)
        {
            var cord = queue.Dequeue();
            if (!mask[cord.Item1, cord.Item2])
            {
                mask[cord.Item1, cord.Item2] = true;
                for (int i = 0; i < mov_horizontal.Length; i++)
                {
                    if (cord.Item1 + mov_horizontal[i] >= 0 && cord.Item1 + mov_horizontal[i] < n && cord.Item2 + mov_vertical[i] >= 0 && cord.Item2 + mov_vertical[i] < n && !mask[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]] && laberinto[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]] == "path")
                    {
                        queue.Enqueue((cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]));
                        Distance_matrix[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]]= Distance_matrix[cord.Item1, cord.Item2] + 1;

                    }
                }
            }
        }
        return Distance_matrix;

    }
    
    public void  Add_Checkpoints()
    {
        (int dist,(int x,int y)) min_of_max_dist = Distances.GetMaxDistance(Players[0]);
        int min_of_max_dist_pla = 0;
        for(int p = 0; p < number_players; p++){
            (int dist, (int x, int y)) max_dist_player = Distances.GetMaxDistance(Players[p]);
            if (max_dist_player.dist < min_of_max_dist.dist)
            {
                min_of_max_dist = max_dist_player;
                min_of_max_dist_pla = p;
            }
        }
        Players[min_of_max_dist_pla].checkpoint = min_of_max_dist.Item2;
        //laberinto[min_of_max_dist.Item2.x, min_of_max_dist.Item2.y] = -1;
        for (int p = 0; p < number_players; p++)
        {
            if (p == min_of_max_dist_pla) continue;
            List<(int dist, (int x, int y))> possibles_checkpoint = Distances.GetDistancesbyvalue(Players[p], min_of_max_dist.dist);
            (int dist, (int x, int y)) pl_checkpoint = possibles_checkpoint[0];

            for (int i = 0; i < possibles_checkpoint.Count; i++)
            {
                bool stop = true;
                for (int pl = 0; pl < p; pl++)
                {
                    if (possibles_checkpoint[i].Item2.x == Players[pl].seed.x && possibles_checkpoint[i].Item2.y == Players[pl].seed.y) stop = false;
                }
                if (stop)
                {
                    pl_checkpoint = possibles_checkpoint[i];
                    break;
                }


            }
            Players[p].checkpoint = pl_checkpoint.Item2;
            //laberinto[pl_checkpoint.Item2.x, pl_checkpoint.Item2.y] = -1;
        }
        for(int p = 0; p < number_players; p++)
        {

            Instantiate(Players[p].checkpoint_texture, new Vector3(Players[p].checkpoint.x + 0.5f, Players[p].checkpoint.y + 0.5f, 2), Quaternion.identity);
        }
            
    }







}

