namespace EsportApp;

public class LolMatch
{
    public string Player { get; }
    public string Champion { get; }
    public string Role { get; }
    public int Kills { get; }
    public int Deaths { get; }
    public int Assists { get; }
    public int Cs { get; }
    public int VisionScore { get; }
    public bool Won { get; }

    public LolMatch(string player, string champion, string role, int kills, int deaths, int assists, int cs, int visionScore, bool won)
    {
        Player = player;
        Champion = champion;
        Role = role;
        Kills = kills;
        Deaths = deaths;
        Assists = assists;
        Cs = cs;
        VisionScore = visionScore;
        Won = won;
    }
}
