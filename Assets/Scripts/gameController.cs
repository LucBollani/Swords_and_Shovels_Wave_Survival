using UnityEngine;
using UnityEngine.SceneManagement;

public class gameController : MonoBehaviour
{
    [System.Serializable]
    public class Spawn
    {
        public GameObject enemy;
        public Transform spawnPoint;
        public int count;
        public float time;

        public void spawn()
        {
            for (int i = count; i > 0; i--)
            {
                Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }

    public int skipToWave = 0;

    public PlayerController player;
    public uiController UIcontroller;
    public float timeBetweenWaves = 5f;

    public Spawn[] wave1;
    public Spawn[] wave2;
    public Spawn[] wave3;
    public Spawn[] wave4;
    public Spawn[] wave5;
    public Spawn[] wave6;
    public Spawn[] wave7;
    public Spawn[] wave8;
    public Spawn[] wave9;
    public Spawn[] wave10;
    public Spawn[] finalWave;
    private Spawn[][] waves;
    private string[] waveNames;
    private bool[] waveUnlocks;
    private string[] waveNewSkillName;
    private int skills;

    private enum GameState { MENU, DEAD, SPAWNING, WAITTING, COUNTING, PREPARING, FINISHED};

    private int waveIndex;
    private int spawnIndex;
    private int waveSize;

    private GameState state;
    private float waitTime;
    private float waveStartTime;

    //auxiliary variables
    private float healthRegenTickTime = 0.1f;
    private float healthRegenTimer;
    private float waveClearTickTime = 0.1f;
    private float waveClearTimer = 2f;

    public Texture2D pointer;

    private void Start()
    {
        waves = new Spawn[][]
        {
            wave1,
            wave2,
            wave3,
            wave4,
            wave5,
            wave6,
            wave7,
            wave8,
            wave9,
            wave10,
            finalWave
        };

        waveNames = new string[]
        {
            "Wave 1", "Wave 2", "Wave 3", "Wave 4",
            "Wave 5\nGreen Invasion", "Wave 6", "Wave 7", "Wave 8\nGhosts Revenge",
            "Wave 9", "Wave 10\nMecha Mayhem", "FINAL  WAVE"
        };

        waveUnlocks = new bool[]
        {
            false, true, false, true,
            false, true, false, true,
            false, true, false
        };

        waveNewSkillName = new string[]
        {
            "", "Unlocked: BOW", "", "Unlocked: HASTE",
            "", "Unlocked: WEAPONS 2", "", "Unlocked: SHIFT",
            "", "Unlocked: Life Regen", ""
        };

        waveIndex = -1;
        spawnIndex = 0;

        state = GameState.MENU;

        UIcontroller.clearDisplay();

        healthRegenTimer = Time.time + healthRegenTickTime;

        waveskip(skipToWave);

        Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        if (state == GameState.PREPARING)
        {
            //prepare values for the next wave
            waveIndex++;
            Debug.Log("WAVEINDEX: " + waveIndex);
            if (waveIndex != 11)
            {
                waveStartTime = Time.time + timeBetweenWaves;
                waveSize = waves[waveIndex].Length;
                spawnIndex = 0;

                //Debug.Log("Preparing wave " + waveIndex + " Size: " + waveSize);
                state = GameState.COUNTING;
            }
            else
            {
                waveStartTime = Time.time + timeBetweenWaves;
                state = GameState.FINISHED;
            }
        }
        else if (state == GameState.COUNTING)
        {
            //Debug.Log(Time.time + " > " + healthRegenTimer);
            //recover players HP. "gradual increment"
            if (Time.time > healthRegenTimer)
            {
                healthRegenTimer = Time.time + healthRegenTickTime;
                player.heal(1);
            }

            //wait until time.time > waitTime, then change state to spawning
            if (Time.time > waveStartTime)
            {
                state = GameState.SPAWNING;
                UIcontroller.displayWave(waveNames[waveIndex], 2f);
            }
        }
        else if (state == GameState.SPAWNING)
        {
            if (spawnIndex < waveSize)  //if there are still enemies to spawn in this wave
            {
                if (Time.time > waves[waveIndex][spawnIndex].time + waveStartTime)  //wait until the right spawnTime
                {
                    //Debug.Log("Spawning wave: " + waveIndex + " spawn: " + spawnIndex);
                    waves[waveIndex][spawnIndex].spawn();
                    spawnIndex++;
                }
            }
            else  //there's nothing else to spawn this wave
            {
                state = GameState.WAITTING;
            }
        }
        else if (state == GameState.WAITTING)
        {
            //keep cheking until there are no enemies alive
            //move to preparing
            if (Time.time > waveClearTimer)
            {
                //Debug.Log("Checking for wave clear");
                waveClearTimer = Time.time + waveClearTickTime;
                
                if (!enemyAlive())
                {
                    state = GameState.PREPARING;

                    UIcontroller.displayCompleted(waveNewSkillName[waveIndex], timeBetweenWaves * 0.8f);
                    if (waveUnlocks[waveIndex])
                    {
                        player.unlockSkill(skills);
                        skills++;
                    }
                }
            }
        }
        else if(state == GameState.MENU)
        {
            UIcontroller.displayStart();
        }
        else if (state == GameState.FINISHED)
        {
            if(Time.time > waveStartTime) UIcontroller.displayWin();
        }
        if (player.dead)
        {
            state = GameState.DEAD;
            UIcontroller.displayDeath();
        }
    }

    //try to find any enemy in the scene
    private bool enemyAlive()
    {
        if (GameObject.FindGameObjectWithTag("Enemy") || GameObject.FindGameObjectWithTag("Ghost") || GameObject.FindGameObjectWithTag("statue")) return true;
        return false;
    }

    private void waveskip(int waveNum)
    {
        waveNum--;
        for (int i = 0; i < waveNum; i++)
        {
            if (waveUnlocks[i])
            {
                player.unlockSkill(skills);
                skills++;
            }
        }
        waveIndex = waveNum - 1;
        state = GameState.MENU;
    }

    public void startGame()
    {
        UIcontroller.clearDisplay();
        state = GameState.PREPARING;
    }

    public void resetGame()
    {
        SceneManager.LoadScene("gameScene");
    }

    public void retry()
    {
        wipeEnemies();
        spawnIndex = 0;
        player.revive();
        state = GameState.PREPARING;
        UIcontroller.clearDisplay();
        if (waveIndex != -1) waveIndex--;
    }

    public void wipeEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject _enemy in enemies)
        {
            Destroy(_enemy);
        }
        enemies = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject _enemy in enemies)
        {
            Destroy(_enemy);
        }
        enemies = GameObject.FindGameObjectsWithTag("statue");
        foreach (GameObject _enemy in enemies)
        {
            Destroy(_enemy);
        }
    }
}
