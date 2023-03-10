using System;
using System.Text;


// This enum represents the possible outcomes of a game.
public enum Results
{
    Win,
    Lose,
    Draw,
    Undetermined
}

// This struct represents a basic game.
public abstract class Game
{
    // A static field to keep track of the index for each game.
    private protected static uint constIndex = 0;

    // Fields to store the first and second player's GameAccount objects.
    public GameAccount FirstPlayer { get; protected set; }
    public GameAccount SecondPlayer { get; protected set; }

    // Fields to store the rating cost and index of the game.
    public uint RatingCost { get; protected set; }
    public uint Index { get; }

    // Fields to store the game's name and result .
    public string GameName { get; }
    public Results Result { get; protected set; }

    // Constructor to initialize the fields with the provided values.
    public Game(GameAccount firstPlayer, GameAccount secondPlayer, uint cost, string gameName)
    {
        FirstPlayer = firstPlayer;
        SecondPlayer = secondPlayer;
        RatingCost = cost;
        Index = constIndex++;
        GameName = gameName;
        Result = Results.Undetermined;
    }

    // Factory constructor.
    public Game(string gameName)
    {
        GameName = gameName;
        Index = constIndex++;
        Result = Results.Undetermined;
    }

    // Main method of a game.
    public abstract void Play();

    // Method to simulate a game match.
    public abstract void SimulatePlay();

    // Factory method.
    public abstract void SimulatePlay(GameAccount firstPlayer, GameAccount secondPlayer, uint cost = default);

}

// Basic version of a game, nothing special.
public class BasicGame : Game
{
    // Constructor to initialize the fields with the provided values.
    public BasicGame(GameAccount firstPlayer, GameAccount secondPlayer, uint cost) : base(firstPlayer, secondPlayer, cost, "Basic Game") { }

    // Factory constructor.
    public BasicGame() : base("Basic Game") { }

    public override void Play()
    {
        // If the game was implemented fully, the main method would be here.
        throw new NotImplementedException();
    }

    // This method simulates a game match.
    public override void SimulatePlay()
    {
        Random r = new Random();
        Result = r.NextDouble() >= 0.5 ? Results.Draw : r.NextDouble() >= 0.5 ? Results.Win : Results.Lose;
        FirstPlayer.CompleteGame(this);
    }

    // In case you spawned game via factory.
    public override void SimulatePlay(GameAccount firstPlayer, GameAccount secondPlayer, uint cost)
    {
        base.FirstPlayer = firstPlayer;
        base.SecondPlayer = secondPlayer;
        base.RatingCost = cost;
        this.SimulatePlay();
    }

}

// Training version - no rating costs.
public class TrainingGame : Game
{
    // Constructor to initialize the fields with the provided values.
    public TrainingGame(GameAccount firstPlayer, GameAccount secondPlayer) : base(firstPlayer, secondPlayer, 0, "Training Game") { }

    // Factory constructor.
    public TrainingGame() : base("Training Game") { }

    public override void Play()
    {
        // If the game was implemented fully, the main method would be here.
        throw new NotImplementedException();
    }

    // This method simulates a game match.
    public override void SimulatePlay()
    {
        Random r = new Random();
        Result = r.NextDouble() >= 0.5 ? Results.Draw : r.NextDouble() >= 0.5 ? Results.Win : Results.Lose;
        FirstPlayer.CompleteGame(this);
    }

    // In case you spawned game via factory.
    public override void SimulatePlay(GameAccount firstPlayer, GameAccount secondPlayer, uint cost = 0)
    {
        base.RatingCost = 0;
        base.FirstPlayer = firstPlayer;
        base.SecondPlayer = secondPlayer;
        this.SimulatePlay();
    }

}

// One-way version - only the "first" player gains/loses rating.
public class OneWayGame : Game
{
    // Constructor to initialize the fields with the provided values.
    public OneWayGame(GameAccount firstPlayer, GameAccount secondPlayer, uint cost) : base(firstPlayer, secondPlayer, cost, "One-way Game") { }

    // Factory constructor.
    public OneWayGame() : base("One-way game") { }

