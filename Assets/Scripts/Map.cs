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
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.Rendering.Universal;


public class Player

{
    //Es una clase que representara a cada jugador,aqui se encontrara la informacion en tiempo real de cada jugador ,asi como las funciones encargadas de ejecutar las habilidades

    public int id;
    public string name;
    public (int x, int y) seed;
    public (int x, int y) checkpoint = (-1, -1);
    public int[,] distances;
    public string role;

    public GameObject texture;

    public GameObject texturePreviewSkill;
    public GameObject instance;
    public GameObject OnTurnIndicatorInstance = null;
    public GameObject TargetIndicatorInstance = null;
    public GameObject checkpoint_texture;

    public List<GameObject> posibles_movements = new List<GameObject>();
    public (bool IsOff, int Radius)[,] NotFog;
    public List<GameObject> ghost_movements = new List<GameObject>();
    public List<GameObject> AreaTarget = new List<GameObject>();
    public int health;
    public int speed;
    public int vision = 1;
    public int damage;
    public int total_turns = 0;
    public string status = "Good";
    public Dictionary<string, Dictionary<string, int>> Players_DB;
    public Dictionary<string, int> Status = new Dictionary<string, int>();
    public Dictionary<string, (bool is_active, bool confirm_use, int turn_off)> SkillsState = new Dictionary<string, (bool is_active, bool confirm_use, int turn_off)>();
    public List<int> SkillTurns = new List<int> { 1 };

    public Player((int, int) seed_, int id_ = -1, string name_ = null, GameObject instance_ = null, int[,] distances_ = null, string role_ = null)
    {
        id = id_;
        name = name_;
        seed = seed_;
        instance = instance_;
        distances = distances_;
        role = role_;


    }
    public void GenerateStistics(Dictionary<string, Dictionary<string, int>> Players_DB_)
    {
        //Initializes the player's stats from the contributed database
        health = Players_DB_[role]["health"];
        damage = Players_DB_[role]["damage"];
        speed = Players_DB_[role]["speed"];
        if (Players_DB == null) Players_DB = Players_DB_;


    }
    public Vector3 GetActualPosition()
    {
        //Returns the player's on-screen coordinates
        Vector3 actual_position = instance.transform.position;
        return actual_position;
    }
    public (int x, int y) GetPlayerLabCord()
    {
        //Return the player's coordinates in the maze matrix
        return ((int)GetActualPosition().x, (int)GetActualPosition().y);
    }
    public (bool is_avaliable, int time_remaing) SkillRefresh()
    {
        //Returns the current state of the skill and the time remaining to make it available again
        if (!SkillsState[role].confirm_use && SkillsState[role].is_active && SkillTurns[SkillTurns.Count - 1] == SkillsState[role].turn_off)
        {
            return (true, 0);
        }
        if (total_turns - SkillTurns[SkillTurns.Count - 1] >= Players_DB[role]["refresh_time"])
        {
            return (true, 0);
        }
        return (false, Players_DB[role]["refresh_time"] - (total_turns - SkillTurns[SkillTurns.Count - 1]));
    }
    public bool IsActiveSkill()
    {
        //It returns if the skill is active, also in case its activation time has already passed ...
        if (!SkillsState.ContainsKey(role))
        {
            SkillsState[role] = (false, false, -1);
            return false;
        }
        if (SkillsState[role].is_active)
        {
            if (SkillsState[role].confirm_use && SkillsState[role].turn_off <= total_turns)
            {
                SkillsState[role] = (false, false, -1);
            }
        }
        if (!SkillsState[role].is_active) return false;
        return true;
    }
    public void RefreshStatistics(List<string> StatsRemoved)
    {
        //It is responsible for reversing alterations made by states in player statistics


        foreach (string stat in StatsRemoved)
        {
            switch (stat)
            {
                case "LessVision":
                    vision = Players_DB[role]["vision"];
                    if (Status.ContainsKey("MoreVision")) vision += 2;
                    break;
                case "MoreVision":
                    vision = Players_DB[role]["vision"];
                    if (Status.ContainsKey("LessVision")) vision -= 2;
                    break;
                case "LimitedSpeed":
                    speed = Players_DB[role]["speed"];
                    if (Status.ContainsKey("IncrementedSpeed")) speed += 3;
                    break;
                case "IncrementedSpeed":
                    speed = Players_DB[role]["speed"];
                    if (Status.ContainsKey("LimitedSpeed")) speed -= 2;
                    if (speed < 1) speed = 1;
                    break;

            }


        }

    }
    public void RefreshStatus()
    {
        //Updates the player's status list by removing effects that have already expired
        List<string> StatsRemoved = new List<string>();
        bool removed = true;
        while (removed)
        {
            removed = false;
            foreach (var status in Status)
            {
                if (status.Value <= total_turns)
                {
                    Status.Remove(status.Key);
                    StatsRemoved.Add(status.Key);
                    removed = true;
                    break;
                }
            }

        };
        RefreshStatistics(StatsRemoved);


    }


    public bool TakeDamage(int damage)
    {

        //It deducts the amount of health indicated to the player in case the life has run out, returns them to their starting position and resets their stats
        if (role == "Capitan America" && IsActiveSkill()) return true;
        health -= damage;
        if (health <= 0)
        {
            instance.transform.position = new Vector3(seed.x + 0.5f, seed.y + 0.5f, -3);
            GenerateStistics(Players_DB);
            Status.Clear();
            return false;//Dead
        }

        return true;//Still alive

    }
    public void AddHealth(int health_)
    {
        //It adds the amount of health indicated to the player


        int player_maxhealth = Players_DB[role]["health"];
        if (health_ >= player_maxhealth)
        {

            return;
        }
        health += health_;
        if (health_ >= player_maxhealth)
        {
            health = player_maxhealth;
            return;
        }



    }


    public void InitPlayerFog(int LabDim)
    {

        //Initializes the fog state matrix, marks each fog box and the player's starting position without fog
        NotFog = new (bool IsOff, int radius)[LabDim, LabDim];
        //vision = LabDim / 10;
        vision = 4;
        Players_DB[role]["vision"] = vision;
        for (int i = 0; i < NotFog.GetLength(0); i++)
        {
            for (int j = 0; j < NotFog.GetLength(0); j++)
            {
                NotFog[i, j] = (false, 0);
            }

        }

    }
    public void UpdatePlayerFog()
    {
        //Updates the player's fog status, removing the fog in the current position
        NotFog[GetPlayerLabCord().x, GetPlayerLabCord().y] = (true, vision);

    }


    //Player's Skills

