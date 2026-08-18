namespace Application.Common.Wrappers
{
    public class Response
    {
        public bool IsSuccess => Errors != null && !Errors.Any();
        public bool HasWarnings => Warnings != null && !Warnings.Any();
        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }

        public Response()
        {
            Errors = [];
            Warnings = [];
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
        }

        public void AddWarnings(IEnumerable<string> warnings)
        {
            Warnings.AddRange(warnings);
        }
    }

    public class Response<T> : Response
    {
        public T Value { get; set; }

        public Response(T value)
        {
            Value = value;
        }
    }
}
