﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestWebAPI.Models
{
    public class Permission
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string code { get; set; }
        [Required]
        public string value { get; set; }
        [Required]
        public DateTime createdAt { get; set; } = DateTime.Now;
        [Required]
        public DateTime updatedAt { get; set; }
        public virtual ICollection<Role_Permission>? Role_Permissions { get; set; }
    }
}
