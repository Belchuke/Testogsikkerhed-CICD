using System.Threading.Tasks;
using Testogsikkerhed_CICD.Services;

namespace Testogsikkerhed_CICD
{
   static  public class DefaultData
    {


        public static async Task CreateDefaultData(IUserService userService)
        {
            try
            {

                await userService.Create(new Models.User()
                {
                    Name = "admin",
                    Email = "admin@gmail.com",
                    Age = 20,

                }, "Passw0rd");
            }
            catch (System.Exception)
            {

                
            }
        }
    }
}
