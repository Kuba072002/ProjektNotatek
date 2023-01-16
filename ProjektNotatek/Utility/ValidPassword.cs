using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Utility {
    public class ValidPassword : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            string password = value.ToString();
            bool hasLowerLetter = false;
            bool hasUpperLetter = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;
            int poolSize = 0;
            foreach (char c in password) {
                if (char.IsLower(c)) {
                    hasLowerLetter = true;
                }
                else if (char.IsUpper(c)) {
                    hasUpperLetter = true;
                }
                else if (char.IsDigit(c)) {
                    hasDigit = true;
                }
                else {
                    hasSpecialChar = true;
                }
            }

            if (hasLowerLetter) {
                poolSize += 26;
            }
            if (hasUpperLetter) {
                poolSize += 26;
            }
            if (hasDigit) {
                poolSize += 10;
            }
            if (hasSpecialChar) {
                poolSize += 32;
            }

            double entropy = password.Length * Math.Log2(poolSize);
            if (entropy < 30) {
                return new ValidationResult
                    ("Bardzo słabe hasło" + entropy.ToString("0.##"));
            }
            if (entropy < 50) {
                return new ValidationResult
                    ("Hasło nie jest wystarczajaco mocne " + entropy.ToString("0.##"));
            }
            else {
                return ValidationResult.Success;
            }
        }
    }
}
