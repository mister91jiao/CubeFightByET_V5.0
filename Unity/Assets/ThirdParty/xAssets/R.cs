namespace libx
{
    public class R
    { 
        public static string GetPrefab(string name)
        {
            return string.Format("{0}.prefab", name);
        }

        public static string GetScene(string name)
        {
            return string.Format("{0}.unity", name); 
        } 
    }
}