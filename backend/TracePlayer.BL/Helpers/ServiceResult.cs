﻿namespace TracePlayer.BL.Helpers
{
    public class ServiceResult<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string ErrorMessage { get; init; } = "An error occurred";

        public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
        public static ServiceResult<T> Fail(string error) => new() { Success = false, ErrorMessage = error };
    }
}
