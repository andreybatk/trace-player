﻿using System.ComponentModel.DataAnnotations;

namespace TracePlayer.Contracts.Auth
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}