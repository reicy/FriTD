using System.Runtime.Remoting.Messaging;

namespace Manager.Core
{
    public class ManagerBuilder
    {
        public static Manager BuildPlayerManager()
        {

            return null;
        }

        public static Manager BuildAiLearningManager()
        {
            var mng = new Manager();
            mng.InsertAi();
            return mng;
        }

        public static Manager BuildObservableAiLearningManager()
        {
            return null;
        }

        public static Manager BuildSimplePlayerManager()
        {
            return new Manager();
        }
    }
}