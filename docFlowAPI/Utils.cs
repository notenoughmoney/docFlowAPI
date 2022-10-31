namespace docFlowAPI
{
    public class Utils
    {
        // штуки ниже генерят случайную строку 
        // нужно для однозначного идентифицирования отправляемых файлов
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