    public void CA_Skill()
    {
        //Controls Captain America's ability, ads Protected status to Status dictionary
        if (!SkillRefresh().is_avaliable) return;
        SkillsState[role] = (true, true, total_turns + 1);
        Status["Protected"] = total_turns + 1;
        SkillTurns.Add(total_turns);

    }
    public void IM_Skill(Player target)
    {
        //Control Iron Man's ability, receive the reference to a player's instance, which would be the target, and add the state of Paralized to it
        if (!SkillRefresh().is_avaliable) return;
        SkillsState[role] = (true, true, total_turns);
        if (target.total_turns < total_turns)
        {
            target.Status["Paralized"] = total_turns + 1;
            SkillTurns.Add(total_turns);
            return;
        }
        target.Status["Paralized"] = total_turns + 2;

        SkillTurns.Add(total_turns);

    }
    public void HE_Skill(Player target)
    {
        //Control Hawk Eyes's ability, receive the reference to the instance of a player, who would be the target, and deduct life from him

        if (!SkillRefresh().is_avaliable) return;
        SkillsState[role] = (true, true, total_turns);
        target.TakeDamage(damage);
        SkillTurns.Add(total_turns);

    }
    public void Thor_Skill(Dictionary<int, Player> Players)
    {
        //Control Thor's ability, receive reference to all players' instances, and deduct health from everyone in close proximity

        if (!SkillRefresh().is_avaliable) return;
        SkillsState[role] = (true, true, total_turns);
        int radius = Players_DB[role]["radius"];
        (float x, float y) ThorPosition = (GetActualPosition().x, GetActualPosition().y);
        foreach (var target in Players)
        {
            (float x, float y) TargetPosition = (target.Value.GetActualPosition().x, target.Value.GetActualPosition().y);
            if (target.Key == id) continue;
            if (Math.Abs(TargetPosition.x - ThorPosition.x) <= radius && Math.Abs(TargetPosition.y - ThorPosition.y) <= radius)
            {
                target.Value.TakeDamage(damage);
            }


        }

        SkillTurns.Add(total_turns);

    }
    public void Hulk_Skill(Player target)
    {
        //Control Hulk's ability, receive the reference to the instance of a player, who would be the target, and deduct life from him
        if (!SkillRefresh().is_avaliable) return;
        SkillsState[role] = (true, true, total_turns);
        target.TakeDamage(damage);
        SkillTurns.Add(total_turns);

    }
    public void Vision_Skill()
    {
        //Controls Vision's ability, sets the status of his ability to true but leaves his confirmation false
        if (!SkillRefresh().is_avaliable) return;
        if (IsActiveSkill()) return;
        SkillsState[role] = (true, false, total_turns);
        SkillTurns.Add(total_turns);

    }





}

public class TrapsClass
{

    public static void Controller(string TrapName, Player Target)
    {
        //It receives the instance of a player and the trap that will be applied to him and from the name of the trap, the method of the corresponding trap
        string Trap = TrapName.Replace("Trap_", "");
        switch (Trap)
        {
            case "LessVision":
                LessVision(Target);
                break;
            case "LowDamage":
                LowDamage(Target);
                break;
            case "HightDamage":
                HightDamage(Target);
                break;
            case "LimitedSpeed":
                LimitedSpeed(Target);
                break;
            case "Returned":
                Returned(Target);
                break;

        }

    }

    public static void LessVision(Player Target)
    {
        //Receives the instance of a player and reduces its vision and adds that status to the player's Statuses
        if (Target.Status.ContainsKey("LessVision"))
        {
            Target.Status["LessVision"] += 4;
            return;
        }
        Target.Status["LessVision"] = Target.total_turns + 4;
        Target.vision -= 2;
    }

    public static void LowDamage(Player Target)
    {
        //Receives the instance of a player and reduces 2 health units
        Target.TakeDamage(2);
    }
    public static void HightDamage(Player Target)
    {
        //Receives the instance of a player and reduces 6 health units
        Target.TakeDamage(6);
    }
    public static void LimitedSpeed(Player Target)
    {
        //Receives the instance of a player and reduces its speed and adds that status to the player's Statuses
        if (Target.Status.ContainsKey("LimitedSpeed"))
        {
            Target.Status["LimitedSpeed"] += 3;
            return;
        }
        Target.Status["LimitedSpeed"] = Target.total_turns + 3;
        Target.speed -= 2;
        if (Target.speed < 1) Target.speed = 1;
    }

    public static void Returned(Player Target)
    {
        //Receives the instance of a player and reinforces it to its initial state and position at the start of the game
        Target.TakeDamage(100);
    }


}
public class RewardsClass
{
    public static void Controller(string RewardName, Player Target)
    {
        //It receives the instance of a player and the reward that will be applied to him and from the name of the reward, the method of the corresponding reward
        string Reward = RewardName.Replace("Reward_", "");
        switch (Reward)
        {
            case "MoreVision":
                MoreVision(Target);
                break;
            case "MoreLife":
                MoreLife(Target);
                break;
            case "IncrementedSpeed":
                MoreSpeed(Target);
                break;
            case "RestoreHealth":
                RestoreHealth(Target);
                break;

        }

    }

    public static void MoreVision(Player Target)
    {
        //Receives the instance of a player and increments its vision and adds that status to the player's Statuses
        if (Target.Status.ContainsKey("MoreVision"))
        {
            Target.Status["MoreVision"] += 4;
            return;
        }
        Target.Status["MoreVision"] = Target.total_turns + 4;
        Target.vision += 2;
    }

    public static void MoreLife(Player Target)
    {
        //Receives the instance of a player and increments its health in 2 units
        Target.health += 3;
    }

    public static void MoreSpeed(Player Target)
    {
        //Receives the instance of a player and increments its speed in 2 units
        if (Target.Status.ContainsKey("IncrementedSpeed"))
        {
            Target.Status["IncrementedSpeed"] += 3;
            return;
        }
        Target.Status["IncrementedSpeed"] = Target.total_turns + 3;
        Target.speed += 3;
    }
    public static void RestoreHealth(Player Target)
    {
        //It receives the instance of a player and restores its life to the maximum and removes the effects of cheating from the state dictionary
        Target.AddHealth(10);
        if (Target.Status.ContainsKey("LimitedSpeed"))
        {
            Target.Status.Remove("LimitedSpeed");

        }
        if (Target.Status.ContainsKey("LessVision"))
        {
            Target.Status.Remove("LessVision");

        }
    }




}

