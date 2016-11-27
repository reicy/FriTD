using System.IO;

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

        public static Manager BuildObservableAiLearningManager(StreamReader reader)
        {
            var mng = new Manager();
            mng.InsertAi(reader);
            return mng;
        }

        public static Manager BuildSimplePlayerManager()
        {
            return new Manager();
        }
    }
}