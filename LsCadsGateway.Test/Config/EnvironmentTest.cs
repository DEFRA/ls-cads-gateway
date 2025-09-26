using Microsoft.AspNetCore.Builder;

namespace LsCadsGateway.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   { 
       var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
       var isDev = LsCadsGateway.Config.Environment.IsDevMode(builder);
       Assert.False(isDev);
   }
}
