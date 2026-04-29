using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

public static class SetupDictionaryIteration4
{
    private const string SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";
    private const string WORDLIST_PATH = "Assets/WordGame/Resources/word_list.txt";

    private static readonly string[] Words = new[]
    {
        "CAT", "DOG", "RUN", "SUN", "RAY", "SEA", "TEA", "EAR", "OAR", "ARM",
        "ART", "EAT", "ATE", "ICE", "OIL", "ORE", "ONE", "TWO", "TEN", "TOE",
        "TOP", "TIE", "TIP", "RAT", "ROT", "ROW", "RAW", "RED", "RIB", "RID",
        "NET", "NOT", "NOR", "NEW", "OWL", "NUT", "OUR", "OUT", "OWN", "ODD",
        "ANT", "ANY", "BAR", "BAT", "BED", "BIT", "BOY", "BUS", "CAB", "CAN",
        "CAR", "COW", "CUP", "DAY", "DEN", "DRY", "EGG", "END", "GAS", "GET",
        "GUN", "HAD", "HAS", "HAT", "HEY", "HIT", "IRE", "LAW", "LET", "OLD",
        "AYE", "BIB", "BIO", "BOA", "BOG", "BOO", "BUD", "BYE", "COB", "COD",
        "CON", "COO", "COP", "COT", "CUB", "CUE", "DAB", "DAD", "DUB", "DUE",
        "DUG", "EBB", "EGO", "ELM", "ERA", "GEM", "GIG", "GIN", "GUM", "GUT",
        "HEM", "HEN", "HER", "HID", "HIM", "HIP", "HIS", "HOB", "HOG", "HOP",
        "HOT", "HOW", "HUB", "HUE", "HUG", "HUM", "HUT", "ION", "ITS", "LAB",
        "LAD", "LAG", "LAP", "LAY", "LED", "LEG", "LID", "LIE", "LIP", "LIT",
        "LOG", "LOT", "LOW", "MAD", "MAN", "MAP", "MAR", "MAT", "MAY", "MEN",
        "MET", "MID", "MOB", "MOM", "MOP", "MUD", "MUG", "NAB", "NAP", "NIL",
        "NIP", "NOD", "NUB", "OAT", "ODE", "OHM", "OOH", "OPT", "ORB", "PAD",
        "PAL", "PAN", "PAR", "PAT", "PAW", "PAY", "PEA", "PEG", "PEN", "PET",
        "PIE", "PIG", "PIN", "PIT", "POD", "POP", "POT", "PRO", "PUB", "PUN",
        "PUT", "RAG", "RAM", "RAP", "RIM", "RIP", "ROB", "ROD", "ROE", "RUB",
        "RUE", "RUG", "RUM", "RUT", "SAD", "SAG", "SAP", "SAT", "SAW", "SAY",
        "SHE", "SIN", "SIP", "SIR", "SIT", "SLY", "SOB", "SOD", "SON", "SOP",
        "SOW", "SOY", "SPA", "SPY", "SUB", "SUE", "SUM", "TAB", "TAD", "TAG",
        "TAN", "TAR", "THE", "TIN", "TON", "TOR", "TOY", "TUB", "USE", "VAN",
        "VAT", "VET", "VIA", "VIE", "VOW", "WAR", "WAS", "WAY", "WEB", "WED",
        "WET", "WHO", "WHY", "WIG", "WIN", "WIT", "WOE", "WON", "WOO", "WOW",
        "YAM", "YAP", "YEA", "YES", "YET", "YOU",

        "RAIN", "ROAR", "ROAD", "RACE", "RIDE", "RIPE", "RICE", "RING", "ROSE", "RUST",
        "TEAR", "TIDE", "TIRE", "TONE", "TORN", "TOUR", "TOWN", "TRIP", "TREE", "TRUE",
        "STAR", "SOAR", "SORT", "SORE", "SAND", "SEAT", "SEED", "SEEN", "SIDE", "SITE",
        "EARN", "EAST", "EDIT", "ENDS", "ICED", "IRON", "IDEA", "INTO", "AREA", "ANTS",
        "HEAR", "HEAT", "HERO", "HOLD", "HOME", "HOPE", "HORN", "HOST", "HOUR", "HUNT",
        "HARD", "HEAD", "BEAN", "BEAR", "BEAT", "BIRD", "BOAT", "BODY", "BONE", "BORN",
        "BOTH", "BOWL", "BUSY", "CALL", "CARE", "CART", "CASE", "CAVE", "CELL", "CITY",
        "COAT", "COIN", "COLD", "COME", "COOL", "CORE", "COST", "DASH", "DEAR", "DEEP",
        "DENY", "DIET", "DIRT", "DONE", "DOOR", "DOWN", "DRAW", "DREW", "DROP", "EACH",
        "ARMY", "AUNT", "BAND", "BARN", "BLOW", "BOOM", "BURN", "BUST", "CANE", "CHAT",

        "STARE", "STORE", "STORM", "STAIN", "STEEP", "STILT", "STIRS", "STONE", "STOOD", "STRIP",
        "TRAIN", "TRADE", "TRIES", "TRUST", "EARNS", "EARTH", "ENTER", "RAISE", "ROAST", "RIVER",
        "ALERT", "ALONE", "ANGER", "BASIC", "BEACH", "BEARD", "BIRTH", "BLAME", "BLEND", "BLIND",
        "BOARD", "BRAIN", "BRAVE", "BREAD", "BRIDE", "CHAIN", "CHAIR", "CHANT", "CHARM", "CHART",
        "CHEAT", "CHEST", "CHILD", "CHILL", "CHINA"
    };

    [MenuItem("WordGame/Setup Dictionary (Iteration 4)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(SCENE_PATH))
        {
            Debug.LogError("Game.unity not found. Run iterations 2 and 3 setup first.");
            return;
        }

        EnsureFolders();
        WriteWordList();
        AssetDatabase.Refresh();

        var scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null)
        {
            Debug.LogError("Canvas not found. Run iteration 2 setup first.");
            return;
        }

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null)
        {
            Debug.LogError("GameController not found. Run iteration 2 setup first.");
            return;
        }
        var controllerGO = controllerTR.gameObject;

        var builder = controllerGO.GetComponent<WordBuilder>();
        if (builder == null)
        {
            Debug.LogError("WordBuilder not found. Run iteration 3 setup first.");
            return;
        }

        var validator = GetOrAdd<WordValidator>(controllerGO);
        validator.minWordLength = 2;
        builder.validator = validator;

        EditorUtility.SetDirty(builder);
        EditorUtility.SetDirty(validator);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#4ECDC4><b>WordGame Iteration 4:</b> Dictionary setup complete (" + Words.Length + " words).</color>");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/WordGame"))
            AssetDatabase.CreateFolder("Assets", "WordGame");
        if (!AssetDatabase.IsValidFolder("Assets/WordGame/Resources"))
            AssetDatabase.CreateFolder("Assets/WordGame", "Resources");
    }

    private static void WriteWordList()
    {
        var content = string.Join("\n", Words);
        File.WriteAllText(WORDLIST_PATH, content);
        Debug.Log("Word list written to " + WORDLIST_PATH + " (" + Words.Length + " words).");
    }

    private static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) c = go.AddComponent<T>();
        return c;
    }
}
