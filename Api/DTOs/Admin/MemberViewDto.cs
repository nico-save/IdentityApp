using System;
using System.Collections.Generic;

namespace Api.DTOs.Admin
{
    public class MemberViewDto
    {
        public string Id { get; set; }
        public string Username { get; set; }


        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public bool IsLocked { get; set; }

        public DateTime DateCreated { get; set; }
         

        public IEnumerable<string> Roles { get; set; }

    }

}