public class Distances
{
    //This class contains methods related to the distances of the from one square on the board to others
    public static (int dist, (int x, int y)) GetMaxDistance(Player obj)
    {
        //Cycle through the player's distance matrix and select the greatest distance
        if (obj.distances == null) throw new Exception("Distances's matrix is null");


        (int dist, (int x, int y)) max_distance = (0, (obj.seed.x, obj.seed.y));
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
        //Cycle through the player's distance matrix and select the distances that correspond to the value variable, returning the coordinates of the squares that match that distance
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

    public GameObject MenuController;
    public TMP_Text Title;
    public TMP_Text Role;
    public TMP_Text Health_indicator;
    public Slider Health_indicatorSlide;
    public TMP_Text Speed_indicator;
    public Slider Speed_indicatorSlide;
    public TMP_Text Status_indicator;
    public TMP_Text Damage_indicator;
    public Slider Damage_indicatorSlide;
    public TMP_Text Vision_indicator;
    public Slider Vision_indicatorSlide;
    public GameObject PlayerPanel;
    public GameObject OnTurnIndicatorTexture;
    public GameObject TargetIndicatorTexture;
    public GameObject CAHabTexture;
    public GameObject HulkHabTexture;
    public GameObject HEHabTexture;
    public GameObject ThorHabTexture;
    public GameObject IMHabTexture;
    public GameObject VisionHabTexture;
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
    public Image PlayerPreview;
    public Image SkillPreview;
    public Image DisponibilitySkillTrue;
    public Image DisponibilitySkillFalse;
    public TMP_Text SkillCount;
    public TMP_Text PlayerTarget;
    public GameObject wallPrefab;
    public GameObject PathTexture;
    public GameObject fogTexture;
    public GameObject AreaTexture;
    public GameObject TrapTexture;
    public GameObject RewardTexture;
    public GameObject Trap_LessVisionTexture;
    public GameObject Trap_LowDamageTexture;
    public GameObject Trap_HightDamageTexture;
    public GameObject Trap_LimitedSpeedTexture;
    public GameObject Trap_ReturnedTexture;

    public GameObject Reward_MoreVision;
    public GameObject Reward_MoreLife;
    public GameObject Reward_IncrementedSpeed;
    public GameObject Reward_RestoreHealth;
    public Camera mainCamera;
    private bool OnCameraMov = false;
    private int n = 31;

    private int trapsProb;
    private int rewardsProb;
    private string[] Traps = {
            "Trap_LessVision",
            "Trap_LowDamage",
            "Trap_HightDamage",
           "Trap_LimitedSpeed",
            "Trap_Returned"

     };
    private Dictionary<string, GameObject> TrapsTextures = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> RewardsTextures = new Dictionary<string, GameObject>();
    private string[] Rewards = {
            "Reward_MoreVision",
            "Reward_MoreLife",
            "Reward_IncrementedSpeed",
            "Reward_RestoreHealth"
     };
    private int total_turns = 0;
    private int number_players;
    private int IDPlayerTarget = -1;
    static List<((int x, int y), (int x, int y))> paths = new List<((int x, int y), (int x, int y))>();
    private Dictionary<int, (int, int)> Players_Seed;

    Dictionary<string, Dictionary<string, int>> Players_db = new Dictionary<string, Dictionary<string, int>>();

    Dictionary<string, Dictionary<string, GameObject>> Textures = new Dictionary<string, Dictionary<string, GameObject>>();
    Dictionary<int, Player> Players = new Dictionary<int, Player>();

    string[,] laberinto;
    GameObject[,] MapFog;
    GameObject[,] Traps_Rewards;
    GameObject[,] Walls_Paths;
    GameObject[,] Checkpoints;
    private static System.Random rand = new System.Random();

    private bool Block_move = false;
    private bool New_Turn = true;
    private bool firstEntrty = true;
    private bool OnMapView = false;
    public bool OnMenu = false;
    private Player PlayerOnTurn;
    public void Start()
    {


        Init_DBS();

        number_players = PlayerPrefs.GetInt("Number_of_Players", 1);

        if (n % 2 == 0)
        {
            //Debug.LogError("Las dimensiones deben ser impares.");
            return;
        }
        Init_Map();




    }

    public void Map_Fill()
    {
        //Fill all map matrix with wall
        for (int i = 0; i < laberinto.GetLength(0); i++)
        {
            for (int j = 0; j < laberinto.GetLength(1); j++)
            {
                laberinto[i, j] = "wall";
            }
        }

    }
    public void Init_DBS()
    {
        //Init all Game's Databases
        n = PlayerPrefs.GetInt("MapSize", 31);
        //n = 7;
        trapsProb = PlayerPrefs.GetInt("TrapsProbability");
        rewardsProb = PlayerPrefs.GetInt("RewardsProbability");
        Players_Seed = new Dictionary<int, (int, int)>    {
        {0,(1,1)},
        {1,(1,n-2)},
        {2,(n-2,1)},
        {3,(n-2,n-2)},
        {4,((n-1)/2,1)},
        {5,((n-1)/2,n-2)},

     };
        laberinto = new string[n, n];
        MapFog = new GameObject[n, n];
        Checkpoints = new GameObject[n, n];
        Traps_Rewards = new GameObject[n, n];
        Walls_Paths = new GameObject[n, n];
        TrapsTextures["Trap_LessVision"] = Trap_LessVisionTexture;
        TrapsTextures["Trap_LowDamage"] = Trap_LowDamageTexture;
        TrapsTextures["Trap_HightDamage"] = Trap_HightDamageTexture;
        TrapsTextures["Trap_LimitedSpeed"] = Trap_LimitedSpeedTexture;
        TrapsTextures["Trap_Returned"] = Trap_ReturnedTexture;

        RewardsTextures["Reward_MoreVision"] = Reward_MoreVision;
        RewardsTextures["Reward_MoreLife"] = Reward_MoreLife;
        RewardsTextures["Reward_IncrementedSpeed"] = Reward_IncrementedSpeed;
        RewardsTextures["Reward_RestoreHealth"] = Reward_RestoreHealth;



        Textures["Capitan America"] = new Dictionary<string, GameObject>
        {
            { "Player", CA_texture },
             { "CHP", CA_CHP },
             { "SkillPrev", CAHabTexture }
        };
        Textures["Iron Man"] = new Dictionary<string, GameObject>
        {
            { "Player", IM_texture },
             { "CHP", IM_CHP },
             { "SkillPrev", IMHabTexture }
        };
        Textures["Thor"] = new Dictionary<string, GameObject>
        {
            { "Player", Thor_texture },
             { "CHP", Thor_CHP },
             { "SkillPrev", ThorHabTexture }
        };
        Textures["Vision"] = new Dictionary<string, GameObject>
        {
            { "Player", Vision_texture },
             { "CHP", Vision_CHP },
             { "SkillPrev", VisionHabTexture }
        };
        Textures["Hawk Eye"] = new Dictionary<string, GameObject>
        {
            { "Player", HE_texture },
             { "CHP", HE_CHP },
             { "SkillPrev", HEHabTexture }
        };
        Textures["Hulk"] = new Dictionary<string, GameObject>
        {
            { "Player",Hulk_texture },
             { "CHP", Hulk_CHP },
             { "SkillPrev", HulkHabTexture }
        };


        Players_db["Capitan America"] = new Dictionary<string, int>
        {
            { "health",5 },
            { "speed", 3},
            { "damage", 0},
            { "refresh_time", 2},
        };
        Players_db["Iron Man"] = new Dictionary<string, int>
        {
            { "health",6 },
            { "speed", 3},
            { "damage", 0},
            { "refresh_time", 3},
            { "radius", -1},
        };
        Players_db["Thor"] = new Dictionary<string, int>
        {
            { "health",7 },
            { "speed", 2},
            { "damage", 4},
            { "refresh_time", 3},
            { "radius", 3},
        };
        Players_db["Vision"] = new Dictionary<string, int>
        {
            { "health",7 },
            { "speed", 2},
            { "damage", 0},
            { "refresh_time", 2},
        };
        Players_db["Hawk Eye"] = new Dictionary<string, int>
        {
            { "health",5 },
            { "speed", 3},
            { "damage", 3},
            { "refresh_time", 4},
            { "radius", -1},
        };
        Players_db["Hulk"] = new Dictionary<string, int>
        {
            { "health",9 },
            { "speed", 1},
            { "damage", 10},
            { "refresh_time", 1},
            { "radius", 1},
        };


    }
    public bool GetProbability(int probability)
    {
        //Generate a random number and check if it is 0, to simulate the probability
        int probability_range = 100 / probability;
        int idx_probability = rand.Next(probability_range);
        if (idx_probability == 0)
        {
            return true;
        }
        return false;

    }
    public void GenerateTraps()
    {
        //Generate the traps on the map, using the configured probability, check that the position to add the trap is valid
        for (int i = 0; i < laberinto.GetLength(0); i++)
        {
            for (int j = 0; j < laberinto.GetLength(0); j++)
            {
                bool OnSeed = false;
                foreach (var seed in Players_Seed)
                {
                    if (seed.Value == (i, j))
                    {
                        OnSeed = true;
                        break;
                    }
                }
                if (OnSeed) continue;

                if (laberinto[i, j] == "wall") continue;

                if (laberinto[i, j] == "path")
                {

                    if (GetProbability(trapsProb))
                    {
                        int aleatory_index = rand.Next(Traps.Length);
                        laberinto[i, j] = Traps[aleatory_index];
                    }
                }
            }
        }
    }
    public void GenerateRewards()
    {
        //Generate the map compensations, using the configured probability, check that the position to add the trap is valid

        for (int i = 0; i < laberinto.GetLength(0); i++)
        {
            for (int j = 0; j < laberinto.GetLength(0); j++)
            {
                bool OnSeed = false;
                foreach (var seed in Players_Seed)
                {
                    if (seed.Value == (i, j))
                    {
                        OnSeed = true;
                        break;
                    }
                }
                if (OnSeed) continue;
                if (laberinto[i, j] == "wall") continue;
                if (laberinto[i, j] == "path")
                {

                    if (GetProbability(rewardsProb))
                    {
                        int aleatory_index = rand.Next(Rewards.Length);
                        laberinto[i, j] = Rewards[aleatory_index];
                    }
                }
            }
        }
    }
    public int[,] BFS(int x, int y)
    {
        //This BFS algorithm receives an initial position and calculates the distance from that position to all the positions on the board
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
                    if (cord.Item1 + mov_horizontal[i] >= 0 && cord.Item1 + mov_horizontal[i] < n && cord.Item2 + mov_vertical[i] >= 0 && cord.Item2 + mov_vertical[i] < n && !mask[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]] && laberinto[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]] != "wall")
                    {
                        queue.Enqueue((cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]));
                        Distance_matrix[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]] = Distance_matrix[cord.Item1, cord.Item2] + 1;

                    }
                }
            }
        }
        return Distance_matrix;

    }
    public List<(int, int y)>[,] BFS_Whit_Route(int x, int y)
    {
        //This BFS algorithm receives a starting position and saves the path to all the squares on the board
        bool[,] mask = new bool[n, n];
        List<(int, int y)>[,] RoutesMatrix = new List<(int, int y)>[n, n];
        Queue<(int, int)> queue = new Queue<(int, int)>();
        int[] mov_horizontal = { 1, -1, 0, 0 };
        int[] mov_vertical = { 0, 0, 1, -1 };
        queue.Enqueue((x, y));
        RoutesMatrix[x, y] = new List<(int, int y)>();
        RoutesMatrix[x, y].Add((x, y));
        while (queue.Count > 0)
        {
            var cord = queue.Dequeue();
            if (!mask[cord.Item1, cord.Item2])
            {
                mask[cord.Item1, cord.Item2] = true;
                for (int i = 0; i < mov_horizontal.Length; i++)
                {
                    if (cord.Item1 + mov_horizontal[i] >= 0 && cord.Item1 + mov_horizontal[i] < n && cord.Item2 + mov_vertical[i] >= 0 && cord.Item2 + mov_vertical[i] < n && !mask[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]] && laberinto[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]] != "wall")
                    {
                        queue.Enqueue((cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]));

                        List<(int, int y)> NewRoute = new List<(int, int y)>();
                        foreach (var item in RoutesMatrix[cord.Item1, cord.Item2])
                        {
                            NewRoute.Add(item);
                        }
                        NewRoute.Add((cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]));

                        RoutesMatrix[cord.Item1 + mov_horizontal[i], cord.Item2 + mov_vertical[i]] = NewRoute;

                    }
                }
            }
        }
        return RoutesMatrix;

    }
    public void Add_Checkpoints()
    {
        //It is in charge of the logic of the control points, that is, the squares that each player has to reach to win.


        (int dist, (int x, int y)) min_of_max_dist = Distances.GetMaxDistance(Players[0]);

        for (int p = 0; p < number_players; p++)
        {
            (int dist, (int x, int y)) max_dist_player = Distances.GetMaxDistance(Players[p]);
            if (max_dist_player.dist < min_of_max_dist.dist)
            {
                min_of_max_dist = max_dist_player;

            }
        }


        for (int p = 0; p < number_players; p++)
        {

            bool ValidCheckpoint = false;
            int CHPDistance = min_of_max_dist.dist;
            while (!ValidCheckpoint)
            //Run until you find valid CHP
            {
                List<(int dist, (int x, int y))> possibles_checkpoint = Distances.GetDistancesbyvalue(Players[p], CHPDistance);
                for (int i = 0; i < possibles_checkpoint.Count; i++)
                //Checking every possible CHP
                {
                    bool correct = true;

                    for (int pl = 0; pl < number_players; pl++)
                    //Checking if the CHP is in any other player's seed or matches their CHP
                    {

                        if (possibles_checkpoint[i].Item2.x == Players[pl].seed.x && possibles_checkpoint[i].Item2.y == Players[pl].seed.y)
                        {

                            correct = false;
                        }
                        if (Players[pl].checkpoint.x == possibles_checkpoint[i].Item2.x && Players[pl].checkpoint.y == possibles_checkpoint[i].Item2.y)
                        {

                            correct = false;
                        }

                    }
                    if (correct)
                    {
                        //A valid CHP was found that did not match or even in a seed
                        Players[p].checkpoint = possibles_checkpoint[i].Item2;
                        ValidCheckpoint = true;
                        break;
                    }



                }
                CHPDistance--;


            }

        }
        for (int p = 0; p < number_players; p++)
        {

            Checkpoints[Players[p].checkpoint.x, Players[p].checkpoint.y] = Instantiate(Players[p].checkpoint_texture, new Vector3(Players[p].checkpoint.x + 0.5f, Players[p].checkpoint.y + 0.5f, -1), Quaternion.identity);
            laberinto[Players[p].checkpoint.x, Players[p].checkpoint.y] = $"CHP_{Players[p].role}";
        }

    }

    public void Generarate()
    {
        //This is the algorithm in charge of the random generation of the maze

        // Leading the way in an odd starting cell
        (int x, int y) cord = (1, 1);
        laberinto[cord.x, cord.y] = "path";
        // Move the initial cell to determine possible contiguous paths
        Add_path(cord);

        //Process the coordinates in the list until they run out
        while (paths.Count > 0)
        {
            // Choose a random sort and remove it from the list
            int index = rand.Next(paths.Count);
            var (wall_cord, path_cord) = paths[index];
            paths.RemoveAt(index);

            // If the connected cell has not been visited
            if (laberinto[path_cord.x, path_cord.y] == "wall")
            {

                laberinto[wall_cord.x, wall_cord.y] = "path";// Cleans the wall between the cells
                laberinto[path_cord.x, path_cord.y] = "path";// Mark the new cell as a path
                Add_path(path_cord); // Map the possible paths adjacent to the current cell
            }
        }
    }

    public void Add_path((int x, int y) cord)
    {
        int x = cord.x;
        int y = cord.y;
        // Add the cell that will be the possible next path and an adjacent cell
        if (x > 1) paths.Add(((x - 1, y), (x - 2, y))); // Up
        if (x < n - 2) paths.Add(((x + 1, y), (x + 2, y))); // Down
        if (y > 1) paths.Add(((x, y - 1), (x, y - 2))); // Left
        if (y < n - 2) paths.Add(((x, y + 1), (x, y + 2))); // Right
    }

    public void Imprimir()
    {
        //Generates the visual part of the labyrinth

        for (int i = 0; i < n; i++)
        {

            for (int j = 0; j < n; j++)
            {
                if (laberinto[i, j].Contains("Trap_"))
                {

                    Traps_Rewards[i, j] = Instantiate(TrapsTextures[laberinto[i, j]], new Vector3(i + 0.5f, j + 0.5f, -2), Quaternion.identity);
                    Traps_Rewards[i, j].SetActive(false);
                }
                if (laberinto[i, j].Contains("Reward_"))
                {
                    Traps_Rewards[i, j] = Instantiate(RewardsTextures[laberinto[i, j]], new Vector3(i + 0.5f, j + 0.5f, -2), Quaternion.identity);
                }
                if (laberinto[i, j] == "wall")
                {
                    //Debug.Log("instantied");
                    Walls_Paths[i, j] = Instantiate(wallPrefab, new Vector3(i + 0.5f, j + 0.5f, 2), Quaternion.identity);
                }
                if (laberinto[i, j] != "wall")
                {

                    Walls_Paths[i, j] = Instantiate(PathTexture, new Vector3(i + 0.5f, j + 0.5f, 2), Quaternion.identity);
                }
                MapFog[i, j] = Instantiate(fogTexture, new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.identity);


            }
        }
    }
    public void CentrarCamara()
    {
        // Calculate the central position of the maze
        float x_camera = n / 2;
        float y_camera = n / 2;
        float z_camera = -10f;
        Vector3 centralPosition = new Vector3(x_camera, y_camera, z_camera); // -10 es la distancia Z para la cámara

        // Move the camera to the center position
        mainCamera.transform.position = centralPosition;

        // Adjust the camera to make the maze look complete

        if (mainCamera.orthographic)
        {
            float orthographic_size = n / 2 + 2;
            mainCamera.orthographicSize = orthographic_size;
        }
    }

    public void Init_Players()
    {
        //Takes care of the initialization of the players

        for (int i = 0; i < number_players; i++)
        {
            string player_role = PlayerPrefs.GetString($"PlayerRole_{i}");
            string player_name = PlayerPrefs.GetString($"PlayerName_{i}");

            (int, int) seed_player = Players_Seed[i];
            Players[i] = new Player(id_: i, seed_: seed_player, name_: player_name, distances_: BFS(seed_player.Item1, seed_player.Item2), role_: player_role);
            Players[i].texture = Textures[player_role]["Player"];
            Players[i].checkpoint_texture = Textures[player_role]["CHP"];
            Players[i].texturePreviewSkill = Textures[player_role]["SkillPrev"];
            Players[i].GenerateStistics(Players_db);


        }
    }

    public void Init_Map()
    {
        //Start the maze map

        Map_Fill();
        Generarate();

        Init_Players();


        CentrarCamara();

        for (int i = 0; i < number_players; i++)
        {
            var seed_player = Players_Seed[i];
            int x = seed_player.Item1;
            int y = seed_player.Item2;
            Players[i].instance = Instantiate(Players[i].texture, new Vector3(x + 0.5f, y + 0.5f, -4), Quaternion.identity);
            Players[i].InitPlayerFog(n);
            Players[i].UpdatePlayerFog();




        }
        Add_Checkpoints();
        GenerateTraps();
        GenerateRewards();
        Imprimir();



    }

    public void UpdateFogState(Player player)
    {
        //It simulates the darkness of the labyrinth, illuminating those areas already visited by the player and leaving the others dark

        PlayerOnTurn.UpdatePlayerFog();
        for (int i = 0; i < MapFog.GetLength(0); i++)
        {
            for (int j = 0; j < MapFog.GetLength(0); j++)
            {

                if (laberinto[i, j] == "wall") continue;
                Walls_Paths[i, j].GetComponent<Light2D>().enabled = player.NotFog[i, j].IsOff;
                Walls_Paths[i, j].GetComponent<Light2D>().pointLightOuterRadius = player.NotFog[i, j].Radius;
            }

        }
    }
    public void SwitchPlayerPreview(int id)
    {
        //Run the current player preview in the player panel

        GameObject PlayerTexture = Players[id].texture;
        SpriteRenderer spritePlayerRenderer = PlayerTexture.GetComponent<SpriteRenderer>();
        GameObject SkillTexture = Players[id].texturePreviewSkill;
        SpriteRenderer spriteSkillRenderer = SkillTexture.GetComponent<SpriteRenderer>();

        if (spritePlayerRenderer != null)
        {
            PlayerPreview.sprite = spritePlayerRenderer.sprite;
            SkillPreview.sprite = spriteSkillRenderer.sprite;
        }
    }



    public void Update()
    {
        PlayerMenu();
        Turn_Simulator();



    }
    public void Turn_Simulator()
    {
        //Takes care of all shift simulation logic

        if (OnMenu) return;
        if (OnCameraMov) return;
        Player player_selected = Players[total_turns % number_players];
        PlayerOnTurn = Players[total_turns % number_players];
        if (firstEntrty)
        {

            player_selected.total_turns++;
            player_selected.IsActiveSkill();
            player_selected.RefreshStatus();
            DisplayPlayerPanel(player_selected);
            ChangePlayerVision(player_selected, 3f);
            if (!player_selected.Status.ContainsKey("Paralized"))
            {
                DisplayPosibleMovements(player_selected);
            }


            firstEntrty = false;
        }

        PlayerInput();
        if (!player_selected.Status.ContainsKey("Paralized"))
        {

            if (!Block_move) Check_Move(player_selected);
            SkillsController(player_selected);
        }
        if (number_players == 1 && Block_move)
        {
            NextTurn();
        }


        SwitchPlayerPreview(player_selected.id);
        Traps_and_RewardsCheck();
        CheckWinner();
        CheckNextTrun();

    }
    public void Traps_and_RewardsCheck()
    {
        //Check to see if the current player is on top of a trap or bounty
        (int x, int y) PlayerLabCords = PlayerOnTurn.GetPlayerLabCord();
        string CeldInfo = laberinto[PlayerLabCords.x, PlayerLabCords.y];
        if (CeldInfo.Contains("Trap_"))
        {
            //Debug.Log($"Trap finded {CeldInfo}");
            TrapsClass.Controller(CeldInfo, PlayerOnTurn);
            DisplayPlayerPanel(PlayerOnTurn);
            ChangePlayerVision(PlayerOnTurn);
            laberinto[PlayerLabCords.x, PlayerLabCords.y] = "path";
            Traps_Rewards[PlayerLabCords.x, PlayerLabCords.y].SetActive(true);


        }
        if (CeldInfo.Contains("Reward_"))
        {

            //Debug.Log($"Reward finded {CeldInfo}");
            RewardsClass.Controller(CeldInfo, PlayerOnTurn);
            DisplayPlayerPanel(PlayerOnTurn);
            ChangePlayerVision(PlayerOnTurn);
            laberinto[PlayerLabCords.x, PlayerLabCords.y] = "path";
            Destroy(Traps_Rewards[PlayerLabCords.x, PlayerLabCords.y]);


        }

    }
    public void CheckWinner()
    {
        //Check if any player meets the condition of winner
        (int x, int y) PlayerLabCords = PlayerOnTurn.GetPlayerLabCord();
        string CeldInfo = laberinto[PlayerLabCords.x, PlayerLabCords.y];
        if ($"CHP_{PlayerOnTurn.role}" != CeldInfo)
        {
            return;
        }
        PlayerPrefs.SetInt($"Winner", PlayerOnTurn.id);
        SceneManager.LoadScene("WinnerScene");

    }

    public void DisplaySkillRefresh(Player player_selected)
    {
        //It is responsible for reflecting the availability of the skill in the player panel
        int remaing = player_selected.SkillRefresh().time_remaing;
        SkillCount.text = remaing.ToString();
        SkillCount.color = Color.red;
        DisponibilitySkillFalse.enabled = true;
        DisponibilitySkillTrue.enabled = false;
        if (remaing == 0)
        {
            SkillCount.color = Color.green;
            DisponibilitySkillFalse.enabled = false;
            DisponibilitySkillTrue.enabled = true;

        }


    }

    public void SkillsController(Player player_selected)
    {
        //Verify that the player has the skill available, in which case proceed with the logic of the skills
        DisplaySkillRefresh(player_selected);
        if (!player_selected.SkillRefresh().is_avaliable) return;
        SkillsInput(player_selected);

    }
    public void PlayerMenu()
    {
        //If player press Escape,pause the game and show the menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            OnMenu = !OnMenu;
            MenuController.SetActive(OnMenu);
        }
    }
    public void PlayerInput()
    {
        //Check the player's input and execute de correspondent action

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!OnMapView)
            {
                CentrarCamara();
                OnMapView = true;
                return;
            }
            OnMapView = false;
            ChangePlayerVision(PlayerOnTurn);


        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (PlayerPanel.activeSelf)
            {
                PlayerPanel.SetActive(false);

            }
            else
            {
                PlayerPanel.SetActive(true);

            }


        }

    }
    public void SkillsInput(Player player_selected)
    {
        //Checks keyboard input for skills, handles communication of skill logic with keyboard input

        if (Input.GetMouseButtonDown(0))
        {
            // Get the Mouse Position on the Screen
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 fixedPosition = new Vector3((float)Math.Floor(mousePosition.x) + 0.5f, (float)Math.Floor(mousePosition.y) + 0.5f, -3f);
            PlayerTargetSelection(fixedPosition, player_selected);

            if (GhostMove(fixedPosition))
            {
                CleanGhostMovements();
                CleanPossibleMovements(PlayerOnTurn);
                DisplayPosibleMovements(PlayerOnTurn);
                ChangePlayerVision(player_selected);
            }

        }
        if (Input.GetKeyDown(KeyCode.H))
        {

            string role = player_selected.role;
            switch (role)
            {

                case "Iron Man":
                    if (IDPlayerTarget == -1)
                    {
                        CentrarCamara();
                        break;
                    }
                    player_selected.IM_Skill(Players[IDPlayerTarget]);
                    player_selected.IsActiveSkill();
                    PlayerTarget.text = "None";
                    if (PlayerOnTurn.TargetIndicatorInstance != null) Destroy(PlayerOnTurn.TargetIndicatorInstance);

                    PlayerTarget.color = Color.white;
                    ChangePlayerVision(player_selected);

                    break;
                case "Capitan America":

                    player_selected.CA_Skill();

                    break;
                case "Vision":

                    player_selected.Vision_Skill();
                    DisplayGhostMovements();

                    break;
                case "Hawk Eye":
                    if (IDPlayerTarget == -1)
                    {
                        CentrarCamara();
                        break;
                    }
                    player_selected.HE_Skill(Players[IDPlayerTarget]);
                    player_selected.IsActiveSkill();
                    PlayerTarget.text = "None";
                    PlayerTarget.color = Color.white;
                    ChangePlayerVision(player_selected);
                    if (PlayerOnTurn.TargetIndicatorInstance != null) Destroy(PlayerOnTurn.TargetIndicatorInstance);

                    break;
                case "Thor":
                    if (PlayerOnTurn.AreaTarget.Count == 0)
                    {
                        DisplayArea();
                        break;
                    }

                    player_selected.Thor_Skill(Players);
                    player_selected.IsActiveSkill();
                    CleanArea();
                    if (PlayerOnTurn.TargetIndicatorInstance != null) Destroy(PlayerOnTurn.TargetIndicatorInstance);
                    break;
                case "Hulk":
                    if (PlayerOnTurn.AreaTarget.Count == 0)
                    {
                        DisplayArea();
                        break;
                    }
                    if (IDPlayerTarget == -1)
                    {
                        CleanArea();
                        break;
                    }
                    player_selected.Hulk_Skill(Players[IDPlayerTarget]);
                    player_selected.IsActiveSkill();
                    PlayerTarget.text = "None";
                    PlayerTarget.color = Color.white;
                    CleanArea();
                    if (PlayerOnTurn.TargetIndicatorInstance != null) Destroy(PlayerOnTurn.TargetIndicatorInstance);
                    break;
            }
            player_selected.RefreshStatus();
            DisplayPlayerPanel(player_selected);
            DisplaySkillRefresh(player_selected);


        }

        if (Input.GetKeyDown(KeyCode.S))
        {

            ChangePlayerVision(player_selected);
        }

    }

    public void PlayerTargetSelection(Vector3 Position, Player player_selected)
    {
        //It takes care of the logic of selecting a target, clicking on an enemy player will mark them as a target.

        if (player_selected.role != "Iron Man" && player_selected.role != "Hawk Eye" && player_selected.role != "Hulk") return;
        int Scope = Players_db[PlayerOnTurn.role]["radius"];
        if (Scope == -1) Scope = int.MaxValue;
        (float x, float y) PlayerPosition = (PlayerOnTurn.GetActualPosition().x, PlayerOnTurn.GetActualPosition().y);
        //Debug.Log("Buscando target");
        foreach (var player in Players)
        {
            if (player_selected.id == player.Key) continue;
            if (player.Value.GetActualPosition().x == Position.x && player.Value.GetActualPosition().y == Position.y && Math.Abs(player.Value.GetActualPosition().y - PlayerPosition.y) <= Scope && Math.Abs(player.Value.GetActualPosition().x - PlayerPosition.x) <= Scope)
            {
                IDPlayerTarget = player.Key;
                //Debug.Log("Target " + player.Key);
                if (PlayerOnTurn.TargetIndicatorInstance != null) Destroy(PlayerOnTurn.TargetIndicatorInstance);
                PlayerOnTurn.TargetIndicatorInstance = Instantiate(TargetIndicatorTexture, new Vector3(Position.x, Position.y, -2), Quaternion.identity);
                break;
            }
        }
        if (IDPlayerTarget == -1) return;


        PlayerTarget.text = Players[IDPlayerTarget].role;
        PlayerTarget.color = Color.red;

    }
    public void DisplayArea()
    {
        //It is responsible for the display of an area on the screen, the area will be displayed as an area marked in red
        int radius = Players_db[PlayerOnTurn.role]["radius"];
        if (PlayerOnTurn.AreaTarget.Count != 0) return;
        (float x, float y) CenterPosition = (PlayerOnTurn.GetActualPosition().x, PlayerOnTurn.GetActualPosition().y);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                (float x, float y) CeldCord = (i + 0.5f, j + 0.5f);
                if (laberinto[i, j] == "wall") continue;
                if (Math.Abs(CenterPosition.x - CeldCord.x) <= radius && Math.Abs(CenterPosition.y - CeldCord.y) <= radius)
                {
                    GameObject AreaCeld = Instantiate(AreaTexture, new Vector3(CeldCord.x, CeldCord.y, -2), Quaternion.identity);
                    PlayerOnTurn.AreaTarget.Add(AreaCeld);
                }

            }

        }
    }
    public void CleanArea()
    {
        //Removes area display
        List<GameObject> AreaCelds = PlayerOnTurn.AreaTarget;
        for (int i = 0; i < AreaCelds.Count; i++)
        {
            Destroy(AreaCelds[i]);
        }
        PlayerOnTurn.AreaTarget.Clear();
    }


    public void ChangePlayerVision(Player player_selected_, float speed = 2f)
    {
        //It is responsible for focusing on the camera on the player in turn

        Vector3 player_pos = player_selected_.GetActualPosition();
        player_pos.z = -10f;

        if (mainCamera.orthographic && n > 9)
        {

            float orthographic_size = 4f;
            mainCamera.orthographicSize = orthographic_size;
        }
        if (PlayerOnTurn.OnTurnIndicatorInstance != null) Destroy(PlayerOnTurn.OnTurnIndicatorInstance);
        PlayerOnTurn.OnTurnIndicatorInstance = Instantiate(OnTurnIndicatorTexture, new Vector3(PlayerOnTurn.GetActualPosition().x, PlayerOnTurn.GetActualPosition().y, -2), Quaternion.identity);
        UpdateFogState(player_selected_);
        Checkpoints[PlayerOnTurn.checkpoint.x, PlayerOnTurn.checkpoint.y].GetComponent<ParpadeoLuz>().Active = true;
        StartCoroutine(FluidCamera(speed));


    }
    IEnumerator FluidCamera(float speed = 1f)
    {
        //Allows fluid movement of the camera, the effect of the camera moving towards the player
        OnCameraMov = true;



        float epsilon = speed / 10;
        while (Math.Abs(mainCamera.transform.position.y - PlayerOnTurn.GetActualPosition().y) > epsilon || Math.Abs(mainCamera.transform.position.x - PlayerOnTurn.GetActualPosition().x) > epsilon)
        {

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(PlayerOnTurn.GetActualPosition().x, PlayerOnTurn.GetActualPosition().y, -10), speed * Time.deltaTime);

            yield return new WaitForSeconds(0.00005f);
        }



        mainCamera.transform.position = new Vector3(PlayerOnTurn.GetActualPosition().x, PlayerOnTurn.GetActualPosition().y, -10);
        OnCameraMov = false;
    }


    public void DisplayPlayerPanel(Player player_selected_)
    {
        //Takes care of updating player panel data
        Title.text = $"{player_selected_.name}(P-{player_selected_.id})";

        Role.text = player_selected_.role;
        Health_indicator.text = $"{player_selected_.health}/{Players_db[player_selected_.role]["health"]}";

        Health_indicatorSlide.maxValue = Players_db[player_selected_.role]["health"];
        Health_indicatorSlide.value = player_selected_.health;
        Health_indicator.color = Color.black;
        if (player_selected_.health > Players_db[player_selected_.role]["health"]) Health_indicator.color = Color.yellow;
        Speed_indicator.text = $"{player_selected_.speed}/{Players_db[player_selected_.role]["speed"]}";
        Speed_indicatorSlide.maxValue = Players_db[player_selected_.role]["speed"];
        Speed_indicatorSlide.value = player_selected_.speed;
        Speed_indicator.color = Color.black;
        if (player_selected_.speed > Players_db[player_selected_.role]["speed"]) Speed_indicator.color = Color.yellow;
        Damage_indicator.text = $"{player_selected_.damage}/{Players_db[player_selected_.role]["damage"]}";
        Damage_indicatorSlide.maxValue = Players_db[player_selected_.role]["damage"];
        Damage_indicatorSlide.value = player_selected_.damage;
        Damage_indicator.color = Color.black;
        if (player_selected_.damage > Players_db[player_selected_.role]["damage"]) Damage_indicator.color = Color.yellow;
        Vision_indicator.text = $"{player_selected_.vision}/{Players_db[player_selected_.role]["vision"]}";
        Vision_indicatorSlide.maxValue = Players_db[player_selected_.role]["vision"];
        Vision_indicatorSlide.value = player_selected_.vision;
        Vision_indicator.color = Color.black;
        if (player_selected_.vision > Players_db[player_selected_.role]["vision"]) Vision_indicator.color = Color.yellow;
        string status_text = "Normal,";
        foreach (var status in player_selected_.Status)
        {
            if (status_text == "Normal,") status_text = "";
            status_text += $"{status.Key},";
        }
        Status_indicator.text = status_text.Remove(status_text.Length - 1);




    }
    public void CheckNextTrun()
    {
        //Check if the current player has taken turns
        if (Input.GetKeyDown(KeyCode.N))
        {

            NextTurn();
        }

    }
    public void NextTurn()
    {
        //Handles shift change logic
        Block_move = false;
        New_Turn = true;
        total_turns++;
        IDPlayerTarget = -1;
        PlayerTarget.text = "None";
        if (PlayerOnTurn.TargetIndicatorInstance != null) Destroy(PlayerOnTurn.TargetIndicatorInstance);
        if (PlayerOnTurn.OnTurnIndicatorInstance != null) Destroy(PlayerOnTurn.OnTurnIndicatorInstance);
        CleanPossibleMovements(PlayerOnTurn);
        CleanGhostMovements();
        CleanArea();
        if (!PlayerOnTurn.SkillsState[PlayerOnTurn.role].confirm_use && PlayerOnTurn.SkillsState[PlayerOnTurn.role].is_active)
        {
            PlayerOnTurn.SkillTurns.RemoveAt(PlayerOnTurn.SkillTurns.Count - 1);
            PlayerOnTurn.SkillsState[PlayerOnTurn.role] = (false, false, -1);
        }

        PlayerTarget.color = Color.white;
        firstEntrty = true;
        PlayerPanel.SetActive(true);
        Checkpoints[PlayerOnTurn.checkpoint.x, PlayerOnTurn.checkpoint.y].GetComponent<ParpadeoLuz>().Active = false;

    }

    public void Check_Move(Player player)
    {
        //Check if an attempt was made to make a move, in case the move is valid it will execute it
        Vector3 player_pos = player.instance.transform.position;
        Vector3 movement;
        if (Input.GetMouseButtonDown(0))
        {
            // Obtener la posición del mouse en la pantalla
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            movement = new Vector3((float)Math.Floor(mousePosition.x) + 0.5f, (float)Math.Floor(mousePosition.y) + 0.5f, -3f);

            //Debug.Log("Clic en: " + Math.Floor(mousePosition.x) + " " + Math.Floor(mousePosition.y) + " =>" + laberinto[(int)Math.Floor(mousePosition.x), (int)Math.Floor(mousePosition.y)]);

            if (Move(movement, player))
            {

                Block_move = true;
                CleanPossibleMovements(player);
                CleanGhostMovements();
                ChangePlayerVision(player);
                CleanArea();
                //Debug.Log(PlayerOnTurn.GetPlayerLabCord());

                return;
            }


        }




    }

    public bool Move(Vector3 target, Player player)
    {
        //Performs a specific player's movement to the indicated position

        bool finded = false;
        List<(int x, int y)> possible_movements = GetPosibleMovements(player);
        for (int i = 0; i < possible_movements.Count; i++)
        {
            if (possible_movements[i] == (Math.Floor(target.x), Math.Floor(target.y))) finded = true;
        }
        if (finded)
        {
            player.instance.transform.position = target;

            return true;
        }
        return false;

    }

    public bool GhostMove(Vector3 target)
    {
        //He takes care to perform Vision's moves when he uses his ability
        Player player = PlayerOnTurn;
        if (!player.IsActiveSkill()) return false;
        if (player.role != "Vision") return false;


        bool finded = false;
        List<(int x, int y)> ghost_movements = GetGhostMovements();
        for (int i = 0; i < ghost_movements.Count; i++)
        {
            if (ghost_movements[i] == (Math.Floor(target.x), Math.Floor(target.y))) finded = true;
        }
        if (finded)
        {
            player.instance.transform.position = target;
            player.SkillsState[player.role] = (true, true, player.total_turns);
            player.IsActiveSkill();
            return true;

        }
        return false;


    }

    public List<(int x, int y)> GetPosibleMovements(Player player_selected_)
    {
        //It is responsible for obtaining the possible moves that the player can make
        List<(int x, int y)> possible_celds = new List<(int x, int y)>();
        int pases = player_selected_.speed;
        Vector3 player_actual_pos = player_selected_.GetActualPosition();

        int[,] player_distances = BFS(Mathf.RoundToInt(player_actual_pos.x - 0.5f), Mathf.RoundToInt(player_actual_pos.y - 0.5f));
        for (int i = 0; i < player_distances.GetLength(0); i++)
        {
            for (int j = 0; j < player_distances.GetLength(1); j++)
            {
                if (player_distances[i, j] <= pases && player_distances[i, j] > 0 && laberinto[i, j] != "wall")
                {
                    bool PlayerOnCeld = false;
                    foreach (var player in Players)
                    {
                        if (player.Value.id == PlayerOnTurn.id) continue;
                        if (player.Value.GetPlayerLabCord() == (i, j))
                        {
                            PlayerOnCeld = true;
                            break;
                        }


                    }

                    if (!PlayerOnCeld) possible_celds.Add((i, j));
                }

            }
        }
        return possible_celds;
    }
    public void DisplayPosibleMovements(Player player_selected_)
    {
        //It is responsible for showing on the screen the possible movements of the current player
        if (Block_move)
        {
            CleanPossibleMovements(player_selected_);
            return;
        }
        List<(int, int)> possible_celds = GetPosibleMovements(player_selected_);
        for (int i = 0; i < possible_celds.Count; i++)
        {
            (int x, int y) cord = possible_celds[i];
            GameObject instance_mov = Instantiate(Posible_Mov, new Vector3(cord.x + 0.5f, cord.y + 0.5f, -3), Quaternion.identity);
            Players[player_selected_.id].posibles_movements.Add(instance_mov);

        }
        New_Turn = false;
    }

    public void CleanPossibleMovements(Player player_selected_)
    {
        //It is responsible for cleaning the possible movements of the current player from the screen

        List<GameObject> posible_movements = player_selected_.posibles_movements;
        for (int i = 0; i < posible_movements.Count; i++)
        {
            Destroy(posible_movements[i]);
        }
        player_selected_.posibles_movements.Clear();
    }
    public List<(int x, int y)> GetGhostMovements()
    {
        //It is responsible for obtaining Vision's smooth moves when it is in ghost mode. Or it takes care to see the walls it can pass through
        Player player_selected_ = PlayerOnTurn;
        List<(int x, int y)> possible_celds = new List<(int x, int y)>();
        Vector3 player_actual_pos = player_selected_.GetActualPosition();
        (int x, int y)[] Movements = { (1, 0), (-1, 0), (0, -1), (0, 1) };
        (float x, float y) GhostPos = (player_selected_.GetActualPosition().x, player_selected_.GetActualPosition().y);
        for (int i = 0; i < Movements.Length; i++)
        {
            (int x, int y) FixedFinalPos = ((int)GhostPos.x + 2 * (Movements[i].x), (int)GhostPos.y + 2 * (Movements[i].y));
            (int x, int y) FixedMiddlePos = ((int)GhostPos.x + (Movements[i].x), (int)GhostPos.y + (Movements[i].y));
            if (FixedFinalPos.x > 0 && FixedFinalPos.y > 0 && FixedFinalPos.x < n && FixedFinalPos.y < n && laberinto[FixedMiddlePos.x, FixedMiddlePos.y] == "wall")
            {
                if (laberinto[FixedFinalPos.x, FixedFinalPos.y] == "wall") continue;
                possible_celds.Add(FixedFinalPos);
            }

        }
        return possible_celds;
    }

    public void DisplayGhostMovements()
    {
        //It is responsible for showing Vision's possible movements on the screen

        Player player_selected_ = PlayerOnTurn;
        if (Players[player_selected_.id].ghost_movements.Count > 0) return;
        List<(int, int)> possible_celds = GetGhostMovements();
        for (int i = 0; i < possible_celds.Count; i++)
        {
            (int x, int y) cord = possible_celds[i];
            GameObject instance_mov = Instantiate(Posible_Mov, new Vector3(cord.x + 0.5f, cord.y + 0.5f, -3), Quaternion.identity);
            Players[player_selected_.id].ghost_movements.Add(instance_mov);

        }

    }

    public void CleanGhostMovements()
    {
        //It is responsible for removing possible Vision movements from the screen
        Player player_selected_ = PlayerOnTurn;
        List<GameObject> posible_movements = player_selected_.ghost_movements;
        for (int i = 0; i < posible_movements.Count; i++)
        {
            Destroy(posible_movements[i]);
        }
        player_selected_.ghost_movements.Clear();
    }



}

