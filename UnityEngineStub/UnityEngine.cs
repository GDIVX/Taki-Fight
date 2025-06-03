namespace UnityEngine
{
    public class SerializeField : System.Attribute {}
    public class TooltipAttribute : System.Attribute { public TooltipAttribute(string text){} }
    public class RangeAttribute : System.Attribute { public RangeAttribute(float a,float b){} }
    public class CreateAssetMenuAttribute : System.Attribute { public string fileName; public string menuName; public int order; }
    public class MonoBehaviour { }
    public class ScriptableObject { }
    public static class Debug {
        public static void Log(string message) {}
        public static void LogWarning(string message) {}
        public static void LogError(string message) {}
    }
    public static class Random {
        private static System.Random _random = new System.Random();
        public static void InitState(int seed) { _random = new System.Random(seed); }
        public static int Range(int min,int max) => _random.Next(min,max);
        public static float Range(float min,float max) => (float)(_random.NextDouble()*(max-min))+min;
    }
}
