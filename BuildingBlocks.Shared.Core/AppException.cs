namespace BuildingBlocks.Shared.Core
{
    public abstract class AppException : Exception
    {
        public string? ErrorCode { get; }
        public string SourceLayer { get; }

        protected AppException(string message, string? errorCode, string sourceLayer)
            : base(message)
        {
            ErrorCode = errorCode;

            SourceLayer = sourceLayer;
        }
    }
}