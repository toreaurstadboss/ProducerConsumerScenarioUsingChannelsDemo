using ProducerConsumerScenarioUsingChannelsDemo;

class Program
{

    private static int _numberOfStoriesDisplayed = 0;

    static async Task<int> Main(string[] args)
    {

        ConsoleColor currentColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine("NEWSREEL - Producer/Consumer Demo");

        Console.ForegroundColor = currentColor;

        var cts = new CancellationTokenSource();
        using ProducerConsumerChannel<NewsStory> channel = new(
            produce: () => WriteStory(cts.Token),
            consume: story => ReadStory(story, cts)
        );

      

        await channel.StartAsync(cts.Token);

        if (_numberOfStoriesDisplayed > 10)
        {
            cts.Cancel(); // Stop the channel after 10 stories have been displayed
        }

        currentColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine($"\nAll stories processed! Number of stories: {_numberOfStoriesDisplayed}");

        Console.ForegroundColor = currentColor;

        return 0;
    }

    private static readonly List<NewsStory> _unseen = NewsFeed.Stories.ToList();

    private static void ReadStory(NewsStory story, CancellationTokenSource cts)
    {
        ConsoleColor currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(story);

        Console.ForegroundColor = currentColor;

        if (_numberOfStoriesDisplayed > 10)
        {
            cts.Cancel();
        }
    }
    

    private static async Task<NewsStory> WriteStory(CancellationToken token)
    {
        NewsStory pick;
        if (_unseen.Count > 0)
        {
            int idx = Random.Shared.Next(_unseen.Count);
            pick = _unseen[idx];
            _unseen.RemoveAt(idx);
        }
        else
        {
            pick = NewsFeed.Stories[Random.Shared.Next(NewsFeed.Stories.Count)];
        }
        pick.NumberOfTimesDisplayed++;

        _numberOfStoriesDisplayed++;        

        await Task.Delay(Random.Shared.Next(100, 800), token);
        return pick;
    }

}