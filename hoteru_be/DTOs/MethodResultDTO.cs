﻿using System.Collections.Generic;
using System.Net;

namespace hoteru_be.DTOs
{
    public class MethodResultDTO
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Message { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }

    }
}
