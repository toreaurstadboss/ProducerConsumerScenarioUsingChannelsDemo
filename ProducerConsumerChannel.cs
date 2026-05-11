using System.Threading.Channels;

namespace ProducerConsumerScenarioUsingChannelsDemo
{

    public class ProducerConsumerChannel<T> : IDisposable
    {

        private readonly Channel<T> channel = Channel.CreateBounded<T>(new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });

        private readonly Func<Task<T>> _produce;
        private readonly Action<T> _consume;

        public ProducerConsumerChannel(Func<Task<T>> produce, Action<T> consume)
        {
            _produce = produce ?? throw new ArgumentNullException(nameof(produce));
            _consume = consume ?? throw new ArgumentNullException(nameof(consume));
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            Task producer = Task.Run(() => ProduceAsync(cancellationToken), cancellationToken);
            Task consumer = Task.Run(() => ConsumeAsync(cancellationToken), cancellationToken);
            await Task.WhenAll(producer, consumer);
        }       

        private async Task ProduceAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    T item =  await _produce();
                    await channel.Writer.WriteAsync(item, cancellationToken);
                }
            }
            finally
            {
                channel.Writer.TryComplete();
            }
        }

        private async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (T item in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    _consume(item);
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation if needed
            }
        }

        public void Dispose()
        {
            channel.Writer.TryComplete();
        }

    }
}
