namespace EsportApp;

public class LolMatch
{
    public string Player { get; }
    public string Champion { get; }
    public int Kills { get; }
    public int Deaths { get; }
    public int Assists { get; }
    public int Cs { get; }
    public int VisionScore { get; }
    public bool Won { get; }

    public LolMatch(string player, string champion, int kills, int deaths, int assists, int cs, int visionScore, bool won)
    {
        Player = player;
        Champion = champion;
        Kills = kills;
        Deaths = deaths;
        Assists = assists;
        Cs = cs;
        VisionScore = visionScore;
        Won = won;
    }
}