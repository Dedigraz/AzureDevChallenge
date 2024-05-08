using StackExchange.Redis;

var connectionString = "concierge-240.redis.cache.windows.net:6380,password=YmzkQxvV960Pyh6C6DrZopD97bFclWC9rAzCaChpFIU=,ssl=True,abortConnect=False";
var redisConnection = ConnectionMultiplexer.Connect(connectionString);
// ^^^ store and re-use this!!!

IDatabase db = redisConnection.GetDatabase();

bool wasSet = db.StringSet("favorite:flavor", "i-love-rocky-road");

string value = db.StringGet("favorite:flavor");
Console.WriteLine(value); // displays: ""i-love-rocky-road""

var stat = new GameStat("Soccer", new DateTime(1950, 7, 16), "FIFA World Cup", 
    new[] { "Uruguay", "Brazil" },
    new[] { ("Uruguay", 2), ("Brazil", 1) });

string serializedValue = Newtonsoft.Json.JsonConvert.SerializeObject(stat);
bool added = db.StringSet("event:1950-world-cup", serializedValue);

var result = db.StringGet("event:1950-world-cup");
stat = Newtonsoft.Json.JsonConvert.DeserializeObject<GameStat>(result.ToString());
Console.WriteLine(stat.Sport); // displays "Soccer"

redisConnection.Dispose();
redisConnection = null;
public class GameStat
{
    public string Id { get; set; }
    public string Sport { get; set; }
    public DateTimeOffset DatePlayed { get; set; }
    public string Game { get; set; }
    public IReadOnlyList<string> Teams { get; set; }
    public IReadOnlyList<(string team, int score)> Results { get; set; }

    public GameStat(string sport, DateTimeOffset datePlayed, string game, string[] teams, IEnumerable<(string team, int score)> results)
    {
        Id = Guid.NewGuid().ToString();
        Sport = sport;
        DatePlayed = datePlayed;
        Game = game;
        Teams = teams.ToList();
        Results = results.ToList();
    }

    public override string ToString()
    {
        return $"{Sport} {Game} played on {DatePlayed.Date.ToShortDateString()} - " +
               $"{String.Join(',', Teams)}\r\n\t" + 
               $"{String.Join('\t', Results.Select(r => $"{r.team } - {r.score}\r\n"))}";
    }
}