    public override void Play()
    {
        // If the game was implemented fully, the main method would be here.
        throw new NotImplementedException();
    }

    // This method simulates a game match.
    public override void SimulatePlay()
    {
        Random r = new Random();
        Result = r.NextDouble() >= 0.5 ? Results.Draw : r.NextDouble() >= 0.5 ? Results.Win : Results.Lose;
        FirstPlayer.CompleteGame(this);
        
    }

    // In case you spawned game via factory.
    public override void SimulatePlay(GameAccount firstPlayer, GameAccount secondPlayer, uint cost)
    {
        base.FirstPlayer = firstPlayer;
        base.SecondPlayer = secondPlayer;
        base.RatingCost = cost;
        this.SimulatePlay();
    }

}

// Game factory class.
// Note that games are one-use instances and using a game object (Play, SimulatePlay)
// more than once causes an error, because indexes are assigned to games on creation
// and using one game twice will break game histories.
public class GameFactory
{

    public Game CreateBasicGame() => new BasicGame();
    public Game CreateTrainingGame() => new TrainingGame();
    public Game CreateOneWayGame() => new OneWayGame();

}

// This class represents a game account.
public class GameAccount
{
    // Field to store the rating of the user.
    private uint rating = 5;

    // Fields to store user's name, games history and games count.
    public string UserName { get; protected set; }
    public List<Game> GameHistory { get; }
    public uint GamesCount { get; protected set; }

    // Get/set for the rating field including the check for it not to be negative.
    public virtual uint CurrentRating
    {
        get => rating;
        protected set
        {
            int temp = (int)value;
            rating = temp < 0 ? 0 : value;
        }
    }

    // Constructor to initialize the fields with the provided values.
    public GameAccount(string name)
    {
        UserName = name;
        GamesCount = 0;
        GameHistory = new List<Game>();
    }

    // Method which is called whenever user completes a game.
    public void CompleteGame(Game game)
    {
        // If the user was in the game, record the game for him and his opponent.
        if (game.FirstPlayer.UserName.Equals(this.UserName))
        {
            RecordGame(game);
            game.SecondPlayer.RecordGame(game);
        }
        else if (game.SecondPlayer.UserName.Equals(this.UserName))
        {
            RecordGame(game);
            game.FirstPlayer.RecordGame(game);
        }

        // If the user was not in the game, stop the app.
        else
        {
            Console.WriteLine("Players cannot complete a game they did not participate in");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }

    // Method to record games.
    private void RecordGame(Game game)
    {
        // If the game is not already recorded,
        if (!GameHistory.Any(g => g.Index == game.Index))
        {
            // count the new rating for the user.
            if (game.FirstPlayer.UserName.Equals(this.UserName))
            {
                if (game.Result == Results.Win)
                    CurrentRating += game.RatingCost;
                else if (game.Result == Results.Lose)
                    CurrentRating -= game.RatingCost;
            }
            // If the game is one-way, only the first player gains/loses rating.
            else if (game.SecondPlayer.UserName.Equals(this.UserName) && !(game is OneWayGame))
            {
                if (game.Result.Equals(Results.Win))
                    CurrentRating -= game.RatingCost;
                else if (game.Result.Equals(Results.Lose))
                    CurrentRating += game.RatingCost;
            }

            // Then add the game to the user's game history and +1 to the games count.
            this.GameHistory.Add(game);
            GamesCount++;
        }
    }

    // Method to write user's stats to the console.
    // That includes user's game history and rating.
    public void GetStats()
    {
        if (this.GameHistory.Count != 0) Console.WriteLine(GameHistoryToString(this.GameHistory));
        Console.WriteLine($"{UserName}'s rating: {CurrentRating}\n");
    }

    // Method to convert game history list to a readable table view and return that as a string.
    public string GameHistoryToString(List<Game> history)
    {
        // Find the maximum lengths of the game, player, and result strings.
        int maxGameNameLength = history.Max(game => game.GameName.Length);
        int maxFirstPlayerNameLength = history.Max(game => game.FirstPlayer.UserName.Length);
        int maxSecondPlayerNameLength = history.Max(game => game.SecondPlayer.UserName.Length);
        int maxResultLength = history.Max(game => game.Result.ToString().Length);
        int maxIndexLength = history.Max(game => game.Index.ToString().Length);
        int maxWagerLength = history.Max(game => game.RatingCost.ToString().Length);

        // Create a StringBuilder to hold the table view string.
        StringBuilder sb = new StringBuilder();

        // Add the table headers.
        sb.Append("\nIndex".PadRight(maxIndexLength + 8) + "| Game Name".PadRight(maxGameNameLength + 10)
        + "| First Player".PadRight(maxFirstPlayerNameLength + 15) + "| Second Player".PadRight(maxSecondPlayerNameLength + 16)
        + "| Result".PadRight(8) + " | Wager\n");

        sb.Append("------".PadRight(maxIndexLength + 7, '-') + "|".PadRight(maxGameNameLength + 10, '-')
        + "|".PadRight(maxFirstPlayerNameLength + 15, '-') + "|".PadRight(maxSecondPlayerNameLength + 16, '-')
        + "|".PadRight(8, '-') + "-|------\n");

        // Iterate over the games in the list and add them to the StringBuilder.
        foreach (Game game in history)
            sb.Append($"{game.Index.ToString().PadRight(maxIndexLength + 6)} | {game.GameName.PadRight(maxGameNameLength + 7)} | {game.FirstPlayer.UserName.PadRight(maxFirstPlayerNameLength + 12)} | {game.SecondPlayer.UserName.PadRight(maxSecondPlayerNameLength + 13)} | " + ((game.Result == Results.Draw || this.UserName.Equals(game.FirstPlayer.UserName)) ? game.Result.ToString().PadRight(6) : (game.Result == Results.Win ? Results.Lose.ToString().PadRight(6) : Results.Win.ToString().PadRight(6))) + $" | {game.RatingCost}\n");

        // Get the final string.
        return sb.ToString();
    }
}

// Premium version of a game account (less points on loses).
public class PremiumGameAccount : GameAccount
{
    // Field which represents the multiplier by which the negative rating is divided (2 by default).
    private uint multiplier;

