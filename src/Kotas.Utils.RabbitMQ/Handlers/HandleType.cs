namespace Kotas.Utils.RabbitMQ.Handlers
{
    /// <summary>
    /// Handle state
    /// </summary>
    public enum HandleType
    {
        /// <summary>
        /// Message was handled successfully and will be removed from queue
        /// </summary>
        Ok,

        /// <summary>
        /// Message will be requeue
        /// </summary>
        Requeue,

        /// <summary>
        /// Message will be rejected and 'dead-lettered'
        /// </summary>
        Reject
    }
}
