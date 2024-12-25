namespace UserAuthApi.Constant
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionList(string module)
        {
            return new List<string>()
            {
                $"Permission.{module}.View",
                $"Permission.{module}.Create",
                $"Permission.{module}.Edit",
                $"Permission.{module}.Delete",


            };
        }

    }


}
