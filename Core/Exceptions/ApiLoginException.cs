using System;

namespace Core.Exceptions
{
    public enum ApiLoginError
    {
        Undefined,
        Login,
        Password
    }
    public class ApiLoginException : Exception
    {

        public ApiLoginException(string message, ApiLoginError error) : base(message)
        {
            ApiError = error;
        }

        public ApiLoginError ApiError { get; }
    }
}