using Newtonsoft.Json.Linq;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;
        HttpClient client = new HttpClient();
        
        int currentPage = 1;
        int totalPages = 1;
        
        do
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}&page={currentPage}";
            
            HttpResponseMessage response = client.GetAsync(url).Result;
            
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                JObject jsonResult = JObject.Parse(content);
                
                totalPages = (int)jsonResult["total_pages"];
                
                JArray data = (JArray)jsonResult["data"];
                foreach (JObject match in data)
                {
                    totalGoals += int.Parse((string)match["team1goals"]);
                }
                
                currentPage++;
            }
            else
            {
                throw new Exception($"Erro ao acessar a API: {response.StatusCode}");
            }
        } while (currentPage <= totalPages);
        
        currentPage = 1;
        totalPages = 1;
        
        do
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team2={team}&page={currentPage}";
            
            HttpResponseMessage response = client.GetAsync(url).Result;
            
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                JObject jsonResult = JObject.Parse(content);
                
                totalPages = (int)jsonResult["total_pages"];
                
                JArray data = (JArray)jsonResult["data"];
                foreach (JObject match in data)
                {
                    totalGoals += int.Parse((string)match["team2goals"]);
                }
                
                currentPage++;
            }
            else
            {
                throw new Exception($"Erro ao acessar a API: {response.StatusCode}");
            }
        } while (currentPage <= totalPages);
        
        client.Dispose();
        
        return totalGoals;
    }
}