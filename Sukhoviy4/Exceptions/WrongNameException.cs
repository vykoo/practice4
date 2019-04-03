using System;

namespace Sukhoviy4.Exceptions
{
    class WrongNameException: Exception
    {
        public WrongNameException(string wrongName)
        {
            WrongName = wrongName;
            Message = $"Received wrong name: {wrongName}";
        }

        public string WrongName { get; }

        public override string Message { get; }
    }
}
