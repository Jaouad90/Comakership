using System;
using System.Collections.Generic;
using System.Text;

namespace Models {
    public class ChangePasswordVM {

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

    }
}
