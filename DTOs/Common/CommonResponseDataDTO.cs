﻿namespace TestWebAPI.DTOs.Common
{
    public class CommonResponseDataDTO<T>
    {

        public int statusCode { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public T data { get; set; }

    }
}
