using Proto;

namespace ProtoClusterDemo.Web.Controllers
{
    public static class ProtoSystem
    {
        public static ActorSystem System = new ActorSystem(new ActorSystemConfig(){
            DeveloperSupervisionLogging = true
        });
    }
}