    // Rating setter is changed to work with multiplier.
    public override uint CurrentRating
    {
        get => base.CurrentRating;
        protected set => base.CurrentRating = base.CurrentRating > value ? ((base.CurrentRating - value) / multiplier) + value : value;
    }

    // Constructor to initialize the fields with the provided values.
    public PremiumGameAccount(string name, uint multiplier = 2) : base(name) { this.multiplier = multiplier; }
}

// PremiumPlus version of a game account (more points on wins, less points on loses).
public class PremiumPlusGameAccount : GameAccount
{
    // Field which represents the multiplier by which the rating value is increased and the negative value is divided (2 by default).
    private uint multiplier;

    // Rating setter is changed to work with multiplier.
    public override uint CurrentRating
    {
        get => base.CurrentRating;
        protected set => base.CurrentRating = base.CurrentRating > value ? ((base.CurrentRating - value) / multiplier) + value : ((value - base.CurrentRating) * multiplier) + base.CurrentRating;
    }

    // Constructor to initialize the fields with the provided values.
    public PremiumPlusGameAccount(string name, uint multiplier = 2) : base(name) { this.multiplier = multiplier; }
}


class Program
{
    public static void Main(string[] args)
    {

        GameFactory factory = new GameFactory();
        List<Game> games = new List<Game> { factory.CreateBasicGame(), factory.CreateTrainingGame(), factory.CreateOneWayGame() };
        List<GameAccount> accounts = new List<GameAccount> { new GameAccount("Basic"), new PremiumGameAccount("Premium"), new PremiumPlusGameAccount("PPlus") };

        games[0].SimulatePlay(accounts[0], accounts[1], 5);
        games[1].SimulatePlay(accounts[0], accounts[2]);
        games[2].SimulatePlay(accounts[1], accounts[2], 5);

        foreach (GameAccount acc in accounts) acc.GetStats();

    }
}