using System;
using System.Collections.Generic;
using System.Text;

namespace Models {
    public class StudentPostVM {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int ProgramId { get; set; }

        public string Nickname { get; set; }

    }
}
