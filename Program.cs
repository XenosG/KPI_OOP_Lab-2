using System;


// This struct represents a basic game.
public abstract class Game
{
    // A static field to keep track of the index for each game.
    private protected static uint constIndex = 0;

    // Fields to store the first and second player's GameAccount objects.
    public string FirstPlayer { get; }
    public string SecondPlayer { get; }

    // Fields to store the rating cost and index of the game.
    public uint RatingCost { get; }
    public uint Index { get; }

    // Fields to store the game's name and result .
    public string GameName { get; }
    public Results Result { get; protected set; }

    // Constructor to initialize the fields with the provided values.
    public Game(string firstPlayer, string secondPlayer, uint cost, string gameName)
    {
        FirstPlayer = firstPlayer;
        SecondPlayer = secondPlayer;
        RatingCost = cost;
        Index = constIndex++;
        GameName = gameName;
        Result = Results.Undetermined;
    }

}

public class TrainingGame : Game{

}

// This class represents a game account.
public abstract class BaseGameAccount
{
    // Field to store the rating of the user.
    private uint rating = 5;

    // Fields to store user's name, games history and games count.
    public string UserName { get; protected set; }
    public List<Game> GameHistory { get; set; }
    public uint GamesCount { get; }

    // Get/set for the rating field including the check for it not to be negative.
    public virtual uint CurrentRating
    {
        get => rating;
        set
        {
            int temp = (int)value;
            rating = temp < 0 ? 0 : value;
        }
    }

    // Constructor to initialize the fields with the provided values.
    public BaseGameAccount(string name)
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
            else if (game.SecondPlayer.UserName.Equals(this.UserName))
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
        Console.WriteLine(GameHistoryToString(this.GameHistory));
        Console.WriteLine($"{UserName}'s rating: {CurrentRating}\n");
    }

    // Method to convert game history list to a readable table view and return that as a string.
    public static string GameHistoryToString(List<Game> history)
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
            sb.Append($"{game.Index.ToString().PadRight(maxIndexLength + 6)} | {game.GameName.PadRight(maxGameNameLength + 7)} | {game.FirstPlayer.UserName.PadRight(maxFirstPlayerNameLength + 12)} | {game.SecondPlayer.UserName.PadRight(maxSecondPlayerNameLength + 13)} | {game.Result.ToString().PadRight(6)} | {game.RatingCost}\n");

        // Get the final string.
        return sb.ToString();
    }
}

class Program{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
    }
}