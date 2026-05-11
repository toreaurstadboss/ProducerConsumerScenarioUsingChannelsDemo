namespace ProducerConsumerScenarioUsingChannelsDemo
{
    
    public class NewsStory : IComparable<NewsStory>
    {
        public string Title { get; init; } = string.Empty;
        public int Priority { get; init; }
        public int NumberOfTimesDisplayed { get; set; }

        public int CompareTo(NewsStory? other)
        {
            if (other is null)
            {
                return 1;
            }
            return Priority.CompareTo(other.Priority);
        }

        public override string ToString() => $"[Priority: {Priority}] {Title} (displayed: {NumberOfTimesDisplayed}x)";

    }
}
