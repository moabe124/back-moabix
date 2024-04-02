namespace moabix.QueueManager
{
    public interface IRabbitManager
    {
        void PublishMessage(string message);
        void ConsumeMessages();
        void CloseConnection();
    }
}
