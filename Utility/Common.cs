
namespace Tangehrine.WebLayer.Utility
{
    public static class GlobalConstant
    {
        /* Default Project Name*/
        public const string DefaultProjectName = "FaceApp";

        public const string AlreadyRegisterd = "The email you have entered, is already exists.";
        public const string NotRegistered = "User not created, Please try again !";
        public const string RegisterSuccessfully = "Registration done successfully.";
        public const string RegisterEmailNotSent = "User is successfully created but we failed to sent email. Please contact administrator for help.";
        public const string InvalidLogin = "Login Failed. Invalid username or password.";
        public const string ConfirmEmail = "Please confirm your account.";
        public const string AccountClosed = "Your account is closed.";
        public const string AccountDeactivated = "Your account is deactivated.";
        public const string LoginSuccessfully = "Logged in successfully.";
        public const string AlreadyLogin = "Already logged in.";
        public const string EmailNotFound = "User not found !";
        public const string EmailConfirm = "Email confirmed successfully.";
        public const string EmailResetPassword = "Please check your Email to reset your password !";
        public const string PasswordChanged = "Password changed successfully.";
        public const string PasswordNotChanged = "Password has not been changed, Please try again later !";
        public const string LogoutSuccessfully = "Logged out successfully.";
        public const string UserUpdatedSuccessfully = "User updated successfully.";
        public const string RoleListSuccessfully = "Get role list successfully.";
        public const string SomethingWrong = "Something went wrong !";
        public const string CompanyNotApproved = "Your account is not approved by admin.";

        public const string UnhandledError = "Unhandled error occured. Please try again later !";
        public const string InvalidModel = "Invalid Details.";
        public const string InvalidLink = "Link you tried to access is invalid.";
        public const string LinkExpired = "Link you tried to access is expired.";

        public const string WrongCode = "You have entered wrong code.";

        //---- Code Already exist ----
        public const string AlreadyExistCode = "Code already exist. Please try another code.";

        //---- Name Already exist ----
        public const string AlreadyExistName = "Name already exist. Please try another Name.";

        /*: Question set is already exist*/
        public const string AlreadyExistId = "Question set already exist. Please try another Id.";

        public const string DetailNotFound = "Detail not found !";

        /*Delete Recored*/
        public const string DeleteRecord = "Record deleted successfully.";

        /* Invalid current password*/
        public const string InvalidCurrentPassword = "Invalid current password !";
        public const string UserNotFound = "User not found.";

        /**Inactive User**/
        public const string DeActiveUser = "Unfortunately your password cannot be reset while your email is disabled. Please contact your Client Admin or System Admin.";


        public const string EncryptionCode = "Tang444";
        public const string localhost = "localhost";
        public const string clientapp = "clientapp.narola.online:1160";

    }

    public static class UserClaims
    {
        public const string UserId = "UserId";
        public const string UserRole = "UserRole";
        public const string FullName = "FullName";

    }
}

