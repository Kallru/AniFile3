using System;
using System.Windows.Controls;

namespace AniFile3.DataStruct
{
    public partial class Subscriptions
    {
        // 내부 생성용
        private static Node CreateNodeFromTypeName(string typeName)
        {
            return Activator.CreateInstance(Type.GetType(typeName), true) as Node;
        }

        public static T CreateNode<T>(string subject, int tabIndex) where T : Node
        {
            var instance = CreateNodeFromTypeName(typeof(T).FullName);
            instance.Subject = subject;
            instance.CurrentTabItem = tabIndex;
            return instance as T;
        }

        public static HomeNode CreateHomeNode()
        {
            return CreateNodeFromTypeName(typeof(HomeNode).FullName) as HomeNode;
        }
    }
}
